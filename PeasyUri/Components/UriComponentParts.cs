using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PeasyUri.Components;

public record UriComponentParts
{
    public string? Scheme { get; }
    public EncodedString HierPart { get; }
    public EncodedString? Authority { get; }
    public string? DecodedAuthority { get; }
    public EncodedString Path { get; }
    public ICollection<string> Segments { get; }
    public EncodedString? Query { get; }
    public EncodedString? Fragment { get; }

    public UriComponentParts(string? scheme, EncodedString hierPart, EncodedString? authority, string? decodedAuthority, EncodedString path, IEnumerable<string> segments,
        EncodedString? query, EncodedString? fragment)
    {
        Scheme = scheme;
        HierPart = hierPart;
        Authority = authority;
        DecodedAuthority = decodedAuthority;
        Path = path;
        Segments = new ReadOnlyCollection<string>(segments.ToArray());
        Query = query;
        Fragment = fragment;
    }
}
