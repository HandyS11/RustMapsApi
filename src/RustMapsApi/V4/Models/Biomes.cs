namespace RustMapsApi.V4.Models;

using System.Text.Json.Serialization;

/// <summary>The per-biome percentage breakdown of a map.</summary>
public sealed record Biomes
{
    /// <summary>The snow (arctic) biome percentage.</summary>
    [JsonPropertyName("s")]
    public decimal Snow { get; init; }

    /// <summary>The desert (arid) biome percentage.</summary>
    [JsonPropertyName("d")]
    public decimal Desert { get; init; }

    /// <summary>The forest (temperate) biome percentage.</summary>
    [JsonPropertyName("f")]
    public decimal Forest { get; init; }

    /// <summary>The tundra biome percentage.</summary>
    [JsonPropertyName("t")]
    public decimal Tundra { get; init; }

    /// <summary>The jungle biome percentage.</summary>
    [JsonPropertyName("j")]
    public decimal Jungle { get; init; }
}
