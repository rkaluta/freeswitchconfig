using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using System.ComponentModel;
using System.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore
{
    [XmlRoot("NPANXXValue")]
    public class NPANXXValue : IComparable, IXmlConvertableObject
    {

        /*
         * Translations
         * N = [2-9]
         * Z = [1-9]
         * X = [0-9]
         * . = \d+
         * | = ignore on output, bracket in the validation
         */

        private static readonly Regex _regValid = new Regex(Constants.NPANXX_REGEX_STRING, RegexOptions.Compiled | RegexOptions.ECMAScript);

        public NPANXXValue() { }

        public NPANXXValue(string value)
        {
            _value = value.Replace(" ", "").Replace("\t", "").Replace("\r", "");
            foreach (string str in _value.Split('\n'))
            {
                if (!_regValid.IsMatch(str))
                    throw new Exception("Unable to use the string " + _value + " as a valid NPANXXValue item");
            }
        }

        private string _value;
        [XmlAttribute("Value")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public bool IsNumberAMatch(string number)
        {
            return new Regex(this.ToString()).IsMatch(number);
        }

        public string ExtractNumberToDial(string number)
        {
            Regex reg = new Regex(this.ToString());
            Match m = reg.Match(number);
            if (m.Groups.Count == 1)
                return m.Value;
            else
            {
                for (int x = 1; x < m.Groups.Count; x++)
                {
                    if (m.Groups[x].Value != "")
                        return m.Groups[x].Value;
                }
            }
            return "";
        }

        public string ToRegexString()
        {
            if (_value == null)
                return null;
            string ret = "";
            foreach (string str in _value.Split('\n'))
            {
                ret += "^";
                if (!str.Contains("|"))
                    ret += "(";
                foreach (char c in str.ToCharArray())
                    ret += TranslateNPACode(c);
                ret += ")$|";
            }
            ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        public override string ToString()
        {
            return _value;
        }

        private string TranslateNPACode(char code)
        {
            string ret;
            switch (code)
            {
                case 'N':
                    ret = "[2-9]";
                    break;
                case 'Z':
                    ret = "[1-9]";
                    break;
                case 'X':
                    ret = "[0-9]";
                    break;
                case '.':
                    ret = "\\d+";
                    break;
                case '|':
                    ret = "(";
                    break;
                default:
                    ret = code.ToString();
                    break;
            }
            return ret;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Log.Trace("Checking if NPANXX Value " + this.ToString() + " is equal to supplied object " + obj.ToString());
            return obj.ToString().Equals(this.ToString());
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static bool operator ==(NPANXXValue x, NPANXXValue y)
        {
            return (((object)x == null && (object)y == null) ? true : (((object)x != null && (object)y != null) ? x.CompareTo(y) == 0 : false));
        }

        public static bool operator !=(NPANXXValue x, NPANXXValue y)
        {
            return !(x == y);
        }

        public static bool operator <(NPANXXValue x, NPANXXValue y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(NPANXXValue x, NPANXXValue y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(NPANXXValue x, NPANXXValue y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(NPANXXValue x, NPANXXValue y)
        {
            return x.CompareTo(y) >= 0;
        }

        public static explicit operator NPANXXValue(string formattedString)
        {
            return new NPANXXValue(formattedString);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            NPANXXValue npv = (NPANXXValue)obj;
            return _value.CompareTo(npv.Value);
        }

        #endregion


        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteValue(_value);
        }

        public void LoadFromElement(XmlElement element)
        {
            _value = element.InnerText;
        }

        #endregion
    }
}
