using System;

namespace PeasyUri;

public class UriParser
{
    public UriComponentParts Parse(string literal)
    {
        if (literal is null)
            throw new ArgumentNullException(nameof(literal));

        var parts = new UriComponentParts();

        var hierPartStart = 0;
        // last character + 1
        int hierPartEnd = literal.Length;

        ExtractScheme(literal, parts, ref hierPartStart);
        ExtractFragment(literal, parts, hierPartStart, ref hierPartEnd);
        ExtractQuery(literal, parts, hierPartStart, ref hierPartEnd);
        ExtractHierPart(literal, parts, hierPartStart, hierPartEnd);

        return parts;
    }

    private static void ExtractHierPart(string literal, UriComponentParts parts, int hierPartStart, int hierPartEnd)
    {
        parts.HierPart = literal.Substring(hierPartStart, hierPartEnd - hierPartStart);
    }

    private static void ExtractQuery(string literal, UriComponentParts parts, int hierPartStart, ref int hierPartEnd)
    {
        var queryIndex = literal.IndexOf('?', hierPartStart, hierPartEnd - hierPartStart);
        if (queryIndex > 0)
        {
            parts.Query = literal.Substring(queryIndex + 1, hierPartEnd - queryIndex - 1);
            hierPartEnd = queryIndex;
        }
    }

    private static void ExtractFragment(string literal, UriComponentParts parts, int hierPartStart, ref int hierPartEnd)
    {
        var fragmentIndex = literal.IndexOf('#', hierPartStart);
        if (fragmentIndex > 0)
        {
            parts.Fragment = literal.Substring(fragmentIndex + 1);
            hierPartEnd = fragmentIndex;
        }
    }

    private static void ExtractScheme(string literal, UriComponentParts parts, ref int hierPartStart)
    {
        var schemeIndex = literal.IndexOf(':');
        if (schemeIndex > 0)
        {
            parts.Scheme = literal.Substring(0, schemeIndex);
            hierPartStart = schemeIndex + 1;
        }
    }
}
