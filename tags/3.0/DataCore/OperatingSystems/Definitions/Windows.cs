using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Definitions
{
    public class Windows : IOSDefinition
    {
        #region IOSDefinition Members

        public string OsName
        {
            get { return "Windows"; }
        }

        public bool IsCurrentOS
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 5;
            }
        }

        public OSVersion Version
        {
            get
            {
                return new OSVersion(Environment.OSVersion.Version.Major.ToString() + "." +
                    Environment.OSVersion.Version.Minor.ToString() + "." +
                    Environment.OSVersion.Version.Build.ToString() + "." +
                    Environment.OSVersion.Version.Revision.ToString());
            }
        }

        public bool UsesMappedSockets
        {
            get { return false; }
        }

        public bool CanUseSysLog
        {
            get
            {
                return false;
            }
        }

        public bool UsesSudo
        {
            get { return false; }
        }

        #endregion
    }
}
