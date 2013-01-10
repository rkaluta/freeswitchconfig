using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules
{
    public class ModuleFirewallRulesChangedEvent : IEvent
    {
        public string ModuleName
        {
            get { return (string)this["ModuleName"]; }
        }

        public ModuleFirewallRulesChangedEvent(string moduleName)
        {
            _pars.Add("ModuleName", moduleName);
        }

        #region IEvent Members

        public string Name
        {
            get { return "Module Firewall Rules Changed"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            throw new Exception("The ModuleFirewallRulesChangedEvent cannot be serialized.");
        }

        public void LoadFromElement(XmlElement element)
        {
            throw new Exception("The ModuleFirewallRulesChangedEvent cannot be deserialized.");
        }

        #endregion
    }
}
