using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components
{
    public class ConnectionEventMonitor : IEventHandler
    {
        private Queue<double> _milliSeconds;
        private double _high = 0;

        public ConnectionEventMonitor()
        {
            _milliSeconds = new Queue<double>();
        }

        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            return Event.Name == "InboundConnectionClosedEvent";
        }

        public void ProcessEvent(IEvent Event)
        {
            lock (_milliSeconds)
            {
                //_milliSeconds.Enqueue(DateTime.Now.Subtract(((InboundConnectionClosedEvent)Event).StartTime).TotalMilliseconds);
            }
        }

        #endregion

        public Dictionary<SystemMetricTypes, object> UpdateValues(Dictionary<SystemMetricTypes, object> values)
        {
            lock (_milliSeconds)
            {
                long cnt = _milliSeconds.Count;
                double high = 0;
                double tot = 0;
                while (_milliSeconds.Count > 0)
                {
                    double tmp = _milliSeconds.Dequeue();
                    tot += tmp;
                    high = (tmp > high ? tmp : high);
                }
                values.Remove(SystemMetricTypes.Average_Inbound_Connection_Duration);
                values.Add(SystemMetricTypes.Average_Inbound_Connection_Duration, new sSystemMetric(SystemMetricTypes.Average_Inbound_Connection_Duration, MetricUnits.MILLISECONDS, (tot == 0 ? (decimal)0 : (decimal)Math.Round(tot / (double)cnt, 2))));
                if (values.ContainsKey(SystemMetricTypes.Max_Inbound_Connection_Duration))
                {
                    if (high>_high){
                        values.Remove(SystemMetricTypes.Max_Inbound_Connection_Duration);
                        values.Add(SystemMetricTypes.Max_Inbound_Connection_Duration, new sSystemMetric(SystemMetricTypes.Max_Inbound_Connection_Duration, MetricUnits.MILLISECONDS, (decimal)high));
                        _high = high;
                    }
                }
                else
                    values.Add(SystemMetricTypes.Max_Inbound_Connection_Duration, new sSystemMetric(SystemMetricTypes.Max_Inbound_Connection_Duration, MetricUnits.MILLISECONDS, (decimal)high));
            }
            return values;
        }
    }
}
