using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WebhookCICD.Api.Exceptions;
using static FastEndpoints.Ep;

namespace WebhookCICD.Api.Endpoints;

public class MHWAppEndpoint(IConfiguration configuration, ILoggerFactory loggerFactory, JsonSerializerOptions jsonOptions) : Req<MHWAppRequest>.NoRes
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = loggerFactory.CreateLogger<MHWAppEndpoint>();
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions;

    public override void Configure()
    {
        Post("/api/mhw-app/webhook");
        AllowAnonymous();
    }

    public override async Task HandleAsync(MHWAppRequest req, CancellationToken _)
    {
        await SendAsync("Accepted", 200, CancellationToken.None);

        try
        {
            var handler = new MHWAppRequestHandler.HandlerBuilder()
                .WithRequest(req, _jsonOptions)
                .WithSecret(_configuration["GitHubWebhookSecret"])
                .WithDockerCompose(_configuration["DockerComposeFile"])
                .WithLogging(loggerFactory)
                .Build();

            handler
                .VerifyRequest()
                .VerifyWebhookType()
                .DeployMHWApp();
        }
        catch (BuilderInitException) { }
        catch (WebhookTypeVerificationException) { }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Unknown exception occured.");
        }
    }

    private class MHWAppRequestHandler
    {
#pragma warning disable CS8618
        public RawWebhookEvent Request { get; private set; }
        public WebhookEvent WebhookEvent { get; private set; }
        public string Secret { get; private set; }
        public string DockerComposePath { get; private set; }

        private ILogger<MHWAppRequestHandler> _logger { get; set; }
#pragma warning restore CS8618

        public MHWAppRequestHandler VerifyRequest()
        {
            if (Request.Signature.Length == 0 || Secret.Length == 0) throw new WebhookSignatureVerificationException();

            var keyBytes = Encoding.UTF8.GetBytes(Secret);
            var bodyBytes = Encoding.UTF8.GetBytes(Request.Content);

            var hash = HMACSHA256.HashData(keyBytes, bodyBytes);
            var hashHex = Convert.ToHexString(hash);
            var expectedHeader = $"sha256={hashHex.ToLower(CultureInfo.InvariantCulture)}";

            var isValid = Request.Signature.ToString() == expectedHeader;

            if (!isValid) throw new WebhookSignatureVerificationException();

            return this;
        }

        public MHWAppRequestHandler VerifyWebhookType()
        {
            var isCorrectEvent = Request.GitHubEvent == "workflow_run";
            var isCompleted = WebhookEvent.WorkflowRun.Status == "completed";
            var isSuccess = WebhookEvent.WorkflowRun.Conclusion == "success";

            if (!isCorrectEvent || !isCompleted || !isSuccess) throw new WebhookTypeVerificationException();

            return this;
        }

        public MHWAppRequestHandler DeployMHWApp()
        {
            _logger.LogInformation("Deploying app");
            return this;
        }

        public class HandlerBuilder
        {
            private readonly MHWAppRequestHandler _instance = new();

            public MHWAppRequestHandler Build()
            {
                if (null == _instance) throw new BuilderInitException();
                return _instance;
            }

            public HandlerBuilder WithRequest(RawWebhookEvent request, JsonSerializerOptions options)
            {
                _instance.Request = request;
                try
                {

                    WebhookEvent? webhookEvent = JsonSerializer.Deserialize<WebhookEvent>(request.Content, options);
                    if (null == webhookEvent) throw new WebhookBodyException();
                    _instance.WebhookEvent = webhookEvent;

                    return this;
                }
                catch (JsonException)
                {
                    throw new BuilderInitException();
                }
            }

            public HandlerBuilder WithSecret(string? secret)
            {
                if (null == secret) throw new BuilderInitException();
                _instance.Secret = secret;

                return this;
            }

            public HandlerBuilder WithDockerCompose(string? path)
            {
                if (null == path) throw new BuilderInitException();
                _instance.DockerComposePath = path;

                return this;
            }

            public HandlerBuilder WithLogging(ILoggerFactory loggerFactory)
            {
                _instance._logger = loggerFactory.CreateLogger<MHWAppRequestHandler>();
                return this;
            }
        }
    }
}


