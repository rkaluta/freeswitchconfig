using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class IncomingSIPTrunkDestroyedEvent : IEvent
    {
        public string ExtensionNumber
        {
            get { return (string)this["ExtensionNumber"]; }
        }

        public string Domain
        {
            get { return (string)this["Domain"]; }
        }

        internal IncomingSIPTrunkDestroyedEvent(string extensionNumber, string domain)
        {
            _pars.Add("ExtensionNumber", extensionNumber);
            _pars.Add("Domain", domain);
        }

        public IncomingSIPTrunkDestroyedEvent() { }

        #region IEvent Members

        public string Name
        {
            get { return "IncomingSIPTrunkDestroyed"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("number", ExtensionNumber);
            writer.WriteAttributeString("domain", Domain);
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("ExtensionNumber",element.Attributes["number"].Value);
            _pars.Add("Domain",element.Attributes["domain"].Value);
        }

        #endregion
    }
}
