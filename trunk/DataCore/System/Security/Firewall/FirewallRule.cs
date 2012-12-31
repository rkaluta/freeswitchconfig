using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall.Rules;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall
{
    public enum ConnectionStateTypes
    {
        New,
        Established,
        Related,
        Invalid
    }

    public enum FireWallChains
    {
        Input,
        Output,
        Forward
    }

    public enum Protocols
    {
        tcp,
        udp,
        icmp,
        ALL
    }

    public enum ICMPTypes
    {
        EchoReply = 0,
        DestinationUnreachable = 3,
        SourceQuench = 4,
        Redirect = 5,
        AlternateHostAddress = 6,
        Echo = 8,
        RouterAdvertisement = 9,
        RouterSolicitation = 10,
        TimeExceeded = 11,
        ParameterProblem = 12,
        Timestamp = 13,
        TimestampReply = 14,
        InformationRequest = 15,
        InformationReply = 16,
        Traceroute = 30,
        DatagramConversionError = 31,
        MobileHostRedirect = 32,
        IPv6WhereAreYou = 33,
        IPv6IAmHere = 34,
        ModileRegistartionRequest = 35,
        MobileRegistrationReply = 36,
        DomainNameRequest = 37,
        DomainNameReply = 38,
        SKIP = 39,
        Photuris = 40
    }

    public abstract class FirewallRule : ICloneable
    {
        private FireWallChains _chain;
        public FireWallChains Chain
        {
            get { return _chain; }
        }

        private Protocols _protocol;
        public Protocols Protocol
        {
            get { return _protocol; }
        }

        private ICMPTypes? _icmpType;
        public ICMPTypes? ICMPType
        {
            get { return _icmpType; }
        }

        private string _interface;
        public string Interface
        {
            get { return _interface; }
        }

        private IPAddress _sourceIP = null;
        public IPAddress SourceIP
        {
            get { return _sourceIP; }
        }

        private IPAddress _sourceNetworkMask = null;
        public IPAddress SourceNetworkMask
        {
            get { return _sourceNetworkMask; }
        }

        private FirewallPort _sourcePort;
        public FirewallPort SourcePort
        {
            get { return _sourcePort; }
        }

        private IPAddress _destinationIP = null;
        public IPAddress DestinationIP
        {
            get { return _destinationIP; }
        }

        private IPAddress _destinationNetworkMask = null;
        public IPAddress DestinationNetworkMask
        {
            get { return _destinationNetworkMask; }
        }

        private FirewallPort _destinationPort;
        public FirewallPort DestinationPort
        {
            get { return _destinationPort; }
        }

        private ConnectionStateTypes[] _connectionStates;
        public ConnectionStateTypes[] ConnectionStates
        {
            get { return _connectionStates; }
        }

        private string _note;
        public string Note
        {
            get { return _note; }
        }

        public string ConnectionStatesString
        {
            get
            {
                string ret = "";
                if (_connectionStates != null)
                {
                    if (_connectionStates.Length > 0)
                    {
                        foreach (ConnectionStateTypes type in _connectionStates)
                            ret += type.ToString().ToUpper() + ",";
                        ret = ret.Substring(0, ret.Length - 1);
                    }
                }
                return ret;
            }
        }

        public abstract string TextDescription { get; }
        public abstract string AdditionalDisplayInformation { get; }
        public abstract string GenerateCommandParameters { get; }

        protected FirewallRule(FireWallChains chain, string networkInterface, Protocols protocol, ICMPTypes? icmpType, IPAddress source, IPAddress sourceNetworkMask,
            FirewallPort sourcePort, IPAddress destination, IPAddress destinationNetworkMask, FirewallPort destinationPort,
            ConnectionStateTypes[] connectionStates,string note)
        {
            _chain = chain;
            _interface = networkInterface;
            _protocol = protocol;
            _sourceIP = source;
            _sourceNetworkMask = sourceNetworkMask;
            _sourcePort = sourcePort;
            _destinationIP = destination;
            _destinationNetworkMask = destinationNetworkMask;
            _destinationPort = destinationPort;
            _connectionStates = connectionStates;
            _icmpType = icmpType;
            _note = note;
            if (protocol != Protocols.icmp && _icmpType.HasValue)
                throw new Exception("Unable to produce a firewall rule with an ICMP type unless ICMP is the protocol specified.");
            if (protocol == Protocols.udp && _connectionStates != null)
                throw new Exception("Unable to produce a firewall rule with udp protocol type and Connection States specified since udp is stateless.");
        }

        public override bool Equals(object obj)
        {
            return ((FirewallRule)obj).AddRulesString == AddRulesString;
        }

        public override int GetHashCode()
        {
            return TextDescription.GetHashCode();
        }

        public virtual string AddRulesString
        {
            get
            {
                if ((this.ConnectionStates == null) &&
                    (this.DestinationIP == null) &&
                    (this.DestinationNetworkMask == null) &&
                    (this.DestinationPort == null) &&
                    (this.ICMPType == null) &&
                    (this.Interface == null) &&
                    (this.Protocol == Protocols.ALL) &&
                    (this.SourceIP == null) &&
                    (this.SourceNetworkMask == null) &&
                    (this.SourcePort == null) &&
                    (this is AcceptRule || this is DropRule || this is RejectRule)) //is default policy?
                    return " -P " + this.Chain.ToString().ToUpper() + GenerateCommandParameters;
                return (this is PortRedirectRule ? " -t nat " : "") +
                    " -A " + (this is PortRedirectRule ? (this.Chain == FireWallChains.Input ? "PREROUTING" : "POSTROUTING") : this.Chain.ToString().ToUpper()) +
                    (this.Interface != null ? (this is PortRedirectRule && this.Chain == FireWallChains.Output ? " -o " : " -i ") + this.Interface : "") +
                    (this.Protocol != Protocols.ALL ? " -p " + this.Protocol.ToString() : "") +
                    (this.ICMPType.HasValue ? " --icmp-type " + ((int)this.ICMPType.Value).ToString() : "") +
                    (this.SourceIP != null && this.SourceIP != IPAddress.Any ? " -s " + this.SourceIP.ToString() + (this.SourceNetworkMask != null ? "/" + this.SourceNetworkMask.ToString() : "") : "") +
                    (this.SourcePort != null ? " --sport " + this.SourcePort.StartPort.ToString() + (this.SourcePort.EndPort.HasValue ? ":" + this.SourcePort.EndPort.Value.ToString() : "") : "") +
                    (this.DestinationIP != null && this.DestinationIP != IPAddress.Any ? " -d " + this.DestinationIP.ToString() + (this.DestinationNetworkMask != null ? "/" + this.DestinationNetworkMask.ToString() : "") : "") +
                    (this.DestinationPort != null ? " --dport " + this.DestinationPort.StartPort.ToString() + (this.DestinationPort.EndPort.HasValue ? ":" + this.DestinationPort.EndPort.Value.ToString() : "") : "") +
                    (this.ConnectionStates != null ? (this.ConnectionStates.Length > 0 ? " -m state --state " + this.ConnectionStatesString : "") : "")
                    + GenerateCommandParameters;
            }
        }
        
        public virtual string DeleteRulesString {
            get
            {
                return (this is PortRedirectRule ? " -t nat " : "") +
                     "-D " + (this is PortRedirectRule ? (this.Chain == FireWallChains.Input ? "PREROUTING" : "POSTROUTING") : this.Chain.ToString().ToUpper()) +
                    (this.Interface != null ? (this is PortRedirectRule && this.Chain == FireWallChains.Output ? " -o " : " -i ") + this.Interface : "") +
                    (this.Protocol != Protocols.ALL ? " -p " + this.Protocol.ToString() : "") +
                    (this.ICMPType.HasValue ? " --icmp-type " + ((int)this.ICMPType.Value).ToString() : "") +
                    (this.SourceIP != null && this.SourceIP != IPAddress.Any ? " -s " + this.SourceIP.ToString() + (this.SourceNetworkMask != null ? "/" + this.SourceNetworkMask.ToString() : "") : "") +
                    (this.SourcePort != null ? " --sport " + this.SourcePort.StartPort.ToString() + (this.SourcePort.EndPort.HasValue ? ":" + this.SourcePort.EndPort.Value.ToString() : "") : "") +
                    (this.DestinationIP != null && this.DestinationIP != IPAddress.Any ? " -d " + this.DestinationIP.ToString() + (this.DestinationNetworkMask != null ? "/" + this.DestinationNetworkMask.ToString() : "") : "") +
                    (this.DestinationPort != null ? " --dport " + this.DestinationPort.StartPort.ToString() + (this.DestinationPort.EndPort.HasValue ? ":" + this.DestinationPort.EndPort.Value.ToString() : "") : "") +
                    (this.ConnectionStates != null ? (this.ConnectionStates.Length > 0 ? " -m state --state " + this.ConnectionStatesString : "") : "")
                    + GenerateCommandParameters;
            }
        }

        protected int SubnetIPToMaskNumber(IPAddress addy)
        {
            int ret = 0;
            if (addy != null)
            {
                int tmp = BitConverter.ToInt32(addy.GetAddressBytes(), 0);
                while ((tmp & 2147483648) == 2147483648)
                {
                    tmp = tmp << 1;
                }
                while ((tmp & 2147483648) != 2147483648)
                {
                    tmp = tmp << 1;
                    ret++;
                }
            }
            return ret;
        }

        #region ICloneable Members

        public abstract object Clone();

        #endregion

        public static List<FirewallRule> DefaultInputRules
        {
            get
            {
                List<FirewallRule> rules = new List<FirewallRule>();
                rules.Add(new AcceptRule(FireWallChains.Input, null, Protocols.ALL, null, IPAddress.Loopback, null, null,IPAddress.Loopback, null, null, null, "Allow all traffic in loopback"));
                rules.Add(new AcceptRule(FireWallChains.Input, null, Protocols.tcp, null, null, null, null, null, null, null, new ConnectionStateTypes[] { ConnectionStateTypes.Related, ConnectionStateTypes.Established }, "Allow established connections"));

                foreach (sIPPortPair ipp in Site.CurrentSite.ListenOn)
                {
                    if (ipp.Address.ToString() != IPAddress.Loopback.ToString())
                    {
                        if (ipp.Address.ToString() != IPAddress.Any.ToString())
                        {
                            string iface = NetworkSettings.Current.GetInterfaceForIPAddress(ipp.Address);
                            rules.Add(new AcceptRule(FireWallChains.Input, iface, Protocols.tcp, null, IPAddress.Parse(NetworkSettings.Current[iface].Network), IPAddress.Parse(NetworkSettings.Current[iface].NetworkMask), new FirewallPort(1025, 65535), ipp.Address, IPAddress.Parse(NetworkSettings.Current[iface].NetworkMask), new FirewallPort(ipp.Port), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site"));
                            rules.Add(new AcceptRule(FireWallChains.Input, iface, Protocols.tcp, null, IPAddress.Loopback, null, new FirewallPort(1025, 65535), ipp.Address, IPAddress.Parse(NetworkSettings.Current[iface].NetworkMask), new FirewallPort(ipp.Port), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site from loopback"));
                            rules.Add(new AcceptRule(FireWallChains.Input, iface, Protocols.tcp, null, IPAddress.Loopback, null, new FirewallPort(1025, 65535), IPAddress.Loopback, null, new FirewallPort(ipp.Port), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site from loopback"));
                        }
                        else
                        {
                            foreach (string str in NetworkSettings.Current.InterfaceNames)
                            {
                                sNetworkCard card = null;
                                if (NetworkSettings.Current[str].Live)
                                {
                                    card = NetworkSettings.Current[str];
                                }
                                if (card!=null)
                                {
                                    rules.Add(new AcceptRule(FireWallChains.Input, str, Protocols.tcp, null, IPAddress.Parse(card.Network), IPAddress.Parse(card.NetworkMask), new FirewallPort(1025, 65535), ipp.Address, IPAddress.Parse(card.NetworkMask), new FirewallPort(ipp.Port), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site"));
                                    rules.Add(new AcceptRule(FireWallChains.Input, str, Protocols.tcp, null, IPAddress.Loopback, null, new FirewallPort(1025, 65535), ipp.Address, IPAddress.Parse(card.NetworkMask), new FirewallPort(ipp.Port), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site from loopback"));
                                    rules.Add(new AcceptRule(FireWallChains.Input, str, Protocols.tcp, null, IPAddress.Loopback, null, new FirewallPort(1025, 65535), IPAddress.Loopback, null, new FirewallPort(ipp.Port), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site from loopback"));
                                }
                            }
                        }
                    }
                }

                foreach (string str in NetworkSettings.Current.InterfaceNames)
                {
                    sNetworkCard card = null;
                    if (NetworkSettings.Current[str].Live)
                    {
                        card = NetworkSettings.Current[str];
                    }
                    if (card!=null)
                    {
                        rules.Add(new AcceptRule(FireWallChains.Input, str, Protocols.tcp, null,IPAddress.Parse(card.Network),IPAddress.Parse(card.NetworkMask) ,null,IPAddress.Parse(card.IPAddress), null, new FirewallPort(22), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow SSH to server from network"));
                        rules.Add(new AcceptRule(FireWallChains.Input, str, Protocols.udp, null, IPAddress.Parse(card.NetworkMask), IPAddress.Parse(card.NetworkMask), null, null, null, new FirewallPort(1900), null, "Allow UPnP discovery protocol from network"));
                        rules.Add(new AcceptRule(FireWallChains.Input, str, Protocols.tcp, null, IPAddress.Parse(card.NetworkMask), IPAddress.Parse(card.NetworkMask), null, null, null, new FirewallPort(3050), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to firebirdSQL server from network"));
                        rules.Add(new AcceptRule(FireWallChains.Input, str, Protocols.icmp, ICMPTypes.Echo, IPAddress.Parse(card.Network), IPAddress.Parse(card.NetworkMask), null, null, null, null, null, "Allow ping requests on network"));
                        rules.Add(new RejectRule(FireWallChains.Input, str, Protocols.icmp, null, null, null, null, null, null, null, null, "Block icmp requests outside of network", RejectRule.RejectOptions.ICMP_Host_Unreachable));
                    }
                }

                foreach (SipProfile profile in SipProfile.LoadAll())
                {
                    if (profile.SIPInterface.Live)
                    {
                        rules.Add(new AcceptRule(FireWallChains.Input, profile.SIPInterface.Name, Protocols.tcp, null, IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), new FirewallPort(1025, 65535), IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(profile.SIPPort), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow tcp sip UAS traffic"));
                        rules.Add(new AcceptRule(FireWallChains.Input, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), new FirewallPort(1025, 65535), IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(profile.SIPPort), null, "Allow udp sip UAS traffic"));
                        rules.Add(new AcceptRule(FireWallChains.Input, profile.SIPInterface.Name, Protocols.tcp, null, IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(1719), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "H.323 gatestat port"));
                        rules.Add(new AcceptRule(FireWallChains.Input, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(1720), null, "H.323 host call port"));
                        rules.Add(new AcceptRule(FireWallChains.Input, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(3478), null, "NAT STUN"));
                        rules.Add(new AcceptRule(FireWallChains.Input, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(3479), null, "NAT traversal"));
                        rules.Add(new AcceptRule(FireWallChains.Input, profile.SIPInterface.Name, Protocols.tcp, null, IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(5002), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "MLP protocol server"));
                        rules.Add(new AcceptRule(FireWallChains.Input, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(5003), null, "Neighbourhood service"));
                    }
                    if (profile.RTPInterface.Live)
                    {
                        rules.Add(new AcceptRule(FireWallChains.Input, profile.RTPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.RTPInterface.Network), IPAddress.Parse(profile.RTPInterface.NetworkMask), new FirewallPort(1025, 65535), IPAddress.Parse(profile.RTPInterface.IPAddress), null, new FirewallPort(16384, 32768), null, "Allow udp RTP traffic(Audio and Video)"));
                    }
                }

                rules.Add(new RejectRule(FireWallChains.Input, null, Protocols.ALL, null, null, null, null, null, null, null, null,"Default input reject rule", RejectRule.RejectOptions.ICMP_Host_Unreachable));

                //clean up duplicate rules
                for (int x = 0; x < rules.Count; x++)
                {
                    for (int y = x+1; y < rules.Count; y++)
                    {
                        if (rules[x].Equals(rules[y]))
                        {
                            rules.RemoveAt(y);
                            y--;
                        }
                    }
                }

                return rules;
            }
        }

        public static List<FirewallRule> DefaultOutputRules
        {
            get
            {
                List<FirewallRule> rules = new List<FirewallRule>();
                rules.Add(new AcceptRule(FireWallChains.Output, null, Protocols.ALL, null, IPAddress.Loopback, null, null, IPAddress.Loopback, null, null, null,"Allow all loopback to loopback traffic"));
                rules.Add(new AcceptRule(FireWallChains.Output, null, Protocols.tcp, null, null, null, null, null, null, null, new ConnectionStateTypes[] { ConnectionStateTypes.Related, ConnectionStateTypes.Established },"Allow all established connections out"));

                foreach (sIPPortPair ipp in Site.CurrentSite.ListenOn)
                {
                    if (ipp.Address.ToString() != IPAddress.Loopback.ToString())
                    {
                        if (ipp.Address.ToString() != IPAddress.Any.ToString())
                        {
                            string iface = NetworkSettings.Current.GetInterfaceForIPAddress(ipp.Address);
                            rules.Add(new AcceptRule(FireWallChains.Output, iface, Protocols.tcp, null, ipp.Address, IPAddress.Parse(NetworkSettings.Current[iface].NetworkMask), new FirewallPort(ipp.Port), IPAddress.Parse(NetworkSettings.Current[iface].Network), IPAddress.Parse(NetworkSettings.Current[iface].NetworkMask), new FirewallPort(1025, 65535), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site"));
                            rules.Add(new AcceptRule(FireWallChains.Output, iface, Protocols.tcp, null, ipp.Address, IPAddress.Parse(NetworkSettings.Current[iface].NetworkMask), new FirewallPort(ipp.Port), IPAddress.Loopback, null, new FirewallPort(1025, 65535), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site from loopback"));
                            rules.Add(new AcceptRule(FireWallChains.Output, iface, Protocols.tcp, null, IPAddress.Loopback, null, new FirewallPort(ipp.Port), IPAddress.Loopback, null, new FirewallPort(1025, 65535), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site from loopback"));
                        }
                        else
                        {
                            foreach (string str in NetworkSettings.Current.InterfaceNames)
                            {
                                if (NetworkSettings.Current[str].Live)
                                {
                                    rules.Add(new AcceptRule(FireWallChains.Output, str, Protocols.tcp, null, ipp.Address, IPAddress.Parse(NetworkSettings.Current[str].NetworkMask), new FirewallPort(ipp.Port), IPAddress.Parse(NetworkSettings.Current[str].Network), IPAddress.Parse(NetworkSettings.Current[str].NetworkMask), new FirewallPort(1025, 65535), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site"));
                                    rules.Add(new AcceptRule(FireWallChains.Output, str, Protocols.tcp, null, ipp.Address, IPAddress.Parse(NetworkSettings.Current[str].NetworkMask), new FirewallPort(ipp.Port), IPAddress.Loopback, null, new FirewallPort(1025, 65535), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site from loopback"));
                                    rules.Add(new AcceptRule(FireWallChains.Output, str, Protocols.tcp, null, IPAddress.Loopback, null, new FirewallPort(ipp.Port), IPAddress.Loopback, null, new FirewallPort(1025, 65535), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to configuration site from loopback"));
                                }
                            }
                        }
                    }
                }

                foreach (string str in NetworkSettings.Current.InterfaceNames)
                {
                    sNetworkCard card = null;
                    if (NetworkSettings.Current[str].Live)
                    {
                        card = NetworkSettings.Current[str];
                    }
                    if (card!=null)
                    {
                        rules.Add(new AcceptRule(FireWallChains.Output, str, Protocols.tcp, null, IPAddress.Parse(card.IPAddress), null, new FirewallPort(22), IPAddress.Parse(card.Network), IPAddress.Parse(card.NetworkMask), null, new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow SSH to server from network"));
                        rules.Add(new AcceptRule(FireWallChains.Output, str, Protocols.udp, null, null, null, new FirewallPort(1900), IPAddress.Parse(card.NetworkMask), IPAddress.Parse(card.NetworkMask), null, null, "Allow UPnP discovery protocol from network"));
                        rules.Add(new AcceptRule(FireWallChains.Output, str, Protocols.tcp, null, null, null, new FirewallPort(3050), IPAddress.Parse(card.NetworkMask), IPAddress.Parse(card.NetworkMask), null, new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow access to firebirdSQL server from network"));
                        rules.Add(new AcceptRule(FireWallChains.Output, str, Protocols.icmp, ICMPTypes.EchoReply, null, null, null, IPAddress.Parse(card.Network), IPAddress.Parse(card.NetworkMask), null, null, "Allow ping requests on network"));
                    }
                }

                foreach (SipProfile profile in SipProfile.LoadAll())
                {
                    if (profile.SIPInterface.Live)
                    {
                        rules.Add(new AcceptRule(FireWallChains.Output, profile.SIPInterface.Name, Protocols.tcp, null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(profile.SIPPort), IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), new FirewallPort(1025, 65535), new ConnectionStateTypes[] { ConnectionStateTypes.New }, "Allow tcp sip UAS traffic"));
                        rules.Add(new AcceptRule(FireWallChains.Output, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(profile.SIPPort), IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), new FirewallPort(1025, 65535), null, "Allow udp sip UAS traffic"));
                        rules.Add(new AcceptRule(FireWallChains.Output, profile.SIPInterface.Name, Protocols.tcp, null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(1719), IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, new ConnectionStateTypes[] { ConnectionStateTypes.New }, "H.323 gatestat port"));
                        rules.Add(new AcceptRule(FireWallChains.Output, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(1720), IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, null, "H.323 host call port"));
                        rules.Add(new AcceptRule(FireWallChains.Output, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(3478), IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, null, "NAT STUN"));
                        rules.Add(new AcceptRule(FireWallChains.Output, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(3479), IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, null, "NAT traversal"));
                        rules.Add(new AcceptRule(FireWallChains.Output, profile.SIPInterface.Name, Protocols.tcp, null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(5002), IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, new ConnectionStateTypes[] { ConnectionStateTypes.New }, "MLP protocol server"));
                        rules.Add(new AcceptRule(FireWallChains.Output, profile.SIPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.SIPInterface.IPAddress), null, new FirewallPort(5003), IPAddress.Parse(profile.SIPInterface.Network), IPAddress.Parse(profile.SIPInterface.NetworkMask), null, null, "Neighbourhood service"));
                    }
                    if (profile.RTPInterface.Live)
                    {
                        rules.Add(new AcceptRule(FireWallChains.Output, profile.RTPInterface.Name, Protocols.udp, null, IPAddress.Parse(profile.RTPInterface.IPAddress), null, new FirewallPort(16384, 32768), IPAddress.Parse(profile.RTPInterface.Network), IPAddress.Parse(profile.RTPInterface.NetworkMask), new FirewallPort(1025, 65535), null, "Allow udp RTP traffic(Audio and Video)"));
                    }
                }

                rules.Add(new RejectRule(FireWallChains.Output, null, Protocols.ALL, null, null, null, null, null, null, null, null,"Default reject output rule", RejectRule.RejectOptions.ICMP_Host_Unreachable));

                //clean up duplicate rules
                for (int x = 0; x < rules.Count; x++)
                {
                    for (int y = x + 1; y < rules.Count; y++)
                    {
                        if (rules[x].Equals(rules[y]))
                        {
                            rules.RemoveAt(y);
                            y--;
                        }
                    }
                }

                return rules;
            }
        }

        public static List<FirewallRule> DefaultForwardRules
        {
            get
            {
                List<FirewallRule> rules = new List<FirewallRule>();
                rules.Add(new RejectRule(FireWallChains.Forward, null, Protocols.ALL, null, null, null, null, null, null, null, null,"Default reject forward rule", RejectRule.RejectOptions.ICMP_Host_Unreachable));
                return rules;
            }
        }
    }
}
