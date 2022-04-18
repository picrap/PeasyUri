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
        var uri = UriParser.Default.TryParse("foo://example.com:8042/over/there?name=ferret#nose");
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.Scheme == "foo");
        Assert.IsTrue(uri.HierPart == "//example.com:8042/over/there");
        Assert.IsTrue(uri.IdnHost == "example.com");
        Assert.IsTrue(uri.Port == 8042);
        Assert.IsTrue(uri.Query == "name=ferret");
        Assert.IsTrue(uri.Fragment == "nose");
    }

    [Test]
    public void UserInfoUri()
    {
        var uri = UriParser.Default.TryParse("foo://bob:doe@example.com:8042");
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.Scheme == "foo");
        Assert.IsTrue(uri.IdnHost == "example.com");
        Assert.IsTrue(uri.Port == 8042);
        Assert.IsTrue(uri.DecodedUserInfo?.UserName == "bob");
        Assert.IsTrue(uri.DecodedUserInfo?.Password == "doe");
    }

    [Test]
    public void EmailUri()
    {
        var emailUri = UriParser.Default.TryParse("mailto:John.Doe@example.com");
        Assert.IsNotNull(emailUri);
        Assert.IsTrue(emailUri.Scheme == "mailto");
        Assert.IsTrue(emailUri.HierPart == "John.Doe@example.com");
    }

    [Test]
    public void RelativeUri()
    {
        var uri = UriParser.Default.TryParse("//example.com:8042/over/there?name=ferret#nose");
        Assert.IsNotNull(uri);
        Assert.IsNull(null, uri.Scheme);
        Assert.IsTrue(uri.HierPart == "//example.com:8042/over/there");
        Assert.IsTrue(uri.Query == "name=ferret");
        Assert.IsTrue(uri.Fragment == "nose");
    }

    [Test]
    public void UriNoFragment()
    {
        var uri = UriParser.Default.TryParse("foo://example.com:8042/over/there?name=ferret");
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.Scheme == "foo");
        Assert.IsTrue(uri.HierPart == "//example.com:8042/over/there");
        Assert.IsTrue(uri.Query == "name=ferret");
        Assert.IsNull(uri.Fragment);
    }

    [Test]
    public void UriNoQuery()
    {
        var uri = UriParser.Default.TryParse("foo://example.com:8042/over/there#nose");
        Assert.IsTrue(uri.Scheme == "foo");
        Assert.IsTrue(uri.HierPart == "//example.com:8042/over/there");
        Assert.IsNull(uri.Query);
        Assert.IsTrue(uri.Fragment == "nose");
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
        var uri = UriParser.Default.TryParse("foo://example.com:8042");
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.Scheme == "foo");
        Assert.IsTrue(uri.HierPart == "//example.com:8042");
        Assert.IsTrue(uri.Authority == "example.com:8042");
        Assert.IsNull(uri.Query);
        Assert.IsNull(uri.Fragment);
    }

    [Test]
    public void Urn()
    {
        var urn = UriParser.Default.TryParse("urn:example:animal:ferret:nose");
        Assert.IsNotNull(urn);
        Assert.IsTrue("urn" == urn.Scheme);
        Assert.IsTrue(urn.HierPart == "example:animal:ferret:nose");
        Assert.IsNull(urn.Query);
        Assert.IsNull(urn.Fragment);
    }

    [Test]
    public void FullHierPart()
    {
        var uri = UriParser.Default.TryParse("//example.com:8042/over/there");
        Assert.IsTrue(uri.Authority == "example.com:8042");
        Assert.IsTrue(uri.AbsolutePath == "/over/there");
    }

    [Test]
    public void NoDecodeAuthority()
    {
        var uri = UriParser.Default.TryParse("foo://xn--bpo-bma");
        var systemUri = new System.Uri("foo://xn--bpo-bma");
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.IdnHost == "xn--bpo-bma");
    }

    [Test]
    public void DecodeAuthority()
    {
        var uri = UriParser.Default.TryParse("foo://bépo");
        var systemUri = new System.Uri("foo://bépo");
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.IdnHost == "xn--bpo-bma");
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
        var uri = UriParser.Default.TryParse("foo://host:1234");
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.IdnHost == "host");
        Assert.IsTrue(uri.Port == 1234);
    }

    [Test]
    public void IPv6HostPort()
    {
        var uri = UriParser.Default.TryParse("foo://[2a00::5678]:1234");
        var systemUri = new System.Uri("foo://[2a00::5678]:1234");
        Assert.IsNotNull(uri);
        Assert.IsTrue(uri.IdnHost == "2a00::5678");
        Assert.IsTrue(uri.DnsSafeHost == "2a00::5678");
        Assert.IsTrue(uri.Port == 1234);
    }


    [Test]
    public void HttpQueryTest()
    {
        var literalUri = "http://host?1=one&2=two#3=notthree";
        var uri = new Uri(EncodedString.FromEncoded(literalUri)!);
        Assert.IsTrue(uri.QueryValues.TryGetValue("1", out var one) && one == "one");
        Assert.IsTrue(uri.QueryValues.TryGetValue("2", out var two) && two == "two");
        Assert.IsFalse(uri.QueryValues.ContainsKey("3"));
    }
}
