namespace RustMapsApi.Serialization;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>Builds the shared <see cref="JsonSerializerOptions"/> for RustMaps payloads.</summary>
internal static class RustMapsJsonOptions
{
    /// <summary>Creates options bound to a version-specific source-gen context.</summary>
    /// <param name="context">The source-gen context that resolves serializable types.</param>
    /// <returns>Options configured for the RustMaps wire format.</returns>
    public static JsonSerializerOptions Create(JsonSerializerContext context)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = context,
        };
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }
}
