using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System;
using System.Xml.Serialization;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators
{
    public class FreeswitchModuleRedeployedEvent : IEvent
    {
        #region IEvent Members

        public string Name
        {
            get { return "FreeswitchModuleRedeployedEvent"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        public string FileName
        {
            get { return (string)_pars["FileName"]; }
        }

        public FreeswitchModuleRedeployedEvent(string fileName)
        {
            _pars.Add("FileName", fileName);
        }

        public FreeswitchModuleRedeployedEvent() { }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("FileName", FileName);
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("FileName",element.Attributes["FileName"].Value);
        }

        #endregion
    }
}
