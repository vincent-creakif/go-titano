namespace Creakif.GoTitano.Extensions;

public static class StringExtensions
{
    public static bool IsWalletAddress(this string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return false;
        }
        return Regex.IsMatch(source, "^(0x)?[0-9a-f]{40}$", RegexOptions.IgnoreCase);
    }

    public static string ShortWalletAddress(this string source)
    {
        return ShortVersion(source, 6, 4);
    }

    public static string ShortVersion(this string source, int leftCount, int rightCount)
    {
        return source[..leftCount] + "..." + source[^rightCount..];
    }
}