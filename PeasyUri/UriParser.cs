﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PeasyUri.Components;
using PeasyUri.Utility;

namespace PeasyUri;

public class UriParser
{
    protected static IdnMapping IdnMapping { get; } = new IdnMapping();

    protected class AuthorityAndPathPart
    {
        public EncodedString? Authority { get; }
        public EncodedString Path { get; }

        public AuthorityAndPathPart(EncodedString? authority, EncodedString path)
        {
            Authority = authority;
            Path = path;
        }
    }

    protected class AuthorityPart
    {
        public EncodedString? UserInfo { get; }
        public EncodedString Host { get; }
        public int? Port { get; }

        public AuthorityPart(EncodedString? userInfo, EncodedString host, int? port)
        {
            UserInfo = userInfo;
            Host = host;
            Port = port;
        }
    }

    public UriComponentParts Parse(string literal)
    {
        return Parse(EncodedString.FromEncoded(literal));
    }

    public UriComponentParts Parse(EncodedString? literal)
    {
        if (literal is null)
            throw new ArgumentNullException(nameof(literal));

        var remainingPartStart = 0;
        var remainingPartEnd = literal.Length; // last character + 1

        var scheme = ExtractScheme(literal, ref remainingPartStart)?.Decode();
        var fragment = ExtractFragment(literal, remainingPartStart, ref remainingPartEnd);
        var query = ExtractQuery(literal, remainingPartStart, ref remainingPartEnd);
        var hierPart = ExtractHierPart(literal, remainingPartStart, remainingPartEnd);
        var authorityAndPath = ParseHierPart(hierPart);
        var segments = ParsePath(authorityAndPath.Path);
        var authority = ParseAuthority(authorityAndPath.Authority);
        var decodedHost = DecodeHost(authority?.Host);
        return new UriComponentParts(scheme, hierPart, authorityAndPath.Authority, authority?.UserInfo, authority?.Host, decodedHost, authority?.Port,
            authorityAndPath.Path, segments, query, fragment);
    }

    protected virtual string? DecodeHost(EncodedString? host)
    {
        if (host is null)
            return null;
        return IdnMapping.GetUnicode(host.Decode());
    }

    protected virtual IEnumerable<string> ParsePath(EncodedString path)
    {
        return path.Split('/').Select(s => s.Decode());
    }

    protected virtual AuthorityAndPathPart ParseHierPart(EncodedString hierPart)
    {
        if (!hierPart.StartsWith("//"))
            return new(null, hierPart);
        var remainingPartStart = 2;
        var remainingPartEnd = hierPart.Length;
        var path = ExtractEndPart(hierPart, '/', remainingPartStart, ref remainingPartEnd, keepDelimiter: true)
                   ?? EncodedString.Empty; // path can be empty, but not null
        var authority = ExtractMiddlePart(hierPart, remainingPartStart, remainingPartEnd);
        return new(authority, path);
    }

    protected virtual AuthorityPart? ParseAuthority(EncodedString? authority)
    {
        if (authority is null)
            return null;
        var remainingPartStart = 0;
        var remainingPartEnd = authority.Length;
        var userInfo = ExtractBeginPart(authority, '@', ref remainingPartStart);
        var literalPort = ExtractEndPart(authority, ':', remainingPartStart, ref remainingPartEnd, (c, _) => c.IsDigit());
        var port = literalPort is null ? (int?)null : int.Parse(literalPort.Decode(), CultureInfo.InvariantCulture);
        var host = ExtractMiddlePart(authority, remainingPartStart, remainingPartEnd);
        return new(userInfo, host, port);
    }

    protected virtual EncodedString? ExtractScheme(EncodedString literal, ref int remainingPartStart) => ExtractBeginPart(literal, ':', ref remainingPartStart, IsScheme);

    protected virtual EncodedString ExtractHierPart(EncodedString literal, int remainingPartStart, int remainingPartEnd) => ExtractMiddlePart(literal, remainingPartStart, remainingPartEnd);

    protected virtual EncodedString? ExtractQuery(EncodedString literal, int remainingPartStart, ref int remainingPartEnd) => ExtractEndPart(literal, '?', remainingPartStart, ref remainingPartEnd);

    protected virtual EncodedString? ExtractFragment(EncodedString literal, int remainingPartStart, ref int remainingPartEnd) => ExtractEndPart(literal, '#', remainingPartStart, ref remainingPartEnd);

    protected virtual EncodedString? ExtractBeginPart(EncodedString literal, char delimiter, ref int remainingPartStart, IsValidCharacterDelegate? isValidCharacter = null)
    {
        var index = literal.IndexOf(delimiter, isValidCharacter, remainingPartStart);
        if (!index.HasValue)
            return null;
        var part = literal.Substring(remainingPartStart, index.Value - remainingPartStart);
        remainingPartStart = index.Value + 1;
        return part;
    }

    protected virtual EncodedString ExtractMiddlePart(EncodedString literal, int remainingPartStart, int remainingPartEnd)
    {
        return literal.Substring(remainingPartStart, remainingPartEnd - remainingPartStart);
    }

    protected virtual EncodedString? ExtractEndPart(EncodedString literal, char delimiter, int remainingPartStart, ref int remainingPartEnd,
        IsValidCharacterDelegate? isValidCharacter = null, bool keepDelimiter = false)
    {
        var index = literal.IndexOf(delimiter, isValidCharacter, remainingPartStart, remainingPartEnd - remainingPartStart);
        if (!index.HasValue)
            return null;
        var offset = keepDelimiter ? 0 : 1;
        var part = literal.Substring(index.Value + offset, remainingPartEnd - index.Value - offset);
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
