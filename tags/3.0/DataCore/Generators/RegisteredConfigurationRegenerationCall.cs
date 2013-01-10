using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml.Serialization;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators
{
    internal class RegisteredConfigurationRegenerationCall : IXmlConvertableObject,IComparable {

        private string _dialPlanType;
        public string DialPlanType
        {
            get { return _dialPlanType; }
        }

        private ADialPlan.sUpdateConfigurationsCall[] _configurationCalls;
        public ADialPlan.sUpdateConfigurationsCall[] ConfigurationCalls
        {
            get { return _configurationCalls; }
        }

        private object[] _triggerEvents;
        public object[] TriggerEvents
        {
            get { return _triggerEvents; }
        }

        public RegisteredConfigurationRegenerationCall() {
        }

        public RegisteredConfigurationRegenerationCall(Type dialPlanType, ADialPlan.sUpdateConfigurationsCall configurationCall, IEvent[] triggerEvents)
            : this(dialPlanType,new ADialPlan.sUpdateConfigurationsCall[]{configurationCall},triggerEvents)
        {
        }

        public RegisteredConfigurationRegenerationCall(Type dialPlanType, ADialPlan.sUpdateConfigurationsCall[] configurationCall,IEvent[] triggerEvents)
        {
            _dialPlanType = dialPlanType.FullName;
            _configurationCalls = configurationCall;
            _triggerEvents = null;
            if (triggerEvents != null)
            {
                _triggerEvents = new object[triggerEvents.Length];
                for (int x = 0; x < triggerEvents.Length; x++)
                    _triggerEvents[x] = (object)triggerEvents[x];
            }
        }

        public static bool operator ==(RegisteredConfigurationRegenerationCall x, RegisteredConfigurationRegenerationCall y)
        {
            return (((object)x == null && (object)y == null) ? true : (((object)x != null && (object)y != null) ? x.CompareTo(y) == 0 : false));
        }

        public static bool operator !=(RegisteredConfigurationRegenerationCall x, RegisteredConfigurationRegenerationCall y)
        {
            return !(x == y);
        }

        public static bool operator <(RegisteredConfigurationRegenerationCall x, RegisteredConfigurationRegenerationCall y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(RegisteredConfigurationRegenerationCall x, RegisteredConfigurationRegenerationCall y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(RegisteredConfigurationRegenerationCall x, RegisteredConfigurationRegenerationCall y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(RegisteredConfigurationRegenerationCall x, RegisteredConfigurationRegenerationCall y)
        {
            return x.CompareTo(y) >= 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public override string ToString()
        {
            string ret = DialPlanType + ".";
            foreach (ADialPlan.sUpdateConfigurationsCall ucc in this.ConfigurationCalls)
            {
                ret+=ucc.MethodName + "(";
                if (ucc.Parameters != null)
                {
                    foreach (NameValuePair nvp in ucc.Parameters)
                        ret += nvp.Name + "=" + (nvp.Value == null ? "null" : nvp.Value.ToString());
                }
                ret += ")";
            }
                ret+="{";
            if (this.TriggerEvents != null)
            {
                foreach (IEvent iev in this.TriggerEvents)
                    ret += "Event=" + iev.Name + ",";
                ret = ret.Substring(0, ret.Length - 1);
            }
            ret += "}";
            return ret;
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("DialPlanType",_dialPlanType );
            writer.WriteStartElement("ConfigCall");
            writer.WriteRaw(Utility.ConvertObjectToXML(_configurationCalls, true));
            writer.WriteEndElement();
            writer.WriteStartElement("TriggerEvents");
            writer.WriteRaw(Utility.ConvertObjectToXML(_triggerEvents, true));
            writer.WriteEndElement();
        }

        public void LoadFromElement(XmlElement element)
        {
            _dialPlanType = element.Attributes["DialPlanType"].Value;
            _configurationCalls = (ADialPlan.sUpdateConfigurationsCall[])Utility.ConvertObjectFromXML(element.ChildNodes[0].InnerXml);
            _triggerEvents = (object[])Utility.ConvertObjectFromXML(element.ChildNodes[1].InnerXml);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(obj.ToString());
        }

        #endregion
    }

    internal class RegisteredExtensionRegenerationCall : RegisteredConfigurationRegenerationCall
    {
        private string _extensionNumber;
        public string ExtensionNumber
        {
            get { return _extensionNumber; }
        }

        private string _domain;
        public string Domain
        {
            get { return _domain; }
        }

        public RegisteredExtensionRegenerationCall() {
        }

        public RegisteredExtensionRegenerationCall(string extensionNumber,string domain,Type dialPlanType, ADialPlan.sUpdateConfigurationsCall configurationCall, IEvent[] triggerEvents)
            : this(extensionNumber,domain,dialPlanType,new ADialPlan.sUpdateConfigurationsCall[]{configurationCall},triggerEvents)
        {
        }

        public RegisteredExtensionRegenerationCall(string extensionNumber,string domain,Type dialPlanType, ADialPlan.sUpdateConfigurationsCall[] configurationCall,IEvent[] triggerEvents)
            : base(dialPlanType,configurationCall,triggerEvents)
        {
            _extensionNumber=extensionNumber;
            _domain=domain;
        }

        public static bool operator ==(RegisteredExtensionRegenerationCall x, RegisteredExtensionRegenerationCall y)
        {
            return (((object)x == null && (object)y == null) ? true : (((object)x != null && (object)y != null) ? x.CompareTo(y) == 0 : false));
        }

        public static bool operator !=(RegisteredExtensionRegenerationCall x, RegisteredExtensionRegenerationCall y)
        {
            return !(x == y);
        }

        public static bool operator <(RegisteredExtensionRegenerationCall x, RegisteredExtensionRegenerationCall y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(RegisteredExtensionRegenerationCall x, RegisteredExtensionRegenerationCall y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(RegisteredExtensionRegenerationCall x, RegisteredExtensionRegenerationCall y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(RegisteredExtensionRegenerationCall x, RegisteredExtensionRegenerationCall y)
        {
            return x.CompareTo(y) >= 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public override string ToString()
        {
            string ret = _extensionNumber+"@"+_domain+":"+DialPlanType + ".";
            foreach (ADialPlan.sUpdateConfigurationsCall ucc in this.ConfigurationCalls)
            {
                ret += ucc.MethodName + "(";
                if (ucc.Parameters != null)
                {
                    foreach (NameValuePair nvp in ucc.Parameters)
                        ret += nvp.Name + "=" + (nvp.Value == null ? "null" : nvp.Value.ToString());
                }
                ret += ")";
            }
            ret += "{";
            if (this.TriggerEvents != null)
            {
                foreach (IEvent iev in this.TriggerEvents)
                    ret += "Event=" + iev.Name + ",";
                ret = ret.Substring(0, ret.Length - 1);
            }
            ret += "}";
            return ret;
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("extensionNumber", _extensionNumber);
            writer.WriteAttributeString("domain", _domain);
            base.SaveToStream(writer);
        }

        public void LoadFromElement(XmlElement element)
        {
            _extensionNumber = element.Attributes["extensionNumber"].Value;
            _domain = element.Attributes["domain"].Value;
            base.LoadFromElement(element);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return ToString().CompareTo(obj.ToString());
        }

        #endregion
    }
}
