using System;
using PeasyUri.Components;
using PeasyUri.Utility;

namespace PeasyUri;

public class UriParser
{
    public UriComponentParts ParseUri(string literal)
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

    public UriComponentAuthorityAndPath ParseHierPart(string hierPart)
    {
        if (!hierPart.StartsWith("//"))
            return new(null, hierPart);
        var hierPartStart = 2;
        var authority = ExtractBeginPart(hierPart, '/', ref hierPartStart);
        var path = ExtractMiddlePart(hierPart, hierPartStart - 1, hierPart.Length);
        return new(authority, path);
    }

    public UriComponentSubParts Parse(string literal)
    {
        var componentParts = ParseUri(literal);
        var authorityAndPath = ParseHierPart(componentParts.HierPart);
        return new(componentParts, authorityAndPath);
    }

    protected virtual string? ExtractScheme(string literal, ref int hierPartStart) => ExtractBeginPart(literal, ':', ref hierPartStart, IsScheme);

    protected virtual string ExtractHierPart(string literal, int hierPartStart, int hierPartEnd) => ExtractMiddlePart(literal, hierPartStart, hierPartEnd);

    protected virtual string? ExtractQuery(string literal, int hierPartStart, ref int hierPartEnd) => ExtractEndPart(literal, '?', hierPartStart, ref hierPartEnd);

    protected virtual string? ExtractFragment(string literal, int hierPartStart, ref int hierPartEnd) => ExtractEndPart(literal, '#', hierPartStart, ref hierPartEnd);

    protected virtual string? ExtractBeginPart(string literal, char delimiter, ref int hierPartStart, IsValidCharacterDelegate? isValidCharacter = null)
    {
        var index = literal.IndexOf(delimiter, isValidCharacter, hierPartStart);
        if (!index.HasValue)
            return null;
        var part = literal.Substring(hierPartStart, index.Value - hierPartStart);
        hierPartStart = index.Value + 1;
        return part;
    }

    protected virtual string ExtractMiddlePart(string literal, int hierPartStart, int hierPartEnd)
    {
        return literal.Substring(hierPartStart, hierPartEnd - hierPartStart);
    }

    protected virtual string? ExtractEndPart(string literal, char delimiter, int hierPartStart, ref int hierPartEnd, IsValidCharacterDelegate? isValidCharacter = null)
    {
        var index = literal.IndexOf(delimiter, isValidCharacter, hierPartStart, hierPartEnd - hierPartStart);
        if (!index.HasValue)
            return null;
        var part = literal.Substring(index.Value + 1, hierPartEnd - index.Value - 1);
        hierPartEnd = index.Value;
        return part;
    }

    protected virtual bool IsScheme(char c, int index)
    {
        return index switch
        {
            0 => c.IsAlpha(),
            _ => c.IsAlpha() || c.IsDigit() || "+-.".Contains(c)
        };
    }
}
