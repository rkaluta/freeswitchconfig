using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall.Rules
{
    public class ULogRule : FirewallRule
    {
        private byte _logGroup;
        public byte LogGroup
        {
            get { return _logGroup; }
        }

        private string _prefix;
        public string Prefix
        {
            get { return _prefix; }
        }

        private uint _bytesToCopy=0;
        public uint BytesToCopy
        {
            get { return _bytesToCopy; }
        }

        private ushort _queueSize=1;
        public ushort QueueSize
        {
            get { return _queueSize; }
        }

        public ULogRule(FireWallChains chain, string networkInterface, Protocols protocol, ICMPTypes? icmpType, IPAddress source, IPAddress sourceNetworkMask,
            FirewallPort sourcePort, IPAddress destination, IPAddress destinationNetworkMask, FirewallPort destinationPort,
            ConnectionStateTypes[] connectionStates,string note,byte logGroup,string prefix,uint bytesToCopy,ushort queueSize) :
            base(chain,networkInterface, protocol, icmpType, source, sourceNetworkMask, sourcePort, destination, destinationNetworkMask, destinationPort, connectionStates,note) 
        {
            _logGroup = logGroup;
            _prefix = prefix;
            _bytesToCopy = bytesToCopy;
            _queueSize = queueSize;
            if (_prefix != null)
            {
                if (_prefix.Length > 32)
                    throw new Exception("Unable to create ULog rule with a message prefix greater than 32 characters.");
            }
            if (_logGroup < 1 || _logGroup > 32)
                throw new Exception("Unable to create ULog rule with a log group not from 1-32.");
            if (_queueSize < 1)
                throw new Exception("Unable to create a ULog rule with a queue size less than 1.");
        }

        public override sealed string TextDescription
        {
            get { return "ULog"; }
        }

        public override sealed string AdditionalDisplayInformation
        {
            get { return null; }
        }

        public override string GenerateCommandParameters
        {
            get
            {
                return " -j ULOG --ulog-nlgroup " + LogGroup.ToString() +
                  (this.Prefix != null ? " --ulog-prefix \"" + this.Prefix + "\"" : "") +
                  " --ulog-cprange " + this.BytesToCopy.ToString() +
                  " --ulog-qthreshold " + this.QueueSize.ToString();
            }
        }

        public override object Clone()
        {
            return new ULogRule(this.Chain, this.Interface, this.Protocol, this.ICMPType,
                this.SourceIP, this.SourceNetworkMask, this.SourcePort, this.DestinationIP,
                this.DestinationNetworkMask, this.DestinationPort, this.ConnectionStates,this.Note,
                this.LogGroup,this.Prefix,this.BytesToCopy,this.QueueSize);
        }
    }
}
