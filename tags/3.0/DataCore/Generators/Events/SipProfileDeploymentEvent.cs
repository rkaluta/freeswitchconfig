using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class SipProfileDeploymentEvent : IEvent
    {
        public sDeployedProfile Profile
        {
            get { return (sDeployedProfile)_pars["Profile"]; }
        }

        internal SipProfileDeploymentEvent(sDeployedProfile profile)
        {
            _pars.Add("Profile", profile);
        }

        public SipProfileDeploymentEvent()
        {
        }

        #region IEvent Members

        public string Name
        {
            get { return "SipProfileDeployment"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteRaw(Utility.ConvertObjectToXML(Profile, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("Profile",(sDeployedProfile)Utility.ConvertObjectFromXML(element.InnerXml));
        }

        #endregion
    }
}
