using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces;
using System.Xml;
using System.Globalization;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet;
using System.Collections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl
{
    [ModelJSFilePath("/resources/scripts/loggedIn.js")]
    [ModelJSFilePath("/mobile/resources/scripts/loggedIn.js")]
    [ModelRoute("/core/models/core/CallExtensionReference")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.Collection|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Routes")]
    public class sCallExtensionReference : IComparable,IModel 
    {
        private string _context;
        [ReadOnlyModelProperty()]
        public string Context
        {
            get { return _context; }
        }

        private string _extension;
        [ReadOnlyModelProperty()]
        public string Extension
        {
            get { return _extension; }
        }

        public sCallExtensionReference(string extension, string context)
        {
            _extension = extension;
            _context=context;
        }

        [ModelLoadMethod()]
        public static sCallExtensionReference Load(string id)
        {
            return (sCallExtensionReference)id;
        }

        [ModelLoadAllMethod()]
        public static sCallExtensionReference[] LoadAll()
        {
            List<sCallExtensionReference> ret = new List<sCallExtensionReference>();
            ret.AddRange(CallControlManager.CallExtensions);
            if (Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core.Context.Current != null)
            {
                for (int x = 0; x < ret.Count; x++)
                {
                    if (ret[x].Context != Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core.Context.Current.Name)
                    {
                        ret.RemoveAt(x);
                        x--;
                    }
                }
            }
            return ret.ToArray();
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> SelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            foreach (sCallExtensionReference cer in LoadAll())
                ret.Add(new sModelSelectOptionValue(cer.id, cer.Extension + "@" + cer.Context));
            return ret;
        }

        public static bool operator ==(sCallExtensionReference x, sCallExtensionReference y)
        {
            return (((object)x == null && (object)y == null) ? true : (((object)x != null && (object)y != null) ? x.CompareTo(y) == 0 : false));
        }

        public static bool operator !=(sCallExtensionReference x, sCallExtensionReference y)
        {
            return !(x == y);
        }

        public static bool operator <(sCallExtensionReference x, sCallExtensionReference y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(sCallExtensionReference x, sCallExtensionReference y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(sCallExtensionReference x, sCallExtensionReference y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(sCallExtensionReference x, sCallExtensionReference y)
        {
            return x.CompareTo(y) >= 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public static explicit operator sCallExtensionReference(string formattedString)
        {
            if (!formattedString.Contains(" XML "))
                throw new Exception("Unable to parse Call Extension Reference from formatted string[" + formattedString + "]");
            return new sCallExtensionReference(formattedString.Substring(0, formattedString.IndexOf(" XML ")),
            formattedString.Substring(formattedString.IndexOf(" XML ") + 5));
        }

        public override string ToString()
        {
            return _extension + " XML " + _context;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            sCallExtensionReference cer = (sCallExtensionReference)obj;
            if (cer.Context == Context)
                return Extension.CompareTo(cer.Extension);
            return Context.CompareTo(cer.Context);
        }

        #endregion

        #region IModel Members

        public string id
        {
            get { return ToString(); }
        }

        #endregion
    }

    public struct sCallContext
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private sCallExtension[] _extensions;
        public sCallExtension[] Extensions
        {
            get { return _extensions; }
        }

        public sCallContext(string name, sCallExtension[] extensions)
        {
            _name = name;
            if (extensions == null)
                throw new Exception("You must have at least one extension in a context");
            else if (extensions.Length == 0)
                throw new Exception("You must have at least one extension in a context");
            _extensions = extensions;
        }
    }

    public struct sCallExtension
    {
        private ICallCondition[] _conditions;
        public ICallCondition[] Conditions
        {
            get { return _conditions; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private bool _inline;
        public bool Inline
        {
            get { return _inline; }
        }

        private bool _continue;
        public bool Continue
        {
            get { return _continue; }
        }

        public sCallExtension(string name,bool inline,bool continueAfter, ICallCondition[] conditions)
        {
            _name = name;
            _inline = inline;
            _continue = continueAfter;
            if (conditions == null)
                throw new Exception("You must have at least one condition in an extension");
            else if (conditions.Length==0)
                throw new Exception("You must have at least one condition in an extension");
            _conditions = conditions;
        }
    }

    public struct sFieldRegexPair {
        private string _field;
        public string Field{
            get{return _field;}
        }

        private string _expression;
        public string Expression{
            get{return _expression;}
        }

        private Regex _regexp;
        public Regex RegExp{
            get{return _regexp;}
        }

        public sFieldRegexPair(string field,string expression){
            _field=field;
            _expression=expression;
            _regexp = new Regex(expression,RegexOptions.Compiled|RegexOptions.ECMAScript);
        }
    }

    public struct sCallRegexCondition : ICallCondition{

        private CallRegexConditionTypes _type;
        public CallRegexConditionTypes Type{
            get{return _type;}
        }

        private sFieldRegexPair[] _regexes;
        public sFieldRegexPair[] Regexes{
            get{return _regexes;}
        }

        public sCallRegexCondition(CallRegexConditionTypes type,sFieldRegexPair[] regexes,ICallAction[] actions,ICallAction[] antiActions,CallConditionBreakTypes? breakType){
            _type=type;
            _regexes=regexes;
            _actions = (actions == null ? new ICallAction[0] : actions);
            _antiActions = (antiActions == null ? new ICallAction[0] : antiActions);
            _break=breakType;
        }

        #region ICallCondition Members

        public NameValuePair[] ConditionTagAttributes {
            get
            {
                return new NameValuePair[] { new NameValuePair("regex", _type.ToString()) };
            }
        }

        public sPreActionsElement[] PreActionElements
        {
            get
            {
                sPreActionsElement[] ret = new sPreActionsElement[_regexes.Length];
                for (int x = 0; x < ret.Length; x++)
                {
                    ret[x] = new sPreActionsElement("regex",
                        new NameValuePair[]{
                            new NameValuePair("field",_regexes[x].Field),
                            new NameValuePair("expression",_regexes[x].Expression)
                        });
                }
                return ret;
            }
        }

        private ICallAction[] _actions;
        public ICallAction[]  Actions
        {
	        get { return _actions; }
        }

        private ICallAction[] _antiActions;
        public ICallAction[]  AntiActions
        {
	        get { return _antiActions; }
        }

        private CallConditionBreakTypes? _break;
        public CallConditionBreakTypes?  Break
        {
	        get { return _break; }
        }

        #endregion
    }

    public struct sCallFieldCondition : ICallCondition{

        private string _conditionField;
        public string ConditionField {
            get { return _conditionField; }
        }

        private string _conditionValue;
        public string ConditionValue {
            get { return _conditionValue; }
        }

        private bool _usesRegex;
        public bool UsesRegex {
            get { return _usesRegex; }
        }

        private Regex _regex;
        public Regex Regex
        {
            get { return _regex; }
        }

        public sCallFieldCondition(string conditionFiled, string conditionValue, bool usesRegex, ICallAction[] actions, ICallAction[] antiActions,CallConditionBreakTypes? breakType)
        {
            _conditionField = conditionFiled;
            _conditionValue = conditionValue;
            _usesRegex = usesRegex;
            _actions = (actions==null ? new ICallAction[0] : actions);
            _antiActions = (antiActions==null ? new ICallAction[0] : antiActions);
            _regex = null;
            _break = breakType;
            if (_usesRegex)
                _regex = new Regex(conditionValue, RegexOptions.Compiled | RegexOptions.ECMAScript);
        }
    
        #region ICallCondition Members

        public NameValuePair[] ConditionTagAttributes
        {
            get
            {
                return new NameValuePair[]{
                    new NameValuePair("field", _conditionField),
                    new NameValuePair("condition", _conditionValue)
                };
            }
        }
        
        public sPreActionsElement[] PreActionElements { get { return null; } }

        private ICallAction[] _actions;
        public ICallAction[] Actions
        {
            get { return _actions; }
        }

        private ICallAction[] _antiActions;
        public ICallAction[] AntiActions
        {
            get { return _antiActions; }
        }

        private CallConditionBreakTypes? _break;
        public CallConditionBreakTypes?  Break
        {
	        get { return _break; }
        }

        #endregion
    }

    public struct sCatchAllCondition : ICallCondition
    {
        public sCatchAllCondition(ICallAction[] actions, ICallAction[] antiActions, CallConditionBreakTypes? breakType)
        {
            _actions = (actions == null ? new ICallAction[0] : actions);
            _antiActions = (antiActions == null ? new ICallAction[0] : antiActions);
            _break = breakType;
        }

        #region ICallCondition Members

        public NameValuePair[] ConditionTagAttributes{   get{   return null;    }    }

        public sPreActionsElement[] PreActionElements { get { return null; } }


        public void AppendToXMLWriter(XmlWriter writer)
        {
            writer.WriteStartElement("condition");
            if (_break.HasValue)
            {
                writer.WriteStartAttribute("break");
                switch (_break.Value)
                {
                    case CallConditionBreakTypes.Never:
                        writer.WriteValue("never");
                        break;
                    case CallConditionBreakTypes.OnFalse:
                        writer.WriteValue("on-false");
                        break;
                    case CallConditionBreakTypes.OnTrue:
                        writer.WriteValue("on-true");
                        break;
                }
                writer.WriteEndAttribute();
            }
            foreach (ICallAction action in Actions)
            {
                writer.WriteStartElement("action");
                foreach (NameValuePair nvp in action.ActionXMLAttributes)
                {
                    writer.WriteStartAttribute(nvp.Name);
                    writer.WriteValue(nvp.Value);
                    writer.WriteEndAttribute();
                }
                writer.WriteEndElement();
            }
            foreach (ICallAction action in AntiActions)
            {
                writer.WriteStartElement("anti-action");
                foreach (NameValuePair nvp in action.ActionXMLAttributes)
                {
                    writer.WriteStartAttribute(nvp.Name);
                    writer.WriteValue(nvp.Value);
                    writer.WriteEndAttribute();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private ICallAction[] _actions;
        public ICallAction[] Actions
        {
            get { return _actions; }
        }

        private ICallAction[] _antiActions;
        public ICallAction[] AntiActions
        {
            get { return _antiActions; }
        }

        private CallConditionBreakTypes? _break;
        public CallConditionBreakTypes? Break
        {
            get { return _break; }
        }

        #endregion
    }

    public enum sCallTimeWeekDays{
        Sunday = 1,
        Monday = 2,
        Tuesday = 3,
        Wednesday = 4,
        Thursday = 5,
        Friday = 6,
        Saturday = 7
    }

    public struct sCallTime : IComparable, IXmlConvertableObject
    {
        private int? _year;
        public int? Year
        {
            get { return _year; }
        }

        private int? _yearDay;
        public int? YearDay
        {
            get { return _yearDay; }
        }

        private byte? _month;
        public byte? Month
        {
            get { return _month; }
        }

        private byte? _monthDay;
        public byte? MonthDay
        {
            get { return _monthDay; }
        }

        private byte? _week;
        public byte? Week
        {
            get { return _week; }
        }

        private byte? _monthWeek;
        public byte? MonthWeek
        {
            get { return _monthWeek; }
        }

        private sCallTimeWeekDays? _weekDay;
        public sCallTimeWeekDays? WeekDay
        {
            get { return _weekDay; }
        }

        private byte? _hour;
        public byte? Hour
        {
            get { return _hour; }
        }

        private byte? _minute;
        public byte? Minute
        {
            get { return _minute; }
        }

        private int? _minuteOfDay;
        public int? MinuteOfDay
        {
            get { return _minuteOfDay; }
        }

        internal bool IsInRange(DateTime date, sCallTime? end)
        {
            int dayOfWeek = 1;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    dayOfWeek = 2;
                    break;
                case DayOfWeek.Tuesday:
                    dayOfWeek = 3;
                    break;
                case DayOfWeek.Wednesday:
                    dayOfWeek = 4;
                    break;
                case DayOfWeek.Thursday:
                    dayOfWeek = 5;
                    break;
                case DayOfWeek.Friday:
                    dayOfWeek = 6;
                    break;
                case DayOfWeek.Saturday:
                    dayOfWeek = 7;
                    break;
            }
            int weekOfYear = GetWeekNumberOfYear(date);
            int minuteOfDay = (date.Hour * 60) + (date.Minute);
            int weekOfMonth = GetWeekNumberOfMonth(date);
            if (end.HasValue)
            {
                return (_year.HasValue ? _year.Value : date.Year) <= date.Year
                    && (_yearDay.HasValue ? _yearDay.Value : date.DayOfYear) <= date.DayOfYear
                    && (_month.HasValue ? _month.Value : date.Month) <= date.Month
                    && (_monthDay.HasValue ? _monthDay.Value : date.Day) <= date.Day
                    && (_week.HasValue ? _week.Value : weekOfYear) <= weekOfYear
                    && (_monthWeek.HasValue ? _monthWeek.Value : weekOfMonth) <= weekOfMonth
                    && (_weekDay.HasValue ? (int)_weekDay.Value : dayOfWeek) <= dayOfWeek
                    && (_hour.HasValue ? _hour.Value : date.Hour) <= date.Hour
                    && (_minute.HasValue ? _minute.Value : date.Minute) <= date.Minute
                    && (_minuteOfDay.HasValue ? _minuteOfDay.Value : minuteOfDay) <= minuteOfDay
                    && (end.Value._year.HasValue ? end.Value._year.Value : date.Year) >= date.Year
                    && (end.Value._yearDay.HasValue ? end.Value._yearDay.Value : date.DayOfYear) >= date.DayOfYear
                    && (end.Value._month.HasValue ? end.Value._month.Value : date.Month) >= date.Month
                    && (end.Value._monthDay.HasValue ? end.Value._monthDay.Value : date.Day) >= date.Day
                    && (end.Value.Week.HasValue ? end.Value.Week.Value : weekOfYear) >= weekOfYear
                    && (end.Value._monthWeek.HasValue ? end.Value._monthWeek.Value : weekOfMonth) >= weekOfMonth
                    && (end.Value._weekDay.HasValue ? (int)end.Value._weekDay.Value : dayOfWeek) >= dayOfWeek
                    && (end.Value._hour.HasValue ? end.Value._hour.Value : date.Hour) >= date.Hour
                    && (end.Value._minute.HasValue ? end.Value._minute.Value : date.Minute) >= date.Minute
                    && (end.Value._minuteOfDay.HasValue ? end.Value._minuteOfDay.Value : minuteOfDay) >= minuteOfDay;
            }
            else
            {
                return (_year.HasValue ? _year.Value : date.Year) == date.Year
                    && (_yearDay.HasValue ? _yearDay.Value : date.DayOfYear) == date.DayOfYear
                    && (_month.HasValue ? _month.Value : date.Month) == date.Month
                    && (_monthDay.HasValue ? _monthDay.Value : date.Day) == date.Day
                    && (_week.HasValue ? _week.Value : weekOfYear) == weekOfYear
                    && (_monthWeek.HasValue ? _monthWeek.Value : weekOfMonth) == weekOfMonth
                    && (_weekDay.HasValue ? (int)_weekDay.Value : dayOfWeek) == dayOfWeek
                    && (_hour.HasValue ? _hour.Value : date.Hour) == date.Hour
                    && (_minute.HasValue ? _minute.Value : date.Minute) == date.Minute
                    && (_minuteOfDay.HasValue ? _minuteOfDay.Value : minuteOfDay) == minuteOfDay;
            }
        }
        
        public sCallTime(int? year, int? yearDay, byte? month,
        byte? monthDay,byte? week, byte? monthWeek, sCallTimeWeekDays? weekDay,
        byte? hour, byte? minute, int? minuteOfDay)
        {
            _year = year;
            _yearDay = yearDay;
            _month = month;
            _monthDay = monthDay;
            _week = week;
            _monthWeek = monthWeek;
            _weekDay = weekDay;
            _hour = hour;
            _minute = minute;
            _minuteOfDay = minuteOfDay;
        }

        public override string ToString()
        {
            return (_year.HasValue ? _year.Value.ToString("0000") : "____") +
                (_yearDay.HasValue ? _yearDay.Value.ToString("000") : "___") +
                (_month.HasValue ? _month.Value.ToString("00") : "__") +
                (_monthDay.HasValue ? _monthDay.Value.ToString("00") : "__") +
                (_week.HasValue ? _week.Value.ToString("00") : "__")+
                (_monthWeek.HasValue ? _monthWeek.Value.ToString("0") : "_") +
                (_weekDay.HasValue ? ((int)_weekDay.Value).ToString("0") : "_") +
                (_hour.HasValue ? _hour.Value.ToString("00") : "__") +
                (_minute.HasValue ? _minute.Value.ToString("00") : "__") +
                (_minuteOfDay.HasValue ? _minuteOfDay.Value.ToString("0000") : "____");
        }

        private int GetWeekNumberOfYear(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
            return weekNum + 1;
        }

        private int GetWeekNumberOfMonth(DateTime dtPassed)
        {
            DateTime dt = new DateTime(dtPassed.Year, dtPassed.Month, 1);
            int offset = (int)dt.DayOfWeek - (int)DayOfWeek.Sunday;
            offset += dtPassed.Day;
            return (int)Math.Floor((double)offset / (double)7) + 1;
        }

        public static bool operator ==(sCallTime x, sCallTime y)
        {
            return (((object)x == null && (object)y == null) ? true : (((object)x != null && (object)y != null) ? x.CompareTo(y) == 0 : false));
        }

        public static bool operator !=(sCallTime x, sCallTime y)
        {
            return !(x == y);
        }

        public static bool operator <(sCallTime x, sCallTime y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(sCallTime x, sCallTime y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(sCallTime x, sCallTime y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(sCallTime x, sCallTime y)
        {
            return x.CompareTo(y) >= 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public static implicit operator sCallTime(string formattedString)
        {
            return new sCallTime(
                (formattedString.Substring(0, 4) == "____" ? (int?)null : (int?)int.Parse(formattedString.Substring(0, 4))),
                (formattedString.Substring(4,3) == "___" ? (int?)null : (int?)int.Parse(formattedString.Substring(4, 3))),
                (formattedString.Substring(7,2) == "__" ? (byte?)null : (byte?)byte.Parse(formattedString.Substring(7, 2))),
                (formattedString.Substring(9, 2) == "__" ? (byte?)null : (byte?)byte.Parse(formattedString.Substring(9, 2))),
                (formattedString.Substring(11, 2) == "__" ? (byte?)null : (byte?)byte.Parse(formattedString.Substring(11, 2))),
                (formattedString.Substring(13,1) == "_" ? (byte?)null : (byte?)byte.Parse(formattedString.Substring(13, 1))),
                (sCallTimeWeekDays?)(formattedString.Substring(14, 1) == "_" ? (byte?)null : (byte?)byte.Parse(formattedString.Substring(14, 1))),
                (formattedString.Substring(15, 2) == "__" ? (byte?)null : (byte?)byte.Parse(formattedString.Substring(15, 2))),
                (formattedString.Substring(17, 2) == "__" ? (byte?)null : (byte?)byte.Parse(formattedString.Substring(17, 2))),
                (formattedString.Substring(19, 4) == "____" ? (int?)null : (int?)int.Parse(formattedString.Substring(19,4)))
            );
        }

        public static implicit operator sCallTime(Hashtable jsonObject)
        {
            return (sCallTime)(
                (jsonObject["Year"]==null ? "____" : jsonObject["Year"].ToString().PadLeft(4,'0'))+
                (jsonObject["YearDay"]==null?"___":jsonObject["YearDay"].ToString().PadLeft(3,'0'))+
                (jsonObject["Month"]==null?"__":jsonObject["Month"].ToString().PadLeft(2,'0'))+
                (jsonObject["MonthDay"]==null?"__":jsonObject["MonthDay"].ToString().PadLeft(2,'0'))+
                (jsonObject["Week"]==null?"__":jsonObject["Week"].ToString().PadLeft(2,'0'))+
                (jsonObject["MonthWeek"]==null?"_":jsonObject["MonthWeek"].ToString().PadLeft(1,'0'))+
                (jsonObject["WeekDay"]==null ? "_" : jsonObject["WeekDay"].ToString().PadLeft(1,'0'))+
                (jsonObject["Hour"]==null?"__":jsonObject["Hour"].ToString().PadLeft(2,'0'))+
                (jsonObject["Minute"]==null?"__":jsonObject["Minute"].ToString().PadLeft(2,'0'))+
                (jsonObject["MinuteOfDay"]==null?"____":jsonObject["MinuteOfDay"].ToString().PadLeft(4,'0')));
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(obj.ToString());
        }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteValue(ToString());
        }

        public void LoadFromElement(XmlElement element)
        {
            sCallTime tmp = (sCallTime)element.InnerText;
            _year = tmp.Year;
            _yearDay = tmp.YearDay;
            _month = tmp.Month;
            _monthDay = tmp.MonthDay;
            _week = tmp.Week;
            _monthWeek = tmp.MonthWeek;
            _weekDay = tmp.WeekDay;
            _hour = tmp.Hour;
            _minute = tmp.Minute;
            _minuteOfDay = tmp.MinuteOfDay;
        }

        #endregion
    }

    public struct sCallTimeOfDayRange
    {
        private byte _startHour;
        public byte StartHour
        {
            get { return _startHour; }
        }

        private byte _startMinute;
        public byte StartMinute
        {
            get { return _startMinute; }
        }

        private byte? _startSecond;
        public byte? StartSecond
        {
            get { return _startSecond; }
        }

        private byte _endHour;
        public byte EndHour
        {
            get { return _endHour; }
        }

        private byte _endMinute;
        public byte EndMinute
        {
            get { return _endMinute; }
        }

        private byte? _endSecond;
        public byte? EndSecond
        {
            get { return _endSecond; }
        }

        public sCallTimeOfDayRange(byte startHour, byte startMinute, byte? startSecond,
            byte endHour, byte endMinute, byte? endSecond)
        {
            _startHour = startHour;
            _startMinute = startMinute;
            _startSecond = startSecond;
            _endHour = endHour;
            _endMinute = endMinute;
            _endSecond = endSecond;
        }

        public sCallTimeOfDayRange(string formattedString)
        {
            string[] split = formattedString.Split('-');
            _startHour = byte.Parse(split[0].Substring(0,2));
            _startMinute = byte.Parse(split[0].Substring(3, 2));
            if (split[0].Length > 5)
                _startSecond = byte.Parse(split[0].Substring(6, 2));
            else
                _startSecond = null;
            _endHour = byte.Parse(split[1].Substring(0, 2));
            _endMinute = byte.Parse(split[1].Substring(3, 2));
            if (split[1].Length > 5)
                _endSecond = byte.Parse(split[1].Substring(6, 2));
            else
                _endSecond = null;
        }

        public override string ToString()
        {
            return _startHour.ToString("00") + ":" + _startMinute.ToString("00") + (_startSecond.HasValue ? _startSecond.Value.ToString(":00") : "") +
                "~"
                + _endHour.ToString("00") + ":" + _endMinute.ToString("00") + (_endSecond.HasValue ? _endSecond.Value.ToString(":00") : "");
        }

        internal bool IsInRange(DateTime date)
        {
            long start = ((long)_startHour * 60 * 60) + ((long)_startMinute * 60) + (_startSecond.HasValue ? (long)_startSecond.Value : (long)0);
            long end = ((long)_endHour * 60 * 60) + ((long)_endMinute * 60) + (_endSecond.HasValue ? (long)_endSecond.Value : (long)0);
            long cur = ((long)date.Hour * 60 * 60) + ((long)date.Minute * 60) + (long)date.Second;
            return cur >= start && cur <= end;
        }
    }

    public struct sCallTimeDateRange
    {
        private DateTime _start;
        public DateTime Start
        {
            get { return _start; }
        }

        private DateTime _end;
        public DateTime End
        {
            get { return _end; }
        }

        private bool _useSeconds;
        public bool Seconds
        {
            get { return _useSeconds; }
        }

        public sCallTimeDateRange(DateTime start, DateTime end, bool useSeconds)
        {
            _start = start;
            _end = end;
            _useSeconds = useSeconds;
        }

        private const string _SECONDS_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private const string _NO_SECONDS_FORMAT = "yyyy-MM-dd HH:mm";

        public sCallTimeDateRange(string formattedString,bool usesSeconds)
        {
            _useSeconds = usesSeconds;
            if (_useSeconds)
            {
                _start = DateTime.ParseExact(formattedString.Substring(0, _SECONDS_FORMAT.Length), _SECONDS_FORMAT, null);
                _end = DateTime.ParseExact(formattedString.Substring(_SECONDS_FORMAT.Length+1), _SECONDS_FORMAT, null);
            }
            else
            {
                _start = DateTime.ParseExact(formattedString.Substring(0, _SECONDS_FORMAT.Length), _NO_SECONDS_FORMAT, null);
                _end = DateTime.ParseExact(formattedString.Substring(_SECONDS_FORMAT.Length + 1),_NO_SECONDS_FORMAT, null);
            }
        }

        public override string ToString()
        {
            if (_useSeconds)
                return _start.ToString(_SECONDS_FORMAT) + "~" + _end.ToString(_SECONDS_FORMAT);
            else
                return _start.ToString(_NO_SECONDS_FORMAT) + "~" + _end.ToString(_NO_SECONDS_FORMAT);
        }

        internal bool IsInRange(DateTime date)
        {
            return date.Ticks >= _start.Ticks && date.Ticks <= _end.Ticks;
        }
    }

    public struct sCallTimeOfDayCondition : ICallCondition
    {
        private sCallTime? _start;
        private sCallTime? _end;
        private sCallTimeOfDayRange? _timeRange;
        private sCallTimeDateRange? _dateRange;

        public sCallTimeOfDayCondition(sCallTime start, sCallTime? end, ICallAction[] actions, ICallAction[] antiActions, CallConditionBreakTypes? breakType) :
            this(start, end, null, null, actions, antiActions, breakType) { }

        public sCallTimeOfDayCondition(sCallTimeOfDayRange timeRange, ICallAction[] actions, ICallAction[] antiActions, CallConditionBreakTypes? breakType)
            : this(null, null, timeRange, null, actions, antiActions, breakType) { }

        public sCallTimeOfDayCondition(sCallTimeDateRange dateRange, ICallAction[] actions, ICallAction[] antiActions, CallConditionBreakTypes? breakType)
            : this(null, null, null, dateRange, actions, antiActions, breakType) { }

        public sCallTimeOfDayCondition(sCallTime? start, sCallTime? end, sCallTimeOfDayRange? timeRange, sCallTimeDateRange? dateRange,
            ICallAction[] actions,ICallAction[] antiActions,CallConditionBreakTypes? breakType)
        {
            _start = start;
            _end = end;
            _timeRange = timeRange;
            _dateRange = dateRange;
            if (_end == null && _start == null && _timeRange == null && _dateRange == null)
                throw new Exception("You must specify at least one type of time condition");
            _actions = (actions == null ? new ICallAction[0] : actions);
            _antiActions = (antiActions == null ? new ICallAction[0] : antiActions);
            _break = breakType;
        }

        public bool IsValid(DateTime date)
        {
            if (_start.HasValue)
                return _start.Value.IsInRange(date, _end);
            else if (_timeRange.HasValue)
                return _timeRange.Value.IsInRange(date);
            else
                return _dateRange.Value.IsInRange(date);
        }

        #region ICallCondition Members

        public NameValuePair[] ConditionTagAttributes
        {
            get
            {
                List<NameValuePair> ret = new List<NameValuePair>();
                if (_start.HasValue)
                {
                    if (_end.HasValue)
                    {
                        if (_start.Value.Year.HasValue)
                            ret.Add(new NameValuePair("year", _start.Value.Year.ToString() + (_end.Value.Year.HasValue ? "-" + _end.Value.Year.ToString() : "")));
                        if (_start.Value.YearDay.HasValue)
                            ret.Add(new NameValuePair("yday", _start.Value.YearDay.ToString() + (_end.Value.YearDay.HasValue ? "-" + _end.Value.YearDay.ToString() : "")));
                        if (_start.Value.Month.HasValue)
                        {
                            ret.Add(new NameValuePair("mon", _start.Value.Month.ToString() + (_end.Value.Month.HasValue ? "-" + _end.Value.Month.ToString() : "")));
                            if (_start.Value.MonthDay.HasValue)
                                ret.Add(new NameValuePair("mday", _start.Value.MonthDay.ToString() + (_end.Value.MonthDay.HasValue ? "-" + _end.Value.MonthDay.ToString() : "")));
                            if (_start.Value.Week.HasValue)
                                ret.Add(new NameValuePair("week", _start.Value.Week.ToString() + (_end.Value.Week.HasValue ? "-" + _end.Value.Week.ToString() : "")));
                            if (_start.Value.MonthWeek.HasValue)
                                ret.Add(new NameValuePair("mweek", _start.Value.MonthWeek.ToString() + (_end.Value.MonthWeek.HasValue ? "-" + _end.Value.MonthWeek.ToString() : "")));
                            if (_start.Value.WeekDay.HasValue)
                                ret.Add(new NameValuePair("wday", ((int)_start.Value.WeekDay.Value).ToString() + (_end.Value.WeekDay.HasValue ? "-" + ((int)_end.Value.WeekDay).ToString() : "")));
                            if (_start.Value.Hour.HasValue)
                                ret.Add(new NameValuePair("hour", _start.Value.Hour.ToString() + (_end.Value.Hour.HasValue ? "-" + _end.Value.Hour.ToString() : "")));
                            if (_start.Value.Minute.HasValue)
                                ret.Add(new NameValuePair("minute", _start.Value.Minute.ToString() + (_end.Value.Minute.HasValue ? "-" + _end.Value.Minute.ToString() : "")));
                            if (_start.Value.MinuteOfDay.HasValue)
                                ret.Add(new NameValuePair("minute-of-day", _start.Value.MinuteOfDay.ToString() + (_end.Value.MinuteOfDay.HasValue ? "-" + _end.Value.MinuteOfDay.ToString() : "")));
                        }
                        else
                        {
                            if (_start.Value.Year.HasValue)
                                ret.Add(new NameValuePair("year", _start.Value.Year));
                            if (_start.Value.YearDay.HasValue)
                                ret.Add(new NameValuePair("yday", _start.Value.YearDay));
                            if (_start.Value.Month.HasValue)
                                ret.Add(new NameValuePair("mon", _start.Value.Month));
                            if (_start.Value.MonthDay.HasValue)
                                ret.Add(new NameValuePair("mday", _start.Value.MonthDay));
                            if (_start.Value.Week.HasValue)
                                ret.Add(new NameValuePair("week", _start.Value.Week));
                            if (_start.Value.MonthWeek.HasValue)
                                ret.Add(new NameValuePair("mweek", _start.Value.MonthWeek));
                            if (_start.Value.WeekDay.HasValue)
                                ret.Add(new NameValuePair("wday", (int)_start.Value.WeekDay.Value));
                            if (_start.Value.Hour.HasValue)
                                ret.Add(new NameValuePair("hour", _start.Value.Hour));
                            if (_start.Value.Minute.HasValue)
                                ret.Add(new NameValuePair("minute", _start.Value.Minute));
                            if (_start.Value.MinuteOfDay.HasValue)
                                ret.Add(new NameValuePair("minute-of-day", _start.Value.MinuteOfDay));
                        }
                    }
                }
                else if (_timeRange.HasValue)
                    ret.Add(new NameValuePair("time-of-day", _timeRange.ToString()));
                else
                    ret.Add(new NameValuePair("date-time", _dateRange.ToString()));
                return ret.ToArray();
            }
        }

        public sPreActionsElement[] PreActionElements { get { return null; } }

        private ICallAction[] _actions;
        public ICallAction[] Actions
        {
            get { return _actions; }
        }

        private ICallAction[] _antiActions;
        public ICallAction[] AntiActions
        {
            get { return _antiActions; }
        }

        private CallConditionBreakTypes? _break;
        public CallConditionBreakTypes? Break
        {
            get { return _break; }
        }

        #endregion
    }

    public struct sDomainExtensionPair : IComparable
    {
        private string _extension;
        public string Extension
        {
            get { return _extension; }
        }

        private string _domain;
        public string Domain
        {
            get { return _domain; }
        }

        public sDomainExtensionPair(string extension, string domain)
        {
            _extension = extension;
            _domain = domain;
        }

        public static bool operator ==(sDomainExtensionPair x, sDomainExtensionPair y)
        {
            return (((object)x == null && (object)y == null) ? true : (((object)x != null && (object)y != null) ? x.CompareTo(y) == 0 : false));
        }

        public static bool operator !=(sDomainExtensionPair x, sDomainExtensionPair y)
        {
            return !(x == y);
        }

        public static bool operator <(sDomainExtensionPair x, sDomainExtensionPair y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(sDomainExtensionPair x, sDomainExtensionPair y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(sDomainExtensionPair x, sDomainExtensionPair y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(sDomainExtensionPair x, sDomainExtensionPair y)
        {
            return x.CompareTo(y) >= 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public static explicit operator sDomainExtensionPair(string formattedString)
        {
            if (!formattedString.Contains("@"))
                throw new Exception("Unable to parse Domain Extension Pair from formatted string[" + formattedString + "]");
            return new sDomainExtensionPair(formattedString.Substring(0, formattedString.IndexOf("@")),
            formattedString.Substring(formattedString.IndexOf("@") + 1));
        }

        public override string ToString()
        {
            return _extension + "@" + _domain;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            sDomainExtensionPair dep = (sDomainExtensionPair)obj;
            if (dep.Domain == Domain)
                return Extension.CompareTo(dep.Extension);
            return Domain.CompareTo(dep.Domain);
        }

        #endregion
    }

    public struct sGatewayNumberPair : IComparable
    {
        private string _number;
        public string Number
        {
            get { return _number; }
        }

        private string _gatewayName;
        public string GatewayName
        {
            get { return _gatewayName; }
        }

        public sGatewayNumberPair(string number, string gatewayName)
        {
            _number = number;
            _gatewayName = gatewayName;
        }

        public static bool operator ==(sGatewayNumberPair x, sGatewayNumberPair y)
        {
            return (((object)x == null && (object)y == null) ? true : (((object)x != null && (object)y != null) ? x.CompareTo(y) == 0 : false));
        }

        public static bool operator !=(sGatewayNumberPair x, sGatewayNumberPair y)
        {
            return !(x == y);
        }

        public static bool operator <(sGatewayNumberPair x, sGatewayNumberPair y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(sGatewayNumberPair x, sGatewayNumberPair y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(sGatewayNumberPair x, sGatewayNumberPair y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(sGatewayNumberPair x, sGatewayNumberPair y)
        {
            return x.CompareTo(y) >= 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public static explicit operator sGatewayNumberPair(string formattedString)
        {
            if (!formattedString.Contains("\t"))
                throw new Exception("Unable to parse Gateway Number Pair from formatted string[" + formattedString + "]");
            return new sGatewayNumberPair(formattedString.Substring(0, formattedString.IndexOf("\t")),
            formattedString.Substring(formattedString.IndexOf("\t") + 1));
        }

        public override string ToString()
        {
            return _number + "\t" + _gatewayName;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            sGatewayNumberPair gnp = (sGatewayNumberPair)obj;
            if (gnp.GatewayName == GatewayName)
                return Number.CompareTo(gnp.Number);
            return GatewayName.CompareTo(gnp.GatewayName);
        }

        #endregion
    }


    public struct sDialableNumber : IComparable
    {
        private static readonly Regex _reg = new Regex("^[d\\*#]+$",RegexOptions.Compiled|RegexOptions.ECMAScript);

        private string _numbers;

        public sDialableNumber(string numbers)
        {
            if (!_reg.IsMatch(numbers))
                throw new Exception("Invalid dialable numbers specific in the constructor, numbers must be 0-9*#, "+numbers+" is not valid");
            _numbers = numbers;
        }

        public static bool operator == (sDialableNumber x, sDialableNumber y)
        {
            
            return (((object)x==null && (object)y==null) ? true : (((object)x!=null && (object)y!=null) ? x.CompareTo(y) == 0 : false));
        }

        public static bool operator !=(sDialableNumber x, sDialableNumber y)
        {
            return !(x == y);
        }

        public static bool operator <(sDialableNumber x, sDialableNumber y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(sDialableNumber x, sDialableNumber y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(sDialableNumber x, sDialableNumber y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(sDialableNumber x, sDialableNumber y)
        {
            return x.CompareTo(y) >= 0;
        }

        public override int GetHashCode()
        {
            return _numbers.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public override string ToString()
        {
            return _numbers;
        }

        public static explicit operator sDialableNumber(string numbers)
        {
            return new sDialableNumber(numbers);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return _numbers.CompareTo(((sDialableNumber)obj)._numbers);
        }

        #endregion
    }
}
