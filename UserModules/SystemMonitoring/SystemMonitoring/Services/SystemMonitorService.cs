using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using System.Collections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Services
{
    [EmbeddedServiceNamespace("FreeswitchConfig.Services")]
    public class SystemMonitorService : EmbeddedService
    {
        protected override bool IsValidAccess(string functionName)
        {
            return User.Current != null;
        }

        [WebMethod(true)]
        public sFreeSwitchStatus? GetFreeswitchStatus()
        {
            return SystemMonitor.Current.CurrentFreeswitchStatus;
        }


        private static readonly SystemMetricTypes[] _graphTypes = new SystemMetricTypes[]{
            SystemMetricTypes.CPU_Free,
            SystemMetricTypes.HD_Used,
            SystemMetricTypes.NET_In,
            SystemMetricTypes.NET_Out,
            SystemMetricTypes.RAM_Used,
            SystemMetricTypes.CALLS_Active,
            SystemMetricTypes.CALLS_Ended,
            SystemMetricTypes.CALLS_Started,
            SystemMetricTypes.Freeswitch_Events,
            SystemMetricTypes.System_Events,
            SystemMetricTypes.Threads,
            SystemMetricTypes.Processes
        };

        [WebMethod(true)]
        public List<NameValuePair> GetGraphValues()
        {
            List<NameValuePair> ret = new List<NameValuePair>();
            foreach (sSystemMetric ssm in SystemMonitor.Current.GetValues(_graphTypes))
            {
                switch (ssm.Type)
                {
                    case SystemMetricTypes.RAM_Used:
                        ret.Add(new NameValuePair(ssm.Type.ToString()+" (%)", double.Parse(ssm.Val.Replace("%", ""))));
                        break;
                    case SystemMetricTypes.NET_In:
                    case SystemMetricTypes.NET_Out:
                        ret.Add(new NameValuePair(ssm.Type.ToString() + " - " + ssm.Additional+" (kBps)", ssm.ToKB()));
                        break;
                    case SystemMetricTypes.CPU_Free:
                        ret.Add(new NameValuePair(ssm.Type.ToString() + " - " + ssm.Additional+" (%)", double.Parse(ssm.Val.Replace("%", ""))));
                        break;
                    case SystemMetricTypes.CALLS_Started:
                    case SystemMetricTypes.CALLS_Ended:
                    case SystemMetricTypes.System_Events:
                    case SystemMetricTypes.Freeswitch_Events:
                        ret.Add(new NameValuePair(ssm.Type.ToString() + "/minute", int.Parse(ssm.Val)));
                        break;
                    case SystemMetricTypes.CALLS_Active:
                        ret.Add(new NameValuePair(ssm.Type.ToString(), int.Parse(ssm.Val)));
                        break;
                    case SystemMetricTypes.HD_Used:
                        ret.Add(new NameValuePair(ssm.Type.ToString() + " - " + ssm.Additional, ssm.Val));
                        break;
                    default:
                        ret.Add(new NameValuePair(ssm.Type.ToString(), ssm.Val));
                        break;
                }
            }
            return ret;
        }

        [WebMethod(true)]
        public Hashtable GetSystemInformation()
        {
            Hashtable ret = new Hashtable();
            ret.Add("CPUs", SystemInformation.Current.CPUs);
            ret.Add("RAM", SystemInformation.Current.RAM.ToString());
            ret.Add("SWAP", SystemInformation.Current.Swap.ToString());
            return ret;
        }

        [WebMethod(true)]
        public Hashtable GetTopProcesses()
        {
            Hashtable ret = new Hashtable();
            foreach (sSystemMetric met in SystemMonitor.Current.GetValues(new SystemMetricTypes[] { SystemMetricTypes.TOP_Cpu, SystemMetricTypes.TOP_Memory }))
                ret.Add(met.Type.ToString(), new NameValuePair(met.Additional, met.Val));
            return ret;
        }

        [WebMethod(true)]
        public List<NameValuePair> PathSizeMetrics()
        {
            List<NameValuePair> ret = new List<NameValuePair>();
            foreach (sSystemMetric met in SystemMonitor.Current.GetValues(new SystemMetricTypes[] { SystemMetricTypes.HD_Configs, SystemMetricTypes.HD_Database, SystemMetricTypes.HD_Logs, SystemMetricTypes.HD_Voicemail }))
            {
                ret.Add(new NameValuePair(met.Type.ToString(), met.Val));
            }
            return ret;
        }

        [WebMethod(true)]
        public List<NameValuePair> GetInboundConnectionStats()
        {
            List<NameValuePair> ret = new List<NameValuePair>();
            foreach (sSystemMetric met in SystemMonitor.Current.GetValues(new SystemMetricTypes[] { SystemMetricTypes.Average_Inbound_Connection_Duration,SystemMetricTypes.Max_Inbound_Connection_Duration}))
            {
                ret.Add(new NameValuePair(met.Type.ToString(), met.Val));
            }
            return ret;
        }
    }
}
