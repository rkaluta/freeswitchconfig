using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class IncomingSIPTrunkDeploymentEvent : IEvent
    {
        public sDeployedIncomingSIPTrunk Trunk
        {
            get { return (sDeployedIncomingSIPTrunk)_pars["Trunk"]; }
        }

        public IncomingSIPTrunkDeploymentEvent(sDeployedIncomingSIPTrunk trunk)
        {
            _pars.Add("Trunk", trunk);
        }

        #region IEvent Members

        public string Name
        {
            get { return "IncomingSIPTrunkDeployment"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteRaw(Utility.ConvertObjectToXML(Trunk, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("Trunk",(sDeployedIncomingSIPTrunk)Utility.ConvertObjectFromXML(element.InnerXml));
        }

        #endregion
    }
}
