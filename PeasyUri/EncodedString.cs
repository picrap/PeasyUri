using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using PeasyUri.Utility;

namespace PeasyUri;

/// <summary>
/// An encoded string is an octet representation for a string
/// It keeps escaped chars as escaped, to make a semantic difference between / and %2F
/// </summary>
[DebuggerDisplay("{DebugLiteral}")]
public class EncodedString : IEquatable<EncodedString>, IEnumerable<byte>
{
    public static readonly EncodedString Empty = FromEncoded("")!;

    private static Encoding? _encoding;
    private static Encoding Encoding => _encoding ??= Encoding.UTF8;

    private readonly byte[] _bytes;

    private string DebugLiteral => Decode();

    public int Length => _bytes.Length;

    private EncodedString(string encoded)
    {
        if (encoded is null)
            throw new ArgumentNullException(nameof(encoded));
        _bytes = ParseString(encoded).ToArray();
    }

    private EncodedString(IEnumerable<byte> bytes)
    {
        _bytes = bytes.ToArray();
    }

    public static EncodedString? FromEncoded(byte[]? encoded) => encoded is null ? null : new(Encoding.ASCII.GetString(encoded));
    public static EncodedString? FromEncoded(string? encoded) => encoded is null ? null : new(encoded);

    public string ToEncodedString() => Encoding.ASCII.GetString(_bytes);

    public string Decode() => Decode(_bytes);

    private static string Decode(byte[] encodedBytes) => Encoding.GetString(DecodeBytes(encodedBytes).ToArray());

    private IEnumerable<byte> DecodeBytes() => DecodeBytes(_bytes);

    private static IEnumerable<byte> DecodeBytes(byte[] encodedBytes)
    {
        for (int byteIndex = 0; byteIndex < encodedBytes.Length; byteIndex++)
        {
            var c = encodedBytes[byteIndex];
            if (c != '%')
            {
                yield return c;
                continue;
            }

            var ec = GetPercentEncoded(encodedBytes, ref byteIndex);
            yield return ec;
        }
    }

    private static IEnumerable<byte> ParseString(string encodedString)
    {
        var bytes = new List<byte>();
        var encodedBytes = Encoding.GetBytes(encodedString);
        for (int byteIndex = 0; byteIndex < encodedBytes.Length; byteIndex++)
        {
            var c = encodedBytes[byteIndex];
            if (c != '%')
            {
                if (((char)c).Is(CharFlags.Reserved | CharFlags.Unreserved))
                    bytes.Add(c);
                else
                    GetAsPercentEncoding(c, bytes);
                continue;
            }

            var ec = GetPercentEncoded(encodedBytes, ref byteIndex);
            if (((char)ec).IsUnreserved())
                bytes.Add(ec);
            else
                GetAsPercentEncoding(ec, bytes);
        }

        return bytes;
    }

    private static byte GetPercentEncoded(byte[] encodedBytes, ref int byteIndex)
    {
        if (byteIndex >= encodedBytes.Length - 2)
            throw new FormatException($"Not enough octets in string after %-sign at {byteIndex}");

        var ec = GetHexDigit(encodedBytes[byteIndex + 1]) * 16 + GetHexDigit(encodedBytes[byteIndex + 2]);
        if (!ec.HasValue)
            throw new FormatException($"Expected two hexadecimal octets after %-sign at index {byteIndex}");
        byteIndex += 2;
        return (byte)ec.Value;
    }

    private static void GetAsPercentEncoding(byte c, List<byte> bytes)
    {
        bytes.AddRange(PercentEncode(c).Select(e => (byte)e));
    }

    private static string PercentEncode(byte b)
    {
        return $"%{b:X2}";
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
        return _bytes.SequenceEqual(_bytes);
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

    public EncodedString Substring(int start, int offset) => new(_bytes.Skip(start).Take(offset));

    public IEnumerable<EncodedString> Split(char c)
    {
        var lastIndex = 0;
        foreach (var index in IndexesOf(c))
        {
            yield return new(_bytes.Skip(lastIndex).Take(index - lastIndex));
            lastIndex = index + 1;
        }

        yield return new(_bytes.Skip(lastIndex));
    }

    public bool Contains(char c) => IndexOf(c).HasValue;

    private IEnumerable<int> IndexesOf(char c, int first = 0, int? count = null)
    {
        var actualCount = count ?? _bytes.Length - first;
        for (int index = first; actualCount > 0; actualCount--, index++)
            if (_bytes[index] == c)
                yield return index;
    }

    public int? IndexOf(char c, int first = 0, int? count = null)
    {
        return IndexesOf(c, first, count).Cast<int?>().FirstOrDefault();
    }

    public int? LastIndexOf(char c, int first = 0, int? count = null)
    {
        return IndexesOf(c, first, count).Cast<int?>().LastOrDefault();
    }

    public bool StartsWith(string s)
    {
        return ContainsAt(s, 0);
    }

    private bool ContainsAt(string s, int offset)
    {
        if (s.Length + offset > _bytes.Length)
            return false;
        for (int i = 0; i < s.Length; i++)
            if (_bytes[offset + i] != s[i])
                return false;
        return true;
    }

    public bool IsValid(IsValidCharacterDelegate? isValidCharacterDelegate)
    {
        if (isValidCharacterDelegate is null)
            return true;

        var index = 0;
        foreach (var c in _bytes)
        {
            if (!isValidCharacterDelegate((char)c, index++))
                return false;
        }
        return true;
    }

    public static EncodedString operator +(EncodedString a, EncodedString b)
    {
        return new EncodedString(a._bytes.Concat(b._bytes));
    }

    public static EncodedString operator +(EncodedString a, string b)
    {
        return new EncodedString(a._bytes.Concat(FromEncoded(b)!._bytes));
    }

    public static EncodedString operator +(string a, EncodedString b)
    {
        return new EncodedString(FromEncoded(a)!._bytes.Concat(b._bytes));
    }

    //public static EncodedString? ReadLine(Stream stream)
    //{
    //    var buffer = new List<byte>();
    //    int eolCount = 0;
    //    for (; ; )
    //    {
    //        var b = stream.ReadByte();
    //        if (b == -1)
    //        {
    //            if (buffer.Count == 0)
    //                return null;
    //            break;
    //        }
    //        if (b == Environment.NewLine[eolCount])
    //        {
    //            if (++eolCount >= Environment.NewLine.Length)
    //                break;
    //            continue;
    //        }
    //        // wrong EOL
    //        if (eolCount > 0)
    //            break;
    //        buffer.Add((byte)b);
    //    }
    //    return new EncodedString(buffer);
    //}

    public static EncodedString? ReadLine(TextReader textReader)
    {
        var line = textReader.ReadLine();
        if (line is null)
            return null;
        return FromEncoded(line);
    }

    public void WriteLine(TextWriter textWriter)
    {
        textWriter.WriteLine(ToEncodedString());
    }
}
