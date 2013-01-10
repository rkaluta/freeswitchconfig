using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class ContextDeploymentEvent : IEvent
    {
        public sDeployedContext Context
        {
            get { return (sDeployedContext)_pars["Context"]; }
        }

        internal ContextDeploymentEvent(sDeployedContext context)
        {
            _pars.Add("Context", context);
        }

        public ContextDeploymentEvent()
        {
        }

        #region IEvent Members

        public string Name
        {
            get { return "ContextDeployment"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteRaw(Utility.ConvertObjectToXML(Context, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("Context",(sDeployedContext)Utility.ConvertObjectFromXML(element.InnerXml));
        }

        #endregion
    }
}
