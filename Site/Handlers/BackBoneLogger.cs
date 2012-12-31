using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Diagnostics;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class BackBoneLogger : ILogWriter
    {
        #region ILogWriter Members

        public void WriteLogMessage(DateTime timestamp, LogLevels level, string message)
        {
            switch (level)
            {
                case LogLevels.Trace:
                    Logger.Trace(message);
                    break;
                case LogLevels.Debug:
                    Logger.Debug(message);
                    break;
                case LogLevels.Critical:
                    Logger.Error(message);
                    break;
            }
        }

        public LogLevels LogLevel
        {
            get {return LogLevels.Trace;}
        }

        #endregion
    }
}
