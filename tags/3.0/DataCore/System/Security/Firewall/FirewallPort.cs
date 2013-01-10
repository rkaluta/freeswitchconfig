using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall
{
    public class FirewallPort
    {
        private int _startPort;
        public int StartPort
        {
            get { return _startPort; }
        }

        private int? _endPort;
        public int? EndPort
        {
            get { return _endPort; }
        }

        public FirewallPort(int port)
        {
            _startPort = port;
            _endPort = null;
        }

        public FirewallPort(int startPort, int endPort)
        {
            _startPort = startPort;
            _endPort = endPort;
        }

        public bool IsPortWithin(int port)
        {
            if (EndPort.HasValue)
                return port >= StartPort && port <= EndPort.Value;
            return port == StartPort;
        }

        public override string ToString()
        {
            return _startPort.ToString() + (_endPort.HasValue ? ":" + _endPort.ToString() : "");
        }
    }
}
