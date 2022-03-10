using System;

namespace PeasyUri.Utility;

internal static class StringUtility
{
    public static bool Contains(this string s, char c) => s.IndexOf(c) >= 0;

    public static int? IndexOf(this string s, char c, Func<char, int, bool>? validCharacter, int first = 0, int? count = null)
    {
        var actualCount = count ?? s.Length - first;
        for (int i = 0, j = first; i < actualCount; i++, j++)
        {
            var sc = s[j];
            if (sc == c)
                return j;
            if (validCharacter is not null && !validCharacter(sc, i))
                return null;
        }
        return null;
    }
}