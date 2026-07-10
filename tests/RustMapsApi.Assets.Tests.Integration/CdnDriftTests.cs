using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RustMapsApi.V4.Assets;
using Xunit;

namespace RustMapsApi.Assets.Tests.Integration;

public sealed class CdnDriftTests
{
    private const string Ua =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
        "(KHTML, like Gecko) Chrome/120.0 Safari/537.36";

    private static bool LiveEnabled =>
        Environment.GetEnvironmentVariable("RUSTMAPS_ASSETS_LIVE") == "1";

    public static IEnumerable<object[]> DistinctSourceUris()
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var type in MonumentAssets.AvailableTypes)
        {
            MonumentAssets.TryGetAsset(type, out var asset);
            if (seen.Add(asset!.AssetName))
            {
                yield return new object[] { asset.SourceUri };
            }
        }
    }

    [SkippableTheory]
    [MemberData(nameof(DistinctSourceUris))]
    public async Task SourceUri_StillReturns200(Uri uri)
    {
        Skip.IfNot(LiveEnabled, "Set RUSTMAPS_ASSETS_LIVE=1 to run live CDN drift checks.");

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("User-Agent", Ua);
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);
        using var response = await http.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
