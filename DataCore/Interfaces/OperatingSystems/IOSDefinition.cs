using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems
{
    public interface IOSDefinition
    {
        string OsName { get; }
        bool IsCurrentOS { get; }
        OSVersion Version { get; }
        bool UsesMappedSockets { get; }
        bool CanUseSysLog { get; }
        bool UsesSudo { get; }
    }
}
