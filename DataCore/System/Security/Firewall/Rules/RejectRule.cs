using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall.Rules
{
    public class RejectRule : FirewallRule
    {
        public enum RejectOptions
        {
            ICMP_Net_Unreachable,
            ICMP_Host_Unreachable,
            ICMP_Port_Unreachable,
            ICMP_Proto_Unreachable,
            ICMP_Net_Prohibited,
            ICMP_Host_Prohibited,
            TCP_Reset
        }

        public RejectOptions _rejectWith;
        public RejectOptions RejectWith
        {
            get { return _rejectWith; }
        }

        public RejectRule(FireWallChains chain, string networkInterface, Protocols protocol, ICMPTypes? icmpType, IPAddress source, IPAddress sourceNetworkMask,
            FirewallPort sourcePort, IPAddress destination, IPAddress destinationNetworkMask, FirewallPort destinationPort,
            ConnectionStateTypes[] connectionStates,string note,RejectOptions rejectWith) :
            base(chain,networkInterface, protocol, icmpType, source, sourceNetworkMask, sourcePort, destination, destinationNetworkMask, destinationPort, connectionStates,note) 
        {
            _rejectWith = rejectWith;
        }

        public override sealed string TextDescription
        {
            get { return "Reject"; }
        }

        public override sealed string AdditionalDisplayInformation
        {
            get { return "Reject-With: " + this.RejectWith.ToString(); }
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
                    return " REJECT --reject-with " + this.RejectWith.ToString().ToLower().Replace("_", "-");
                return " -j REJECT --reject-with " + this.RejectWith.ToString().ToLower().Replace("_", "-");
            }
        }

        public override object Clone()
        {
            return new RejectRule(this.Chain, this.Interface, this.Protocol, this.ICMPType,
                this.SourceIP, this.SourceNetworkMask, this.SourcePort, this.DestinationIP,
                this.DestinationNetworkMask, this.DestinationPort, this.ConnectionStates,this.Note,this.RejectWith);
        }
    }
}
