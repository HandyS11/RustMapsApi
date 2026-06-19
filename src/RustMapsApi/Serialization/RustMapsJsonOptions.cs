using System.Text.Json;
using System.Text.Json.Serialization;

namespace RustMapsApi.Serialization;

/// <summary>Builds the shared <see cref="JsonSerializerOptions"/> for RustMaps payloads.</summary>
internal static class RustMapsJsonOptions
{
    /// <summary>Creates options bound to a version-specific source-gen context.</summary>
    /// <param name="context">The source-gen context that resolves serializable types.</param>
    /// <param name="integerEnumConverters">
    /// Version-specific converters for integer-on-the-wire enums. They are registered
    /// before the catch-all camelCase string-enum converter so the more-specific
    /// converter wins for those enums; every other enum stays a camelCase string.
    /// </param>
    /// <returns>Options configured for the RustMaps wire format.</returns>
    public static JsonSerializerOptions Create(
        JsonSerializerContext context,
        params JsonConverter[] integerEnumConverters)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = context,
        };

        foreach (var converter in integerEnumConverters)
        {
            options.Converters.Add(converter);
        }

        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }
}
