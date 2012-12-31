using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Events
{
    public class GenericEvent : IEvent
    {
        #region IEvent Members

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        public GenericEvent(string name, NameValuePair[] attributes)
        {
            _name = name;
            foreach (NameValuePair nvp in attributes)
            {
                if (_pars.ContainsKey(nvp.Name))
                    _pars.Remove(nvp.Name);
                _pars.Add(nvp.Name, nvp.Value);
            }
        }

        public GenericEvent() {
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("EventName", _name);
            writer.WriteRaw(Utility.ConvertObjectToXML(_pars, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _name = element.Attributes["EventName"].Value;
            _pars = (Dictionary<string,object>)Utility.ConvertObjectFromXML(element.InnerXml);
        }

        #endregion
    }
}
