using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class ModuleDeploymentEvent : IEvent
    {
        public string ModuleName
        {
            get { return (string)_pars["ModuleName"]; }
        }

        public bool Destroyed
        {
            get { return (bool)_pars["Destroyed"]; }
        }

        internal ModuleDeploymentEvent(string moduleName,bool destroyed)
        {
            _pars.Add("ModuleName", moduleName);
            _pars.Add("Destroyed", destroyed);
        }

        public ModuleDeploymentEvent()
        {
        }

        #region IEvent Members

        public string Name
        {
            get { return "ModuleDeployment"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("moduleName", ModuleName);
            writer.WriteAttributeString("destroyed", Destroyed.ToString());
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("ModuleName",element.Attributes["moduleName"].Value);
            _pars.Add("Destroyed",bool.Parse(element.Attributes["destroyed"].Value));
        }

        #endregion
    }
}
