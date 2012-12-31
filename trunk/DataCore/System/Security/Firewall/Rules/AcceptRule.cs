using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall.Rules
{
    public class AcceptRule : FirewallRule
    {

        public AcceptRule(FireWallChains chain, string networkInterface, Protocols protocol, ICMPTypes? icmpType, IPAddress source, IPAddress sourceNetworkMask,
            FirewallPort sourcePort, IPAddress destination, IPAddress destinationNetworkMask, FirewallPort destinationPort,
            ConnectionStateTypes[] connectionStates,string note) :
            base(chain,networkInterface, protocol, icmpType, source, sourceNetworkMask, sourcePort, destination, destinationNetworkMask, destinationPort, connectionStates,note) { }

        public override sealed string TextDescription
        {
            get { return "Allow"; }
        }

        public override sealed string AdditionalDisplayInformation
        {
            get { return null; }
        }

        public override string GenerateCommandParameters
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
                (this.SourcePort == null)) //is default policy?
                    return " ACCEPT";
                return " -j ACCEPT"; 
            }
        }

        public override object Clone()
        {
            return new AcceptRule(this.Chain, this.Interface, this.Protocol, this.ICMPType,
                this.SourceIP, this.SourceNetworkMask, this.SourcePort, this.DestinationIP,
                this.DestinationNetworkMask, this.DestinationPort, this.ConnectionStates,this.Note);
        }
    }
}
