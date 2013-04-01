using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet;
using System.ComponentModel;

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
    public class sNetworkCard : IModel
    {
        private static readonly string[] _READONLY_INTERFACES = new string[]{
            "lo",
            "ppp",
            "venet"
        };

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

        private NetworkSpeeds _speed;
        public NetworkSpeeds Speed
        {
            get { return _speed; }
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

        private bool _bondMaster;
        public bool IsBondMaster
        {
            get { return _bondMaster; }
        }

        private bool _bondSlave;
        public bool IsBondSlave
        {
            get { return _bondSlave; }
        }

        private string _bondParent;
        public string BondParent
        {
            get { return _bondParent; }
        }

        private string _broadcast;
        public string Broadcast
        {
            get { return _broadcast; }
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

        internal sNetworkCard(string name)
        {
            _name = name;
            _live = NetworkSettings.Current.IsInterfaceLive(name);
            _isDHCP = NetworkSettings.Current.GetDHCP(name);
            _bondMaster = NetworkSettings.Current.IsInterfaceBondMaster(name);
            if (!_bondMaster)
                _bondSlave = NetworkSettings.Current.IsInterfaceBondSlave(name);
            if (_bondSlave)
                _bondParent = NetworkSettings.Current.GetBondParent(name);
            if ((_live && _isDHCP) || !_isDHCP)
            {
                _ipAddress = NetworkSettings.Current.GetIPAddress(name);
                _speed = NetworkSettings.Current.GetInterfaceSpeed(name);
                _networkMask = NetworkSettings.Current.GetNetworkMask(name);
                _broadcast = NetworkSettings.Current.GetBroadcast(name);
                _gateway = NetworkSettings.Current.GetGateway(name);
                _mac = NetworkSettings.Current.GetMACAddress(name);
                _network = null;
            }
        }

        [ModelLoadMethod()]
        public static sNetworkCard Load(string iface)
        {
            return NetworkSettings.Current[iface];
        }

        [ModelLoadAllMethod()]
        public static List<sNetworkCard> LoadAll()
        {
            List<sNetworkCard> ret = new List<sNetworkCard>();
            foreach (string str in NetworkSettings.Current.InterfaceNames)
                ret.Add(NetworkSettings.Current[str]);
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> GetAllInterfaces()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            foreach (string str in NetworkSettings.Current.InterfaceNames)
            {
                if (!NetworkSettings.Current[str].IsBondSlave)
                    ret.Add(new sModelSelectOptionValue(str, str));
            }
            return ret;
        }

        public static implicit operator sNetworkCard(string name)
        {
            return new sNetworkCard(name);
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
                return new sNetworkCard((string)value);
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
