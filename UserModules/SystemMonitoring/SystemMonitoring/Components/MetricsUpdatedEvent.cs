﻿using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Components
{
    public class MetricsUpdatedEvent : IEvent
    {
        #region IEvent Members

        public string Name
        {
            get { return "MetricsUpdatedEvent"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string, object>();

        public object this[string name]
        {
            get
            {
                if (_pars.ContainsKey(name))
                    return _pars[name];
                return null;
            }
        }

        public Dictionary<string, object>.KeyCollection Keys
        {
            get { return _pars.Keys; }
        }

        #endregion

        public MetricsUpdatedEvent(List<sSystemMetric> metrics)
        {
            foreach (sSystemMetric sm in metrics)
                _pars.Add(sm.Type.ToString() + (sm.Additional == null ? "" : "[" + sm.Additional + "]"), sm.Val);
        }

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
