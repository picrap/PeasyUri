using NUnit.Framework;
using PeasyUri;
#pragma warning disable CS8602

namespace PeasyUriTest;

[TestFixture]
public class UriParserTests
{
    [Test]
    public void FullUri()
    {
        var parts = UriParser.Default.TryParse("foo://example.com:8042/over/there?name=ferret#nose");
        Assert.IsNotNull(parts);
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsTrue(parts.IdnHost == "example.com");
        Assert.IsTrue(parts.Port == 8042);
        Assert.IsTrue(parts.Query == "name=ferret");
        Assert.IsTrue(parts.Fragment == "nose");
    }

    [Test]
    public void UserInfoUri()
    {
        var parts = UriParser.Default.TryParse("foo://bob:doe@example.com:8042");
        Assert.IsNotNull(parts);
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.IdnHost == "example.com");
        Assert.IsTrue(parts.Port == 8042);
        Assert.IsTrue(parts.DecodedUserInfo?.UserName == "bob");
        Assert.IsTrue(parts.DecodedUserInfo?.Password == "doe");
    }

    [Test]
    public void RelativeUri()
    {
        var parts = UriParser.Default.TryParse("//example.com:8042/over/there?name=ferret#nose");
        Assert.IsNotNull(parts);
        Assert.IsNull(null, parts.Scheme);
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsTrue(parts.Query == "name=ferret");
        Assert.IsTrue(parts.Fragment == "nose");
    }

    [Test]
    public void UriNoFragment()
    {
        var parts = UriParser.Default.TryParse("foo://example.com:8042/over/there?name=ferret");
        Assert.IsNotNull(parts);
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsTrue(parts.Query == "name=ferret");
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void UriNoQuery()
    {
        var parts = UriParser.Default.TryParse("foo://example.com:8042/over/there#nose");
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsNull(parts.Query);
        Assert.IsTrue(parts.Fragment == "nose");
    }

    [Test]
    public void UriPathOnly()
    {
        var parts = UriParser.Default.TryParse("foo://example.com:8042/over/there");
        Assert.IsNotNull(parts);
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042/over/there");
        Assert.IsNull(parts.Query);
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void UriNoPath()
    {
        var parts = UriParser.Default.TryParse("foo://example.com:8042");
        Assert.IsNotNull(parts);
        Assert.IsTrue(parts.Scheme == "foo");
        Assert.IsTrue(parts.HierPart == "//example.com:8042");
        Assert.IsTrue(parts.Authority == "example.com:8042");
        Assert.IsNull(parts.Query);
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void Urn()
    {
        var parts = UriParser.Default.TryParse("urn:example:animal:ferret:nose");
        Assert.IsNotNull(parts);
        Assert.IsTrue("urn" == parts.Scheme);
        Assert.IsTrue(parts.HierPart == "example:animal:ferret:nose");
        Assert.IsNull(parts.Query);
        Assert.IsNull(parts.Fragment);
    }

    [Test]
    public void FullHierPart()
    {
        var parts = UriParser.Default.TryParse("//example.com:8042/over/there");
        Assert.IsTrue(parts.Authority == "example.com:8042");
        Assert.IsTrue(parts.Path == "/over/there");
    }

    [Test]
    public void NoDecodeAuthority()
    {
        var parts = UriParser.Default.TryParse("foo://xn--bpo-bma");
        var uri = new System.Uri("foo://xn--bpo-bma");
        Assert.IsNotNull(parts);
        Assert.IsTrue(parts.IdnHost == "xn--bpo-bma");
    }

    [Test]
    public void DecodeAuthority()
    {
        var parts = UriParser.Default.TryParse("foo://bépo");
        var uri = new System.Uri("foo://bépo");
        Assert.IsNotNull(parts);
        Assert.IsTrue(parts.IdnHost == "xn--bpo-bma");
    }

    [Test]
    public void EncodedUriTest()
    {
        var literalUri = "foo://hé:bé@høst:1234/oṽ%c4%93r?name=ƒer%c5%99e%74#nŏ%c5%9fe";
        var uri = new System.Uri(literalUri);
        var peasyUri = new Uri(EncodedString.FromEncoded(literalUri)!);
        var d = peasyUri.Encode();
        var de = d.ToEncodedString();
    }

    [Test]
    public void IPv4HostPort()
    {
        var parts = UriParser.Default.TryParse("foo://host:1234");
        Assert.IsNotNull(parts);
        Assert.IsTrue(parts.IdnHost == "host");
        Assert.IsTrue(parts.Port == 1234);
    }

    [Test]
    public void IPv6HostPort()
    {
        var parts = UriParser.Default.TryParse("foo://[2a00::5678]:1234");
        var uri = new System.Uri("foo://[2a00::5678]:1234");
        Assert.IsNotNull(parts);
        Assert.IsTrue(parts.IdnHost == "2a00::5678");
        Assert.IsTrue(parts.DnsSafeHost == "2a00::5678");
        Assert.IsTrue(parts.Port == 1234);
    }
}
