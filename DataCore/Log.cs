using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Logging;
using System.Reflection;
using Org.Reddragonit.EmbeddedWebServer.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore
{
    public class Log : ILogWriter
    {
        #region Trace
        public static void Trace(string message)
        {
            _LogMessage(LogLevel.Trace, message);
        }

        public static void Trace(Exception ex)
        {
            Exception tmp = ex;
            while (tmp != null)
            {
                _LogMessage(LogLevel.Trace,ex.Message);
                _LogMessage(LogLevel.Trace, ex.Source);
                _LogMessage(LogLevel.Trace, ex.StackTrace);
                tmp = tmp.InnerException;
            }
        }
        #endregion

        #region Security
        public static void Security(string message)
        {
            Logger.LogMessage(DiagnosticsLevels.SECURITY, message);
        }

        public static void Security(Exception ex)
        {
            Exception tmp = ex;
            while (tmp != null)
            {
                Logger.LogMessage(DiagnosticsLevels.SECURITY, ex.Message);
                Logger.LogMessage(DiagnosticsLevels.SECURITY, ex.Source);
                Logger.LogMessage(DiagnosticsLevels.SECURITY, ex.StackTrace);
                tmp = tmp.InnerException;
            }
        }
        #endregion

        #region Debug
        public static void Debug(string message)
        {
            _LogMessage(LogLevel.Debug, message);
        }

        public static void Debug(Exception ex)
        {
            Exception tmp = ex;
            while (tmp != null)
            {
                _LogMessage(LogLevel.Debug, ex.Message);
                _LogMessage(LogLevel.Debug, ex.Source);
                _LogMessage(LogLevel.Debug, ex.StackTrace);
                tmp = tmp.InnerException;
            }
        }
        #endregion

        #region Error
        public static void Error(string message)
        {
            _LogMessage(LogLevel.Critical, message);
        }

        public static void Error(Exception ex)
        {
            Exception tmp = ex;
            while (tmp != null)
            {
                _LogMessage(LogLevel.Critical, ex.Message);
                _LogMessage(LogLevel.Critical, ex.Source);
                _LogMessage(LogLevel.Critical, ex.StackTrace);
                tmp = tmp.InnerException;
            }
        }
        #endregion

        private static void _LogMessage(LogLevel level, object entry)
        {
            switch (level)
            {
                case LogLevel.Database:
                case LogLevel.Trace:
                    Logger.LogMessage(DiagnosticsLevels.TRACE, entry.ToString());
                    break;
                case LogLevel.Debug:
                    Logger.LogMessage(DiagnosticsLevels.DEBUG, entry.ToString());
                    break;
                case LogLevel.Critical:
                    Logger.LogMessage(DiagnosticsLevels.CRITICAL, entry.ToString());
                    break;
            }
        }

        #region ILogWriter Members

        public void LogMessage(Assembly assembly, LogLevel level, object entry)
        {
            if (level == LogLevel.Critical && 
                ((entry is Exception) || (entry.GetType().IsSubclassOf(typeof(Exception)))))
                EventController.TriggerEvent(new ErrorOccuredEvent((Exception)entry));
            _LogMessage(level, entry);
        }

        #endregion
    }
}
