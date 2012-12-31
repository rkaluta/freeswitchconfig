using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components
{
    class WrappedPerformanceCounter
    {
        private static readonly Regex _regNet = new Regex("(\\d+)\\s+\\d+\\s+\\d+\\s+\\d+\\s+\\d+\\s+\\d+\\s+\\d+\\s+\\d+\\s+(\\d+)\\s+\\d+\\s+\\d+\\s+\\d+\\s+\\d+\\s+\\d+\\s+\\d+\\s+\\d+", RegexOptions.Compiled | RegexOptions.ECMAScript);

        private PerformanceCounter _counter;
        private int _pid;
        private long _preCPUTime;
        private long _prePROCTime;
        private long _preBytes;

        public string InstanceName
        {
            get { return _counter.InstanceName; }
        }

        public string CounterName
        {
            get { return _counter.CounterName; }
        }

        private long ReadCurCPUTime()
        {
            long ret = 0;
            StreamReader sr = new StreamReader(new FileStream("/proc/stat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            string line = sr.ReadLine();
            sr.Close();
            string[] split = line.Replace("cpu", "").TrimStart(' ').Split(' ');
            foreach (string str in split)
            {
                if (str.Length > 0)
                    ret += long.Parse(str);
            }
            return ret;
        }

        private long ReadCurProcessTime()
        {
            StreamReader sr = new StreamReader(new FileStream("/proc/" + _pid.ToString() + "/stat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            string line = sr.ReadToEnd();
            sr.Close();
            string[] split = line.Split(' ');
            return long.Parse(split[14]) + long.Parse(split[15]);
        }

        public WrappedPerformanceCounter(string category, string counterName, string instanceName)
        {
            _counter = null;
            if (Utility.MonoVersion != null && Environment.OSVersion.Platform == PlatformID.Unix)
            {
                switch (category)
                {
                    case "Process":
                        switch (counterName)
                        {
                            case "Thread Count":
                            case "Working Set":
                            case "% Processor Time":
                                if (instanceName == "_Current")
                                {
                                    _pid = Process.GetCurrentProcess().Id;
                                }
                                else if (instanceName != "_Total")
                                {
                                    foreach (Process p in Process.GetProcesses())
                                    {
                                        if (p.ProcessName == instanceName)
                                        {
                                            _pid = p.Id;
                                            break;
                                        }
                                    }
                                }
                                if (counterName == "% Processor Time")
                                {
                                    _preCPUTime = ReadCurCPUTime();
                                    _prePROCTime = ReadCurProcessTime();
                                }
                                break;
                        }
                        break;
                    case "Network Interface":
                        StreamReader sr = new StreamReader(new FileStream("/proc/net/dev", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                        string line = sr.ReadToEnd();
                        sr.Close();
                        foreach (string str in line.Split('\n'))
                        {
                            if (str.TrimStart().StartsWith(instanceName))
                            {
                                switch (counterName)
                                {
                                    case "Bytes Recieved/sec":
                                        _preBytes = long.Parse(_regNet.Match(str.Split(':')[1]).Groups[1].Value);
                                        break;
                                    case "Bytes Sent/sec":
                                        _preBytes = long.Parse(_regNet.Match(str.Split(':')[1]).Groups[2].Value);
                                        break;
                                }
                                break;
                            }
                        }
                        break;
                }
            }
            if (_counter == null)
                _counter = new PerformanceCounter(category, counterName, instanceName);
        }

        public float NextValue()
        {
            float ret = -1;
            if (Utility.MonoVersion != null && Environment.OSVersion.Platform == PlatformID.Unix)
            {
                StreamReader sr;
                string line;
                switch (_counter.CategoryName)
                {
                    case "Process":
                        switch (_counter.CounterName)
                        {
                            case "Thread Count":
                                if (InstanceName == "_Total")
                                {
                                    ret = 0;
                                    foreach (Process pr in Process.GetProcesses())
                                    {
                                        sr = new StreamReader(new FileStream("/proc/" + pr.Id.ToString() + "/status", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                                        line = sr.ReadToEnd();
                                        sr.Close();
                                        foreach (string str in line.Split('\n'))
                                        {
                                            string[] split = str.Split(':');
                                            if (split[0] == "Threads")
                                            {
                                                ret += long.Parse(split[1]);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    sr = new StreamReader(new FileStream("/proc/" + _pid.ToString() + "/status", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                                    line = sr.ReadToEnd();
                                    sr.Close();
                                    foreach (string str in line.Split('\n'))
                                    {
                                        string[] split = str.Split(':');
                                        if (split[0] == "Threads")
                                        {
                                            ret = long.Parse(split[1]);
                                            break;
                                        }
                                    }
                                }
                                break;
                            case "Working Set":
                                switch (_counter.InstanceName)
                                {
                                    case "_Total":
                                        sr = new StreamReader(new FileStream("/proc/meminfo", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                                        line = sr.ReadToEnd();
                                        sr.Close();
                                        long tot = 0;
                                        long free = 0;
                                        foreach (string str in line.Split('\n'))
                                        {
                                            string[] split = str.Split(':');
                                            switch (split[0])
                                            {
                                                case "MemTotal":
                                                    tot = long.Parse(split[1].Replace("kB", "").Replace(" ", "")) * 1024;
                                                    break;
                                                case "MemFree":
                                                    free = long.Parse(split[1].Replace("kB", "").Replace(" ", "")) * 1024;
                                                    break;
                                            }
                                        }
                                        ret = tot - free;
                                        break;
                                    default:
                                        sr = new StreamReader(new FileStream("/proc/" + _pid.ToString() + "/status", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                                        line = sr.ReadToEnd();
                                        sr.Close();
                                        foreach (string str in line.Split('\n'))
                                        {
                                            string[] split = str.Split(':');
                                            if (split[0] == "VmSize")
                                            {
                                                ret = long.Parse(split[1].Replace("kB", "").Replace(" ", "")) * 1024;
                                                break;
                                            }
                                        }
                                        break;
                                }
                                break;
                            case "% Processor Time":
                                long curCPUTime = ReadCurCPUTime();
                                long curProcessTime = ReadCurProcessTime();
                                ret = ((float)curProcessTime - (float)_prePROCTime) / ((float)curCPUTime - (float)_preCPUTime) * 100;
                                _prePROCTime = curProcessTime;
                                _preCPUTime = curCPUTime;
                                break;
                        }
                        break;
                    case "Network Interface":
                        sr = new StreamReader(new FileStream("/proc/net/dev", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                        line = sr.ReadToEnd();
                        sr.Close();
                        long curBytes = 0;
                        foreach (string str in line.Split('\n'))
                        {
                            if (str.TrimStart().StartsWith(InstanceName))
                            {
                                switch (CounterName)
                                {
                                    case "Bytes Recieved/sec":
                                        curBytes = long.Parse(_regNet.Match(str.Split(':')[1]).Groups[1].Value);
                                        break;
                                    case "Bytes Sent/sec":
                                        curBytes = long.Parse(_regNet.Match(str.Split(':')[1]).Groups[2].Value);
                                        break;
                                }
                                break;
                            }
                        }
                        ret = curBytes - _preBytes;
                        _preBytes = curBytes;
                        break;
                }
            }
            if (ret < 0)
                ret = _counter.NextValue();
            return ret;
        }
    }
}

