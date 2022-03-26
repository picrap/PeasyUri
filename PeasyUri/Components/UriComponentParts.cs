using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PeasyUri.Components;

public record UriComponentParts
{
    public string? Scheme { get; }
    public EncodedString HierPart { get; }
    public EncodedString? Authority { get; }
    public EncodedString? UserInfo { get; }
    public EncodedString? Host { get; }
    public string? DecodedHost { get; }
    public int? Port { get; }
    public EncodedString Path { get; }
    public ICollection<string> Segments { get; }
    public EncodedString? Query { get; }
    public EncodedString? Fragment { get; }

    public UriComponentParts(string? scheme, EncodedString hierPart, EncodedString? authority, 
        EncodedString? userInfo, EncodedString? host, string? decodedHost, int? port, 
        EncodedString path, IEnumerable<string> segments, EncodedString? query, EncodedString? fragment)
    {
        Scheme = scheme;
        HierPart = hierPart;
        Authority = authority;
        UserInfo = userInfo;
        Host = host;
        DecodedHost = decodedHost;
        Port = port;
        Path = path;
        Segments = new ReadOnlyCollection<string>(segments.ToArray());
        Query = query;
        Fragment = fragment;
    }
}
