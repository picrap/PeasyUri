using NUnit.Framework;
using PeasyUri;

namespace PeasyUriTest;

[TestFixture]
public class Parser
{
    [Test]
    public void FullUri()
    {
        var uriParser = new UriParser();
        var parts = uriParser.Parse("foo://example.com:8042/over/there?name=ferret#nose");
        Assert.AreEqual("foo", parts.Scheme);
        Assert.AreEqual("//example.com:8042/over/there", parts.HierPart);
        Assert.AreEqual("name=ferret", parts.Query);
        Assert.AreEqual("nose", parts.Fragment);
    }

    [Test]
    public void RelativeUri()
    {
        var uriParser = new UriParser();
        var parts = uriParser.Parse("//example.com:8042/over/there?name=ferret#nose");
        Assert.IsNull(null, parts.Scheme);
        Assert.AreEqual("//example.com:8042/over/there", parts.HierPart);
        Assert.AreEqual("name=ferret", parts.Query);
        Assert.AreEqual("nose", parts.Fragment);
    }

    [Test]
    public void UriNoFragment()
    {
        var uriParser = new UriParser();
        var parts = uriParser.Parse("foo://example.com:8042/over/there?name=ferret");
        Assert.AreEqual("foo", parts.Scheme);
        Assert.AreEqual("//example.com:8042/over/there", parts.HierPart);
        Assert.AreEqual("name=ferret", parts.Query);
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void UriNoQuery()
    {
        var uriParser = new UriParser();
        var parts = uriParser.Parse("foo://example.com:8042/over/there#nose");
        Assert.AreEqual("foo", parts.Scheme);
        Assert.AreEqual("//example.com:8042/over/there", parts.HierPart);
        Assert.IsNull(parts.Query);
        Assert.AreEqual("nose", parts.Fragment);
    }


    [Test]
    public void UriPathOnly()
    {
        var uriParser = new UriParser();
        var parts = uriParser.Parse("foo://example.com:8042/over/there");
        Assert.AreEqual("foo", parts.Scheme);
        Assert.AreEqual("//example.com:8042/over/there", parts.HierPart);
        Assert.IsNull(parts.Query);
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void Urn()
    {
        var uriParser = new UriParser();
        var parts = uriParser.Parse("urn:example:animal:ferret:nose");
        Assert.AreEqual("urn", parts.Scheme);
        Assert.AreEqual("example:animal:ferret:nose", parts.HierPart);
        Assert.IsNull(parts.Query);
        Assert.IsNull(parts.Fragment);
    }
}
