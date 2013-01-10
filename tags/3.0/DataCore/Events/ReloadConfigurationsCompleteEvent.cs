using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml.Serialization;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Events
{
    public class ReloadConfigurationsCompleteEvent : IEvent
    {
        #region IEvent Members

        public string Name
        {
            get { return "Reload Configurations Complete"; }
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
