using System.Text;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.V4;

/// <summary>Builds query strings for search/filter calls, skipping null values.</summary>
internal static class QueryStringBuilder
{
    public static string Build(int page, SearchOptions? options)
    {
        var builder = new StringBuilder("?page=").Append(page);
        if (options is null)
        {
            return builder.ToString();
        }

        Append(builder, "staging", options.Staging);
        Append(builder, "customMaps", options.CustomMaps);
        Append(builder, "includeAllProtocols", options.IncludeAllProtocols);
        Append(builder, "ignoreVisitedMaps", options.IgnoreVisitedMaps);
        if (options.SortBy is { Length: > 0 } sortBy)
        {
            builder.Append("&sortBy=").Append(Uri.EscapeDataString(sortBy));
        }

        return builder.ToString();
    }

    private static void Append(StringBuilder builder, string key, bool? value)
    {
        if (value.HasValue)
        {
            builder.Append('&').Append(key).Append('=').Append(value.Value ? "true" : "false");
        }
    }
}
