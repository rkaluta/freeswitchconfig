using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class DomainDestroyedEvent : IEvent
    {
        public string DomainName
        {
            get { return (string)this["DomainName"]; }
        }

        internal DomainDestroyedEvent(string domainName){
            _pars.Add("DomainName", domainName);
        }

        public DomainDestroyedEvent() { }

        #region IEvent Members

        public string Name
        {
            get { return "DomainDestroyed"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("domainName", DomainName);
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("DomainName",element.Attributes["domainName"].Value);
        }

        #endregion
    }
}
