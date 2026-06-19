namespace RustMapsApi.V4.Models;

/// <summary>Webhook notification settings for a custom map.</summary>
public sealed record WebhookSettings
{
    /// <summary>Whether the webhook is enabled.</summary>
    public bool Enabled { get; init; }

    /// <summary>The webhook URL to notify.</summary>
    public string? Url { get; init; }
}
