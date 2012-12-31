using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones
{
    public class HttpCDREvent : IEvent
    {
        internal HttpCDREvent(XmlElement cdrElement)
        {
            foreach (XmlElement elem in cdrElement.ChildNodes)
            {
                if (elem.Name == "variables")
                {
                    foreach (XmlElement e in elem.ChildNodes)
                    {
                        if (_pars.ContainsKey(e.Name))
                            _pars.Remove(e.Name);
                        _pars.Add(e.Name, e.InnerText);
                    }
                }
                else
                    _pars.Add(elem.Name, elem);
            }

        }

        #region IEvent Members

        public string Name
        {
            get { return "HTTP CDR Event"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public Dictionary<string, object>.KeyCollection Keys
        {
            get { return _pars.Keys; }
        }

        public object this[string name]
        {
            get { return (_pars.ContainsKey(name) ? _pars[name] : null);  }
        }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public void LoadFromElement(XmlElement element)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
