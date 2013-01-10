using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Definitions
{
    public class Debian : IOSDefinition
    {
        #region IOSDefinition Members

        public string OsName
        {
            get { return "Debian"; }
        }

        public bool IsCurrentOS
        {
            get {
                FileInfo fi = new FileInfo("/etc/debian_release");
                return fi.Exists;
            }
        }

        public OSVersion Version
        {
            get
            {
                string ver = Utility.ExecuteProgram("/bin/cat", "/etc/debian_release", true);
                return new OSVersion(ver);
            }
        }

        public bool UsesMappedSockets
        {
            get { return true; }
        }

        public bool CanUseSysLog
        {
            get { return true; }
        }

        public bool UsesSudo
        {
            get { return true; }
        }

        #endregion
    }
}
