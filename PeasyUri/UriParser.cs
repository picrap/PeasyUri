using System;
using PeasyUri.Utility;

namespace PeasyUri;

public class UriParser
{
    public UriComponentParts Parse(string literal)
    {
        if (literal is null)
            throw new ArgumentNullException(nameof(literal));

        var hierPartStart = 0;
        // last character + 1
        int hierPartEnd = literal.Length;

        var scheme = ExtractScheme(literal, ref hierPartStart);
        var fragment = ExtractFragment(literal, hierPartStart, ref hierPartEnd);
        var query = ExtractQuery(literal, hierPartStart, ref hierPartEnd);
        var hierPart = ExtractHierPart(literal, hierPartStart, hierPartEnd);

        return new UriComponentParts(scheme, hierPart, query, fragment);
    }

    private static string? ExtractScheme(string literal, ref int hierPartStart)
    {
        var schemeIndex = literal.IndexOf(':', CharUtility.IsScheme, hierPartStart);
        if (!schemeIndex.HasValue)
            return null;
        var scheme = literal.Substring(hierPartStart, schemeIndex.Value);
        hierPartStart = schemeIndex.Value + 1;
        return scheme;
    }

    private static string ExtractHierPart(string literal, int hierPartStart, int hierPartEnd)
    {
        return literal.Substring(hierPartStart, hierPartEnd - hierPartStart);
    }

    private static string? ExtractQuery(string literal, int hierPartStart, ref int hierPartEnd)
    {
        var queryIndex = literal.IndexOf('?', hierPartStart, hierPartEnd - hierPartStart);
        if (queryIndex <= 0)
            return null;
        var query = literal.Substring(queryIndex + 1, hierPartEnd - queryIndex - 1);
        hierPartEnd = queryIndex;
        return query;
    }

    private static string? ExtractFragment(string literal, int hierPartStart, ref int hierPartEnd)
    {
        var fragmentIndex = literal.IndexOf('#', hierPartStart);
        if (fragmentIndex <= 0)
            return null;
        var fragment = literal.Substring(fragmentIndex + 1);
        hierPartEnd = fragmentIndex;
        return fragment;
    }
}
