using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;

using System.Xml.Serialization;
using System.Xml;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Events
{
    public class HttpRequestCompleteEvent: IEvent
    {
        #region IEvent Members

        public string Name
        {
            get
            {
                return "Http Request Complete";
            }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        public HttpRequest Request
        {
            get { return (HttpRequest)this["Request"]; }
        }

        public HttpRequestCompleteEvent(HttpRequest request)
        {
            _pars.Add("Request", request);
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            throw new Exception("The HttpRequestCompleteEvent cannot be serialized.");
        }

        public void LoadFromElement(XmlElement element)
        {
            throw new Exception("The HttpRequestCompleteEvent cannot be deserialized.");
        }

        #endregion
    }
}
