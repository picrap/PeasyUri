using System;
using PeasyUri.Components;
using PeasyUri.Utility;

namespace PeasyUri;

public class UriParser
{
    protected class AuthorityAndPath
    {
        public string? Authority { get; }
        public string Path { get; }

        public AuthorityAndPath(string? authority, string path)
        {
            Authority = authority;
            Path = path;
        }
    }

    public UriComponentParts Parse(string literal)
    {
        if (literal is null)
            throw new ArgumentNullException(nameof(literal));

        var remainingPartStart = 0;
        var remainingPartEnd = literal.Length; // last character + 1

        var scheme = ExtractScheme(literal, ref remainingPartStart);
        var fragment = ExtractFragment(literal, remainingPartStart, ref remainingPartEnd);
        var query = ExtractQuery(literal, remainingPartStart, ref remainingPartEnd);
        var hierPart = ExtractHierPart(literal, remainingPartStart, remainingPartEnd);
        var authorityAndPath = ParseHierPart(hierPart);

        return new UriComponentParts(scheme, hierPart, authorityAndPath.Authority, authorityAndPath.Path, query, fragment);
    }

    protected virtual AuthorityAndPath ParseHierPart(string hierPart)
    {
        if (!hierPart.StartsWith("//"))
            return new(null, hierPart);
        var remainingPartStart = 2;
        var authority = ExtractBeginPart(hierPart, '/', ref remainingPartStart);
        var path = ExtractMiddlePart(hierPart, remainingPartStart - 1, hierPart.Length);
        return new(authority, path);
    }
    
    protected virtual string? ExtractScheme(string literal, ref int remainingPartStart) => ExtractBeginPart(literal, ':', ref remainingPartStart, IsScheme);

    protected virtual string ExtractHierPart(string literal, int remainingPartStart, int remainingPartEnd) => ExtractMiddlePart(literal, remainingPartStart, remainingPartEnd);

    protected virtual string? ExtractQuery(string literal, int remainingPartStart, ref int remainingPartEnd) => ExtractEndPart(literal, '?', remainingPartStart, ref remainingPartEnd);

    protected virtual string? ExtractFragment(string literal, int remainingPartStart, ref int remainingPartEnd) => ExtractEndPart(literal, '#', remainingPartStart, ref remainingPartEnd);

    protected virtual string? ExtractBeginPart(string literal, char delimiter, ref int remainingPartStart, IsValidCharacterDelegate? isValidCharacter = null)
    {
        var index = literal.IndexOf(delimiter, isValidCharacter, remainingPartStart);
        if (!index.HasValue)
            return null;
        var part = literal.Substring(remainingPartStart, index.Value - remainingPartStart);
        remainingPartStart = index.Value + 1;
        return part;
    }

    protected virtual string ExtractMiddlePart(string literal, int remainingPartStart, int remainingPartEnd)
    {
        return literal.Substring(remainingPartStart, remainingPartEnd - remainingPartStart);
    }

    protected virtual string? ExtractEndPart(string literal, char delimiter, int remainingPartStart, ref int remainingPartEnd, IsValidCharacterDelegate? isValidCharacter = null)
    {
        var index = literal.IndexOf(delimiter, isValidCharacter, remainingPartStart, remainingPartEnd - remainingPartStart);
        if (!index.HasValue)
            return null;
        var part = literal.Substring(index.Value + 1, remainingPartEnd - index.Value - 1);
        remainingPartEnd = index.Value;
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
