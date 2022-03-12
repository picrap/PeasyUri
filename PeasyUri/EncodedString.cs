using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PeasyUri;

[DebuggerDisplay("{_encodedString}")]
public class EncodedString : IEquatable<EncodedString>
{
    private static readonly Encoding Encoding = Encoding.UTF8;

    private readonly string _encodedString;

    private EncodedString(string encoded)
    {
        _encodedString = encoded;
    }

    public static EncodedString? FromEncoded(string? encoded) => encoded is null ? null : new(encoded);

    public string ToEncodedString() => _encodedString;

    public string Decode()
    {
        return Encoding.GetString(DecodeBytes().ToArray());
    }

    private IEnumerable<byte> DecodeBytes()
    {
        var bytes = Encoding.GetBytes(_encodedString);
        for (int byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
        {
            var c = bytes[byteIndex];
            if (c != '%')
            {
                yield return c;
                continue;
            }

            if (byteIndex >= bytes.Length - 2)
                throw new FormatException($"Not enough octets in string after %-sign at {byteIndex}");

            var ec = GetHexDigit(bytes[byteIndex + 1]) * 16 + GetHexDigit(bytes[byteIndex + 2]);
            if (!ec.HasValue)
                throw new FormatException($"Expected two hexadecimal octets after %-sign at index {byteIndex}");
            yield return (byte)ec.Value;
            byteIndex += 2;
        }
    }

    private static int? GetHexDigit(byte b)
    {
        if (b >= '0' && b <= '9')
            return b - '0';
        if (b >= 'A' && b <= 'F')
            return b - 'A' + 10;
        if (b >= 'a' && b <= 'f')
            return b - 'a' + 10;
        return null;
    }

    public override bool Equals(object? obj)
    {
        if (obj is EncodedString encodedString)
            return Equals(encodedString);
        if (obj is string s)
            return Equals(FromEncoded(s)!);
        return false;
    }

    public bool Equals(string? other)
    {
        return Equals(FromEncoded(other));
    }

    public bool Equals(EncodedString? other)
    {
        if (other is null)
            return false;
        return DecodeBytes().SequenceEqual(other.DecodeBytes());
    }

    public override int GetHashCode()
    {
        var b = DecodeBytes().Take(3).ToArray();
        return b.ElementAtOrDefault(2) << 16 | b.ElementAtOrDefault(1) << 16 | b.ElementAtOrDefault(0);
    }

    public static bool operator ==(EncodedString? a, EncodedString? b)
    {
        if (a is null)
            return b is null;
        return a.Equals(b);
    }

    public static bool operator !=(EncodedString? a, EncodedString? b) => !(a == b);

    public static bool operator ==(EncodedString? a, string? b)
    {
        if (a is null)
            return b is null;
        return a.Equals(b);
    }

    public static bool operator !=(EncodedString? a, string? b) => !(a == b);
}