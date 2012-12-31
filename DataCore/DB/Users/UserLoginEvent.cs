using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Net;
using System.Xml.Serialization;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users
{
    public class UserLoginEvent : IEvent
    {

        public enum LoginEventTypes
        {
            SUCCESS,
            FAILURE,
            ATTEMPTS_EXCEEDED,
            ACCOUNT_LOCKED
        }

        #region IEvent Members

        public string Name
        {
            get { return "User Login Event"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name] { get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }
        #endregion

        public string UserName
        {
            get { return (string)_pars["UserName"]; }
        }

        public IPAddress Host
        {
            get { return (IPAddress)_pars["Host"]; }
        }

        public LoginEventTypes Type
        {
            get { return (LoginEventTypes)_pars["Type"]; }
        }

        internal UserLoginEvent(string username, IPAddress host, LoginEventTypes type)
        {
            _pars.Add("Username", username);
            _pars.Add("Host", host);
            _pars.Add("Type", type);
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("UserName", UserName);
            writer.WriteAttributeString("Host", Host.ToString());
            writer.WriteAttributeString("Type", Type.ToString());
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Add("UserName",element.Attributes["UserName"].Value);
            _pars.Add("Host",IPAddress.Parse(element.Attributes["Host"].Value));
            _pars.Add("Type",(LoginEventTypes)Enum.Parse(typeof(LoginEventTypes), element.Attributes["Type"].Value));
        }

        #endregion
    }
}
