namespace PeasyUri.Components;

// not full record, it fails with .NET4
public record UriComponentParts(string? Scheme, EncodedString HierPart, string? Authority, EncodedString Path, EncodedString? Query, EncodedString? Fragment) 
{
    public string? Scheme { get; } = Scheme;
    public EncodedString HierPart { get; } = HierPart;
    public string? Authority { get; } = Authority;
    public EncodedString Path { get; } = Path;
    public EncodedString? Query { get; } = Query;
    public EncodedString? Fragment { get; } = Fragment;
}