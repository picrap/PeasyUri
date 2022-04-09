using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace PeasyUri.Components;

public record UriComponentParts
{
    public string? Scheme { get; }
    public EncodedString HierPart { get; }
    public EncodedString? Authority { get; }
    public EncodedString? UserInfo { get; }
    public NetworkCredential? DecodedUserInfo { get; }
    public EncodedString? Host { get; }
    public string? IdnHost { get; }
    public string? DnsSafeHost { get; }
    public int? Port { get; }
    public EncodedString Path { get; }
    public ICollection<string> Segments { get; }
    public EncodedString? Query { get; }
    public EncodedString? Fragment { get; }

    public UriComponentParts(string? scheme, EncodedString hierPart, EncodedString? authority,
        EncodedString? userInfo, NetworkCredential? decodedUserInfo, EncodedString? host, string? idnHost,
        string? dnsSafeHost, int? port, EncodedString path, IEnumerable<string> segments, EncodedString? query, EncodedString? fragment)
    {
        Scheme = scheme;
        HierPart = hierPart;
        Authority = authority;
        UserInfo = userInfo;
        DecodedUserInfo = decodedUserInfo;
        Host = host;
        IdnHost = idnHost;
        DnsSafeHost = dnsSafeHost;
        Port = port;
        Path = path;
        Segments = new ReadOnlyCollection<string>(segments.ToArray());
        Query = query;
        Fragment = fragment;
    }
}
