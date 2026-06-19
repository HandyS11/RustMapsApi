namespace RustMapsApi.Samples.Shared;

/// <summary>Small helpers for reading typed input from the console.</summary>
public static class ConsolePrompt
{
    /// <summary>Prompts for a line of text and returns it trimmed.</summary>
    public static string ReadLine(string label)
    {
        Console.Write(label);
        return (Console.ReadLine() ?? string.Empty).Trim();
    }

    /// <summary>Prompts for an integer, returning <paramref name="fallback"/> when input is blank or invalid.</summary>
    public static int ReadInt(string label, int fallback)
    {
        var raw = ReadLine($"{label} [{fallback}]: ");
        return int.TryParse(raw, out var value) ? value : fallback;
    }

    /// <summary>Prints a warning and returns true only if the user types "yes".</summary>
    public static bool Confirm(string warning)
    {
        Console.WriteLine(warning);
        var answer = ReadLine("Type 'yes' to continue: ");
        return string.Equals(answer, "yes", StringComparison.OrdinalIgnoreCase);
    }
}
