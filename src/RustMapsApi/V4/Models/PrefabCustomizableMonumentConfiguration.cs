namespace RustMapsApi.V4.Models;

/// <summary>Configuration for a monument that supports a custom prefab.</summary>
public sealed record PrefabCustomizableMonumentConfiguration : LargeMonumentConfiguration
{
    /// <summary>The custom prefab to apply.</summary>
    public CustomPrefab? CustomPrefab { get; init; }
}
