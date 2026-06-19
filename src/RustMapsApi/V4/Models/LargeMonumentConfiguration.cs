namespace RustMapsApi.V4.Models;

/// <summary>Configuration for a large monument, adding a desired flag.</summary>
public record LargeMonumentConfiguration : BasicMonumentConfiguration
{
    /// <summary>Whether the monument is explicitly desired.</summary>
    public bool Desired { get; init; }
}
