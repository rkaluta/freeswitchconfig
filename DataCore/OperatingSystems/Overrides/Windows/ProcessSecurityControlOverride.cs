using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using System.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Overrides.Windows
{
    public class ProcessSecurityControlOverride : MarshalByRefObject, IOperatingSystemOverrideContainer
    {

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.ProcessSecurityControl",
            "RunDiagnostics",
            "Windows",
            "5.0",
            "6.9")]
        public List<string> RunDiagnostics()
        {
            return new List<string>();
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.ProcessSecurityControl",
            "CurrentProcessUserID",
            "Windows",
            "5.0",
            "6.9")]
        public uint CurrentProcessUserID
        {
            get
            {
                return 0;
            }
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.ProcessSecurityControl",
            "GetUIDForUser",
            "Windows",
            "5.0",
            "6.9")]
        public uint GetUIDForUser(string username)
        {
            return 0;
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.ProcessSecurityControl",
            "RunningUserID",
            "Windows",
            "5.0",
            "6.9")]
        public uint RunningUserID
        {
            get
            {
                return 0;
            }
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.ProcessSecurityControl",
            "_CloseProcessSecurity",
            "Windows",
            "5.0",
            "6.9")]
        internal void _CloseProcessSecurity()
        {
        }
    }
}
