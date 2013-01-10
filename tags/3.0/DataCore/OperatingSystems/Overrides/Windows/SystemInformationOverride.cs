using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using System.Reflection;
using System.Collections;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems.Overrides.Windows
{
    public class SystemInformationOverride : MarshalByRefObject, IOperatingSystemOverrideContainer
    {
        private const string REGISTRY_TYPE_PATH = "Microsoft.Win32.Registry";
        private const string REGISTRY_KEY_TYPE_PATH = "Microsoft.Win32.RegistryKey";
        private const string MANAGEMENT_ASSEMBLY_NAME = "System.Management, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        private const string MANAGEMENT_OBJECT_SEARCHER_TYPE_PATH = "System.Management.ManagementObjectSearcher";
        private const string MANAGEMENT_OBJECT_TYPE_PATH = "System.Management.ManagementObject";

        private Assembly assembly
        {
            get
            {
                Assembly ret = Assembly.Load(MANAGEMENT_ASSEMBLY_NAME);
                return ret;
            }
        }

        private object LocalMachine
        {
            get {
                return Type.GetType(REGISTRY_TYPE_PATH).GetField("LocalMachine").GetValue(null);
            }
        }

        private MethodInfo GetValueMethod
        {
            get
            {
                return Type.GetType(REGISTRY_KEY_TYPE_PATH).GetMethod("GetValue", new Type[] { typeof(string) }); 
            }
        }

        private MethodInfo GetSubKeyNamesMethod
        {
            get {
                return Type.GetType(REGISTRY_KEY_TYPE_PATH).GetMethod("GetSubKeyNames"); 
            }
        }

        private MethodInfo OpenSubKeyMethod
        {
            get {
                return Type.GetType(REGISTRY_KEY_TYPE_PATH).GetMethod("OpenSubKey", new Type[] { typeof(string) }); 
            }
        }

        private IEnumerable ExecuteManagementQuery(string query)
        {
            object searcher = assembly.GetType(MANAGEMENT_OBJECT_SEARCHER_TYPE_PATH).GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { query });
            return (IEnumerable)assembly.GetType(MANAGEMENT_OBJECT_SEARCHER_TYPE_PATH).GetMethod("Get",Type.EmptyTypes).Invoke(searcher, new object[] { });
        }

        private MethodInfo GetPropertyValueMethod
        {
            get { 
                return assembly.GetType(MANAGEMENT_OBJECT_TYPE_PATH).GetMethod("GetPropertyValue",new Type[]{typeof(string)}); 
            }
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.SystemInformation",
            "CPUs",
            "Windows",
            "5.0",
            "6.9")]
        public List<sCPU> CPUs
        {
            get
            {
                List<sCPU> ret = new List<sCPU>();
                object lkey = LocalMachine;
                object cps = OpenSubKeyMethod.Invoke(lkey,new object[]{"HARDWARE\\DESCRIPTION\\System\\CentralProcessor"});
                foreach (string cpuNum in (string[])GetSubKeyNamesMethod.Invoke(cps, new object[] { }))
                {
                    object cpKey = OpenSubKeyMethod.Invoke(cps,new object[]{cpuNum});
                    ret.Add(new sCPU(int.Parse(cpuNum), int.Parse(GetValueMethod.Invoke(cpKey, new object[] { "~MHz" }).ToString())));
                }
                return ret;
            }
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.SystemInformation",
            "RAM",
            "Windows",
            "5.0",
            "6.9")]
        public sMemoryValue RAM
        {
            get
            {
                long val = 0;
                foreach (object obj in ExecuteManagementQuery("SELECT * FROM Win32_OperatingSystem"))
                {
                    val = long.Parse(GetPropertyValueMethod.Invoke(obj, new object[] { "TotalVisibleMemorySize" }).ToString());
                }
                return new sMemoryValue(val, ByteUnits.KiloByte);
            }
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.SystemInformation",
            "Swap",
            "Windows",
            "5.0",
            "6.9")]
        public sMemoryValue Swap
        {
            get
            {
                long val = 0;
                foreach (object obj in ExecuteManagementQuery("SELECT * FROM Win32_OperatingSystem"))
                {
                    val = long.Parse(GetPropertyValueMethod.Invoke(obj, new object[] { "TotalVirtualMemorySize" }).ToString()) - long.Parse(GetPropertyValueMethod.Invoke(obj, new object[] { "TotalVisibleMemorySize" }).ToString());
                }
                return new sMemoryValue(val, ByteUnits.KiloByte);
            }
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.SystemInformation",
            "Drives",
            "Windows",
            "5.0",
            "6.9")]
        public List<string> Drives
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (object obj in ExecuteManagementQuery("SELECT * FROM Win32_DiskDrive"))
                {
                    ret.Add(GetPropertyValueMethod.Invoke(obj,new object[]{"DeviceID"}).ToString());
                }
                return ret;
            }
        }

        [OperatingSystemPropertyOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.SystemInformation",
            "Partitions",
            "Windows",
            "5.0",
            "6.9")]
        public List<string> Partitions
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (DriveInfo di in DriveInfo.GetDrives())
                {
                    if (di.DriveType == DriveType.Fixed)
                        ret.Add(di.Name);
                }
                return ret;
            }
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.SystemInformation",
            "GetPercentageUsedForPartition",
            "Windows",
            "5.0",
            "6.9")]
        public double GetPercentageUsedForPartition(string partition)
        {
            DriveInfo di = new DriveInfo(partition);
            return (1 - (double)Math.Round((decimal)di.TotalFreeSpace / (decimal)di.TotalSize,6))*100;
        }

        [OperatingSystemFunctionOverride("Org.Reddragonit.FreeSwitchConfig.DataCore.System.SystemInformation",
            "GetMountPointForPartition",
            "Windows",
            "5.0",
            "6.9")]
        public string GetMountPointForPartition(string partition)
        {
            return partition;
        }
    }
}
