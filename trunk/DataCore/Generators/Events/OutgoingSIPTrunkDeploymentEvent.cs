using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class OutgoingSIPTrunkDeploymentEvent : IEvent
    {
        public sDeployedOutgoingSipTrunk Trunk
        {
            get { return (sDeployedOutgoingSipTrunk)_pars["Trunk"]; }
        }

        internal OutgoingSIPTrunkDeploymentEvent(sDeployedOutgoingSipTrunk trunk)
        {
            _pars.Add("Trunk", trunk);
        }

        public OutgoingSIPTrunkDeploymentEvent() { }

        #region IEvent Members

        public string Name
        {
            get { return "OutgoingSIPTrunkDeployment"; }
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
            _pars.Add("Trunk",(sDeployedOutgoingSipTrunk)Utility.ConvertObjectFromXML(element.InnerXml));
        }

        #endregion
    }
}
