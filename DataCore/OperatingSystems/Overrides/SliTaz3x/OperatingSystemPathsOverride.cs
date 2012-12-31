using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Overrides.SliTaz3x
{
    public class OperatingSystemPathsOverride : MarshalByRefObject, IOperatingSystemOverrideContainer
    {

        [OperatingSystemPropertyOverride(
            "Org.Reddragonit.FreeSwitchConfig.DataCore.System.OperatingSystemPaths",
            "FirebirdDataPath",
            "SliTaz",
            "3.0",
            null)]
        public string FirebirdDataPath
        {
            get { return "/opt/firebird/data"; }
        }
    }
}
