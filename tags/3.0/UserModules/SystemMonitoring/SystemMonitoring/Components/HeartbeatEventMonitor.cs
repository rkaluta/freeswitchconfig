using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components
{
    public class HeartbeatEventMonitor : IEventHandler
    {
        internal const string EVENT_NAME = "HEARTBEAT";

        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            Log.Trace("Checking if the event of type " + Event.GetType().FullName + " Should be processed by the Hearbeat Monitor.");
            if (Event.Name == "Socket Event")
            {
                Log.Trace("Checking if the event name "+Event["Event-Name"].ToString()+" is HEARTBEAT");
                return Event["Event-Name"].ToString() == EVENT_NAME;
            }
            return false;
        }

        public void ProcessEvent(IEvent Event)
        {
            Log.Trace("Processing event for HEARTBEAT...");
            Log.Trace("Setting current freeswitch status with UpTime: " + Event["Up-Time"].ToString());
            Log.Trace("Setting current freeswitch status with Session Count: " + Event["Session-Count"].ToString());
            Log.Trace("Setting current freeswitch status with Session per Second: " + Event["Session-Per-Sec"].ToString());
            Log.Trace("Setting current freeswitch status with Session Since Startup: " + Event["Session-Since-Startup"].ToString());
            SystemMonitor.Current.UpdateFreeswitchMetrics(
                new sFreeSwitchUpTime("UP " + Event["Up-Time"].ToString()),
                long.Parse(Event["Session-Count"].ToString()),
                long.Parse(Event["Session-Per-Sec"].ToString()),
                long.Parse(Event["Session-Since-Startup"].ToString()));
        }

        #endregion

        public HeartbeatEventMonitor() { }
    }
}
