using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Text.RegularExpressions;
using System.Collections;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.API
{
    public class sConfigElement 
    {
        protected string _name;
        public string Name
        {
            get { return _name; }
        }

        protected Dictionary<string, string> _attributes;
        public string this[string attributeName]
        {
            get
            {
                if (_attributes.ContainsKey(attributeName))
                    return _attributes[attributeName];
                return null;
            }
        }

        public Dictionary<string, string>.KeyCollection Keys
        {
            get { return _attributes.Keys; }
        }

        protected List<sConfigElement> _children;
        public List<sConfigElement> Children
        {
            get { return _children; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<" + Name);
            foreach (string str in Keys)
                sb.Append(" " + str + "=\"" + this[str] + "\"");
            if (Children.Count > 0)
            {
                sb.AppendLine(">");
                foreach (sConfigElement ce in Children)
                    sb.AppendLine(ce.ToString());
                sb.AppendLine("</" + Name + ">");
            }
            else
                sb.AppendLine("/>");
            return sb.ToString();
        }

        protected sConfigElement() {
            _attributes = new Dictionary<string, string>();
            _children = new List<sConfigElement>();
        }

        internal sConfigElement(XmlElement element)
        {
            _attributes = new Dictionary<string, string>();
            _name = element.Name;
            foreach (XmlAttribute att in element.Attributes)
                _attributes.Add(att.Name, att.Value);
            _children = new List<sConfigElement>();
            foreach (XmlNode node in element.ChildNodes)
            {
                if (node is XmlElement)
                    _children.Add(new sConfigElement((XmlElement)node));
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(_name);
            if (_attributes.Count > 0)
            {
                foreach (string str in _attributes.Keys)
                    writer.WriteAttributeString(str, _attributes[str]);
            }
            foreach (sConfigElement elem in _children)
                elem.WriteXml(writer);
            writer.WriteEndElement();
        }
    }

    public class sFreeSwitchModuleFile : sConfigElement
    {

        public string Description
        {
            get { return this["description"]; }
        }

        public sFreeSwitchModuleFile(XmlElement element)
            : base(element)
        {
        }

        public sFreeSwitchModuleFile(string name, string description, string configurationSection)
            :base()
        {
            _name = name;
            _attributes.Add("description",description);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\"?>"+configurationSection);
            XmlNode node = doc.ChildNodes[1];
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n is XmlElement)
                    _children.Add(new sConfigElement((XmlElement)n));
            }
        }

        public string ToConfigurationString(string tagname)
        {
            if (tagname == "configuration")
                return ConfigurationSectionString;
            else
            {
                string ret = ConfigurationSectionString;
                ret = ret.Substring("<configuration".Length);
                ret = "<" + tagname + ret;
                return ret.Substring(0, ret.Length - "</configuration>".Length) + "</" + tagname + ">";
            }
        }

        public string ConfigurationSectionString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<configuration name=\"" + Name + "\"");
                foreach (string str in Keys)
                    sb.Append(" " + str + "=\"" + this[str] + "\"");
                sb.AppendLine(">");
                foreach (sConfigElement ce in Children)
                    sb.AppendLine(ce.ToString());
                sb.AppendLine("</configuration>");
                return sb.ToString();
            }
        }
    }

    public struct VoicemailEntry
    {
    }
}
