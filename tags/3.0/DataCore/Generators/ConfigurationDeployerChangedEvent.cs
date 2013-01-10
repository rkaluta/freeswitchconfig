using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators
{
    public class ConfigurationDeployerChangedEvent : IEvent
    {
        public Type PreviousType
        {
            get { return (Type)this["PreviousType"]; }
        }

        public Type NewType
        {
            get { return (Type)this["NewType"]; }
        }

        public ConfigurationDeployerChangedEvent(Type previousType, Type newType)
        {
            _pars.Add("PreviousType", previousType);
            _pars.Add("NewType", newType);
        }

        public ConfigurationDeployerChangedEvent() { }

        #region IEvent Members

        public string Name
        {
            get { return "ConfigurationDeployerChangedEvent"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("PreviousType", PreviousType.FullName);
            writer.WriteAttributeString("NewType", NewType.FullName);
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("PreviousType",Utility.LocateType(element.Attributes["PrevoiusType"].Value));
            _pars.Add("NewType",Utility.LocateType(element.Attributes["NewType"].Value));
        }

        #endregion
    }
}
