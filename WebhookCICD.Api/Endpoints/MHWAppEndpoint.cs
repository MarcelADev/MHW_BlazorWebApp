using Ductus.FluentDocker.Builders;
using System.Globalization;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WebhookCICD.Api.Exceptions;
using static FastEndpoints.Ep;

namespace WebhookCICD.Api.Endpoints;

public class MHWAppEndpoint(
    ILogger<MHWAppEndpoint> logger,
    MHWAppRequestHandler handler
) : Req<MHWAppRequest>.NoRes
{
    private readonly ILogger _logger = logger;

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
            handler
                .ForRequest(req)
                .VerifyRequest()
                .VerifyWebhookType()
                .DeployMHWApp();
        }
        catch (WebhookSignatureVerificationException) { }
        catch (WebhookTypeVerificationException) { }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unknown exception occured.");
        }
    }
}

public class MHWAppRequestHandler(
        IConfiguration configuration,
        JsonSerializerOptions jsonOptions,
        ILogger<MHWAppRequestHandler> logger
    )
{
    public required RawWebhookEvent Request { get; set; }

    private readonly IConfiguration _configuration = configuration;
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions;
    private readonly ILogger<MHWAppRequestHandler> _logger = logger;

    private string Secret => _configuration["GitHubWebhookSecret"]!;
    private WebhookEvent WebhookEvent
    {
        get
        {
            WebhookEvent? webhookEvent = JsonSerializer.Deserialize<WebhookEvent>(Request.Content, _jsonOptions);
            if (null == webhookEvent) throw new WebhookBodyException();

            return webhookEvent;
        }
    }

    public MHWAppRequestHandler ForRequest(MHWAppRequest request)
    {
        Request = request;

        return this;
    }

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

        var filePath = Path.Combine(AppContext.BaseDirectory, "docker-compose.yml");

        using (var svc = new Builder()
            .UseContainer()
            .UseCompose()
            .AssumeComposeVersion(Ductus.FluentDocker.Model.Compose.ComposeVersion.V2)
            .FromFile(filePath)
            .AlwaysPull()
            .ForceRecreate()
            .Build())
        {
            svc.Start();
        }

        return this;
    }

}
