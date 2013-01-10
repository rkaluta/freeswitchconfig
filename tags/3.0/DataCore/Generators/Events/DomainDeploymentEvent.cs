using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class DomainDeploymentEvent : IEvent
    {
        public sDeployedDomain Domain
        {
            get { return (sDeployedDomain)_pars["Domain"]; }
        }

        internal DomainDeploymentEvent(sDeployedDomain domain)
        {
            _pars.Add("Domain", domain);
        }

        public DomainDeploymentEvent()
        {
        }

        #region IEvent Members

        public string Name
        {
            get { return "DomainDeployment"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteRaw(Utility.ConvertObjectToXML(Domain, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("Domain",(sDeployedDomain)Utility.ConvertObjectFromXML(element.InnerXml));
        }

        #endregion
    }
}
