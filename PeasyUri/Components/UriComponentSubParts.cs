namespace PeasyUri.Components;

public record UriComponentSubParts : UriComponentParts
{
    public UriComponentAuthorityAndPath AuthorityAndPath { get; }
    public string? Authority => AuthorityAndPath.Authority;
    public string Path => AuthorityAndPath.Path;

    public UriComponentSubParts(string? scheme, string hierPart, string? query, string? fragment, string? authority, string path)
        : base(scheme, hierPart, query, fragment)
    {
        AuthorityAndPath = new(authority, path);
    }

    public UriComponentSubParts(UriComponentParts componentParts, UriComponentAuthorityAndPath authorityAndPath)
        : base(componentParts)
    {
        AuthorityAndPath = authorityAndPath;
    }

    public void Deconstruct(out string? scheme, out string hierPart, out string? query, out string? fragment, out string? authority, out string path)
    {
        scheme = Scheme;
        hierPart = HierPart;
        query = Query;
        fragment = Fragment;
        authority = Authority;
        path = Path;
    }
}