using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class ExtensionDeploymentEvent : IEvent
    {
        public sDeployedExtension Extension
        {
            get { return (sDeployedExtension)_pars["Extension"]; }
        }

        internal ExtensionDeploymentEvent(sDeployedExtension extension)
        {
            _pars.Add("Extension", extension);
        }

        public ExtensionDeploymentEvent()
        {
        }

        #region IEvent Members

        public string Name
        {
            get { return "ExtensionDeployment"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteRaw(Utility.ConvertObjectToXML(Extension, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("Extension",(sDeployedExtension)Utility.ConvertObjectFromXML(element.InnerXml));
        }

        #endregion
    }
}
