using System;

namespace PeasyUri.Utility;

public delegate bool IsValidCharacterDelegate(char c, int index);

internal static class StringUtility
{
    public static bool Contains(this string s, char c) => s.IndexOf(c) >= 0;

    public static int? IndexOf(this string s, char c, int first = 0, int? count = null)
    {
        var actualCount = count ?? s.Length - first;
        for (int i = 0, j = first; i < actualCount; i++, j++)
        {
            var sc = s[j];
            if (sc == c)
                return j;
        }
        return null;
    }

    public static int? LastIndexOf(this string s, char c, int first = 0, int? count = null)
    {
        var actualCount = count ?? s.Length - first;
        for (int i = first + actualCount - 1; i >= 0; i--)
        {
            var sc = s[i];
            if (sc == c)
                return i;
        }
        return null;
    }
}
