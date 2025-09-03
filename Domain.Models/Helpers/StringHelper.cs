namespace Domain.Models.Helpers;

public static class StringHelper
{
    public static string CapitalizeName(this string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        name = name.Trim();
        if (name.Length == 1) return name.ToUpper();
        char? separator = name.Contains(' ') ? ' ' : name.Contains('-') ? '-' : null;
        if (separator is not null)
        {
            var parts = name.Split(separator.Value, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].CapitalizeName();
            return string.Join(separator.Value, parts);
        }
        return char.ToUpper(name[0]) + name[1..].ToLower();
    }
}
