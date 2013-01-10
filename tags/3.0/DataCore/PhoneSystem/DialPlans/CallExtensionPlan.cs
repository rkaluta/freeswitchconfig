using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using System.Collections;
using Procurios.Public;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.DialPlans
{
    public class CallExtensionPlan : ADialPlan
    {
        private const int DEFAULT_TIMEOUT = 10;
        private const string _HAS_VOICEMAIL_FIELD_ID = "hasVoicemail";
        private const string _TIMEOUT_FIELD_ID = "timeout";
        private const string _DOMAIN_FIELD_ID = "domain";
        private const string _EXTENSION_FIELD_ID = "extension";

        public CallExtensionPlan()
        {
        }

        public override DialPlanPriorities Priority
        {
            get { return DialPlanPriorities.Normal; }
        }

        public override DialPlanPriorities SubPriority
        {
            get { return DialPlanPriorities.Low; }
        }

        public override bool Movable
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "CallExtension"; }
        }

        public override string Description
        {
            get { return "This dialplan handles all calls made to a given extension."; }
        }

        public override List<string> ContextsUsed
        {
            get
            {
                List<string> ret = new List<string>();
                lock (_lock)
                {
                    foreach (string str in StoredConfiguration.Keys)
                        ret.Add(str);
                }
                return ret;
            }
        }

        public override sCallExtensionReference[] DefinedCallExtensions
        {
            get
            {
                List<sCallExtensionReference> ret = new List<sCallExtensionReference>();
                lock (_lock)
                {
                    foreach (string cont in StoredConfiguration.Keys)
                    {
                        foreach (Hashtable ext in (ArrayList)StoredConfiguration[cont])
                            ret.Add(new sCallExtensionReference("ext_" + (string)ext[_EXTENSION_FIELD_ID], cont));
                    }
                }
                return ret.ToArray();
            }
        }

        public override sCallContext[] CallContexts
        {
            get
            {
                List<sCallContext> ret = new List<sCallContext>();
                lock (_lock)
                {
                    foreach (string cont in StoredConfiguration.Keys)
                    {
                        List<sCallExtension> exts = new List<sCallExtension>();
                        foreach (Hashtable ext in (ArrayList)StoredConfiguration[cont])
                        {
                            List<ICallAction> actions = new List<ICallAction>();
                            actions.Add(new Actions.RingReady());
                            actions.Add(new Actions.Set("hangup_after_bridge", "true"));
                            if ((bool)ext[_HAS_VOICEMAIL_FIELD_ID])
                            {
                                actions.Add(new Actions.Set("call_timeout", ext[_TIMEOUT_FIELD_ID].ToString()));
                                actions.Add(new Actions.Set("continue_on_fail", "true"));
                            }
                            else
                                actions.Add(new Actions.Set("continue_on_fail", "false"));
                            actions.Add(new Actions.BridgeToExtension(new sDomainExtensionPair((string)ext[_EXTENSION_FIELD_ID], (string)ext[_DOMAIN_FIELD_ID]), true));
                            if ((bool)ext[_HAS_VOICEMAIL_FIELD_ID])
                            {
                                actions.Add(new Actions.Answer());
                                actions.Add(new Actions.Sleep(1000));
                                actions.Add(new Actions.Voicemail(cont, new sDomainExtensionPair((string)ext[_EXTENSION_FIELD_ID], (string)ext[_DOMAIN_FIELD_ID])));
                            }
                            exts.Add(new sCallExtension("ext_" + (string)ext[_EXTENSION_FIELD_ID], true, false, new ICallCondition[]{
                                new sCallFieldCondition("destination_number",(string)ext[_EXTENSION_FIELD_ID],false,actions.ToArray(),null,null)
                            }));
                        }
                        ret.Add(new sCallContext(cont, exts.ToArray()));
                    }
                }
                return ret.ToArray();
            }
        }

        public override void LoadInitialConfiguration()
        {
            lock (_lock)
            {
                Hashtable ht = new Hashtable();
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones",
                "SELECT ext.Number,ext.VoicemailTimeout,vm.Number,ext.Context.Name,ext.Domain.Name FROM Extension ext LEFT JOIN VoiceMail vm ON ext.Number = vm.Number ORDER BY ext.Context.Name");
                cq.Execute();
                string curContext = "";
                ArrayList exts = new ArrayList();
                while (cq.Read())
                {
                    if (curContext != cq[3].ToString())
                    {
                        if (exts.Count > 0)
                        {
                            ht.Add(curContext, exts);
                            exts = new ArrayList();
                            curContext = cq[3].ToString();
                        }
                    }
                    Hashtable ext = new Hashtable();
                    ext.Add(_EXTENSION_FIELD_ID, cq[0].ToString());
                    ext.Add(_HAS_VOICEMAIL_FIELD_ID, !cq.IsDBNull(2));
                    ext.Add(_TIMEOUT_FIELD_ID, (cq.IsDBNull(1) ? DEFAULT_TIMEOUT : cq.GetInt32(1)));
                    ext.Add(_DOMAIN_FIELD_ID, cq[4].ToString());
                    exts.Add(ext);
                }
                if (exts.Count > 0)
                    ht.Add(curContext, exts);
                cq.Close();
                StoredConfiguration = ht;
            }
        }

        protected void DeleteExtension(string context, string extension)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                if (ht.ContainsKey(context))
                {
                    ArrayList cont = (ArrayList)ht[context];
                    for (int x = 0; x < cont.Count; x++)
                    {
                        Hashtable ext = (Hashtable)cont[x];
                        if ((string)ext[_EXTENSION_FIELD_ID] == extension)
                        {
                            cont.RemoveAt(x);
                            break;
                        }
                    }
                    ht.Remove(context);
                    ht.Add(context, cont);
                }
                StoredConfiguration = ht;
            }
        }

        protected void AddExtension(string context, string extension, string domain, bool hasVoicemail, int? timeout)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList cont = new ArrayList();
                if (ht.ContainsKey(context))
                    cont = (ArrayList)ht[context];
                bool add = true;
                foreach (Hashtable ext in cont)
                {
                    if ((string)ext[_EXTENSION_FIELD_ID] == extension)
                    {
                        add = false;
                        break;
                    }
                }
                if (!add)
                    throw new Exception("Unable to add Extension, 1 already exists in the context[" + context + "] for the number[" + extension + "]");
                Hashtable ex = new Hashtable();
                ex.Add(_EXTENSION_FIELD_ID, extension);
                ex.Add(_DOMAIN_FIELD_ID, domain);
                ex.Add(_HAS_VOICEMAIL_FIELD_ID, hasVoicemail);
                ex.Add(_TIMEOUT_FIELD_ID, (timeout.HasValue ? timeout.Value : DEFAULT_TIMEOUT));
                cont.Add(ex);
                ht.Remove(context);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateExtension(string context, string extension, string newExtension, string domain, bool hasVoicemail, int? timeout)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList cont = new ArrayList();
                if (ht.ContainsKey(context))
                    cont = (ArrayList)ht[context];
                int index = cont.Count;
                for (int x = 0; x < cont.Count; x++)
                {
                    Hashtable ext = (Hashtable)cont[x];
                    if ((string)ext[_EXTENSION_FIELD_ID] == extension)
                    {
                        index = x;
                        cont.RemoveAt(x);
                        break;
                    }
                }
                Hashtable ex = new Hashtable();
                ex.Add(_EXTENSION_FIELD_ID, extension);
                ex.Add(_DOMAIN_FIELD_ID, domain);
                ex.Add(_HAS_VOICEMAIL_FIELD_ID, hasVoicemail);
                ex.Add(_TIMEOUT_FIELD_ID, (timeout.HasValue ? timeout.Value : DEFAULT_TIMEOUT));
                cont.Insert(index, ex);
                ht.Remove(context);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void AddVoicemail(string context, string extension)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList cont = new ArrayList();
                if (ht.ContainsKey(context))
                    cont = (ArrayList)ht[context];
                int index = cont.Count;
                Hashtable ex = null;
                for (int x = 0; x < cont.Count; x++)
                {
                    Hashtable ext = (Hashtable)cont[x];
                    if ((string)ext[_EXTENSION_FIELD_ID] == extension)
                    {
                        index = x;
                        ex = (Hashtable)cont[x];
                        cont.RemoveAt(x);
                        break;
                    }
                }
                if (ex != null)
                {
                    ex.Remove(_HAS_VOICEMAIL_FIELD_ID);
                    ex.Add(_HAS_VOICEMAIL_FIELD_ID, true);
                    cont.Insert(index, ex);
                    ht.Remove(context);
                    ht.Add(context, cont);
                    StoredConfiguration = ht;
                }
            }
        }

        protected void RemoveVoicemail(string context, string extension)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList cont = new ArrayList();
                if (ht.ContainsKey(context))
                    cont = (ArrayList)ht[context];
                int index = cont.Count;
                Hashtable ex = null;
                for (int x = 0; x < cont.Count; x++)
                {
                    Hashtable ext = (Hashtable)cont[x];
                    if ((string)ext[_EXTENSION_FIELD_ID] == extension)
                    {
                        index = x;
                        ex = (Hashtable)cont[x];
                        cont.RemoveAt(x);
                        break;
                    }
                }
                if (ex != null)
                {
                    ex.Remove(_HAS_VOICEMAIL_FIELD_ID);
                    ex.Add(_HAS_VOICEMAIL_FIELD_ID, false);
                    cont.Insert(index, ex);
                    ht.Remove(context);
                    ht.Add(context, cont);
                    StoredConfiguration = ht;
                }
            }
        }
    }
}
