using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components
{
    public class EventMonitor : IEventHandler
    {
        private long _systemEvents;
        private long _freeswitchEvents;

        public EventMonitor()
        {
            _systemEvents = 0;
            _freeswitchEvents = 0;
        }

        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            return true;
        }

        public void ProcessEvent(IEvent Event)
        {
            if (Event.Name == "Socket Event")
                _freeswitchEvents++;
            else
                _systemEvents++;
        }

        #endregion

        public Dictionary<SystemMetricTypes, object> UpdateValues(Dictionary<SystemMetricTypes, object> values)
        {
            values.Remove(SystemMetricTypes.Freeswitch_Events);
            values.Remove(SystemMetricTypes.System_Events);
            values.Add(SystemMetricTypes.Freeswitch_Events, new sSystemMetric(SystemMetricTypes.Freeswitch_Events, MetricUnits.GENERIC, _freeswitchEvents));
            values.Add(SystemMetricTypes.System_Events, new sSystemMetric(SystemMetricTypes.System_Events, MetricUnits.GENERIC, _systemEvents));
            _systemEvents = 0;
            _freeswitchEvents = 0;
            return values;
        }
    }
}
