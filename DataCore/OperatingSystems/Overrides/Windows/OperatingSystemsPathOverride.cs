using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Overrides.Windows
{
    public class OperatingSystemsPathOverride : MarshalByRefObject, IOperatingSystemOverrideContainer
    {

        [OperatingSystemPropertyOverride(
            "Org.Reddragonit.FreeSwitchConfig.DataCore.System.OperatingSystemPaths",
            "FirebirdDataPath",
            "Windows",
            "5.0",
            "6.9")]
        public string FirebirdDataPath
        {
            get { return "Program Files\\Firebird\\Firebird_2_5\\data"; }
        }
    }
}
