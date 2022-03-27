using System;
using PeasyUri.Components;

namespace PeasyUri;

public class Uri
{
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

        uri = new(uriComponents);
        return true;
    }

    public Uri(EncodedString encodedString)
    : this(UriParser.Default.TryParse(encodedString) ?? throw new UriFormatException($"Invalid Uri format: {encodedString.ToEncodedString()}"))
    {

    }

    private Uri(UriComponentParts parts)
    {

    }
}
