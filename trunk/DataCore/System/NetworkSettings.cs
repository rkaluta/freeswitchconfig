using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Org.Reddragonit.Stringtemplate;
using System.Text.RegularExpressions;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using System.Collections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using Org.Reddragonit.EmbeddedWebServer.Components;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System
{
    [RequiredSudoPath("ifconfig",IFCONFIGCommand,"Get the ip address information")]
    [RequiredSudoPath("arp",ARPCommand,"Get the MAC address for IP")]
    [RequiredSudoPath("arping",ARPPINGCommand,"Used to Ping and Address to obtain its MAC")]
    [RequiredSudoPath("ip",IPCommand,"Used to list interfaces and obtain the default gateway")]
    [RequiredSudoPath("ping",PINGCommand,"Used to test connectivity to an IP")]
    [RequiredSudoPath("mii-tool",MIITOOLCommand,"Used to obtain the Interface Speed")]
    public class NetworkSettings : MarshalByRefObject,IDiagnosable,IBackgroundOperationContainer
    {       
        private const string IFACE_STATE_FILE_PATH = "/sys/class/net/{0}/operstate";
        private const int _CACHE_TIMEOUT = 10;

        private static readonly Regex _regSearch = new Regex("([\\d\\.]+)%\\s+packet\\s+loss", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static readonly Regex _regIface = new Regex("^(lo|((eth|wlan|vboxnet|pan|vmnet|ppp|venet)\\d+(:\\d+)?))\\s+",RegexOptions.Compiled|RegexOptions.ECMAScript);
        private static readonly Regex _regSubInterface = new Regex("^((lo|((eth|wlan|vboxnet|pan|vmnet|ppp|venet)\\d+):\\d+$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        
        public const string IFCONFIGCommand = "/sbin/ifconfig";
        public const string ARPCommand = "/sbin/arp";
        public const string ARPPINGCommand = "/sbin/arping";
        public const string IPCommand = "/sbin/ip";
        public const string PINGCommand = "/bin/ping";
        public const string MIITOOLCommand = "/sbin/mii-tool";
        public const string BondingProcessDirectory = "/proc/net/bonding";

        [OperatingSystemOverridablePropertyAttribute("The list of required command paths and command names in order to run the network configurations.  It returnes these values in a List<NameValuePair> where name is the command name description, value is the command.")]
        public List<NameValuePair> RequiredCommands
        {
            get
            {
                return new List<NameValuePair>(
                    new NameValuePair[]{
                        new NameValuePair("arp", ARPCommand),
                        new NameValuePair("arpping",ARPPINGCommand),
                        new NameValuePair("ifconfig",IFCONFIGCommand),
                        new NameValuePair("ip",IPCommand),
                        new NameValuePair("ping",PINGCommand),
                        new NameValuePair("mii-tool",MIITOOLCommand)
                    }
                );
            }
        }


        [DiagnosticFunctionAttribute("Network Settings")]
        public static List<string> RunDefaultDiagnostics()
        {
            return NetworkSettings.Current.RunDiagnostics();
        }

        [OperatingSystemOverridableFunctionAttribute("The diagnostics analysis function for the network settings.  It returns a List<string> variable and has no inputs")]
        public List<string> RunDiagnostics()
        {
            List<String> ret = new List<string>();
            List<string> results = ProcessSecurityControl.Current.SudoCommands;
            foreach (NameValuePair nvp in Current.RequiredCommands)
            {
                if (!results.Contains(nvp.Value.ToString()))
                    ret.Add("Running user " + (string)Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME] + " DOES NOT have access to " + nvp.Name + " command through sudo.");
                else
                    ret.Add("Running user " + (string)Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME] + " has access to " + nvp.Name + " command through sudo.");
            }
            foreach (string str in NetworkSettings.Current.InterfaceNames)
            {
                if (!NetworkSettings.Current[str].Live)
                    ret.Add("The interface " + str + " has NO link detected.");
                else
                    ret.Add("The interface " + str + " has a link detected.");
            }
            return ret;
        }

        [BackgroundOperationCall(-1, -1, -1, -1, BackgroundOperationDaysOfWeek.All)]
        public static void CleanupCache()
        {
            lock (_cards)
            {
                DateTime start = DateTime.Now;
                string[] keys = new string[_cards.Count];
                _cards.Keys.CopyTo(keys, 0);
                foreach (string key in keys)
                {
                    if (start.Subtract(_cards[key].LastAccess).TotalMinutes > _CACHE_TIMEOUT)
                        _cards.Remove(key);
                }
            }
        }

        private static Dictionary<string, CachedItemContainer> _cards = new Dictionary<string, CachedItemContainer>();
        public sNetworkCard this[string name]
        {
            get {
                sNetworkCard ret = null;
                lock (_cards)
                {
                    if (!_cards.ContainsKey(name))
                    {
                        ret = new sNetworkCard(name);
                        _cards.Add(name, new CachedItemContainer(ret));
                    }
                    else
                        ret = (sNetworkCard)_cards[name].Value;
                }
                return ret; 
            }
        }

        public List<string> InterfaceNames
        {
            get
            {
                return NetworkSettings.Current.Interfaces;
            }
        }

        [OperatingSystemOverridablePropertyAttribute("This property returns the list of available interfaces on the system.(This is a readonly property)")]
        internal List<string> Interfaces
        {
            get {
                try
                {
                    List<string> ret = new List<string>();
                    string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, IFCONFIGCommand+" -a", true);
                    Log.Trace("Results from "+IPCommand+": " + results);
                    foreach (string str in results.Split('\n'))
                    {
                        if (_regIface.Matches(str).Count > 0)
                            ret.Add(_regIface.Matches(str)[0].Value.Trim());
                    }
                    return ret;
                }
                catch (Exception e)
                {
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                    Log.Error(e);
                    return null;
                }
            }
        }

        [OperatingSystemOverridableFunctionAttribute("This function returns (NetworkSpeeds enum value) the current speed of the specified itnerface.(Parameters (string interface)).")]
        internal NetworkSpeeds GetInterfaceSpeed(string interfaceName)
        {
            NetworkSpeeds ret = NetworkSpeeds.Unknown;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, MIITOOLCommand + " "+interfaceName,true);
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
                            case "10baseTx-HD":
                                ret = NetworkSpeeds.HalfDuplex_10MBps;
                                break;
                            case "10baseT-FD":
                            case "10baseTx-FD":
                                ret = NetworkSpeeds.FullDuplex_10MBps;
                                break;
                            case "100baseT-HD":
                            case "100baseTx-HD":
                                ret = NetworkSpeeds.HalfDuplex_100MBps;
                                break;
                            case "100baseT-FD":
                            case "100baseTx-FD":
                                ret = NetworkSpeeds.FullDuplex_100MBps;
                                break;
                            case "1000baseT-HD":
                            case "1000baseTx-HD":
                                ret = NetworkSpeeds.HalfDuplex_1GBps;
                                break;
                            case "1000baseT-FD":
                            case "1000baseTx-FD":
                                ret = NetworkSpeeds.FullDuplex_1GBps;
                                break;
                        }
                    }
                    break;
                }
            }
            return ret;
        }

        private Mutex _mut = new Mutex();
        private static NetworkSettings _current;

        public static NetworkSettings Current
        {
            get { return _current; }
        }

        static NetworkSettings(){
            if (Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME] == null)
                Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME] = Constants.DEFAULT_RUNNING_USERNAME;
            if (_current == null)
            {
                try
                {
                    _current = (NetworkSettings)OSClassProxy.Instance(new NetworkSettings());
                }
                catch (Exception e)
                {
                    _current = null;
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                    Log.Error(e);
                }
            }
        }

        private NetworkSettings()
        {
        }

        private static readonly Regex _inetRegex = new Regex("inet addr:"+Constants.IPADDRESS_REGEX_STRING+"\\s",RegexOptions.Compiled|RegexOptions.ECMAScript);

        [OperatingSystemOverridableFunction("Function call to get the IP Address of a specified interface.  Parameters(string interfaceName)")]
        internal string GetIPAddress(string interfaceName)
        {
            string ret = null;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand,IFCONFIGCommand+" "+interfaceName,true);
            ret = _inetRegex.Match(results).Value;
            Log.Trace("Inet address line found for interface "+interfaceName+" = "+ret);
            ret = ret.Substring(ret.IndexOf(":")+1).Trim();
            Log.Trace("Returning IP Address "+ret+" for interface "+interfaceName);
            return ret;
        }

        public string GetInterfaceForIPAddress(IPAddress address)
        {
            if (address == IPAddress.Any)
                return null;
            foreach (string str in NetworkSettings.Current.InterfaceNames)
            {
                if (address.ToString() == NetworkSettings.Current[str].IPAddress)
                    return str;
            }
            return null;
        }

        [OperatingSystemOverridableFunctionAttribute("Function call to specify if an interface is DHCP or not.  Parameters(string interfaceName)")]
        internal bool GetDHCP(string interfaceName)
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
                    if (line.Contains("dhclient") && line.EndsWith(interfaceName))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        private static readonly Regex _netMaskRegex = new Regex("Mask:"+Constants.IPADDRESS_REGEX_STRING+"\\s",RegexOptions.Compiled|RegexOptions.ECMAScript);

        [OperatingSystemOverridableFunction("Function call to get the Network Mask of a specified interface.  Parameters(string interfaceName)")]
        internal string GetNetworkMask(string interfaceName)
        {
            string ret = null;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, IFCONFIGCommand + " " + interfaceName,true);
            ret = _netMaskRegex.Match(results).Value;
            ret = ret.Substring(ret.IndexOf(":")+1).Trim();
            Log.Trace("Obtained network mask of " + ret + " for interface " + interfaceName);
            ret = (ret.Length == 0 ? null : ret);
            return ret;
        }

        [OperatingSystemOverridableFunction("Function call to Determine if an interface is a bond master.  Parameters(string interfaceName)")]
        internal bool IsInterfaceBondMaster(string interfaceName)
        {
            DirectoryInfo di = new DirectoryInfo(BondingProcessDirectory);
            if (di.Exists)
                return di.GetFiles(interfaceName).Length > 0;
            return false;
        }

        [OperatingSystemOverridableFunction("Function call to Determine if an interface is a bond slave.  Parameters(string interfaceName)")]
        internal bool IsInterfaceBondSlave(string interfaceName)
        {
            DirectoryInfo di = new DirectoryInfo(BondingProcessDirectory);
            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    StreamReader sr = new StreamReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
                    string content = sr.ReadToEnd();
                    sr.Close();
                    if (content.Contains("Slave Interface: " + interfaceName + "\n"))
                        return true;
                }
            }
            return false;
        }

        [OperatingSystemOverridableFunction("Function call to get the name of the bond master interface for the slave interface.  Parameters(string interfaceName)")]
        internal string GetBondParent(string interfaceName)
        {
            DirectoryInfo di = new DirectoryInfo(BondingProcessDirectory);
            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    StreamReader sr = new StreamReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
                    string content = sr.ReadToEnd();
                    sr.Close();
                    if (content.Contains("Slave Interface: " + interfaceName + "\n"))
                        return fi.Name;
                }
            }
            return null;
        }

        private static readonly Regex _broadcastRegex = new Regex("Bcast:" + Constants.IPADDRESS_REGEX_STRING + "\\s", RegexOptions.Compiled | RegexOptions.ECMAScript);

        [OperatingSystemOverridableFunction("Function call to get the Broadcast of an interface.  Parameters(string interfaceName)")]
        internal string GetBroadcast(string interfaceName)
        {
            string ret = null;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, IFCONFIGCommand + " " + interfaceName,true);
            ret = _broadcastRegex.Match(results).Value;
            ret = ret.Substring(ret.IndexOf(":") + 1).Trim();
            return ret;
        }

        [OperatingSystemOverridableFunction("Function call to set the Gateway of an interface.  Parameters(string interfaceName)")]
        internal string GetGateway(string interfaceName)
        {
            string ret = null;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, IPCommand + " route list dev " + interfaceName, true);
            foreach (string str in results.Split('\n'))
            {
                if (str.StartsWith("default"))
                {
                    ret = str.Substring(str.IndexOf("via")+3);
                    ret = ret.Trim();
                    break;
                }
            }
            return ret;
        }

        private static readonly Regex _regHWMAC = new Regex("HWaddr\\s*\\[(([0-9A-Fa-f][0-9A-Fa-f]:){5}[0-9A-Fa-f][0-9A-Fa-f])\\]\\s*", RegexOptions.Compiled | RegexOptions.ECMAScript);

        [OperatingSystemOverridableFunction("Function call to set the MACAddress of an interface.  Parameters(string interfaceName)")]
        internal string GetMACAddress(string interfaceName)
        {
            string ret = null;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, IFCONFIGCommand + " " + interfaceName,true);
            ret = ret = _regHWMAC.Match(results).Value;
            ret = ret.Substring(ret.IndexOf(" ") + 1).Trim();
            return ret;
        }

        [OperatingSystemOverridableFunction("Function call to determine if an interface is live.Parameters(string interfaceName)")]
        internal bool IsInterfaceLive(string interfaceName)
        {
            if (new FileInfo(string.Format(IFACE_STATE_FILE_PATH, interfaceName)).Exists)
            {
                StreamReader sr = new StreamReader(new FileStream(string.Format(IFACE_STATE_FILE_PATH, interfaceName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                string results = sr.ReadToEnd();
                sr.Close();
                return results.Trim().ToUpper() == "UP" || results.Trim().ToUpper() == "UNKOWN";
            }
            else if (_regSubInterface.IsMatch(interfaceName))
                return IsInterfaceLive(_regSubInterface.Match(interfaceName).Groups[1].Value);
            return true;
        }

        [OperatingSystemOverridableFunction("Function call to determine the packet loss to a given destination by an interface.Parameters(string interfaceName,string destination,int count)")]
        public string GetPacketLoss(string interfaceName, string destination,int count)
        {
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, PINGCommand + " " + "-c " + count.ToString() + " -I " + interfaceName + " " + destination,true);
            Log.Trace("Results from PING: " + results);
            Log.Trace("RegMatch: " + _regSearch.Match(results).Value);
            Log.Trace("RegGroup(% Loss): " + _regSearch.Match(results).Groups[1].Value);
            return _regSearch.Match(results).Groups[1].Value;
        }

        [OperatingSystemOverridableFunction("Function call to determine the MAC address for a given IP.Parameters(string interfaceName,string ipaddress)")]
        public string GetMACForIP(string interfaceName, string ipaddress)
        {
            string ret = "";
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, ARPPINGCommand + " -c 1 -I " + interfaceName + " " + ipaddress, true);
            Log.Trace("Getting results for arp from " + results);
            if (results.Contains("reply from " + ipaddress))
            {
                foreach (string str in results.Split('\n'))
                {
                    if (str.Contains("reply from") && str.Contains(ipaddress) && (_regMAC.Matches(str).Count > 0))
                    {
                        Log.Trace("Using line " + str + " for arp.");
                        Match m = _regMAC.Match(str);
                        ret = _regMAC.Match(str).Groups[1].Value;
                        Regex r = new Regex(Constants.IPADDRESS_REGEX_STRING.Substring(1, Constants.IPADDRESS_REGEX_STRING.Length - 2), RegexOptions.Compiled | RegexOptions.ECMAScript);
                        ret = r.Match(str).Groups[0].Value;
                        Log.Trace("return MAC of " + ret + " for arp");
                        break;
                    }
                }
            }
            return (ret == "" ? null : ret);
        }

        internal static readonly Regex _regMAC = new Regex("HWaddr\\s*\\[(([0-9A-Fa-f][0-9A-Fa-f]:){5}[0-9A-Fa-f][0-9A-Fa-f])\\]\\s*", RegexOptions.Compiled | RegexOptions.ECMAScript);

        [OperatingSystemOverridableFunction("Function call to determine the IP address for a given MAC.Parameters(string mac)")]
        public string GetIPForMAC(string mac)
        {
            mac = mac.Replace(":", "-").Replace("-", "");
            string ret = null;
            string results = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, ARPCommand + " -na",true);
            Log.Trace("Getting results for arp from " + results);
            foreach (string str in results.Split('\n'))
            {
                Match m = _regMAC.Match(str);
                if (mac == _regMAC.Match(str).Groups[1].Value.Replace(":", ""))
                {
                    Log.Trace("Using line " + str + " for arp.");
                    Log.Trace("return IP of " + ret + " for arp mac "+mac);
                }
            }
            return ret;
        }
    }
}
