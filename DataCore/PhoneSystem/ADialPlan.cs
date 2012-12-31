using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using System.Collections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;
using System.Reflection;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Procurios.Public;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem
{
    public abstract class ADialPlan : IComparable
    {
        private const string _DIAL_PLANS_ID = "DialPlans";
        protected object _lock = new object();

        public struct sUpdateConfigurationsCall : IXmlConvertableObject
        {
            private string _methodName;
            public string MethodName
            {
                get { return _methodName; }
            }

            private NameValuePair[] _parameters;
            public NameValuePair[] Parameters
            {
                get { return _parameters; }
            }

            public sUpdateConfigurationsCall(string methodName,NameValuePair[] parameters){
                _methodName = methodName;
                _parameters = parameters;
            }

            #region IXmlConvertableObject Members

            public void SaveToStream(XmlWriter writer)
            {
                writer.WriteAttributeString("methodName", _methodName);
                writer.WriteRaw(Utility.ConvertObjectToXML(_parameters,true));
            }

            public void LoadFromElement(XmlElement element)
            {
                _methodName = element.Attributes["methodName"].Value;
                _parameters = (NameValuePair[])Utility.ConvertObjectFromXML(element.InnerXml);
            }

            #endregion
        }

        public ADialPlan() {
        }

        public void UpdateConfiguration(sUpdateConfigurationsCall call)
        {
            MethodInfo mi=null;
            object[] pars = new object[call.Parameters.Length];
            foreach (MethodInfo m in this.GetType().GetMethods())
            {
                if (m.Name == call.MethodName)
                {
                    if (m.GetParameters().Length == call.Parameters.Length)
                    {
                        bool isvalid = false;
                        int x = 0;
                        foreach (ParameterInfo pi in m.GetParameters())
                        {
                            bool found = false;
                            foreach (NameValuePair nvp in call.Parameters)
                            {
                                if (nvp.Name == pi.Name)
                                {
                                    found = true;
                                    pars[x] = nvp.Value;
                                    found = true;
                                    x++;
                                }
                            }
                            if (!found)
                            {
                                isvalid = false;
                                break;
                            }
                        }
                        if (isvalid)
                        {
                            mi = m;
                            break;
                        }
                    }
                }
            }
            if (mi == null)
                throw new Exception("Unable to update configuration for dialplan [" + Name + "], no valid method found");
            mi.Invoke(this, pars);
        }

        protected Hashtable StoredConfiguration
        {
            get
            {
                if (Settings.Current[CallControlManager._MODULE_NAME, _DIAL_PLANS_ID] != null)
                    return (Hashtable)JSON.JsonDecode((string)Settings.Current[CallControlManager._MODULE_NAME, _DIAL_PLANS_ID]);
                return new Hashtable();
            }
            set
            {
                Settings.Current[CallControlManager._MODULE_NAME, _DIAL_PLANS_ID] = JSON.JsonEncode(value);
            }
        }

        public abstract DialPlanPriorities Priority { get; }
        public abstract DialPlanPriorities SubPriority { get; }
        public abstract bool Movable { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract sCallContext[] CallContexts { get; }
        //remove code when done task
        //public abstract void RunDialPlanWithConditions(DialedNumberTester tester, out bool exit, out bool startFromTop);
        public abstract void LoadInitialConfiguration();
        public abstract List<string> ContextsUsed { get; }
        public abstract sCallExtensionReference[] DefinedCallExtensions { get; }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj.GetType().IsSubclassOf(typeof(ADialPlan)))
            {
                ADialPlan adp = (ADialPlan)obj;
                if (((int)adp.Priority).CompareTo((int)Priority) == 0)
                    if (((int)adp.SubPriority).CompareTo((int)SubPriority) == 0)
                        return adp.Name.CompareTo(Name);
                    else
                        return ((int)adp.SubPriority).CompareTo((int)SubPriority);
                else
                    return ((int)adp.Priority).CompareTo((int)Priority);
            }
            throw new Exception("Unable to compare an object of the type DialPlan with a non-DialPlan object.");
        }

        #endregion

        public override sealed bool Equals(object obj)
        {
            if (obj.GetType().IsSubclassOf(typeof(ADialPlan)))
                return Name == ((ADialPlan)obj).Name;
            return false;
        }

        public override sealed int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
