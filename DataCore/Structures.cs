/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 09/11/2009
 * Time: 10:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.Xml;
using System.Collections;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore
{
	public class NameValuePair : IComparable,IXmlConvertableObject{
        private static readonly Regex _regInteger = new Regex("^\\d+$", RegexOptions.Compiled | RegexOptions.ECMAScript);

		private string _name;
		public string Name{
			get{return _name;}
		}
		
		private object _value;
		public object Value{
			get{return _value;}
		}
		
		public NameValuePair(string name,object val){
			_name=name;
			_value=val;
		}

        internal NameValuePair()
        {}

        public static implicit operator NameValuePair(Hashtable jsonObject){
            return new NameValuePair(jsonObject["Name"].ToString(),jsonObject["Value"]);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (_regInteger.IsMatch(Name) && _regInteger.IsMatch(((NameValuePair)obj).Name))
                return int.Parse(Name).CompareTo(int.Parse(((NameValuePair)obj).Name));
            return Name.CompareTo(((NameValuePair)obj).Name);
        }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("name", _name);
            writer.WriteRaw(Utility.ConvertObjectToXML(_value, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _name = element.Attributes["name"].Value;
            _value = Utility.ConvertObjectFromXML(element.InnerXml);
        }

        #endregion
    }

    public struct sExtensionContextPair
    {
        private string _extension;
        public string Extension
        {
            get { return _extension; }
        }

        private string _context;
        public string Context
        {
            get { return _context; }
        }

        public sExtensionContextPair(string extension,string context){
            _extension = extension;
            _context = context;
        }
    }
}
