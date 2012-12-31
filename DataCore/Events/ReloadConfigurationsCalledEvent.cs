using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml.Serialization;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Events
{
    public class ReloadConfigurationsCalledEvent : IEvent
    {
        #region IEvent Members

        public string Name
        {
            get { return "Reload Configuations Called"; }
        }

        public object this[string name] { get { return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return null; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
        }

        public void LoadFromElement(XmlElement element)
        {
        }

        #endregion
    }
}
