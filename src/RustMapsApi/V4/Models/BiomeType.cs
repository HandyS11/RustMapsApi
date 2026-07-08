namespace RustMapsApi.V4.Models;

/// <summary>A Rust biome type. Values match the RustMaps wire protocol.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design", "CA1027:Mark enums with FlagsAttribute",
    Justification = "Values mirror the wire protocol (mostly powers of two, plus the -1 Unknown fallback) and are used as single values, not bitwise flags.")]
public enum BiomeType
{
    /// <summary>An unrecognized biome type not known to this library version. Not part of the
    /// wire protocol; the tolerant-deserialization fallback for any undefined value.</summary>
    Unknown = -1,

    /// <summary>The snow (arctic) biome.</summary>
    Snow = 2,

    /// <summary>The desert (arid) biome.</summary>
    Desert = 4,

    /// <summary>The forest (temperate) biome.</summary>
    Forest = 8,

    /// <summary>The tundra biome.</summary>
    Tundra = 16,

    /// <summary>The jungle biome.</summary>
    Jungle = 32,
}
