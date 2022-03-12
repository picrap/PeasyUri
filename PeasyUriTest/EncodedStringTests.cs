using NUnit.Framework;
using PeasyUri;

namespace PeasyUriTest;

[TestFixture]
public class EncodedStringTests
{
    [Test]
    public void NoEncoding()
    {
        var s = EncodedString.FromEncoded("a")!;
        Assert.IsTrue(s.Decode() == "a");
    }

    [Test]
    public void Encoding()
    {
        var s = EncodedString.FromEncoded("%20")!;
        Assert.IsTrue(s.Decode() == " ");
    }

    [Test]
    public void WithUnicode()
    {
        var s = EncodedString.FromEncoded("🤪")!;
        Assert.IsTrue(s.Decode() == "🤪");
    }
}
