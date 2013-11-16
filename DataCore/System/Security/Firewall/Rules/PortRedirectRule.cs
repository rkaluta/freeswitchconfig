using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall.Rules
{
    public class PortRedirectRule : FirewallRule
    {
        private int _toPort;
        public int toPort
        {
            get { return _toPort; }
        }

        public PortRedirectRule(FireWallChains chain, string networkInterface, Protocols protocol, IPAddress source, FirewallPort sourcePort, FirewallPort destinationPort,
            ConnectionStateTypes[] connectionStates,string note, int toPort) :
            base(chain, networkInterface, protocol, null, source, null, sourcePort, null, null, destinationPort, connectionStates,note)
        {
            this._toPort = toPort;
        }

        public override string AdditionalDisplayInformation
        {
            get
            {
                return
                    "Direction: " + (this.Chain == FireWallChains.Input ? "IN" : "OUT") + "\n" +
                "To-Port: " + _toPort.ToString();
            }
        }

        public override string TextDescription
        {
            get { return "Redirect"; }
        }

        public override string  GenerateCommandParameters
        {
            get
            {
                if (this.Chain == FireWallChains.Input)
                    return " -j REDIRECT --to-ports " + _toPort.ToString();
                else
                {
                    if (this.Interface != null)
                        return " -j SNAT --to-source " + sNetworkCard.Load(this.Interface).IPAddress + ":" + _toPort.ToString();
                    else
                        throw new Exception("Cannot do a port redirection without specifying the interface.");
                };
            }
        }

        public override object Clone()
        {
            return new PortRedirectRule(this.Chain, this.Interface, this.Protocol, this.SourceIP,
                this.SourcePort,this.DestinationPort,this.ConnectionStates,this.Note,this.toPort);
        }
    }
}
