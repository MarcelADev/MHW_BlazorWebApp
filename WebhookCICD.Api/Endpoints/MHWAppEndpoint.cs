using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Services;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WebhookCICD.Api.Exceptions;
using static FastEndpoints.Ep;

namespace WebhookCICD.Api.Endpoints;

/// <summary>
/// Webhook Api endpoint for redeplying the MHWApp.
/// </summary>
/// <param name="logger">DI logger.</param>
/// <param name="handler">DI request handler.</param>
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

/// <summary>
/// Request handler for MHWApp endpoint.
/// </summary>
/// <param name="configuration">DI API configuration.</param>
/// <param name="jsonOptions">DI json serialization options.</param>
/// <param name="logger">DI logger.</param>
public class MHWAppRequestHandler(
        IConfiguration configuration,
        JsonSerializerOptions jsonOptions,
        ILogger<MHWAppRequestHandler> logger
    )
{
    /// <summary>
    /// Request to handle.
    /// </summary>
    private RawWebhookEvent? Request { get; set; }

    private readonly IConfiguration _configuration = configuration;
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions;
    private readonly ILogger<MHWAppRequestHandler> _logger = logger;

    private string Secret => _configuration["GitHubWebhookSecret"]!;
    private WebhookEvent WebhookEvent
    {
        get
        {
            if (null == Request) throw new WebhookBodyException();
            WebhookEvent? webhookEvent = JsonSerializer.Deserialize<WebhookEvent>(Request.Content, _jsonOptions);
            if (null == webhookEvent) throw new WebhookBodyException();

            return webhookEvent;
        }
    }

    /// <summary>
    /// Set the request to handle.
    /// </summary>
    /// <param name="request">Request to handle.</param>
    /// <returns><see cref="MHWAppRequestHandler">MHWAppRequestHandler</see></returns>
    public MHWAppRequestHandler ForRequest(MHWAppRequest request)
    {
        Request = request;

        return this;
    }

    /// <summary>
    /// Verify the request signature.
    /// </summary>
    /// <returns><see cref="MHWAppRequestHandler">MHWAppRequestHandler</see></returns>
    /// <exception cref="WebhookBodyException"></exception>
    /// <exception cref="WebhookSignatureVerificationException"></exception>
    public MHWAppRequestHandler VerifyRequest()
    {
        if (null == Request) throw new WebhookBodyException();
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

    /// <summary>
    /// Verify the request type by examining the event type and status.
    /// </summary>
    /// <returns><see cref="MHWAppRequestHandler">MHWAppRequestHandler</see></returns>
    /// <exception cref="WebhookBodyException"></exception>
    /// <exception cref="WebhookTypeVerificationException"></exception>
    public MHWAppRequestHandler VerifyWebhookType()
    {
        if (null == Request) throw new WebhookBodyException();

        var isCorrectEvent = Request.GitHubEvent == "workflow_run";
        var isCompleted = WebhookEvent.WorkflowRun?.Status == "completed";
        var isSuccess = WebhookEvent.WorkflowRun?.Conclusion == "success";

        if (!isCorrectEvent || !isCompleted || !isSuccess) throw new WebhookTypeVerificationException();

        return this;
    }

    /// <summary>
    /// Deploy the MHWApp.
    /// </summary>
    /// <returns><see cref="MHWAppRequestHandler">MHWAppRequestHandler</see></returns>
    public MHWAppRequestHandler DeployMHWApp()
    {
        _logger.LogInformation("Deploying app");

        var hosts = new Hosts().Discover();
        var docker = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/docker-compose.yaml");

        docker?.Host.ComposeDown(
            composeFile: filePath,
            certificates: docker.Certificates,
            removeVolumes: true,
            removeOrphanContainers: true
        );

        docker?.Host.ComposeUpCommand(new Compose.ComposeUpCommandArgs
        {
            ComposeFiles = [filePath],
            Certificates = docker.Certificates,
        });

        return this;
    }

}
