using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using System.Reflection;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using System.Collections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;
using System.Net.Sockets;
using System.Xml;
using System.Net;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators
{
    public class ConfigurationController
    {
        private const string _RELOAD_CONFIGS_COMMAND = "auth {0}\n\nreloadxml\n\nexit\n\n";
        private static object _lock = new object();
        private const string MODULE_ID = "ConfigurationController";
        private const string ITEMS_ID = "Changes";
        private const string _MODULE_FILE_FORMAT = "ModuleFile:{0}";
        private const string _DESTROY_TRUNK_FORMAT = "DestroyOutgoingTrunk:{0}";
        private const string _DEPLOY_TRUNK_FORMAT = "DeployOutgoingTrunk:{0}";
        private const string _DESTROY_INCOMING_TRUNK_FORMAT = "DestroyIncomingTrunk:{0}";
        private const string _DEPLOY_INCOMING_TRUNK_FORMAT = "DeployIncomingTrunk:{0}";

        private static ArrayList _Changes
        {
            get { return (ArrayList)Settings.Current[MODULE_ID, ITEMS_ID]; }
            set { Settings.Current[MODULE_ID, ITEMS_ID] = value; }
        }

        public static bool HasChangesToMake
        {
            get {
                Monitor.Enter(_lock);
                bool ret = _Changes != null;
                Monitor.Exit(_lock);
                return ret;
            }
        }

        public static void RegisterExtensionChangeCall(string extensionNumber, string domain, Type dialPlanType, ADialPlan.sUpdateConfigurationsCall configurationCall, IEvent[] triggerEvents)
        {
            RegisterExtensionChangeCall(extensionNumber, domain, dialPlanType, new ADialPlan.sUpdateConfigurationsCall[] { configurationCall }, triggerEvents);
        }

        public static void RegisterExtensionChangeCall(string extensionNumber, string domain, Type dialPlanType, ADialPlan.sUpdateConfigurationsCall[] configurationCalls, IEvent[] triggerEvents)
        {
            Monitor.Enter(_lock);
            ArrayList changes = _Changes;
            if (changes == null)
                changes = new ArrayList();
            bool add = true;
            RegisteredExtensionRegenerationCall call = new RegisteredExtensionRegenerationCall(extensionNumber, domain, dialPlanType, configurationCalls, triggerEvents);
            foreach (object obj in changes)
            {
                if (obj is RegisteredExtensionRegenerationCall)
                {
                    if (call == (RegisteredExtensionRegenerationCall)obj)
                    {
                        add = false;
                        break;
                    }
                }
            }
            if (add)
                changes.Add(call);
            _Changes = changes;
            Monitor.Exit(_lock);
        }

        public static void RegisterChangeCall(Type dialPlanType, ADialPlan.sUpdateConfigurationsCall configurationCall, IEvent[] triggerEvents)
        {
            RegisterChangeCall(dialPlanType, new ADialPlan.sUpdateConfigurationsCall[] { configurationCall }, triggerEvents);
        }

        public static void RegisterChangeCall(Type dialPlanType, ADialPlan.sUpdateConfigurationsCall[] configurationCalls, IEvent[] triggerEvents)
        {
            Monitor.Enter(_lock);
            ArrayList changes = _Changes;
            if (changes == null)
                changes = new ArrayList();
            bool add = true;
            RegisteredConfigurationRegenerationCall call = new RegisteredConfigurationRegenerationCall(dialPlanType, configurationCalls, triggerEvents);
            foreach (object obj in changes)
            {
                if (obj is RegisteredConfigurationRegenerationCall)
                {
                    if (call.Equals(obj))
                    {
                        add = false;
                        break;
                    }
                }
            }
            if (add)
                changes.Add(call);
            _Changes = changes;
            Monitor.Exit(_lock);
        }

        public static void RegisterModuleFileRedeployment(string moduleFile)
        {
            Monitor.Enter(_lock);
            ArrayList changes = _Changes;
            if (changes == null)
                changes = new ArrayList();
            bool add = true;
            foreach (object obj in changes)
            {
                if (obj is string)
                {
                    if ((string)obj == string.Format(_MODULE_FILE_FORMAT,moduleFile))
                    {
                        add = false;
                        break;
                    }
                }
            }
            if (add)
                changes.Add(string.Format(_MODULE_FILE_FORMAT, moduleFile));
            _Changes = changes;
            Monitor.Exit(_lock);
        }

        public static void RegisterDestroyTrunkRedeployment(string trunkName,string profileName)
        {
            Monitor.Enter(_lock);
            ArrayList changes = _Changes;
            if (changes == null)
                changes = new ArrayList();
            bool add = true;
            foreach (object obj in changes)
            {
                if (obj is string)
                {
                    if ((string)obj == string.Format(_DESTROY_TRUNK_FORMAT, trunkName+"@"+profileName))
                    {
                        add = false;
                        break;
                    }
                }
            }
            if (add)
                changes.Add(string.Format(_DESTROY_TRUNK_FORMAT, trunkName));
            _Changes = changes;
            Monitor.Enter(_lock);
        }

        public static void RegisterDeployTrunkRedployment(string trunkName)
        {
            Monitor.Enter(_lock);
            ArrayList changes = _Changes;
            if (changes == null)
                changes = new ArrayList();
            bool add = true;
            foreach (object obj in changes)
            {
                if (obj is string)
                {
                    if ((string)obj == string.Format(_DEPLOY_TRUNK_FORMAT, trunkName))
                    {
                        add = false;
                        break;
                    }
                }
            }
            if (add)
                changes.Add(string.Format(_DEPLOY_TRUNK_FORMAT, trunkName));
            _Changes = changes;
            Monitor.Enter(_lock);
        }

        public static void RegisterDestroyIncomingTrunkRedeployment(string number, string domain)
        {
            Monitor.Enter(_lock);
            ArrayList changes = _Changes;
            if (changes == null)
                changes = new ArrayList();
            bool add = true;
            foreach (object obj in changes)
            {
                if (obj is string)
                {
                    if ((string)obj == string.Format(_DESTROY_INCOMING_TRUNK_FORMAT, number + "@" + domain))
                    {
                        add = false;
                        break;
                    }
                }
            }
            if (add)
                changes.Add(string.Format(_DESTROY_TRUNK_FORMAT, number));
            _Changes = changes;
            Monitor.Enter(_lock);
        }

        public static void RegisterDeployIncomingTrunkRedployment(string number,string domain)
        {
            Monitor.Enter(_lock);
            ArrayList changes = _Changes;
            if (changes == null)
                changes = new ArrayList();
            bool add = true;
            foreach (object obj in changes)
            {
                if (obj is string)
                {
                    if ((string)obj == string.Format(_DEPLOY_INCOMING_TRUNK_FORMAT, number+"@"+domain))
                    {
                        add = false;
                        break;
                    }
                }
            }
            if (add)
                changes.Add(string.Format(_DEPLOY_TRUNK_FORMAT, number));
            _Changes = changes;
            Monitor.Enter(_lock);
        }

        public static void ProcessChanges()
        {
            Monitor.Enter(_lock);
            ArrayList changes = _Changes;
            List<IEvent> events = new List<IEvent>();
            if (changes != null)
            {
                foreach (object obj in changes)
                {
                    if (obj is RegisteredConfigurationRegenerationCall)
                    {
                        RegisteredConfigurationRegenerationCall rcgc = (RegisteredConfigurationRegenerationCall)obj;
                        if (rcgc.TriggerEvents != null)
                        {
                            foreach (IEvent ev in rcgc.TriggerEvents)
                                events.Add(ev);
                        }
                        foreach (ADialPlan.sUpdateConfigurationsCall ucc in rcgc.ConfigurationCalls)
                            CallControlManager.UpdateDialPlan(Utility.LocateType(rcgc.DialPlanType), ucc);
                    }
                    else if (obj is RegisteredExtensionRegenerationCall)
                    {
                        RegisteredExtensionRegenerationCall regc = (RegisteredExtensionRegenerationCall)obj;
                        if (regc.TriggerEvents != null)
                        {
                            foreach (IEvent ev in regc.TriggerEvents)
                                events.Add(ev);
                        }
                        foreach (ADialPlan.sUpdateConfigurationsCall ucc in regc.ConfigurationCalls)
                            CallControlManager.UpdateDialPlan(Utility.LocateType(regc.DialPlanType), ucc);
                        CoreGenerator.RegenerateExtensionFiles(regc.ExtensionNumber, Domain.Load(regc.Domain));
                    }
                    else if (obj is string)
                    {
                        if (((string)obj).StartsWith(string.Format(_MODULE_FILE_FORMAT, "")))
                        {
                            string mod = ((string)obj).Substring(string.Format(_MODULE_FILE_FORMAT, "").Length);
                            CoreGenerator.RegenerateModuleFile(mod);
                            EventController.TriggerEvent(new FreeswitchModuleRedeployedEvent(mod));
                        }
                        else if (((string)obj).StartsWith(string.Format(_DEPLOY_TRUNK_FORMAT, "")))
                        {
                            string depTrunkName = ((string)obj).Substring(string.Format(_DEPLOY_TRUNK_FORMAT, "").Length);
                            CoreGenerator.GenerateOutgoingSIPTrunkConfiguration(depTrunkName);
                            EventController.TriggerEvent(new GenericEvent("OutgoingSIPTrunkCreated",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",depTrunkName)
                                    }));
                        }
                        else if (((string)obj).StartsWith(string.Format(_DESTROY_TRUNK_FORMAT, "")))
                        {
                            string desTrunkName = ((string)obj).Substring(string.Format(_DESTROY_TRUNK_FORMAT, "").Length);
                            string desProfileName = desTrunkName.Split('@')[1];
                            desTrunkName = desTrunkName.Substring(0, desTrunkName.IndexOf("@"));
                            CoreGenerator.DestroyOutgoingSIPTrunk(desTrunkName, desProfileName);
                            EventController.TriggerEvent(new GenericEvent("OutgoingSIPTrunkDeleted",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",desTrunkName),
                                        new NameValuePair("ProfileName",desProfileName)
                                    }));
                        }
                        else if (((string)obj).StartsWith(string.Format(_DEPLOY_INCOMING_TRUNK_FORMAT, "")))
                        {
                            string depIncomingTrunkName = ((string)obj).Substring(string.Format(_DEPLOY_INCOMING_TRUNK_FORMAT, "").Length);
                            string depIncomingDomainName = depIncomingTrunkName.Split('@')[1];
                            depIncomingTrunkName = depIncomingTrunkName.Substring(0, depIncomingTrunkName.IndexOf("@"));
                            CoreGenerator.RegenerateIncomingSIPTrunkFile(depIncomingTrunkName, Domain.Load(depIncomingDomainName));
                            EventController.TriggerEvent(new GenericEvent("IncomingSIPTrunkDeployed",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",depIncomingTrunkName),
                                        new NameValuePair("Domain",depIncomingDomainName)
                                    }));
                        }
                        else if (((string)obj).StartsWith(string.Format(_DESTROY_INCOMING_TRUNK_FORMAT, "")))
                        {
                            string desIncomingTrunkName = ((string)obj).Substring(string.Format(_DESTROY_INCOMING_TRUNK_FORMAT, "").Length);
                            string desIncomingDomainName = desIncomingTrunkName.Split('@')[1];
                            desIncomingTrunkName = desIncomingTrunkName.Substring(0, desIncomingTrunkName.IndexOf("@"));
                            CoreGenerator.DestroyIncomingSIPTrunkFile(desIncomingTrunkName, desIncomingDomainName);
                            EventController.TriggerEvent(new GenericEvent("IncomingSIPTrunkDestroyed",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",desIncomingTrunkName),
                                        new NameValuePair("Domain",desIncomingDomainName)
                                    }));
                        }
                    }
                }
            }
            _Changes = null;
            if (ModuleController.Current.IsModuleEnabled("ESL"))
                ModuleController.Current.InvokeModuleMethod("ESL", "ReloadConfigs", false);
            else
                _ReloadConfigurations();
            foreach (IEvent ie in events)
                EventController.TriggerEvent(ie);
            Monitor.Exit(_lock);
        }

        private static void _ReloadConfigurations()
        {
            string ip = "127.0.0.1";
            int port = 8021;
            string password = "ClueCon";
            sFreeSwitchModuleFile swm = CoreGenerator.ReadCurrentModuleSettings("event_socket");
            if (swm != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<?xml version=\"1.0\"?>" + swm.ConfigurationSectionString);
                foreach (XmlNode node in doc.GetElementsByTagName("param"))
                {
                    if (!(node is XmlComment))
                    {
                        switch (node.Attributes["name"].Value)
                        {
                            case "listen_port":
                                port = int.Parse(node.Attributes["value"].Value);
                                break;
                            case "password":
                                password = node.Attributes["value"].Value;
                                break;
                        }
                    }
                }
            }
            TcpClient cl = new TcpClient();
            cl.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            cl.Client.Send(ASCIIEncoding.ASCII.GetBytes(string.Format(_RELOAD_CONFIGS_COMMAND, password)));
            cl.Close();
        }

        public static void ReloadConfigs()
        {
            if (ModuleController.Current.IsModuleEnabled("ESL"))
                ModuleController.Current.InvokeModuleMethod("ESL", "ReloadConfigs", false);
            else
                _ReloadConfigurations();
        }
    }
}
