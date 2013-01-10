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

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans
{
    public class HuntGroupPlan : ADialPlan
    {
        private const string _EXTENSION_FIELD_ID = "extension";
        private const string _SEQUENTIAL_FIELD_ID = "sequential";
        private const string _EXTENSIONS_FIELD_ID = "extensions";

        public HuntGroupPlan()
        {
        }

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
            get { return "Hunt Group"; }
        }

        public override string Description
        {
            get { return "This dialplan handles processing of hunt groups and bridging the call to the hunt group by the specified extension."; }
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

        public override sCallContext[] CallContexts
        {
            get {
                List<sCallContext> ret = new List<sCallContext>();
                lock (_lock)
                {
                    foreach (string str in StoredConfiguration.Keys)
                    {
                        List<sCallExtension> exts = new List<sCallExtension>();
                        foreach (Hashtable hgroup in (ArrayList)StoredConfiguration[str])
                        {
                            List<sDomainExtensionPair> exs =new List<sDomainExtensionPair>();
                            foreach (string ext in (ArrayList)hgroup[_EXTENSIONS_FIELD_ID])
                                exs.Add(new sDomainExtensionPair(ext.Substring(0,ext.IndexOf("@")),ext.Substring(ext.IndexOf("@")+1)));
                            exts.Add(new sCallExtension("hunt_group_" + (string)hgroup[_EXTENSION_FIELD_ID],
                                false,
                                false,
                                new ICallCondition[]{
                                    new sCallFieldCondition("destination_number",(string)hgroup[_EXTENSION_FIELD_ID],
                                        false,
                                        new ICallAction[]{
                                            new Actions.BridgeToMultipleExtensions(exs.ToArray(),(bool)hgroup[_SEQUENTIAL_FIELD_ID],true)
                                        },
                                        null,
                                        null)
                                }));
                        }
                        ret.Add(new sCallContext(str,exts.ToArray()));
                    }
                }
                return ret.ToArray();
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
                        foreach (Hashtable hgroup in (ArrayList)StoredConfiguration[cont])
                            ret.Add(new sCallExtensionReference("hunt_group_" + (string)hgroup[_EXTENSION_FIELD_ID], cont));
                    }
                }
                return ret.ToArray();
            }
        }

        public override void LoadInitialConfiguration()
        {
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents",
                "SELECT hg.Number,hg.RingSequential,hg.Context.Name FROM HuntGroup hg ORDER BY hg.Context.Name");
            ClassQuery cqExts = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents",
                "SELECT hg.Extensions.Number,hg.Extensions.Domain.Name FROM HuntGroup hg WHERE hg.Number = @extNumber");
            lock (_lock)
            {
                Hashtable hgroups = new Hashtable();
                cq.Execute();
                string curContext = "";
                ArrayList groups = new ArrayList();
                while (cq.Read())
                {
                    if (curContext != cq[2].ToString())
                    {
                        if (groups.Count > 0)
                        {
                            hgroups.Add(curContext, groups);
                            groups = new ArrayList();
                            curContext = cq[2].ToString();
                        }
                    }
                    cqExts.Execute(new IDbDataParameter[]{
                        cqExts.CreateParameter("@extNumber",cq[0].ToString())
                    });
                    ArrayList exts = new ArrayList();
                    while (cqExts.Read())
                    {
                        exts.Add(cqExts[0].ToString()+"@"+ cqExts[1].ToString());
                    }
                    cqExts.Close();
                    Hashtable hgroup = new Hashtable();
                    hgroup.Add(_EXTENSION_FIELD_ID, cq[0].ToString());
                    hgroup.Add(_EXTENSIONS_FIELD_ID, exts);
                    hgroup.Add(_SEQUENTIAL_FIELD_ID, cq.GetBoolean(1));
                    groups.Add(hgroup);
                }
                cq.Close();
                if (groups.Count > 0)
                    hgroups.Add(curContext, groups);
            }
        }

        protected void DeleteHuntGroup(string context, string extension)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                if (ht.ContainsKey(context))
                {
                    ArrayList cont = (ArrayList)ht[context];
                    for (int x = 0; x < cont.Count; x++)
                    {
                        Hashtable hgroup = (Hashtable)cont[x];
                        if (hgroup[_EXTENSION_FIELD_ID].ToString() == extension)
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

        protected void AddHuntGroup(string context, string extension, bool sequential, sDomainExtensionPair[] extensions)
        {
            lock(_lock){
                Hashtable ht = StoredConfiguration;
                ArrayList cont = new ArrayList();
                if (ht.ContainsKey(context))
                    cont = (ArrayList)ht[context];
                bool add = true;
                foreach (Hashtable hgroup in cont)
                {
                    if (hgroup[_EXTENSION_FIELD_ID].ToString() == extension)
                    {
                        add = false;
                        break;
                    }
                }
                if (!add)
                    throw new Exception("Unable to add Hunt Group, 1 already exists in the context[" + context + "] for the number[" + extension + "]");
                Hashtable hgrp = new Hashtable();
                hgrp.Add(_EXTENSION_FIELD_ID, extension);
                hgrp.Add(_SEQUENTIAL_FIELD_ID, sequential);
                ArrayList exts = new ArrayList();
                foreach (sDomainExtensionPair ext in extensions)
                    exts.Add(ext.Extension + "@" + ext.Domain);
                hgrp.Add(_EXTENSIONS_FIELD_ID, exts);
                cont.Add(hgrp);
                ht.Remove(context);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateHuntGroup(string context, string extension,string newExtension, bool sequential, sDomainExtensionPair[] extensions)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList cont = new ArrayList();
                if (ht.ContainsKey(context))
                    cont = (ArrayList)ht[context];
                bool add = true;
                int index = cont.Count;
                for(int x=0;x<cont.Count;x++)
                {
                    Hashtable hgroup = (Hashtable)cont[x];
                    if (hgroup[_EXTENSION_FIELD_ID].ToString() == extension)
                    {
                        cont.RemoveAt(x);
                        index = x;
                        break;
                    }
                }
                if (!add)
                    throw new Exception("Unable to add Hunt Group, 1 already exists in the context[" + context + "] for the number[" + extension + "]");
                Hashtable hgrp = new Hashtable();
                hgrp.Add(_EXTENSION_FIELD_ID, newExtension);
                hgrp.Add(_SEQUENTIAL_FIELD_ID, sequential);
                ArrayList exts = new ArrayList();
                foreach (sDomainExtensionPair ext in extensions)
                    exts.Add(ext.Extension + "@" + ext.Domain);
                hgrp.Add(_EXTENSIONS_FIELD_ID, exts);
                cont.Insert(index, hgrp);
                ht.Remove(context);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }
    }
}
