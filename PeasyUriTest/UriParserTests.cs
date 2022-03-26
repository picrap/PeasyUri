using NUnit.Framework;
using PeasyUri;

namespace PeasyUriTest;

[TestFixture]
public class UriParserTests
{
    [Test]
    public void FullUri()
    {
        var parts = UriParser.Default.Parse("foo://example.com:8042/over/there?name=ferret#nose");
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsTrue(parts.Query == "name=ferret");
        Assert.IsTrue(parts.Fragment == "nose");
    }

    [Test]
    public void RelativeUri()
    {
        var parts = UriParser.Default.Parse("//example.com:8042/over/there?name=ferret#nose");
        Assert.IsNull(null, parts.Scheme);
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsTrue(parts.Query == "name=ferret");
        Assert.IsTrue(parts.Fragment == "nose");
    }

    [Test]
    public void UriNoFragment()
    {
        var parts = UriParser.Default.Parse("foo://example.com:8042/over/there?name=ferret");
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsTrue(parts.Query == "name=ferret");
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void UriNoQuery()
    {
        var parts = UriParser.Default.Parse("foo://example.com:8042/over/there#nose");
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsNull(parts.Query);
        Assert.IsTrue(parts.Fragment == "nose");
    }

    [Test]
    public void UriPathOnly()
    {
        var parts = UriParser.Default.Parse("foo://example.com:8042/over/there");
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsNull(parts.Query);
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void UriNoPath()
    {
        var parts = UriParser.Default.Parse("foo://example.com:8042");
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042");
        Assert.IsTrue(parts.Authority == "example.com:8042");
        Assert.IsNull(parts.Query);
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void Urn()
    {
        var parts = UriParser.Default.Parse("urn:example:animal:ferret:nose");
        Assert.IsTrue("urn" == parts.Scheme);
        Assert.IsTrue(parts.HierPart == "example:animal:ferret:nose");
        Assert.IsNull(parts.Query);
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void FullHierPart()
    {
        var parts = UriParser.Default.Parse("//example.com:8042/over/there");
        Assert.IsTrue(parts.Authority == "example.com:8042");
        Assert.IsTrue(parts.Path == "/over/there");
    }

    [Test]
    public void DecodeAuthority()
    {
        var parts = UriParser.Default.Parse("foo://xn--bpo-bma");
        Assert.IsTrue(parts.DecodedHost == "bépo");
    }
}
