using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using System.Diagnostics;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Overrides.CentOS5x
{
    public class NetworkSettingsOverride : MarshalByRefObject,IOperatingSystemOverrideContainer
    {
        internal const string NETWORK_CONFIGS_DIRECTORY = "/etc/sysconfig/network-scripts";
        internal const string NETWORK_CONFIG_FILENAME = "ifcfg-$deviceName$";

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "RunDiagnostics",
            "CentOS",
            "5.0",
            null)]
        public List<string> RunDiagnostics()
        {
            List<string> ret = new List<string>();
            if (!(new DirectoryInfo(NETWORK_CONFIGS_DIRECTORY).Exists))
                ret.Add("Unable to locate the network config scripts directory on the system.");
            else
                ret.Add("Succesffully located the network config scripts directory on the system.");
            return ret;
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "RequiredCommands",
            "CentOS",
            "5.0",
            null)]
        public List<NameValuePair> RequiredCommands
        {
            get
            {
                return new List<NameValuePair>(
                    new NameValuePair[]{
                        new NameValuePair("arp", NetworkSettings.ARPCommand),
                        new NameValuePair("arpping",NetworkSettings.ARPPINGCommand),
                        new NameValuePair("ifconfig",NetworkSettings.IFCONFIGCommand),
                        new NameValuePair("ip",NetworkSettings.IPCommand),
                        new NameValuePair("mii-tool",NetworkSettings.MIITOOLCommand),
                        new NameValuePair("ping",NetworkSettings.PINGCommand),
                    }
                );
            }
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetDHCP",
            "CentOS",
            "5.0",
            null)]
        public bool GetDHCP(string interfaceName)
        {
            bool ret = false;
            FileInfo fi = new FileInfo(NETWORK_CONFIGS_DIRECTORY+"/"+NETWORK_CONFIG_FILENAME.Replace("$deviceName$",interfaceName));
            if (fi.Exists){
                StreamReader sr = new StreamReader(fi.OpenRead());
                string content = sr.ReadToEnd();
                sr.Close();
                ret = content.Contains("BOOTPROTO=dhcp");
            }
            return ret;
        }
    }
}
