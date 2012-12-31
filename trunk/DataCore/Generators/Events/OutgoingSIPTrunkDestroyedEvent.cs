using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class OutgoingSIPTrunkDestroyedEvent : IEvent
    {
        public string TrunkName
        {
            get { return (string)_pars["TrunkName"]; }
        }

        public string ProfileName
        {
            get { return (string)_pars["ProfileName"]; }
        }

        internal OutgoingSIPTrunkDestroyedEvent(string trunkName, string profileName)
        {
            _pars.Add("TrunkName", trunkName);
            _pars.Add("ProfileName", profileName);
        }

        public OutgoingSIPTrunkDestroyedEvent() { }

        #region IEvent Members

        public string Name
        {
            get { return "OutgoingSIPTrunkDestroyed"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("profileName", ProfileName);
            writer.WriteAttributeString("trunkName", TrunkName);
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("ProfileName",element.Attributes["profileName"].Value);
            _pars.Add("TrunkName",element.Attributes["trunkName"].Value);
        }

        #endregion
    }
}
