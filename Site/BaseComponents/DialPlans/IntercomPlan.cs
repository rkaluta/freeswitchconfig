using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using System.Collections;
using Procurios.Public;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans
{
    public class IntercomPlan : ADialPlan
    {
        private const string _EXTENSION_FIELD_NAME = "Extension";
        private const string _ONE_WAY_FIELD_NAME = "OneWay";
        private const string _MAPS_FIELD_NAME = "Extensions";

        public override DialPlanPriorities Priority
        {
            get { return DialPlanPriorities.Normal; }
        }

        public override DialPlanPriorities SubPriority
        {
            get { return DialPlanPriorities.Normal; }
        }

        public override bool Movable
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "Intercoms"; }
        }

        public override string Description
        {
            get { return "This dial plan controls the ability to map a given extension number to an intercom and allow for intercom capabilities."; }
        }

        public override List<string> ContextsUsed
        {
            get {
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
            get {
                List<sCallExtensionReference> ret = new List<sCallExtensionReference>();
                lock (_lock)
                {
                    foreach (string cont in StoredConfiguration.Keys)
                    {
                        foreach (Hashtable icom in (ArrayList)StoredConfiguration[cont])
                            ret.Add(new sCallExtensionReference("icom_" + (string)icom[_EXTENSION_FIELD_NAME], cont));
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
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents",
                        "SELECT i.Number,i.Context.Name,i.OneWay FROM Intercom i ORDER BY i.Context.Name");
                cq.Execute();
                ClassQuery cqExts = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents",
                    "SELECT i.Extensions.Number,i.Extensions.Domain.Name FROM Intercom i WHERE i.Number = @intercomNumber");
                ArrayList icoms = new ArrayList();
                string curContext = "";
                while (cq.Read())
                {
                    cqExts.Execute(new IDbDataParameter[]{
                    cqExts.CreateParameter("@intercomNumber",cq[0].ToString())
                });
                    ArrayList pairs = new ArrayList();
                    while (cqExts.Read())
                    {
                        pairs.Add(cqExts[0].ToString() + "@" + cqExts[1].ToString());
                    }
                    cqExts.Close();
                    if (curContext != cq[1].ToString())
                    {
                        if (icoms.Count > 0)
                            ht.Add(curContext, icoms);
                        curContext = cq[1].ToString();
                        icoms = new ArrayList();
                    }
                    Hashtable icom = new Hashtable();
                    icom.Add(_EXTENSION_FIELD_NAME, cq[0].ToString());
                    icom.Add(_MAPS_FIELD_NAME, pairs);
                    icom.Add(_ONE_WAY_FIELD_NAME, cq.GetBoolean(2));
                    icoms.Add(icom);
                }
                cq.Close();
                if (icoms.Count > 0)
                {
                    ht.Add(curContext, icoms);
                }
                StoredConfiguration = ht;
            }
        }

        public IntercomPlan(){
        }

        public override sCallContext[] CallContexts
        {
            get {
                List<sCallContext> ret = new List<sCallContext>();
                lock (_lock)
                {
                    Hashtable ht = StoredConfiguration;
                    foreach (string cont in ht.Keys)
                    {
                        List<sCallExtension> exts = new List<sCallExtension>();
                        foreach (Hashtable icom in (ArrayList)ht[cont])
                        {
                            List<ICallCondition> condAutoCalls = new List<ICallCondition>();
                            condAutoCalls.Add(new sCallFieldCondition("destination_number",
                                (string)icom[_EXTENSION_FIELD_NAME],
                                false,
                                new ICallAction[]{
                                new Actions.Set("api_hangup_hook","conference "+(string)icom[_EXTENSION_FIELD_NAME]+" kick all"),
                                new Actions.Answer(),
                                new Actions.ExportSettings("sip_invite_params","intercom=true"),
                                new Actions.ExportSettings("sip_auto_answer","true"),
                                new Actions.Set("conference_auto_outcall_caller_id_name","$${effective_caller_id_name}"),
                                new Actions.Set("conference_auto_outcall_caller_id_number","$${effective_caller_id_number}"),
                                new Actions.Set("conference_auto_outcall_timeout","5"),
                                new Actions.Set("conference_auto_outcall_flags",((bool)icom[_ONE_WAY_FIELD_NAME] ? "mute" : "none"))
                            },
                                null,
                                null));
                            foreach (string ext in (ArrayList)icom[_MAPS_FIELD_NAME])
                            {
                                condAutoCalls.Add(
                                    new sCallFieldCondition("${variable_accountcode}@${variable_domain_name}",
                                        ext,
                                        false,
                                        null,
                                        new ICallAction[]{
                                        new Actions.ConferenceSetAutoOutCall(new sDomainExtensionPair(ext.Substring(0,ext.IndexOf("@")),ext.Substring(ext.IndexOf("@")+1)))
                                    },
                                        CallConditionBreakTypes.Never
                                    ));
                            }
                            condAutoCalls.Add(new sCatchAllCondition(
                                new ICallAction[]{
                                new Actions.JoinConference((string)icom[_EXTENSION_FIELD_NAME],true),
                                new Actions.KickFromConference((string)icom[_EXTENSION_FIELD_NAME],"all",true)
                            },
                                null,
                                null
                            ));
                            exts.Add(new sCallExtension("icom_" + (string)icom[_EXTENSION_FIELD_NAME], true, false, condAutoCalls.ToArray()));
                        }
                        ret.Add(new sCallContext(cont, exts.ToArray()));
                    }
                }
                return ret.ToArray();
            }
        }

        protected void DeleteIntercom(string context, string intercomExtension)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList tmp = new ArrayList();
                if (ht.ContainsKey(context))
                    tmp = (ArrayList)ht[context];
                for (int x = 0; x < tmp.Count; x++)
                {
                    Hashtable icom = (Hashtable)tmp[x];
                    if ((string)icom[_EXTENSION_FIELD_NAME] == intercomExtension)
                    {
                        tmp.RemoveAt(x);
                        break;
                    }
                }
                ht.Remove(context);
                if (tmp.Count > 0)
                    ht.Add(context, tmp);
                StoredConfiguration = ht;
            }
        }

        protected void AddIntercom(string context, string intercomExtension, bool oneWay, sDomainExtensionPair[] extensions)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList tmp = new ArrayList();
                if (ht.ContainsKey(context))
                    tmp = (ArrayList)ht[context];
                bool add = true;
                foreach (Hashtable icom in tmp)
                {
                    if ((string)icom[_EXTENSION_FIELD_NAME] == intercomExtension)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    Hashtable icom = new Hashtable();
                    icom.Add(_EXTENSION_FIELD_NAME, intercomExtension);
                    icom.Add(_ONE_WAY_FIELD_NAME, oneWay);
                    ArrayList map = new ArrayList();
                    foreach (sDomainExtensionPair dep in extensions)
                        map.Add(dep.Extension + "@" + dep.Domain);
                    icom.Add(_MAPS_FIELD_NAME, map);
                    tmp.Add(icom);
                }
                ht.Remove(context);
                if (tmp.Count > 0)
                    ht.Add(context, tmp);
                StoredConfiguration = ht; 
            }
        }

        protected void UpdateIntercom(string context, string intercomExtension,string newIntercomExtension, bool oneWay, sDomainExtensionPair[] extensions)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList tmp = new ArrayList();
                if (ht.ContainsKey(context))
                    tmp = (ArrayList)ht[context];
                int index = tmp.Count;
                Hashtable icom;
                for(int x=0;x<tmp.Count;x++)
                {
                    icom = (Hashtable)tmp[x];
                    if ((string)icom[_EXTENSION_FIELD_NAME] == intercomExtension)
                    {
                        index = x;
                        tmp.RemoveAt(x);
                        break;
                    }
                }
                icom = new Hashtable();
                icom.Add(_EXTENSION_FIELD_NAME, newIntercomExtension);
                icom.Add(_ONE_WAY_FIELD_NAME, oneWay);
                ArrayList map = new ArrayList();
                foreach (sDomainExtensionPair dep in extensions)
                    map.Add(dep.Extension + "@" + dep.Domain);
                icom.Add(_MAPS_FIELD_NAME, map);
                tmp.Insert(index, icom);
                ht.Remove(context);
                if (tmp.Count > 0)
                    ht.Add(context, tmp);
                StoredConfiguration = ht;
            }
        }
    }
}
