namespace PeasyUri.Components;

public record UriComponentAuthorityAndPath(string? Authority, string Path)
{
    public string? Authority { get; } = Authority;
    public string Path { get; } = Path;
}