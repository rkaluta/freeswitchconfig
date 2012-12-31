using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml.Schema;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Events
{
    public class ErrorOccuredEvent : IEvent
    {
        #region IEvent Members

        public string Name
        {
            get { return "ErrorOccured"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject
        public void SaveToStream(XmlWriter writer)
        {
            throw new Exception("The ErrorOccuredEvent cannot be serialized.");
        }

        public void LoadFromElement(XmlElement element)
        {
            throw new Exception("The ErrorOccuredEvent cannot be deserialized.");
        }
        #endregion

        public Exception Exception
        {
            get { return (Exception)this["Exception"]; }
        }

        public ErrorOccuredEvent(Exception exception)
        {
            _pars.Add("Exception", exception);
        }

        public ErrorOccuredEvent(string message) : this(new Exception(message)){}
    }
}
