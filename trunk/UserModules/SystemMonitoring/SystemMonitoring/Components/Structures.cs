using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components
{
    public struct sFreeSwitchUpTime
    {
        private int _year;
        public int Year
        {
            get { return _year; }
        }

        private int _day;
        public int Day
        {
            get { return _day; }
        }

        private int _hour;
        public int Hour
        {
            get { return _hour; }
        }

        private int _minute;
        public int Minute
        {
            get { return _minute; }
        }

        private int _second;
        public int Second{
            get{return _second;}
        }

        private int _milliSeconds;
        public int MillieSeconds
        {
            get { return _milliSeconds; }
        }

        private int _microSeconds;
        public int MicroSeconds
        {
            get { return _microSeconds; }
        }

        internal sFreeSwitchUpTime(string msg)
        {
            Regex _regUpTime = new Regex("UP\\s+(\\d+)\\s+years?,\\s+(\\d+)\\s+days?,\\s+(\\d+)\\s+hours?,\\s+(\\d+)\\s+minutes?,\\s+(\\d+)\\s+seconds?,\\s+(\\d+)\\s+milliseconds?,\\s+(\\d+)\\s+microseconds?\\s*", RegexOptions.Compiled | RegexOptions.ECMAScript);
            Match m = _regUpTime.Match(msg);
            _year = int.Parse(m.Groups[1].Value);
            _day = int.Parse(m.Groups[2].Value);
            _hour = int.Parse(m.Groups[3].Value);
            _minute = int.Parse(m.Groups[4].Value);
            _second = int.Parse(m.Groups[5].Value);
            _milliSeconds = int.Parse(m.Groups[6].Value);
            _microSeconds = int.Parse(m.Groups[7].Value);
        }

        public override string ToString()
        {
            return string.Format("{0} years, {1} days, {2} hours, {3} minutes, {4} seconds, {5} milliseconds, {6} microseconds",
                new object[] { _year, _day, _hour, _minute, _second, _milliSeconds, _microSeconds });
        }
    }

    public struct sFreeSwitchStatus
    {
        public string UpTimeString
        {
            get { return _uptime.ToString(); }
        }

        private long _totalSessions;
        public long TotalSessions
        {
            get { return _totalSessions; }
        }

        private long _currentSessions;
        public long CurrentSessions
        {
            get { return _currentSessions; }
        }

        private long _sessionsPerSecond;
        public long SessionsPerSecond
        {
            get { return _sessionsPerSecond; }
        }

        private sFreeSwitchUpTime _uptime;
        public sFreeSwitchUpTime UpTime
        {
            get { return _uptime; }
        }

        public sFreeSwitchStatus(long totalSessions, long currentSessions, long sessionsPerSecond, sFreeSwitchUpTime uptime)
        {
            _totalSessions = totalSessions;
            _currentSessions = currentSessions;
            _sessionsPerSecond = sessionsPerSecond;
            _uptime = uptime;
        }
    }

    public struct sSystemMetric
    {
        private SystemMetricTypes _type;
        public SystemMetricTypes Type
        {
            get { return _type; }
        }

        private string _additional;
        public string Additional
        {
            get { return _additional; }
        }

        public long ToKB()
        {
            long ret = -1;
            switch (_unit)
            { 
                case MetricUnits.B:
                    ret = (long)Math.Floor((decimal)_val / (decimal)Constants.KB_BYTE_COUNT);
                    break;
                case MetricUnits.GB:
                    ret = (long)Math.Floor((decimal)_val / ((decimal)Constants.GB_BYTE_COUNT/(decimal)Constants.KB_BYTE_COUNT));
                    break;
                case MetricUnits.KB:
                    ret = _val;
                    break;
                case MetricUnits.MB:
                    ret = (long)Math.Floor((decimal)_val / ((decimal)Constants.MB_BYTE_COUNT / (decimal)Constants.KB_BYTE_COUNT));
                    break;
            }
            return ret;
        }

        private MetricUnits _unit;
        private long _val;

        public string Val
        {
            get
            {
                string ret = "";
                switch (_unit)
                {
                    case MetricUnits.PERCENTAGE:
                        ret = Math.Round((decimal)_val / (decimal)100, 2).ToString() + "%";
                        break;
                    case MetricUnits.B:
                        if (_val > Constants.GB_BYTE_COUNT)
                            ret = Math.Round((decimal)_val / (decimal)Constants.GB_BYTE_COUNT).ToString() + " GB";
                        else if (_val > Constants.MB_BYTE_COUNT)
                            ret = Math.Round((decimal)_val / (decimal)Constants.MB_BYTE_COUNT).ToString() + " MB";
                        else if (_val > Constants.KB_BYTE_COUNT)
                            ret = Math.Round((decimal)_val / (decimal)Constants.KB_BYTE_COUNT).ToString() + " KB";
                        else
                            ret = _val.ToString() + " B";
                        break;
                    case MetricUnits.KB:
                        if (_val > (Constants.GB_BYTE_COUNT / Constants.GB_BYTE_COUNT))
                            ret = Math.Round((decimal)_val / (decimal)(Constants.GB_BYTE_COUNT / Constants.GB_BYTE_COUNT)).ToString() + " GB";
                        else if (_val > (Constants.GB_BYTE_COUNT / Constants.MB_BYTE_COUNT))
                            ret = Math.Round((decimal)_val / (decimal)(Constants.GB_BYTE_COUNT / Constants.MB_BYTE_COUNT)).ToString() + " MB";
                        else
                            ret = _val.ToString() + " KB";
                        break;
                    case MetricUnits.MB:
                        if (_val > (Constants.GB_BYTE_COUNT/Constants.MB_BYTE_COUNT))
                            ret = Math.Round((decimal)_val / (decimal)(Constants.GB_BYTE_COUNT / Constants.MB_BYTE_COUNT)).ToString() + " GB";
                        else
                            ret = _val.ToString() + " MB";
                        break;
                    case MetricUnits.GB:
                        ret = _val.ToString() + " GB";
                        break;
                    case MetricUnits.MILLISECONDS:
                        if (_val > (60 * 60 * 1000))
                            ret = Math.Round((decimal)_val / (decimal)(60 * 60 * 1000), 2).ToString() + " hours";
                        else if (_val > (60 * 1000))
                            ret = Math.Round((decimal)_val / (decimal)(60 * 1000), 2).ToString() + " minutes";
                        else if (_val > 1000)
                            ret = Math.Round((decimal)_val / (decimal)1000, 2).ToString() + " seconds";
                        else
                            ret = _val.ToString() + " milliseconds";
                        break;
                    case MetricUnits.SECONDS:
                        if (_val > (60 * 60))
                            ret = Math.Round((decimal)_val / (decimal)(60 * 60), 2).ToString() + " hours";
                        else if (_val > 60)
                            ret = Math.Round((decimal)_val / (decimal)60, 2).ToString() + " minutes";
                        else
                            ret = _val.ToString() + " seconds";
                        break;
                    case MetricUnits.MINUTES:
                        if (_val > 60)
                            ret = Math.Round((decimal)_val / (decimal)60, 2).ToString() + " hours";
                        else
                            ret = _val.ToString() + " minutes";
                        break;
                    case MetricUnits.HOURS:
                        ret = _val.ToString() + " hours";
                        break;
                    default:
                        ret = _val.ToString();
                        break;
                }
                return ret;
            }
        }

        public sSystemMetric(SystemMetricTypes type, MetricUnits unit, long val)
        {
            _unit = unit;
            _type = type;
            _val = val;
            _additional = null;
        }

        public sSystemMetric(SystemMetricTypes type, MetricUnits unit, float val)
            : this(type, unit, (decimal)val) { }

        public sSystemMetric(SystemMetricTypes type, MetricUnits unit, decimal val) :
            this(type,unit,(long)Math.Floor(val))
        {
        }

        public sSystemMetric(SystemMetricTypes type, MetricUnits unit, long val,string additional)
        {
            _unit = unit;
            _type = type;
            _val = val;
            _additional = additional;
            if (_additional.Length > 20)
                _additional = _additional.Substring(0, 17) + "...";
        }

        public sSystemMetric(SystemMetricTypes type, MetricUnits unit, float val,string additional)
            : this(type, unit, (decimal)val,additional) { }

        public sSystemMetric(SystemMetricTypes type, MetricUnits unit, decimal val, string additional):
            this(type,unit,(long)Math.Floor(val),additional)
        { }
    }
}
