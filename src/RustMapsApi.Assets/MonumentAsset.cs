using System.Text;
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>A monument icon asset (SVG) resolved from a <see cref="MonumentType"/>.</summary>
public sealed class MonumentAsset
{
    private const string ResourcePrefix = "RustMapsApi.Assets.Assets.";
    private const string CdnBase = "https://content.rustmaps.com/assets/";

    /// <summary>Creates an asset descriptor. Only the internal map constructs these.</summary>
    /// <param name="monumentType">The monument type this asset was resolved for.</param>
    /// <param name="assetName">The CDN base name of the asset (no extension).</param>
    internal MonumentAsset(MonumentType monumentType, string assetName)
    {
        MonumentType = monumentType;
        AssetName = assetName;
    }

    /// <summary>The monument type this asset was resolved for.</summary>
    public MonumentType MonumentType { get; }

    /// <summary>The RustMaps CDN base name of the asset, e.g. <c>"Nuclear_Missile_Silo"</c>.</summary>
    public string AssetName { get; }

    /// <summary>The asset file name, e.g. <c>"Nuclear_Missile_Silo.svg"</c>.</summary>
    public string FileName => AssetName + ".svg";

    /// <summary>The MIME media type of the asset — always <c>"image/svg+xml"</c>.</summary>
    public string MediaType { get; } = "image/svg+xml";

    /// <summary>The canonical RustMaps CDN URL this asset was sourced from.</summary>
    public Uri SourceUri => new(CdnBase + FileName);

    /// <summary>Opens a new read stream over the embedded SVG. The caller owns and disposes it.</summary>
    /// <returns>A readable stream of the SVG bytes.</returns>
    /// <exception cref="InvalidOperationException">The embedded monument asset resource was not found.</exception>
    public Stream OpenStream()
    {
        var stream = typeof(MonumentAsset).Assembly
            .GetManifestResourceStream(ResourcePrefix + FileName);
        return stream ?? throw new InvalidOperationException(
            $"Embedded monument asset '{FileName}' was not found.");
    }

    /// <summary>Reads the full SVG as a byte array.</summary>
    /// <returns>The SVG bytes.</returns>
    public byte[] GetBytes()
    {
        using var stream = OpenStream();
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    /// <summary>Reads the full SVG markup as a UTF-8 string.</summary>
    /// <returns>The SVG markup.</returns>
    public string GetSvg()
    {
        using var stream = OpenStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
