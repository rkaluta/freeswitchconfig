using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;

using System.Xml;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Events
{
    public class HttpRequestErrorEvent : IEvent
    {
        #region IEvent Members

        public string Name
        {
            get { return "Http Request Error"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        public HttpRequest Request
        {
            get { return (HttpRequest)_pars["Request"]; }
        }

        public Exception Error{
            get{return (Exception)_pars["Error"];}
        }

        public HttpRequestErrorEvent(HttpRequest request,Exception error)
        {
            _pars.Add("Request", request);
            _pars.Add("Error", error);
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            throw new Exception("The HttpRequestErrorEvent cannot be serialized.");
        }

        public void LoadFromElement(XmlElement element)
        {
            throw new Exception("The HttpRequestErrorEvent cannot be deserialized.");
        }

        #endregion
    }
}
