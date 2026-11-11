namespace RustMapsApi.Samples.Shared;

/// <summary>Small helpers for reading typed input from the console.</summary>
public static class ConsolePrompt
{
    /// <summary>Prompts for a line of text and returns it trimmed.</summary>
    /// <param name="label">The prompt written before reading input.</param>
    /// <returns>The trimmed input, or an empty string at end of input.</returns>
    public static string ReadLine(string label)
    {
        Console.Write(label);
        return (Console.ReadLine() ?? string.Empty).Trim();
    }

    /// <summary>Prompts for an integer, returning <paramref name="fallback"/> when input is blank or invalid.</summary>
    /// <param name="label">The prompt written before reading input.</param>
    /// <param name="fallback">The value used when input is blank or not an integer.</param>
    /// <returns>The parsed integer, or <paramref name="fallback"/>.</returns>
    public static int ReadInt(string label, int fallback)
    {
        var raw = ReadLine($"{label} [{fallback}]: ");
        return int.TryParse(raw, out var value) ? value : fallback;
    }

    /// <summary>Prints a warning and returns true only if the user types "yes".</summary>
    /// <param name="warning">The warning shown before asking for confirmation.</param>
    /// <returns><c>true</c> when the user typed "yes" (case-insensitive).</returns>
    public static bool Confirm(string warning)
    {
        Console.WriteLine(warning);
        var answer = ReadLine("Type 'yes' to continue: ");
        return string.Equals(answer, "yes", StringComparison.OrdinalIgnoreCase);
    }
}
