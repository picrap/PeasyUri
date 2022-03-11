
namespace PeasyUri.Utility;

internal static class CharUtility
{
    // TODO:  optimize all these (using a char-indexed flags table)

    public static bool IsReserved(this char c) => IsGenDelim(c) || IsSubDelim(c);

    public static bool IsGenDelim(this char c)
    {
        return ":/?#[]@".Contains(c);
    }
    public static bool IsSubDelim(this char c)
    {
        return "!$&'()*+,;=".Contains(c);
    }

    public static bool IsAlpha(this char c) => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".Contains(c);
    public static bool IsDigit(this char c) => "0123456789".Contains(c);
    public static bool IsUnreserved(this char c) => IsAlpha(c) || IsDigit(c) || "-._~".Contains(c);

}