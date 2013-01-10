using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events
{
    public class ContextDestroyedEvent : IEvent
    {
        public sDeployedContext Context
        {
            get { return (sDeployedContext)_pars["Context"]; }
        }

        internal ContextDestroyedEvent(sDeployedContext context)
        {
            _pars.Add("Context", context);
        }

        public ContextDestroyedEvent() { }

        #region IEvent Members

        public string Name
        {
            get { return "ContextDestroyed"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteStartElement("context");
            Context.SaveToStream(writer);
            writer.WriteEndElement();
        }

        public void LoadFromElement(XmlElement element)
        {
            sDeployedContext context = new sDeployedContext();
            context.LoadFromElement((XmlElement)element.ChildNodes[0]);
            _pars.Add("Context", context);
        }

        #endregion
    }
}
