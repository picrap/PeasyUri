
namespace PeasyUri.Utility;

internal static class CharUtility
{
    private static readonly CharFlags[] Flags =
    {
        // something odd: according to the RFC, some characters are reserved, some are unreserved.
        // and some are none of the above 🤔
        0, // ' ', ASCII=32
        CharFlags.SubDelim, // '!', ASCII=33
        0, // '"', ASCII=34
        CharFlags.GenDelim, // '#', ASCII=35
        CharFlags.SubDelim, // '$', ASCII=36
        0, // '%', ASCII=37
        CharFlags.SubDelim, // '&', ASCII=38
        CharFlags.SubDelim, // ''', ASCII=39
        CharFlags.SubDelim, // '(', ASCII=40
        CharFlags.SubDelim, // ')', ASCII=41
        CharFlags.SubDelim, // '*', ASCII=42
        CharFlags.SubDelim, // '+', ASCII=43
        CharFlags.SubDelim, // ',', ASCII=44
        CharFlags.OtherUnreserved, // '-', ASCII=45
        CharFlags.OtherUnreserved, // '.', ASCII=46
        CharFlags.GenDelim, // '/', ASCII=47
        CharFlags.Digit, // '0', ASCII=48
        CharFlags.Digit, // '1', ASCII=49
        CharFlags.Digit, // '2', ASCII=50
        CharFlags.Digit, // '3', ASCII=51
        CharFlags.Digit, // '4', ASCII=52
        CharFlags.Digit, // '5', ASCII=53
        CharFlags.Digit, // '6', ASCII=54
        CharFlags.Digit, // '7', ASCII=55
        CharFlags.Digit, // '8', ASCII=56
        CharFlags.Digit, // '9', ASCII=57
        CharFlags.GenDelim, // ':', ASCII=58
        CharFlags.SubDelim, // ';', ASCII=59
        0, // '<', ASCII=60
        CharFlags.SubDelim, // '=', ASCII=61
        0, // '>', ASCII=62
        CharFlags.GenDelim, // '?', ASCII=63
        CharFlags.GenDelim, // '@', ASCII=64
        CharFlags.Alpha | CharFlags.UpperCase, // 'A', ASCII=65
        CharFlags.Alpha | CharFlags.UpperCase, // 'B', ASCII=66
        CharFlags.Alpha | CharFlags.UpperCase, // 'C', ASCII=67
        CharFlags.Alpha | CharFlags.UpperCase, // 'D', ASCII=68
        CharFlags.Alpha | CharFlags.UpperCase, // 'E', ASCII=69
        CharFlags.Alpha | CharFlags.UpperCase, // 'F', ASCII=70
        CharFlags.Alpha | CharFlags.UpperCase, // 'G', ASCII=71
        CharFlags.Alpha | CharFlags.UpperCase, // 'H', ASCII=72
        CharFlags.Alpha | CharFlags.UpperCase, // 'I', ASCII=73
        CharFlags.Alpha | CharFlags.UpperCase, // 'J', ASCII=74
        CharFlags.Alpha | CharFlags.UpperCase, // 'K', ASCII=75
        CharFlags.Alpha | CharFlags.UpperCase, // 'L', ASCII=76
        CharFlags.Alpha | CharFlags.UpperCase, // 'M', ASCII=77
        CharFlags.Alpha | CharFlags.UpperCase, // 'N', ASCII=78
        CharFlags.Alpha | CharFlags.UpperCase, // 'O', ASCII=79
        CharFlags.Alpha | CharFlags.UpperCase, // 'P', ASCII=80
        CharFlags.Alpha | CharFlags.UpperCase, // 'Q', ASCII=81
        CharFlags.Alpha | CharFlags.UpperCase, // 'R', ASCII=82
        CharFlags.Alpha | CharFlags.UpperCase, // 'S', ASCII=83
        CharFlags.Alpha | CharFlags.UpperCase, // 'T', ASCII=84
        CharFlags.Alpha | CharFlags.UpperCase, // 'U', ASCII=85
        CharFlags.Alpha | CharFlags.UpperCase, // 'V', ASCII=86
        CharFlags.Alpha | CharFlags.UpperCase, // 'W', ASCII=87
        CharFlags.Alpha | CharFlags.UpperCase, // 'X', ASCII=88
        CharFlags.Alpha | CharFlags.UpperCase, // 'Y', ASCII=89
        CharFlags.Alpha | CharFlags.UpperCase, // 'Z', ASCII=90
        CharFlags.GenDelim, // '[', ASCII=91
        0, // '\', ASCII=92
        CharFlags.GenDelim, // ']', ASCII=93
        0, // '^', ASCII=94
        CharFlags.OtherUnreserved, // '_', ASCII=95
        0, // '`', ASCII=96
        CharFlags.Alpha, // 'a', ASCII=97
        CharFlags.Alpha, // 'b', ASCII=98
        CharFlags.Alpha, // 'c', ASCII=99
        CharFlags.Alpha, // 'd', ASCII=100
        CharFlags.Alpha, // 'e', ASCII=101
        CharFlags.Alpha, // 'f', ASCII=102
        CharFlags.Alpha, // 'g', ASCII=103
        CharFlags.Alpha, // 'h', ASCII=104
        CharFlags.Alpha, // 'i', ASCII=105
        CharFlags.Alpha, // 'j', ASCII=106
        CharFlags.Alpha, // 'k', ASCII=107
        CharFlags.Alpha, // 'l', ASCII=108
        CharFlags.Alpha, // 'm', ASCII=109
        CharFlags.Alpha, // 'n', ASCII=110
        CharFlags.Alpha, // 'o', ASCII=111
        CharFlags.Alpha, // 'p', ASCII=112
        CharFlags.Alpha, // 'q', ASCII=113
        CharFlags.Alpha, // 'r', ASCII=114
        CharFlags.Alpha, // 's', ASCII=115
        CharFlags.Alpha, // 't', ASCII=116
        CharFlags.Alpha, // 'u', ASCII=117
        CharFlags.Alpha, // 'v', ASCII=118
        CharFlags.Alpha, // 'w', ASCII=119
        CharFlags.Alpha, // 'x', ASCII=120
        CharFlags.Alpha, // 'y', ASCII=121
        CharFlags.Alpha, // 'z', ASCII=122
        0, // '{', ASCII=123
        0, // '|', ASCII=124
        0, // '}', ASCII=125
        CharFlags.OtherUnreserved, // '~', ASCII=126
        0, // '', ASCII=127
    };
    
    public static bool Is(this char c, CharFlags flags)
    {
        if (c < 32 || c > 127)
            return false;
        return (Flags[c - 32] & flags) != 0;
    }

    public static bool IsAlpha(this char c) => Is(c, CharFlags.Alpha);
    public static bool IsDigit(this char c) => Is(c, CharFlags.Digit);
    public static bool IsUnreserved(this char c) => Is(c, CharFlags.Unreserved);
    public static bool IsReserved(this char c) => Is(c, CharFlags.Reserved);

}