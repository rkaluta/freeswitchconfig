using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using System.Threading;
using System.Text.RegularExpressions;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Overrides.Debian
{
    [RequiredSudoPath("arp",ARPCommand, "Debian", "5.0", null, "Get the MAC address for IP")]
    [RequiredSudoPath("arping",ARPPINGCommand, "Debian", "5.0", null, "Used to Ping and Address to obtain its MAC")]
    internal class NetworkSettingsOverride : MarshalByRefObject, IOperatingSystemOverrideContainer
    {
        internal const string ARPCommand = "/usr/sbin/arp";
        internal const string ARPPINGCommand = "/usr/sbin/arping";
        internal const string NETWORK_CONFIGS_FILE = "/etc/network/interfaces";

        private Mutex _mut = new Mutex();

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "RunDiagnostics",
            "Debian",
            "5.0",
            null)]
        internal List<string> RunDiagnostics()
        {
            List<string> ret = new List<string>();
            if (!(new FileInfo(NETWORK_CONFIGS_FILE).Exists))
                ret.Add("Unable to locate the interfaces configuration file on the system.");
            else
                ret.Add("Succesffully located the interfaces configuration file on the system.");
            return ret;
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "RequiredCommands",
            "Debian",
            "5.0",
            null)]
        internal List<NameValuePair> RequiredCommands
        {
            get
            {
                return new List<NameValuePair>(
                    new NameValuePair[]{
                        new NameValuePair("arp", ARPCommand),
                        new NameValuePair("arping",ARPPINGCommand),
                        new NameValuePair("ifconfig",NetworkSettings.IFCONFIGCommand),
                        new NameValuePair("ip",NetworkSettings.IPCommand),
                        new NameValuePair("ping",NetworkSettings.PINGCommand),
                    }
                );
            }
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetDHCP",
            "Debian",
            "5.0",
            null)]
        internal bool GetDHCP(string interfaceName)
        {
            return NetworkConfigFileContent.Contains("iface " + interfaceName + " inet dhcp");
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "IsInterfaceBondMaster",
            "Debian",
            "5.0",
            null)]
        internal bool IsInterfaceBondMaster(string interfaceName)
        {
            return NetworkConfigFileContent.Contains("iface " + interfaceName + " inet");
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "IsInterfaceBondSlave",
            "Debian",
            "5.0",
            null)]
        internal bool IsInterfaceBondSlave(string interfaceName)
        {
            string content = NetworkConfigFileContent;
            bool ret = false;
            foreach (string str in content.Split('\n'))
            {
                if (str.StartsWith("up /sbin/ifenslave "))
                {
                    ret |= str.Contains(" " + interfaceName);
                }
            }
            return ret;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetBondParent",
            "Debian",
            "5.0",
            null)]
        internal string GetBondParent(string interfaceName)
        {
            string ret = null;
            string[] content = NetworkConfigFileContent.Split('\n');
            for (int x = 0; x < content.Length; x++)
            {
                if (content[x].StartsWith("iface"))
                {
                    string iface = content[x].Split(' ')[1];
                    while (content[x] != "")
                    {
                        if (content[x].StartsWith("up /sbin/ifenslave "))
                        {
                            if (content[x].Contains(" " + interfaceName))
                            {
                                ret = iface;
                                break;
                            }
                        }
                        x++;
                    }
                }
                if (ret != null)
                    break;
            }
            return ret;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetMACForIP",
            "Debian",
            "5.0",
            null)]
        internal string GetMACForIP(string interfaceName, string ipaddress)
        {
            string ret = "";
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, ARPPINGCommand + " -c 1 -I " + interfaceName + " " + ipaddress, true);
            Log.Trace("Getting results for arp from " + results);
            if (results.Contains("reply from " + ipaddress))
            {
                foreach (string str in results.Split('\n'))
                {
                    if (str.Contains("reply from") && str.Contains(ipaddress) && (NetworkSettings._regMAC.Matches(str).Count > 0))
                    {
                        Log.Trace("Using line " + str + " for arp.");
                        Match m = NetworkSettings._regMAC.Match(str);
                        ret = m.Groups[1].Value;
                        Regex r = new Regex(Constants.IPADDRESS_REGEX_STRING.Substring(1, Constants.IPADDRESS_REGEX_STRING.Length - 2), RegexOptions.Compiled | RegexOptions.ECMAScript);
                        ret = r.Match(str).Groups[0].Value;
                        Log.Trace("return MAC of " + ret + " for arp");
                        break;
                    }
                }
            }
            return (ret == "" ? null : ret);
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetIPForMAC",
            "Debian",
            "5.0",
            null)]
        internal string GetIPForMAC(string mac)
        {
            mac = mac.Replace(":", "-").Replace("-", "");
            string ret = null;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, ARPCommand + " -na", true);
            Log.Trace("Getting results for arp from " + results);
            foreach (string str in results.Split('\n'))
            {
                Match m = NetworkSettings._regMAC.Match(str);
                if (mac == m.Groups[1].Value.Replace(":", ""))
                {
                    Log.Trace("Using line " + str + " for arp.");
                    Log.Trace("return IP of " + ret + " for arp mac " + mac);
                }
            }
            return ret;
        }

        private string NetworkConfigFileContent
        {
            get
            {
                _mut.WaitOne();
                StreamReader sr = new StreamReader(new FileStream(NETWORK_CONFIGS_FILE, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                string ret = sr.ReadToEnd();
                sr.Close();
                return ret;
            }
        }
    }
}
