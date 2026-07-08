using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RustMapsApi.Serialization;

/// <summary>
/// Converts an Int32-backed integer-on-the-wire enum, mapping any value not defined in
/// <typeparamref name="TEnum"/> to a supplied fallback member instead of throwing, so one
/// unrecognized value never aborts the whole payload. <see cref="Write"/> emits the numeric value.
/// </summary>
/// <typeparam name="TEnum">The Int32-backed enum type.</typeparam>
/// <remarks>Creates the converter with the fallback used for unrecognized values.</remarks>
/// <param name="fallback">The member returned for any value not defined in <typeparamref name="TEnum"/>.</param>
internal sealed class TolerantNumberEnumConverter<TEnum>(TEnum fallback) : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private readonly TEnum _fallback = fallback;

    /// <summary>
    /// Overridden so <see cref="Read"/> is invoked for JSON null too, letting a null wire value
    /// degrade to the fallback instead of the serializer throwing before the converter runs.
    /// </summary>
    public override bool HandleNull => true;

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number
            && reader.TryGetInt32(out var value)
            && Enum.IsDefined(typeof(TEnum), value))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        // Any undefined or non-numeric value degrades to the fallback rather than throwing.
        if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
        {
            reader.Skip();
        }

        return _fallback;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        => writer.WriteNumberValue(Convert.ToInt32(value, CultureInfo.InvariantCulture));
}
