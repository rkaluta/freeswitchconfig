using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using System.Net.NetworkInformation;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Overrides.Windows
{
    public class NetworkSettingsOverride : MarshalByRefObject, IOperatingSystemOverrideContainer
    {

        private static readonly Regex _regSearch = new Regex("\\s+Lost\\s+=\\s+\\d+\\s+\\(([\\d\\.]+)%\\s+loss\\)", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static readonly Regex _regMAC = new Regex("\\s*(([a-e0-9]{2}-){5}[a-e0-9]{2})\\s*", RegexOptions.Compiled | RegexOptions.ECMAScript);

        private Mutex _mut = new Mutex();

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "RunDiagnostics",
            "Windows",
            "5.0",
            "6.9")]
        public List<string> RunDiagnostics()
        {
            return new List<string>();
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "RequiredCommands",
            "Windows",
            "5.0",
            "6.9")]
        public List<NameValuePair> RequiredCommands
        {
            get
            {
                return new List<NameValuePair>();
            }
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "Interfaces",
            "Windows",
            "5.0",
            "6.9")]
        public List<string> Interfaces
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (!ni.Name.StartsWith("isatap")&&!(ni.Name.Contains("Pseudo")&&!ni.Name.Contains("Loopback"))&&!(ni.Name.Contains("Virtual")&&ni.Name.Contains("Miniport")))
                        ret.Add(ni.Name);
                }
                return ret;
            }
        }

        private NetworkInterface GetInterfaceByName(string interfaceName)
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.Name == interfaceName)
                    return ni;
            }
            return null;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetInterfaceSpeed",
            "Windows",
            "5.0",
            "6.9")]
        public NetworkSpeeds GetInterfaceSpeed(string interfaceName)
        {
            return NetworkSpeeds.Unknown;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetIPAddress",
            "Windows",
            "5.0",
            "6.9")]
        public string GetIPAddress(string interfaceName)
        {
            string[] tmp = Utility.ExecuteProgram("netsh", "interface ip show addresses \"" + interfaceName + "\"", true).Split('\n');
            for (int x = 0; x < tmp.Length; x++)
            {
                if (tmp[x].Contains("IP Address"))
                {
                    return tmp[x].Split(':')[1].ToLower().Trim();
                }
            }
            return null;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetDHCP",
            "Windows",
            "5.0",
            "6.9")]
        public bool GetDHCP(string interfaceName)
        {
            bool ret = false;
            string[] tmp = Utility.ExecuteProgram("netsh", "interface ip show addresses \""+interfaceName+"\"", true).Split('\n');
            for (int x = 0; x < tmp.Length; x++)
            {
                if (tmp[x].Contains("DHCP enabled"))
                {
                    ret = tmp[x].Split(':')[1].ToLower().Trim() == "yes";
                }
            }
            return ret;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetNetworkMask",
            "Windows",
            "5.0",
            "6.9")]
        public string GetNetworkMask(string interfaceName)
        {
            string[] tmp = Utility.ExecuteProgram("netsh", "interface ip show addresses \"" + interfaceName + "\"", true).Split('\n');
            for (int x = 0; x < tmp.Length; x++)
            {
                if (tmp[x].Contains("Subnet Prefix"))
                {
                    return tmp[x].Substring(tmp[x].IndexOf("mask ") + "mask ".Length).Replace(")", "").Trim();
                }
            }
            return null;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "IsInterfaceBondMaster",
            "Windows",
            "5.0",
            "6.9")]
        public bool IsInterfaceBondMaster(string interfaceName)
        {
            return false;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "IsInterfaceBondSlave",
            "Windows",
            "5.0",
            "6.9")]
        public bool IsInterfaceBondSlave(string interfaceName)
        {
            return false;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetBondParent",
            "Windows",
            "5.0",
            "6.9")]
        public string GetBondParent(string interfaceName)
        {
            return null;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetBroadcast",
            "Windows",
            "5.0",
            "6.9")]
        public string GetBroadcast(string interfaceName)
        {
            string addy = GetIPAddress(interfaceName);
            string mask = GetNetworkMask(interfaceName);
            if ((addy == null) || (mask == null))
                return null;
            string ret = null;
            try
            {
                ret = IPAddressExtensions.GetBroadcastAddress(IPAddress.Parse(addy), IPAddress.Parse(mask)).ToString();
            }
            catch (Exception e)
            {
                ret = null;
            }
            return ret;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetGateway",
            "Windows",
            "5.0",
            "6.9")]
        public string GetGateway(string interfaceName)
        {
            string[] tmp = Utility.ExecuteProgram("netsh", "interface ip show addresses \""+interfaceName+"\"", true).Split('\n');
            string ret = "";
            for (int x = 0; x < tmp.Length; x++)
            {
                if (tmp[x].Contains("Default Gateway:"))
                {
                    ret = tmp[x].Split(':')[1].Trim();
                }
            }
            return ret;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetMACAddress",
            "Windows",
            "5.0",
            "6.9")]
        public string GetMACAddress(string interfaceName)
        {
            NetworkInterface ni = GetInterfaceByName(interfaceName);
            if (ni!=null)
                return ni.GetPhysicalAddress().ToString();
            return null;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "IsInterfaceLive",
            "Windows",
            "5.0",
            "6.9")]
        public bool IsInterfaceLive(string interfaceName)
        {
            NetworkInterface ni = GetInterfaceByName(interfaceName);
            if (ni!=null)
                return ni.OperationalStatus == OperationalStatus.Up;
            return false;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetPacketLoss",
            "Windows",
            "5.0",
            "6.9")]
        public string GetPacketLoss(string interfaceName, string destination, int count)
        {
            string res = Utility.ExecuteProgram("ping", "-n " + count.ToString() + " -S " + GetIPAddress(interfaceName), true);
            return _regSearch.Match(res).Groups[1].Value;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetMACForIP",
            "Windows",
            "5.0",
            "6.9")]
        public string GetMACForIP(string interfaceName, string ipaddress)
        {
            GetPacketLoss(interfaceName, ipaddress, 1);
            string res = Utility.ExecuteProgram("arp", "-a", true);
            string ret = "";
            foreach (string str in res.Split('\n'))
            {
                if (str.Trim().StartsWith(ipaddress))
                {
                    ret = _regMAC.Match(str).Groups[1].Value;
                    break;
                }
            }
            return ret;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetIPForMAC",
            "Windows",
            "5.0",
            "6.9")]
        public string GetIPForMAC(string mac)
        {
            mac = mac.Replace(":", "-").Replace("-", "");
            string res = Utility.ExecuteProgram("arp", "-a", true);
            string ret = "";
            foreach (string str in res.Split('\n'))
            {
                if (_regMAC.Matches(str).Count > 0)
                {
                    if (_regMAC.Match(str).Groups[1].Value.Replace("-","")==mac)
                    {
                        ret = str.Trim().Substring(0, str.Trim().IndexOf(" "));
                        break;
                    }
                }
            }
            return ret;
        }
    }
}
