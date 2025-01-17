using FastEndpoints;

namespace WebhookCICD.Api.Endpoints;

public record MHWAppRequest : RawWebhookEvent
{
}

/// <summary>
/// WebhookEvent record with content body as raw string in <see cref="RawWebhookEvent.Content"/>.
/// </summary>
public record RawWebhookEvent : IPlainTextRequest
{
    public required string Content { get; set; }
    [FromHeader("x-github-event")]
    public required string GitHubEvent { get; set; }
    [FromHeader("x-hub-signature-256")]
    public required string Signature { get; set; }
}

public record WebhookEvent
{
    public required string Action { get; set; }
    public required WorkflowRun WorkflowRun { get; set; }
}

public record WorkflowRun
{
    public required string Status { get; set; }
    public required string Conclusion { get; set; }
}