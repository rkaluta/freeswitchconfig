using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class SipProfileDestroyedEvent : IEvent
    {
        public string ProfileName
        {
            get { return (string)this["ProfileName"]; }
        }

        internal SipProfileDestroyedEvent(string profileName)
        {
            _pars.Add("ProfileName", profileName);
        }

        public SipProfileDestroyedEvent() { }

        #region IEvent Members

        public string Name
        {
            get { return "SipProfileDestroyed"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("profileName", ProfileName);
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("ProfileName",element.Attributes["profileName"].Value);
        }

        #endregion
    }
}
