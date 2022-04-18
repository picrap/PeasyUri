using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;

namespace PeasyUri;

public class Uri
{
    public EncodedString AbsolutePath { get; }

    public NetworkCredential? DecodedUserInfo { get; }

    public EncodedString? UserInfo { get; }

    public ICollection<string> Segments { get; }

    public string? Scheme { get; }

    public EncodedString HierPart { get; }

    public int? Port { get; }

    public EncodedString OriginalString { get; }

    public string LocalPath { get; }

    public string? IdnHost { get; }

    public string? DnsSafeHost { get; }

    public EncodedString? Authority { get; }

    public EncodedString? Query { get; }

    public EncodedString? Fragment { get; }

    public IDictionary<string, string?>? QueryValues { get; }

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

        uri = UriParser.Default.TryParse(encodedString);
        return uri is not null;
    }

    public Uri(EncodedString encodedString)
    : this(UriParser.Default.TryParse(encodedString) ?? throw new UriFormatException($"Invalid Uri format: {encodedString.ToEncodedString()}"))
    {
    }

    public Uri(string encodedString)
    : this(UriParser.Default.TryParse(encodedString) ?? throw new UriFormatException($"Invalid Uri format: {encodedString}"))
    {
    }

    private Uri(Uri uri)
    : this(uri.OriginalString, uri.Scheme, uri.HierPart, uri.Authority, uri.UserInfo, uri.DecodedUserInfo, uri.IdnHost, uri.DnsSafeHost, uri.Port, uri.AbsolutePath, uri.Segments, uri.Query, uri.Fragment)
    {
    }

    public Uri(EncodedString encodedString, string? scheme, EncodedString hierPart, EncodedString? authority,
        EncodedString? userInfo, NetworkCredential? decodedUserInfo, string? idnHost,
        string? dnsSafeHost, int? port, EncodedString path, IEnumerable<string> segments, EncodedString? query, EncodedString? fragment)
    {
        OriginalString = encodedString;
        AbsolutePath = path;
        Authority = authority;
        DnsSafeHost = dnsSafeHost;
        IdnHost = idnHost;
        LocalPath = path.Decode();
        Port = port;
        Scheme = scheme;
        HierPart = hierPart;
        Segments = new ReadOnlyCollection<string>(segments.ToList());
        UserInfo = userInfo;
        DecodedUserInfo = decodedUserInfo;
        Query = query;
        Fragment = fragment;
        if (query is not null)
            QueryValues = ParseQuery(query).ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private IEnumerable<KeyValuePair<string, string?>> ParseQuery(EncodedString query)
    {
        var keyValues = query.Split('&');
        foreach (var keyValue in keyValues)
        {
            var keyIndex = keyValue.IndexOf('=');
            if (keyIndex.HasValue)
            {
                var key = keyValue.Substring(0, keyIndex.Value);
                var value = keyValue.Substring(keyIndex.Value + 1);
                yield return new(key.Decode(), value.Decode());
            }
            else
            {
                yield return new(keyValue.Decode(), null);
            }
        }
    }

    public EncodedString Encode()
    {
        var encodedString = EncodedString.Empty;
        if (Scheme is not null)
            encodedString += Scheme + ":";
        // TODO: use IdnHost and cracked authority
        //if (IdnHost is not null)
        //    encodedString += "//"+ IdnHost;
        if (Authority is not null)
            encodedString += "//" + Authority;
        encodedString += AbsolutePath;
        if (Query is not null)
            encodedString += "?" + Query;
        if (Fragment is not null)
            encodedString += "#" + Fragment;
        return encodedString;
    }

    public static Uri? TryRead(TextReader textReader)
    {
        var encodedString = EncodedString.ReadLine(textReader);
        if (encodedString is null)
            return null;
        TryParse(encodedString, out var uri);
        return uri;
    }

    public void Write(TextWriter textWriter)
    {
        Encode().WriteLine(textWriter);
    }
}
