using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System
{
    public enum NetworkSpeeds
    {
        HalfDuplex_10MBps,
        FullDuplex_10MBps,
        HalfDuplex_100MBps,
        FullDuplex_100MBps,
        HalfDuplex_1GBps,
        FullDuplex_1GBps,
        Unknown
    }

    public enum ByteUnits
    {
        Byte,
        KiloByte,
        MegaByte,
        GigaByte,
        TeraByte
    }
}
