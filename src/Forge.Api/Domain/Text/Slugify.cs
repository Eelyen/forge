using System.Text.RegularExpressions;

namespace Forge.Api.Domain.Text;

public static partial class Slugify
{
    [GeneratedRegex("-{2,}", RegexOptions.Compiled)]
    private static partial Regex MultiDashRegex();

    public static string Normalize(string value)
    {
        value = value.Trim().ToLowerInvariant()
            .Replace('_', '-')
            .Replace(' ', '-');

        // The Regex handles "--", "---", "----", etc., in one single pass.
        return MultiDashRegex().Replace(value, "-");
    }
}
