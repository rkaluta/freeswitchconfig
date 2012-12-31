using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using System.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Overrides.SliTaz3x
{
    [RequiredSudoPath("ip",IPCommand, "SliTaz","3.0",null, "Used to list interfaces and obtain the default gateway")]
    public class NetworkSettingsOverride : MarshalByRefObject, IOperatingSystemOverrideContainer
    {
        private const string DHCP_CLIENT = "/sbin/udhcpc-i{0}";
        public const string IPCommand = "/bin/ip";

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "RequiredCommands",
            "SliTaz",
            "3.0",
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
                        new NameValuePair("ip",IPCommand),
                        new NameValuePair("ping",NetworkSettings.PINGCommand),
                        new NameValuePair("mii-tool",NetworkSettings.MIITOOLCommand)
                    }
                );
            }
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetInterfaceSpeed",
            "SliTaz",
            "3.0",
            null)]
        public NetworkSpeeds GetInterfaceSpeed(string interfaceName)
        {
            NetworkSpeeds ret = NetworkSpeeds.HalfDuplex_10MBps;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, NetworkSettings.MIITOOLCommand + " " + interfaceName,true);
            Log.Trace("Results from mii-tool: " + results);
            foreach (string str in results.Split('\n'))
            {
                if (str.StartsWith(interfaceName))
                {
                    if (str.Contains("negotiated"))
                    {
                        switch (str.Split(' ')[2])
                        {
                            case "10baseT-HD":
                                ret = NetworkSpeeds.HalfDuplex_10MBps;
                                break;
                            case "10baseT-FD":
                                ret = NetworkSpeeds.FullDuplex_10MBps;
                                break;
                            case "100baseT-HD":
                                ret = NetworkSpeeds.HalfDuplex_100MBps;
                                break;
                            case "100baseT-FD":
                                ret = NetworkSpeeds.FullDuplex_100MBps;
                                break;
                            case "1000baseT-HD":
                                ret = NetworkSpeeds.HalfDuplex_1GBps;
                                break;
                            case "1000baseT-FD":
                                ret = NetworkSpeeds.FullDuplex_1GBps;
                                break;
                        }
                    }
                    break;
                }
            }
            return ret;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetDHCP",
            "SliTaz",
            "3.0",
            null)]
        public bool GetDHCP(string interfaceName)
        {
            bool ret = false;
            foreach (DirectoryInfo di in new DirectoryInfo("/proc").GetDirectories())
            {
                FileInfo fi = new FileInfo(di.FullName + "/cmdline");
                if (fi.Exists)
                {
                    StreamReader sr = new StreamReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    string line = sr.ReadToEnd();
                    sr.Close();
                    if (line == string.Format(DHCP_CLIENT, interfaceName))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "Interfaces",
            "SliTaz",
            "3.0",
            null)]
        public List<string> Interfaces
        {
            get {
                List<string> ret = new List<string>();
                string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, NetworkSettings.IFCONFIGCommand + " -a", true);
                Log.Trace("Results from ifconfig: " + results);
                foreach (string str in results.Split('\n'))
                {
                    if (str.StartsWith("eth")||str.StartsWith("bond"))
                        ret.Add(str.Substring(0, str.IndexOf(" ")));
                }
                return ret;
            }
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.NetworkSettings",
            "GetGateway",
            "SliTaz",
            "3.0",
            null)]
        public string GetGateway(string interfaceName)
        {
            string ret = null;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, IPCommand + " route show 0.0.0.0/0 dev " + interfaceName, true);
            ret = results.Substring(results.IndexOf("via") + 3);
            ret = ret.Trim();
            return ret;
        }
    }
}
