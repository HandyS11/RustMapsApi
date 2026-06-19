namespace RustMapsApi.V4.Requests;

/// <summary>A numeric range filter.</summary>
/// <param name="Min">The inclusive minimum.</param>
/// <param name="Max">The inclusive maximum.</param>
public sealed record MinMaxFilter(int Min, int Max);
