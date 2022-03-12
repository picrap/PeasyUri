namespace PeasyUri.Components;

public record UriComponentParts(string? Scheme, string HierPart, string? Authority, string Path, string? Query, string? Fragment) // not full record, it fails with .NET4
{
    public string? Scheme { get; } = Scheme;
    public string HierPart { get; } = HierPart;
    public string? Authority { get; } = Authority;
    public string Path { get; } = Path;
    public string? Query { get; } = Query;
    public string? Fragment { get; } = Fragment;
}