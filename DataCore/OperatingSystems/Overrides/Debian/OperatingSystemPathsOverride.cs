using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Overrides.Debian
{
    public class OperatingSystemPathsOverride : MarshalByRefObject, IOperatingSystemOverrideContainer
    {

        [OperatingSystemPropertyOverride(
            "Org.Reddragonit.FreeSwitchConfig.DataCore.System.OperatingSystemPaths",
            "SQLLITECommand",
            "Debian",
            "5.0",
            null)]
        public string SQLLITECommand
        {
            get { return "/usr/bin/sqlite3"; }
        }

        [OperatingSystemPropertyOverride(
            "Org.Reddragonit.FreeSwitchConfig.DataCore.System.OperatingSystemPaths",
            "FirebirdDataPath",
            "Debian",
            "5.0",
            null)]
        public string FirebirdDataPath
        {
            get { return "/var/lib/firebird/2.1/data"; }
        }
    }
}
