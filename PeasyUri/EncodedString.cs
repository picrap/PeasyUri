using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PeasyUri.Utility;

namespace PeasyUri;

[DebuggerDisplay("{_encodedString}")]
public class EncodedString : IEquatable<EncodedString>, IEnumerable<byte>
{
    public static EncodedString Empty = FromEncoded("")!;

    private static readonly Encoding Encoding = Encoding.UTF8;

    private readonly string _encodedString;

    public int Length => _encodedString.Length;

    private EncodedString(string encoded)
    {
        _encodedString = encoded ?? throw new ArgumentNullException(nameof(encoded));
    }

    public static EncodedString? FromEncoded(byte[]? encoded) => encoded is null ? null : new(Encoding.ASCII.GetString(encoded));
    public static EncodedString? FromEncoded(string? encoded) => encoded is null ? null : new(encoded);

    public string ToEncodedString() => _encodedString;

    public string Decode()
    {
        return Encoding.GetString(DecodeBytes().ToArray());
    }

    private IEnumerable<byte> DecodeBytes()
    {
        var encodedBytes = Encoding.GetBytes(_encodedString);
        for (int byteIndex = 0; byteIndex < encodedBytes.Length; byteIndex++)
        {
            var c = encodedBytes[byteIndex];
            if (c != '%')
            {
                yield return c;
                continue;
            }

            if (byteIndex >= encodedBytes.Length - 2)
                throw new FormatException($"Not enough octets in string after %-sign at {byteIndex}");

            var ec = GetHexDigit(encodedBytes[byteIndex + 1]) * 16 + GetHexDigit(encodedBytes[byteIndex + 2]);
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

    public IEnumerator<byte> GetEnumerator()
    {
        return DecodeBytes().GetEnumerator();
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

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

    public EncodedString Substring(int start, int offset) => new(_encodedString.Substring(start, offset));

    public IEnumerable<EncodedString> Split(char c) => _encodedString.Split(c).Select(FromEncoded)!;

    public bool Contains(char c) => IndexOf(c).HasValue;

    public int? IndexOf(char c, IsValidCharacterDelegate? validCharacter = null, int first = 0, int? count = null)
    {
        return _encodedString.IndexOf(c, validCharacter, first, count);
    }

    public bool StartsWith(string s)
    {
        return IndexOf(s) == 0;
    }

    public int? IndexOf(string s)
    {
        var indexOf = _encodedString.IndexOf(s, StringComparison.InvariantCulture);
        return indexOf < 0 ? null : indexOf;
    }
}
