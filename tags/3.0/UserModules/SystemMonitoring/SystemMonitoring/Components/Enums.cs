using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components
{
    public enum SystemMetricTypes
    {
        Threads,
        Processes,
        CPU_Free,
        HD_Used,
        RAM_Used,
        NET_In,
        NET_Out,
        HD_Logs,
        HD_Configs,
        HD_Database,
        HD_Voicemail,
        CALLS_Started,
        CALLS_Ended,
        CALLS_Active,
        TOP_Cpu,
        TOP_Memory,
        System_Events,
        Freeswitch_Events,
        Average_Inbound_Connection_Duration,
        Max_Inbound_Connection_Duration,
        Config_Server_Threads,
        Config_Server_Memory,
        Config_Server_CPU_Used
    }

    public enum MetricUnits
    {
        PERCENTAGE,
        KB,
        MB,
        B,
        GB,
        GENERIC,
        MILLISECONDS,
        SECONDS,
        MINUTES,
        HOURS
    }
}
