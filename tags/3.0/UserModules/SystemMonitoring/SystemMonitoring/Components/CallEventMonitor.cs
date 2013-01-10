using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using System.Threading;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components
{
    public class CallEventMonitor : IEventHandler
    {
        internal const string CREATE_EVENT_NAME = "CHANNEL_CREATE";
        internal const string DESTROY_EVENT_NAME = "CHANNEL_DESTROY";

        private object _lock;
        private int _started;
        private int _ended;
        private int _current;

        public CallEventMonitor()
        {
            _lock = new object();
            _started = 0;
            _ended = 0;
            _current = 0;
        }

        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            if (Event.Name=="Socket Event")
                return (string)Event["Event-Name"] == CREATE_EVENT_NAME || (string)Event["Event-Name"] == DESTROY_EVENT_NAME;
            return false;
        }

        public void ProcessEvent(IEvent Event)
        {
            Monitor.Enter(_lock);
            if ((string)Event["Event-Name"]== CREATE_EVENT_NAME)
            {
                _started++;
                _current++;
            }
            else if ((string)Event["Event-Name"]== DESTROY_EVENT_NAME)
            {
                _ended++;
                _current--;
            }
            if (_current < 0)
                _current = 0;
            Monitor.Exit(_lock);
        }

        #endregion

        public Dictionary<SystemMetricTypes,object> UpdateValues(Dictionary<SystemMetricTypes, object> values)
        {
            values.Remove(SystemMetricTypes.CALLS_Active);
            values.Remove(SystemMetricTypes.CALLS_Ended);
            values.Remove(SystemMetricTypes.CALLS_Started);
            Monitor.Enter(_lock);
            values.Add(SystemMetricTypes.CALLS_Active, new sSystemMetric(SystemMetricTypes.CALLS_Active, MetricUnits.GENERIC, _current));
            values.Add(SystemMetricTypes.CALLS_Ended, new sSystemMetric(SystemMetricTypes.CALLS_Ended, MetricUnits.GENERIC, _ended));
            values.Add(SystemMetricTypes.CALLS_Started, new sSystemMetric(SystemMetricTypes.CALLS_Started, MetricUnits.GENERIC, _started));
            _ended = 0;
            _started = 0;
            Monitor.Exit(_lock);
            return values;
        }
    }
}
