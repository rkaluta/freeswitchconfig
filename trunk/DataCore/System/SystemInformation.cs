using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System
{
    public class SystemInformation : MarshalByRefObject
    {
        private static SystemInformation _current;
        public static SystemInformation Current
        {
            get
            {
                if (_current == null)
                    _current = (SystemInformation)OSClassProxy.Instance(new SystemInformation());
                return _current;
            }
        }

        private string ReadFile(string path)
        {
            StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            string ret = sr.ReadToEnd();
            sr.Close();
            return ret;
        }

        private SystemInformation()
        {
        }

        [OperatingSystemOverridablePropertyAttribute("Path to DF command.")]
        public string DFCommand
        {
            get { return "/bin/df"; }
        }

        [OperatingSystemOverridablePropertyAttribute("This property call returns the CPU's available on the system.")]
        public List<sCPU> CPUs
        {
            get
            {
                List<sCPU> ret = new List<sCPU>();
                string tmp = ReadFile("/proc/cpuinfo");
                int sockNum = -1;
                foreach (string str in tmp.Split('\n'))
                {
                    if (str.StartsWith("processor"))
                        sockNum = int.Parse(str.Substring(str.LastIndexOf(":") + 1).Trim());
                    else if (str.StartsWith("cpu MHz"))
                    {
                        double freq = double.Parse(str.Substring(str.IndexOf(":") + 1).Trim());
                        ret.Add(new sCPU(sockNum, (int)Math.Ceiling(freq)));
                    }
                }
                return ret;
            }
        }

        [OperatingSystemOverridablePropertyAttribute("This property call returns the total available RAM.")]
        public sMemoryValue RAM
        {
            get
            {
                string res = ReadFile("/proc/meminfo");
                ByteUnits unit = ByteUnits.Byte;
                long val = 0;
                foreach (string str in res.Split('\n'))
                {
                    if (str.StartsWith("MemTotal"))
                    {
                        string tmp = str.Substring(str.IndexOf(":") + 1).Trim();
                        if (tmp.Contains("MB"))
                        {
                            unit = ByteUnits.MegaByte;
                            tmp = tmp.Replace("MB", "");
                        }
                        else if (tmp.Contains("kB"))
                        {
                            unit = ByteUnits.KiloByte;
                            tmp = tmp.Replace("kB", "");
                        }
                        else if (tmp.Contains("B"))
                        {
                            tmp = tmp.Replace("B", "");
                        }
                        val = long.Parse(tmp);
                    }
                }
                return new sMemoryValue(val, unit);
            }
        }

        [OperatingSystemOverridablePropertyAttribute("This property call returns the total available Swap space.")]
        public sMemoryValue Swap
        {
            get
            {
                string res = ReadFile("/proc/meminfo");
                ByteUnits unit = ByteUnits.Byte;
                long val = 0;
                foreach (string str in res.Split('\n'))
                {
                    if (str.StartsWith("SwapTotal"))
                    {
                        string tmp = str.Substring(str.IndexOf(":") + 1).Trim();
                        if (tmp.Contains("MB"))
                        {
                            unit = ByteUnits.MegaByte;
                            tmp = tmp.Replace("MB", "");
                        }
                        else if (tmp.Contains("kB"))
                        {
                            unit = ByteUnits.KiloByte;
                            tmp = tmp.Replace("kB", "");
                        }
                        else if (tmp.Contains("B"))
                        {
                            tmp = tmp.Replace("B", "");
                        }
                        val = long.Parse(tmp);
                    }
                }
                return new sMemoryValue(val, unit);
            }
        }

        private static readonly Regex _regDrive = new Regex("^\\s*\\d+\\s+\\d+\\s+\\d+\\s+((hd|sd)[a-z])\\s*$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        [OperatingSystemOverridablePropertyAttribute("This property call returns the list of available drives (USB,Sata,IDE,cdrom)")]
        public List<string> Drives{
            get{
                string res = ReadFile("/proc/partitions");
                List<string> drives = new List<string>();
                foreach (string str in res.Split('\n'))
                {
                    if (_regDrive.IsMatch(str.Trim()))
                        drives.Add(_regDrive.Match(str.Trim()).Groups[1].Value);
                }
                return drives;
            }
        }

        private static readonly Regex _regPart = new Regex("^\\s*\\d+\\s+\\d+\\s+\\d+\\s+((hd|sd)[a-z]\\d+)\\s*$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        [OperatingSystemOverridablePropertyAttribute("This property call returns the list of partitions")]
        public List<string> Partitions
        {
            get
            {
                string res = ReadFile("/proc/partitions");
                List<string> parts = new List<string>();
                foreach (string str in res.Split('\n'))
                {
                    if (_regPart.IsMatch(str.Trim()))
                        parts.Add(_regPart.Match(str.Trim()).Groups[1].Value);
                }
                return parts;
            }
        }

        private static readonly Regex _regPercentageUsed = new Regex("^\\s*.+\\s+\\d+\\s+\\d+\\s+\\d+\\s+(\\d+)%\\s+.+\\s*$",RegexOptions.Compiled|RegexOptions.ECMAScript);
        [OperatingSystemOverridableFunctionAttribute("This method is called to return the percentage of space used on the partition specified.Parameters (string partition)")]
        public double GetPercentageUsedForPartition(string partition)
        {
            if (!partition.StartsWith("/dev/")&&
                (partition.StartsWith("hd")||partition.StartsWith("sd")))
                partition = "/dev/" + partition;
            double ret = -1;
            string res = "";
            if (ProcessSecurityControl.Current.SecurityClosed)
                res = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, this.DFCommand + " " + partition, true);
            else
                res = Utility.ExecuteProgram(this.DFCommand, partition, true);
            foreach (string str in res.Split('\n'))
            {
                if (_regPercentageUsed.IsMatch(str))
                    ret = double.Parse(_regPercentageUsed.Match(str).Groups[1].Value);
            }
            return ret;
        }

        [OperatingSystemOverridableFunctionAttribute("This method is called to return the percentage of space used on the partition specified.Parameters (string partition)")]
        public string GetMountPointForPartition(string partition)
        {
            if (!partition.StartsWith("/dev") && !partition.StartsWith("hd") && !partition.StartsWith("sd"))
                return partition;
            if (!partition.StartsWith("/dev/"))
                partition = "/dev/" + partition;
            string ret = null;
            string res = ReadFile("/proc/mounts");
            foreach (string str in res.Split('\n'))
            {
                if (str.StartsWith(partition+" "))
                {
                    ret = str.Split(' ')[1];
                    break;
                }
            }
            return ret;
        }
    }
}
