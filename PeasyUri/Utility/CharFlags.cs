using System;

namespace PeasyUri.Utility;

[Flags]
internal enum CharFlags
{
    GenDelim = 0x0001,
    SubDelim = 0x0002,
    Reserved = GenDelim | SubDelim,

    Alpha = 0x0010,
    Digit = 0x0020,
    UpperCase = 0x0040,
    OtherUnreserved = 0x0080,

    Unreserved = Alpha | Digit | OtherUnreserved,
}