using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;

using System.Xml;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Events
{
    public class HttpRequestRecievedEvent : IEvent
    {
        #region IEvent Members

        public string Name
        {
            get
            {
                return "Http Request Recieved";
            }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        public HttpRequest Request
        {
            get { return (HttpRequest)_pars["Request"]; }
        }

        public HttpRequestRecievedEvent(HttpRequest request)
        {
            _pars.Add("Request", request);
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            throw new Exception("The HttpRequestRecievedEvent cannot be serialized.");
        }

        public void LoadFromElement(XmlElement element)
        {
            throw new Exception("The HttpRequestRecievedEvent cannot be deserialized.");
        }

        #endregion
    }
}
