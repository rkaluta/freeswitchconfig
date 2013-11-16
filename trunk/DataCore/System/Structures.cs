using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet;
using System.ComponentModel;
using System.Net.NetworkInformation;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System
{
    public struct sCPU
    {
        private int _socketNumber;
        public int SocketNumber
        {
            get { return _socketNumber; }
        }

        private int _speed;
        public int Speed
        {
            get { return _speed; }
        }

        public sCPU(int socketNumber, int speed)
        {
            _socketNumber = socketNumber;
            _speed = speed;
        }
    }

    public struct sMemoryValue
    {
        private long _value;
        public long Value
        {
            get { return _value; }
        }

        private ByteUnits _unit;
        public ByteUnits Unit
        {
            get { return _unit; }
        }

        public sMemoryValue(long value, ByteUnits unit)
        {
            _value = value;
            _unit=unit;
        }

        public override string ToString()
        {
            string ret = "";
            long tmp;
            switch (_unit)
            {
                case ByteUnits.Byte:
                    if (_value >= Constants.TB_BYTE_COUNT)
                        ret = Math.Round((decimal)_value / (decimal)Constants.TB_BYTE_COUNT, 2).ToString() + " TB";
                    else if (_value >= Constants.GB_BYTE_COUNT)
                        ret = Math.Round((decimal)_value / (decimal)Constants.GB_BYTE_COUNT, 2).ToString() + " GB";
                    else if (_value >= Constants.MB_BYTE_COUNT)
                        ret = Math.Round((decimal)_value / (decimal)Constants.MB_BYTE_COUNT, 2).ToString() + " MB";
                    else if (_value >= Constants.KB_BYTE_COUNT)
                        ret = Math.Round((decimal)_value / (decimal)Constants.KB_BYTE_COUNT, 2).ToString() + " kB";
                    else
                        ret = _value.ToString() + " B";
                    break;
                case ByteUnits.KiloByte:
                    tmp = _value * Constants.KB_BYTE_COUNT;
                    if (tmp >= Constants.TB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.TB_BYTE_COUNT, 2).ToString() + " TB";
                    else if (tmp >= Constants.GB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.GB_BYTE_COUNT, 2).ToString() + " GB";
                    else if (tmp >= Constants.MB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.MB_BYTE_COUNT, 2).ToString() + " MB";
                    else if (tmp >= Constants.KB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.KB_BYTE_COUNT, 2).ToString() + " kB";
                    else
                        ret = tmp.ToString() + " B";
                    break;
                case ByteUnits.MegaByte:
                    tmp = _value * Constants.MB_BYTE_COUNT;
                    if (tmp >= Constants.TB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.TB_BYTE_COUNT, 2).ToString() + " TB";
                    else if (tmp >= Constants.GB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.GB_BYTE_COUNT, 2).ToString() + " GB";
                    else if (tmp >= Constants.MB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.MB_BYTE_COUNT, 2).ToString() + " MB";
                    else if (tmp >= Constants.KB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.KB_BYTE_COUNT, 2).ToString() + " kB";
                    else
                        ret = tmp.ToString() + " B";
                    break;
                case ByteUnits.GigaByte:
                    tmp = _value * Constants.GB_BYTE_COUNT;
                    if (tmp >= Constants.TB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.TB_BYTE_COUNT, 2).ToString() + " TB";
                    else if (tmp >= Constants.GB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.GB_BYTE_COUNT, 2).ToString() + " GB";
                    else if (tmp >= Constants.MB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.MB_BYTE_COUNT, 2).ToString() + " MB";
                    else if (tmp >= Constants.KB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.KB_BYTE_COUNT, 2).ToString() + " kB";
                    else
                        ret = tmp.ToString() + " B";
                    break;
                case ByteUnits.TeraByte:
                    tmp = _value * Constants.TB_BYTE_COUNT;
                    if (tmp >= Constants.TB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.TB_BYTE_COUNT, 2).ToString() + " TB";
                    else if (tmp >= Constants.GB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.GB_BYTE_COUNT, 2).ToString() + " GB";
                    else if (tmp >= Constants.MB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.MB_BYTE_COUNT, 2).ToString() + " MB";
                    else if (tmp >= Constants.KB_BYTE_COUNT)
                        ret = Math.Round((decimal)tmp / (decimal)Constants.KB_BYTE_COUNT, 2).ToString() + " kB";
                    else
                        ret = tmp.ToString() + " B";
                    break;
            }
            return ret;
        }

        public decimal ToUnit(ByteUnits unit)
        {
            if (unit==_unit)
                return (decimal)_value;
            decimal ret = 0;
            switch (_unit)
            {
                case ByteUnits.Byte:
                    switch (unit)
                    {
                        case ByteUnits.KiloByte:
                            ret = (decimal)_value / (decimal)Constants.KB_BYTE_COUNT;
                            break;
                        case ByteUnits.MegaByte:
                            ret = (decimal)_value / (decimal)Constants.MB_BYTE_COUNT;
                            break;
                        case ByteUnits.GigaByte:
                            ret = (decimal)_value / (decimal)Constants.GB_BYTE_COUNT;
                            break;
                        case ByteUnits.TeraByte:
                            ret = (decimal)_value / (decimal)Constants.TB_BYTE_COUNT;
                            break;
                    }
                    break;
                case ByteUnits.KiloByte:
                    switch (unit)
                    {
                        case ByteUnits.Byte:
                            ret = (decimal)_value * (decimal)Constants.KB_BYTE_COUNT;
                            break;
                        case ByteUnits.MegaByte:
                            ret = (decimal)_value / ((decimal)Constants.MB_BYTE_COUNT/(decimal)Constants.KB_BYTE_COUNT);
                            break;
                        case ByteUnits.GigaByte:
                            ret = (decimal)_value / ((decimal)Constants.GB_BYTE_COUNT / (decimal)Constants.KB_BYTE_COUNT);
                            break;
                        case ByteUnits.TeraByte:
                            ret = (decimal)_value / ((decimal)Constants.TB_BYTE_COUNT / (decimal)Constants.KB_BYTE_COUNT);
                            break;
                    }
                    break;
                case ByteUnits.MegaByte:
                    switch (unit)
                    {
                        case ByteUnits.Byte:
                            ret = (decimal)_value * (decimal)Constants.MB_BYTE_COUNT;
                            break;
                        case ByteUnits.KiloByte:
                            ret = (decimal)_value * ((decimal)Constants.MB_BYTE_COUNT / (decimal)Constants.KB_BYTE_COUNT);
                            break;
                        case ByteUnits.GigaByte:
                            ret = (decimal)_value / ((decimal)Constants.GB_BYTE_COUNT / (decimal)Constants.MB_BYTE_COUNT);
                            break;
                        case ByteUnits.TeraByte:
                            ret = (decimal)_value / ((decimal)Constants.TB_BYTE_COUNT / (decimal)Constants.MB_BYTE_COUNT);
                            break;
                    }
                    break;
                case ByteUnits.GigaByte:
                    switch (unit)
                    {
                        case ByteUnits.Byte:
                            ret = (decimal)_value * (decimal)Constants.GB_BYTE_COUNT;
                            break;
                        case ByteUnits.KiloByte:
                            ret = (decimal)_value * ((decimal)Constants.GB_BYTE_COUNT / (decimal)Constants.KB_BYTE_COUNT);
                            break;
                        case ByteUnits.MegaByte:
                            ret = (decimal)_value * ((decimal)Constants.GB_BYTE_COUNT / (decimal)Constants.MB_BYTE_COUNT);
                            break;
                        case ByteUnits.TeraByte:
                            ret = (decimal)_value / ((decimal)Constants.TB_BYTE_COUNT / (decimal)Constants.GB_BYTE_COUNT);
                            break;
                    }
                    break;
                case ByteUnits.TeraByte:
                    switch (unit)
                    {
                        case ByteUnits.Byte:
                            ret = (decimal)_value * (decimal)Constants.TB_BYTE_COUNT;
                            break;
                        case ByteUnits.KiloByte:
                            ret = (decimal)_value * ((decimal)Constants.TB_BYTE_COUNT / (decimal)Constants.KB_BYTE_COUNT);
                            break;
                        case ByteUnits.MegaByte:
                            ret = (decimal)_value * ((decimal)Constants.TB_BYTE_COUNT / (decimal)Constants.MB_BYTE_COUNT);
                            break;
                        case ByteUnits.GigaByte:
                            ret = (decimal)_value * ((decimal)Constants.TB_BYTE_COUNT / (decimal)Constants.GB_BYTE_COUNT);
                            break;
                    }
                    break;
            }
            return ret;
        }
    }

    [ModelRoute("/core/models/sysconfig/NetworkInterface")]
    [ModelJSFilePath("/resources/scripts/Core/SystemConfig/NetworkConfig.js")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View|ModelBlockJavascriptGenerations.CollectionView)]
    [TypeConverter(typeof(sNetworkCardConverter))]
    [ModelNamespace("FreeswitchConfig.System")]
    public class sNetworkCard : IModel, IDiagnosable
    {
        private static readonly string[] _READONLY_INTERFACES = new string[]{
            "lo",
            "ppp",
            "venet"
        };

        [DiagnosticFunctionAttribute("Network Settings")]
        public static List<string> RunDefaultDiagnostics()
        {
            List<String> ret = new List<string>();
            foreach (sNetworkCard crd in LoadAll())
            {
                if (!crd.Live)
                    ret.Add("The interface " + crd.Name + " has NO link detected.");
                else
                    ret.Add("The interface " + crd.Name + " has a link detected.");
            }
            return ret;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public bool ReadOnly
        {
            get
            {
                bool ret = false;
                foreach (string str in _READONLY_INTERFACES)
                {
                    if (_name.StartsWith(str))
                    {
                        ret = true;
                        break;
                    }
                }
                return ret;
            }
        }

        private string _ipAddress;
        public string IPAddress
        {
            get { return _ipAddress; }
        }

        private bool _isDHCP;
        public bool IsDHCP
        {
            get { return _isDHCP; }
        }

        private string _networkMask;
        public string NetworkMask
        {
            get { return _networkMask; }
        }

        private string _broadcast;
        public string Broadcast
        {
            get {
                if (_broadcast == null && _ipAddress != null && _networkMask != null)
                {
                    _broadcast = "";
                    string[] ip = IPAddress.Split('.');
                    string[] mask = NetworkMask.Split('.');
                    if (ip.Length != 4 || mask.Length != 4)
                        return _broadcast;
                    for (int x = 0; x < 4; x++)
                    {
                        int i = int.Parse(ip[x]);
                        int m = int.Parse(mask[x]);
                        _broadcast += ((int)(i | ~m)).ToString() + ".";
                    }
                    _broadcast = _broadcast.Substring(0, _broadcast.Length - 1); ;
                }
                return _broadcast;
            }
        }

        private string _gateway;
        public string Gateway
        {
            get { return _gateway; }
        }

        private string _mac;
        public string MAC {
            get { return _mac; }
        }

        private bool _live;
        public bool Live
        {
            get { return _live; }
        }

        private string _network;
        public string Network
        {
            get {
                if (_network == null &&_ipAddress!=null && _networkMask!=null)
                {
                    _network = "";
                    string[] ip = IPAddress.Split('.');
                    string[] mask = NetworkMask.Split('.');
                    if (ip.Length != 4 || mask.Length != 4)
                        return _network;
                    for (int x = 0; x < 4; x++)
                    {
                        int i = int.Parse(ip[x]);
                        int m = int.Parse(mask[x]);
                        _network += ((int)(i & m)).ToString() + ".";
                    }
                    _network = _network.Substring(0, _network.Length - 1); ;
                }
                return _network; 
            }
        }

        internal sNetworkCard(NetworkInterface ni)
        {
            _name = ni.Name;
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            {
                _mac = null;
                _isDHCP = false;
                _ipAddress = "127.0.0.1";
                _networkMask = "255.255.255.0";
                _broadcast = "127.0.0.1";
                _live = true;
            }else if (ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            {
                _mac = ni.GetPhysicalAddress().ToString();
                _live = ni.OperationalStatus == OperationalStatus.Up;
                _isDHCP = ni.GetIPProperties().GetIPv4Properties().IsDhcpEnabled;
                if ((_live && _isDHCP) || !_isDHCP)
                {
                    foreach (UnicastIPAddressInformation uipa in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (uipa.Address.AddressFamily == global::System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            _ipAddress = uipa.Address.ToString();
                            _networkMask = uipa.IPv4Mask.ToString();
                            break;
                        }
                    }
                    foreach (GatewayIPAddressInformation gipi in ni.GetIPProperties().GatewayAddresses)
                    {
                        if (gipi.Address.AddressFamily == global::System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            _gateway = gipi.Address.ToString();
                            break;
                        }
                    }
                    _network = null;
                    _broadcast = null;
                }
            }
        }

        [ModelLoadMethod()]
        public static sNetworkCard Load(string iface)
        {
            sNetworkCard ret = null;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.Name == iface)
                    ret = new sNetworkCard(ni);
                else
                {
                    if (ni.GetIPProperties().GetIPv4Properties().IsDhcpEnabled && ni.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation uipa in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (uipa.Address.AddressFamily == global::System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                if (iface == uipa.Address.ToString())
                                {
                                    ret = new sNetworkCard(ni);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (ret != null)
                    break;
            }
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<sNetworkCard> LoadAll()
        {
            List<sNetworkCard> ret = new List<sNetworkCard>();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                switch(ni.NetworkInterfaceType){
                    case NetworkInterfaceType.FastEthernetFx:
                    case NetworkInterfaceType.FastEthernetT:
                    case NetworkInterfaceType.GigabitEthernet:
                    case NetworkInterfaceType.Loopback:
                    case NetworkInterfaceType.Unknown:
                    case NetworkInterfaceType.Ethernet:
                    case NetworkInterfaceType.Ethernet3Megabit:
                    case NetworkInterfaceType.Wireless80211:
                    ret.Add(new sNetworkCard(ni));
                break;
                }
            }
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> GetAllInterfaces()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            foreach (string str in AllInterfaceNames)
                ret.Add(new sModelSelectOptionValue(str, str));
            return ret;
        }

        public static List<string> AllInterfaceNames
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    switch (ni.NetworkInterfaceType)
                    {
                        case NetworkInterfaceType.FastEthernetFx:
                        case NetworkInterfaceType.FastEthernetT:
                        case NetworkInterfaceType.GigabitEthernet:
                        case NetworkInterfaceType.Loopback:
                        case NetworkInterfaceType.Unknown:
                        case NetworkInterfaceType.Ethernet:
                        case NetworkInterfaceType.Ethernet3Megabit:
                        case NetworkInterfaceType.Wireless80211:
                            ret.Add(ni.Name);
                            break;
                    }
                }
                return ret;
            }
        }

        public static implicit operator sNetworkCard(string name)
        {
            return sNetworkCard.Load(name);
        }

        public override string ToString()
        {
            return id;
        }

        #region IModel Members

        public string id
        {
            get { return Name; }
        }

        #endregion

    }

    public class sNetworkCardConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.Equals(typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.Equals(typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
                return sNetworkCard.Load((string)value);
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.Equals(typeof(string)))
                return ((sNetworkCard)value).ToString();
            return null;
        }
    }
}
