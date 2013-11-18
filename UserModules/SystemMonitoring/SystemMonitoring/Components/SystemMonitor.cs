using System;
using System.Collections.Generic;
using System.Text;

using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using Org.Reddragonit.EmbeddedWebServer.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components
{
    public class SystemMonitor : IBackgroundOperationContainer,IEventHandler
    {
        private Dictionary<SystemMetricTypes, object> _values;
        private static object _lock = new object();
        private CallEventMonitor _callMonitor;
        private EventMonitor _eventMonitor;
        private ConnectionEventMonitor _connectionMonitor;
        private sFreeSwitchStatus? _curStatus;

        private List<WrappedPerformanceCounter> cpus;
        private WrappedPerformanceCounter processes_threads;
        private WrappedPerformanceCounter current_process_thread;
        private WrappedPerformanceCounter current_process_cpu;
        private WrappedPerformanceCounter current_process_mem;
        private WrappedPerformanceCounter memory;
        private List<WrappedPerformanceCounter> processes_cpu;
        private List<WrappedPerformanceCounter> processes_mem;
        private List<WrappedPerformanceCounter> networks;
        private List<string> _drivesMonitored;

        private long GetFolderSizeInBytes(string path)
        {
            long ret = 0;
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
                return 0;
            foreach (FileInfo fi in di.GetFiles())
                ret += fi.Length;
            foreach (DirectoryInfo d in di.GetDirectories())
                ret += GetFolderSizeInBytes(d.FullName);
            return ret;
        }

        [BackgroundOperationCall(-1,-1,-1,-1,BackgroundOperationDaysOfWeek.All)]
        public static void BackgroundUpdateMetrics()
        {
            Monitor.Enter(_lock);
            try
            {
                Current.UpdateMetrics();
                EventController.TriggerEvent(new MetricsUpdatedEvent(Current.GetValues((SystemMetricTypes[])Enum.GetValues(typeof(SystemMetricTypes)))));
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        private static SystemMonitor _current;
        public static SystemMonitor Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new SystemMonitor();
                }
                return _current;
            }
        }

        internal void UpdateFreeswitchMetrics(sFreeSwitchUpTime uptime, long sessionCount, long sessionsPerSecond,long sessionsSinceStart)
        {
            try
            {
                _curStatus = new sFreeSwitchStatus(sessionsSinceStart, sessionCount, sessionsPerSecond, uptime);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private SystemMonitor()
        {
            EventController.RegisterEventHandler(this);
            if (Settings.Current[Constants.BASE_PATH_NAME] == null)
                Settings.Current[Constants.BASE_PATH_NAME] = Constants.DEFAULT_BASE_BATH;
            _drivesMonitored = SystemInformation.Current.Drives;
            _values = new Dictionary<SystemMetricTypes, object>();
            _callMonitor = new CallEventMonitor();
            _eventMonitor = new EventMonitor();
            _connectionMonitor = new ConnectionEventMonitor();
            if (ModuleController.Current.IsModuleEnabled("ESL"))
            {
                ModuleController.Current.InvokeModuleMethod("ESL", "RegisterEvent", new NameValuePair[]{new NameValuePair("eventName",CallEventMonitor.CREATE_EVENT_NAME)},false);
                ModuleController.Current.InvokeModuleMethod("ESL", "RegisterEvent", new NameValuePair[] { new NameValuePair("eventName", CallEventMonitor.DESTROY_EVENT_NAME) }, false);
                ModuleController.Current.InvokeModuleMethod("ESL", "RegisterEvent", new NameValuePair[] { new NameValuePair("eventName", HeartbeatEventMonitor.EVENT_NAME) }, false);
            }
            EventController.RegisterEventHandler(_callMonitor);
            EventController.RegisterEventHandler(_eventMonitor);
            EventController.RegisterEventHandler(new HeartbeatEventMonitor());
            cpus = new List<WrappedPerformanceCounter>();
            for (int x = 0; x < Environment.ProcessorCount; x++)
            {
                cpus.Add(new WrappedPerformanceCounter("Processor", "% Processor Time", x.ToString()));
            }
            memory = new WrappedPerformanceCounter("Process", "Working Set", "_Total");
            processes_cpu = new List<WrappedPerformanceCounter>();
            processes_mem = new List<WrappedPerformanceCounter>();
            processes_threads = new WrappedPerformanceCounter("Process", "Thread Count", "_Total");
            current_process_thread = new WrappedPerformanceCounter("Process", "Thread Count", "_Current");
            current_process_cpu = new WrappedPerformanceCounter("Process", "% Processor Time", "_Current");
            current_process_mem = new WrappedPerformanceCounter("Process", "Working Set", "_Current");
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName != "Idle")
                {
                    processes_cpu.Add(new WrappedPerformanceCounter("Process", "% Processor Time", p.ProcessName));
                    processes_mem.Add(new WrappedPerformanceCounter("Process", "Working Set", p.ProcessName));
                }
            }
            networks = new List<WrappedPerformanceCounter>();
            foreach (string str in new PerformanceCounterCategory("Network Interface").GetInstanceNames())
            {
                networks.Add(new WrappedPerformanceCounter("Network Interface", "Bytes Recieved/sec", str));
                networks.Add(new WrappedPerformanceCounter("Network Interface", "Bytes Sent/sec", str));
            }
            EventController.RegisterEventHandler(this);
        }

        private void Init()
        {
        }

        ~SystemMonitor()
        {
            EventController.UnRegisterEventHandler(_callMonitor);
        }

        public List<sSystemMetric> GetValues(SystemMetricTypes[] types)
        {
            Monitor.Enter(_lock);
            List<sSystemMetric> ret = new List<sSystemMetric>();
            foreach (SystemMetricTypes type in types)
            {
                if (_values.ContainsKey(type))
                {
                    if (_values[type] is List<sSystemMetric>)
                        ret.AddRange((List<sSystemMetric>)_values[type]);
                    else if (_values[type] is sSystemMetric)
                        ret.Add((sSystemMetric)_values[type]);
                }
            }
            Monitor.Exit(_lock);
            return ret;
        }

        private void UpdateMetrics()
        {
            Log.Trace("Updating the System Metrics with Call Event Monitor.");
            try
            {
                _values = _callMonitor.UpdateValues(_values);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            Log.Trace("Updating the System Metrics with Event Monitor.");
            try{
                _values = _eventMonitor.UpdateValues(_values);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            Log.Trace("Updating the System Metrics with Connection Event Monitor.");
            try{
                _values = _connectionMonitor.UpdateValues(_values);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            Log.Trace("Gathering the System Metrics from dstat.");
            try{
                Current.UpdatePerformanceMetrics(ref _values);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            Log.Trace("Gathering the System Metrics from the file usage.");
            try{
                Current.UpdateFileUsageInformation(ref _values);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            Log.Trace("Gathering the System Metrics from the partition usage.");
            try{
                UpdateDriveUsageInformation();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void UpdateDriveUsageInformation()
        {
            _values.Remove(SystemMetricTypes.HD_Used);
            List<sSystemMetric> tmp = new List<sSystemMetric>();
            foreach (string str in _drivesMonitored)
            {
                tmp.Add(new sSystemMetric(SystemMetricTypes.HD_Used, MetricUnits.PERCENTAGE, (long)Math.Ceiling(SystemInformation.Current.GetPercentageUsedForPartition(str) * 100), SystemInformation.Current.GetMountPointForPartition(str)));
            }
            _values.Add(SystemMetricTypes.HD_Used, tmp);
        }

        private static Regex _regDuNumber = new Regex("^(\\d+)\\s*.+$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        internal void UpdateFileUsageInformation(ref Dictionary<SystemMetricTypes,object> values)
        {
            values.Remove(SystemMetricTypes.HD_Logs);
            values.Add(SystemMetricTypes.HD_Logs, new sSystemMetric(SystemMetricTypes.HD_Logs, MetricUnits.B, GetFolderSizeInBytes(OperatingSystemPaths.Current.LogsPath)));
            values.Remove(SystemMetricTypes.HD_Database);
            values.Add(SystemMetricTypes.HD_Database, new sSystemMetric(SystemMetricTypes.HD_Database, MetricUnits.B, GetFolderSizeInBytes(OperatingSystemPaths.Current.FirebirdDataPath)));
            values.Remove(SystemMetricTypes.HD_Configs);
            values.Add(SystemMetricTypes.HD_Configs, new sSystemMetric(SystemMetricTypes.HD_Configs, MetricUnits.B, GetFolderSizeInBytes(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR)));
            values.Remove(SystemMetricTypes.HD_Voicemail);
            values.Add(SystemMetricTypes.HD_Voicemail, new sSystemMetric(SystemMetricTypes.HD_Voicemail, MetricUnits.B,GetFolderSizeInBytes(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar +Constants.DEFAULT_VOICEMAIL_PATH)));
        }

        internal void UpdatePerformanceMetrics(ref Dictionary<SystemMetricTypes, object> values)
        {
            values.Remove(SystemMetricTypes.CPU_Free);
            List<sSystemMetric> cpuMets = new List<sSystemMetric>();
            for (int x = 0; x < cpus.Count; x++)
                cpuMets.Add(new sSystemMetric(SystemMetricTypes.CPU_Free, MetricUnits.PERCENTAGE, (decimal)(100 - cpus[x].NextValue())*100, cpus[x].InstanceName));
            values.Add(SystemMetricTypes.CPU_Free, cpuMets);

            List<WrappedPerformanceCounter> pcpu = new List<WrappedPerformanceCounter>();
            List<WrappedPerformanceCounter> pmem = new List<WrappedPerformanceCounter>();
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName != "Idle")
                {
                    bool add = true;
                    for (int x = 0; x < processes_cpu.Count; x++)
                    {
                        if (processes_cpu[x].InstanceName == p.ProcessName)
                        {
                            add = false;
                            pcpu.Add(processes_cpu[x]);
                            pmem.Add(processes_mem[x]);
                            break;
                        }
                    }
                    if (add)
                    {
                        pcpu.Add(new WrappedPerformanceCounter("Process", "% Processor Time", p.ProcessName));
                        pmem.Add(new WrappedPerformanceCounter("Process", "Working Set", p.ProcessName));
                    }
                }
            }
            processes_cpu = pcpu;
            processes_mem = pmem;
            
            values.Remove(SystemMetricTypes.Processes);
            values.Add(SystemMetricTypes.Processes, new sSystemMetric(SystemMetricTypes.Processes, MetricUnits.GENERIC, processes_cpu.Count));

            values.Remove(SystemMetricTypes.Threads);
            values.Add(SystemMetricTypes.Threads, new sSystemMetric(SystemMetricTypes.Threads, MetricUnits.GENERIC, processes_threads.NextValue()));
            values.Remove(SystemMetricTypes.Config_Server_Threads);
            values.Add(SystemMetricTypes.Config_Server_Threads, new sSystemMetric(SystemMetricTypes.Config_Server_Threads, MetricUnits.GENERIC, current_process_thread.NextValue()));
            values.Remove(SystemMetricTypes.RAM_Used);
            values.Add(SystemMetricTypes.RAM_Used, new sSystemMetric(SystemMetricTypes.RAM_Used, MetricUnits.B, memory.NextValue()));
            values.Remove(SystemMetricTypes.Config_Server_CPU_Used);
            values.Add(SystemMetricTypes.Config_Server_CPU_Used, new sSystemMetric(SystemMetricTypes.Config_Server_CPU_Used, MetricUnits.PERCENTAGE, current_process_cpu.NextValue() * 100));
            values.Remove(SystemMetricTypes.Config_Server_Memory);
            values.Add(SystemMetricTypes.Config_Server_Memory, new sSystemMetric(SystemMetricTypes.Config_Server_Memory, MetricUnits.B, current_process_mem.NextValue()));

            values.Remove(SystemMetricTypes.TOP_Cpu);
            float highCPU = float.MinValue;
            string highCPUName = "";
            for (int x = 0; x < processes_cpu.Count; x++)
            {
                float tmp = processes_cpu[x].NextValue();
                if (tmp > highCPU)
                {
                    highCPU = tmp;
                    highCPUName = processes_cpu[x].InstanceName;
                }
            }
            values.Add(SystemMetricTypes.TOP_Cpu, new sSystemMetric(SystemMetricTypes.TOP_Cpu, MetricUnits.PERCENTAGE, highCPU*100, highCPUName));

            values.Remove(SystemMetricTypes.TOP_Memory);
            float highMem = float.MinValue;
            string highMemName = "";
            for (int x = 0; x < processes_mem.Count; x++)
            {
                float tmp = processes_mem[x].NextValue();
                if (tmp > highMem)
                {
                    highMem = tmp;
                    highMemName = processes_mem[x].InstanceName;
                }
            }
            values.Add(SystemMetricTypes.TOP_Memory, new sSystemMetric(SystemMetricTypes.TOP_Memory, MetricUnits.B, highMem, highMemName));

            values.Remove(SystemMetricTypes.NET_In);
            values.Remove(SystemMetricTypes.NET_Out);
            List<sSystemMetric> net_in = new List<sSystemMetric>();
            List<sSystemMetric> net_out = new List<sSystemMetric>();
            foreach (WrappedPerformanceCounter wpc in networks)
            {
                if (wpc.CounterName == "Bytes Recieved/sec")
                    net_in.Add(new sSystemMetric(SystemMetricTypes.NET_In, MetricUnits.B, wpc.NextValue(), wpc.InstanceName));
                else
                    net_out.Add(new sSystemMetric(SystemMetricTypes.NET_Out, MetricUnits.B, wpc.NextValue(), wpc.InstanceName));
            }
            values.Add(SystemMetricTypes.NET_In, net_in);
            values.Add(SystemMetricTypes.NET_Out, net_out);
        }

        public sFreeSwitchStatus? CurrentFreeswitchStatus
        {
            get
            {
                sFreeSwitchStatus? ret = null;
                Monitor.Enter(_lock);
                ret = _curStatus;
                Monitor.Exit(_lock);
                return ret;
            }
        }

        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            return Event.Name == "EventSocketReconnect"
                || Event is ModuleEnabledEvent;
        }

        public void ProcessEvent(IEvent Event)
        {
            switch (Event.Name)
            {
                case "ModuleEnabledEvent":
                case "EventSocketReconnect":
                    if (((ModuleEnabledEvent)Event).Module.ModuleName == "ESL")
                    {
                        ModuleController.Current.InvokeModuleMethod("ESL", "RegisterEvent", new NameValuePair[] { new NameValuePair("eventName", CallEventMonitor.CREATE_EVENT_NAME) }, false);
                        ModuleController.Current.InvokeModuleMethod("ESL", "RegisterEvent", new NameValuePair[] { new NameValuePair("eventName", CallEventMonitor.DESTROY_EVENT_NAME) }, false);
                        ModuleController.Current.InvokeModuleMethod("ESL", "RegisterEvent", new NameValuePair[] { new NameValuePair("eventName", HeartbeatEventMonitor.EVENT_NAME) }, false);
                    }
                    break;
            }
        }

        #endregion
    }
}
