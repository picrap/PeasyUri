using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using PeasyUri.Components;

namespace PeasyUri;

public class Uri
{
    public EncodedString AbsolutePath { get; }

    public NetworkCredential? DecodedUserInfo { get; }

    public EncodedString? UserInfo { get; }

    public ICollection<string> Segments { get; }

    public string? Scheme { get; }

    public int? Port { get; }

    public EncodedString OriginalString { get; }

    public string LocalPath { get; }

    public string? IdnHost { get; }

    public string? DnsSafeHost { get; }

    public EncodedString? Authority { get; }

    public EncodedString? Query { get; }

    public EncodedString? Fragment { get; }

    public static bool TryParse(string? rawString, out Uri? uri) => TryParse(EncodedString.FromEncoded(rawString), out uri);

    /// <summary>
    /// Tries to parse the given string
    /// </summary>
    /// <param name="encodedString"></param>
    /// <param name="uri">filled if parsing succeeds, null otherwise</param>
    /// <returns>true if parsing was successful</returns>
    public static bool TryParse(EncodedString? encodedString, out Uri? uri)
    {
        if (encodedString is null)
        {
            uri = null;
            return false;
        }

        var uriComponents = UriParser.Default.TryParse(encodedString);
        if (uriComponents is null)
        {
            uri = null;
            return false;
        }

        uri = new(encodedString, uriComponents);
        return true;
    }

    public Uri(EncodedString encodedString)
    : this(encodedString, UriParser.Default.TryParse(encodedString) ?? throw new UriFormatException($"Invalid Uri format: {encodedString.ToEncodedString()}"))
    {
    }

    public Uri(string encodedString)
    : this(EncodedString.FromEncoded(encodedString)!, UriParser.Default.TryParse(encodedString) ?? throw new UriFormatException($"Invalid Uri format: {encodedString}"))
    {
    }

    private Uri(EncodedString encodedString, UriComponentParts parts)
    {
        OriginalString = encodedString;
        AbsolutePath = parts.Path;
        Authority = parts.Authority;
        DnsSafeHost = parts.DnsSafeHost;
        IdnHost = parts.IdnHost;
        LocalPath = parts.Path.Decode();
        Port = parts.Port;
        Scheme = parts.Scheme;
        Segments = new ReadOnlyCollection<string>(parts.Segments.ToList());
        UserInfo = parts.UserInfo;
        DecodedUserInfo = parts.DecodedUserInfo;
        Query = parts.Query;
        Fragment = parts.Fragment;
    }

    public EncodedString Encode()
    {
        var encodedString = EncodedString.Empty;
        if (Scheme is not null)
            encodedString += Scheme + ":";
        // TODO: use IdnHost and cracked authority
        if (Authority is not null)
            encodedString += "//" + Authority;
        encodedString += AbsolutePath;
        if (Query is not null)
            encodedString += "?" + Query;
        if (Fragment is not null)
            encodedString += "#" + Fragment;
        return encodedString;
    }
}
