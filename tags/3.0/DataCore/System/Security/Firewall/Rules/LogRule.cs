using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall.Rules
{
    public class LogRule : FirewallRule 
    {
        public enum LogLevels
        {
            debug = 2,
            info=3,
            notice=4,
            warning=1,
            error=5,
            crit=6,
            alert=7,
            emerg=8
        }

        private LogLevels _logLevel;
        public LogLevels LogLevel
        {
            get { return _logLevel; }
        }

        private string _logPrefix;
        public string LogPrefix
        {
            get { return _logPrefix; }
        }

        private bool _logTcpOptions;
        public bool LogTcpOptions
        {
            get { return _logTcpOptions; }
        }

        private bool _logTcpSequence;
        public bool LogTcpSequence
        {
            get { return _logTcpSequence; }
        }

        private bool _logIPOptions;
        public bool LogIPOptions
        {
            get { return _logIPOptions; }
        }

        public LogRule(FireWallChains chain, string networkInterface, Protocols protocol, ICMPTypes? icmpType, IPAddress source, IPAddress sourceNetworkMask,
            FirewallPort sourcePort, IPAddress destination, IPAddress destinationNetworkMask, FirewallPort destinationPort,
            ConnectionStateTypes[] connectionStates,string note,LogLevels level,string logPrefix,bool logTcpOptions,bool logTcpSequence,bool logIPOptions) :
            base(chain,networkInterface, protocol, icmpType, source, sourceNetworkMask, sourcePort, destination, destinationNetworkMask, destinationPort, connectionStates,note) 
        {
            _logLevel = level;
            _logPrefix = logPrefix;
            _logTcpOptions = logTcpOptions;
            _logTcpSequence = logTcpSequence;
            _logIPOptions = logIPOptions;
            if (_logPrefix != null)
            {
                if (_logPrefix.Length > 29)
                    throw new Exception("The log prefix " + _logPrefix + " is too long, it cannot exceed 29 characters.");
            }
        }

        public override sealed string AdditionalDisplayInformation
        {
            get {
                return "Log-Level: " + LogLevel.ToString() + "\n" +
                    (this.LogPrefix != null ? "Log-Prefix: " + LogPrefix + "\n" : "") +
                    "Log-Tcp-Sequence: " + (this._logTcpSequence ? "Yes" : "No") + "\n" +
                    "Log-Tcp-Options: " + (this._logTcpOptions ? "Yes" : "No") + "\n" +
                    "Log-Ip-Options: " + (this._logIPOptions ? "Yes" : "No");
            }
        }

        public override sealed string TextDescription
        {
            get { return "Log"; }
        }

        public override string GenerateCommandParameters
        {
            get
            {
                return " -j LOG --log-level " + LogLevel.ToString() +
                  (this.LogPrefix != null ? " --log-prefix \"" + this.LogPrefix + "\'" : "") +
                  (this.LogTcpSequence ? " --log-tcp-sequence" : "") +
                  (this.LogTcpOptions ? " --log-tcp-options" : "") +
                  (this.LogIPOptions ? " --log-ip-options" : "");
            }
        }

        public override object Clone()
        {
            return new LogRule(this.Chain, this.Interface, this.Protocol, this.ICMPType,
                this.SourceIP, this.SourceNetworkMask, this.SourcePort, this.DestinationIP,
                this.DestinationNetworkMask, this.DestinationPort, this.ConnectionStates,this.Note,
                this.LogLevel,this.LogPrefix,this.LogTcpOptions,this.LogTcpSequence,this.LogIPOptions);
        }
    }
}
