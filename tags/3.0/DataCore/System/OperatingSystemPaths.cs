using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using System.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System
{
    public class OperatingSystemPaths : MarshalByRefObject
    {
        private static OperatingSystemPaths _current;
        public static OperatingSystemPaths Current
        {
            get
            {
                if (_current == null)
                    _current = (OperatingSystemPaths)OSClassProxy.Instance(new OperatingSystemPaths());
                return _current;
            }
        }

        private OperatingSystemPaths()
        {
            if (Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME] == null)
                Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME] = Constants.DEFAULT_RUNNING_USERNAME;
        }

        [OperatingSystemOverridablePropertyAttribute("Path to SQLLite command.")]
        public string SQLLITECommand
        {
            get { return "/usr/local/bin/sqlite3"; }
        }

        [OperatingSystemOverridablePropertyAttribute("Path to SUDO command.")]
        public string SUDOCommand
        {
            get { return "/usr/bin/sudo"; }
        }

        [OperatingSystemOverridablePropertyAttribute("Path to firebird databases.")]
        public string FirebirdDataPath
        {
            get { return "/var/lib/firebird/data"; }
        }

        [OperatingSystemOverridablePropertyAttribute("Path to logs folder.")]
        public string LogsPath
        {
            get { return "/var/log"; }
        }
    }
}
