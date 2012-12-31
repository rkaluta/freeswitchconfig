using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Definitions
{
    public class CentOS5x : IOSDefinition
    {
        #region IOSDefinition Members

        public string OsName
        {
            get { return "CentOS"; }
        }

        public bool IsCurrentOS
        {
            get {
                try
                {
                    FileInfo fi = new FileInfo("/etc/redhat-release");
                    if (fi.Exists)
                    {
                        string ver = Utility.ExecuteProgram("/bin/cat", "/etc/redhat-release",true);
                        return ver.Trim().StartsWith("CentOS release 5");
                    }
                }
                catch (Exception e) {
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                    Log.Error(e);
                }
                return false;
            }
        }

        public OSVersion Version
        {
            get {
                string ver = Utility.ExecuteProgram("/bin/cat", "/etc/redhat-release", true);
                return new OSVersion(ver.Split(' ')[2]);
            }
        }

        public bool UsesMappedSockets
        {
            get { return true; }
        }

        public bool CanUseSysLog
        {
            get
            {
                return true;
            }
        }

        public bool UsesSudo
        {
            get { return true; }
        }

        #endregion
    }
}
