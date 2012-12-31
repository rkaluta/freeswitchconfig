using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using System.Xml;
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
    public class DirectLinePlan : ADialPlan
    {
        private const string _EXTENSION_FIELD_NAME = "extension";
        private const string _CONTEXT_FIELD_NAME = "context";
        private const string _DIALED_NUMBER_FIELD_NAME = "dialedNumber";

        public DirectLinePlan()
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
            get { return "DirectLines"; }
        }

        public override string Description
        {
            get { return "This dialplan handles all incoming direct lines."; }
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
                    foreach (string str in StoredConfiguration.Keys)
                    {
                        foreach (Hashtable dline in (ArrayList)StoredConfiguration[str])
                            ret.Add(new sCallExtensionReference("direct_" + (string)dline[_DIALED_NUMBER_FIELD_NAME], str));
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
                    "SELECT dl.DialedContext.Name,dl.ExternalNumber,dl.Number,dl.Context.Name FROM DirectLine dl ORDER BY dl.DialedContext.Name");
                cq.Execute();
                string curContext = "";
                ArrayList lines = new ArrayList();
                while (cq.Read())
                {
                    if (curContext != cq[0].ToString())
                    {
                        if (lines.Count > 0)
                            ht.Add(curContext, lines);
                        curContext = cq[0].ToString();
                        lines = new ArrayList();
                    }
                    Hashtable dline = new Hashtable();
                    dline.Add(_DIALED_NUMBER_FIELD_NAME, cq[1].ToString());
                    dline.Add(_EXTENSION_FIELD_NAME, cq[2].ToString());
                    dline.Add(_CONTEXT_FIELD_NAME, cq[3].ToString());
                }
                cq.Close();
                if (lines.Count > 0)
                    ht.Add(curContext, lines);
                StoredConfiguration = ht;
            }
        }

        protected void DeleteDirectLine(string context, string dialedNumber)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                if (ht.ContainsKey(context))
                {
                    ArrayList al = (ArrayList)ht[context];
                    for (int x = 0; x < al.Count; x++)
                    {
                        Hashtable dline = (Hashtable)al[x];
                        if ((string)dline[_DIALED_NUMBER_FIELD_NAME] == dialedNumber)
                        {
                            al.RemoveAt(x);
                            break;
                        }
                    }
                    ht.Remove(context);
                    ht.Add(context, al);
                }
                StoredConfiguration = ht;
            }
        }

        protected void AddDirectLine(string externalContext, string dialedNumber, string internalContext, string extension)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList al = new ArrayList();
                if (ht.ContainsKey(externalContext))
                    al = (ArrayList)ht[externalContext];
                bool add = true;
                foreach (Hashtable dline in al)
                {
                    if ((string)dline[_DIALED_NUMBER_FIELD_NAME] == dialedNumber)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    Hashtable dline = new Hashtable();
                    dline.Add(_DIALED_NUMBER_FIELD_NAME, dialedNumber);
                    dline.Add(_EXTENSION_FIELD_NAME, extension);
                    dline.Add(_CONTEXT_FIELD_NAME, internalContext);
                    al.Add(dline);
                    ht.Remove(externalContext);
                    ht.Add(externalContext, al);
                }
                StoredConfiguration = ht;
            }
        }

        protected void UpdateDirectLine(string externalContext, string originalDialedNumber, string dialedNumber, string internalContext, string extension)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList al = new ArrayList();
                if (ht.ContainsKey(externalContext))
                    al = (ArrayList)ht[externalContext];
                for (int x = 0; x < al.Count; x++)
                {
                    Hashtable dline = (Hashtable)al[x];
                    if ((string)dline[_DIALED_NUMBER_FIELD_NAME] == originalDialedNumber)
                    {
                        dline = new Hashtable();
                        dline.Add(_DIALED_NUMBER_FIELD_NAME, dialedNumber);
                        dline.Add(_EXTENSION_FIELD_NAME, extension);
                        dline.Add(_CONTEXT_FIELD_NAME, internalContext);
                        al.RemoveAt(x);
                        al.Insert(x, dline);
                        break;
                    }
                }
                ht.Remove(externalContext);
                ht.Add(externalContext, al);
                StoredConfiguration = ht;
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
                        foreach (Hashtable dline in (ArrayList)StoredConfiguration[cont])
                        {
                            exts.Add(new sCallExtension("direct_" + (string)dline[_DIALED_NUMBER_FIELD_NAME], false, true,
                                    new ICallCondition[]{
                                    new sCallFieldCondition("destination_number",(string)dline[_DIALED_NUMBER_FIELD_NAME],false,
                                        new ICallAction[]{
                                            new Actions.Transfer((string)dline[_EXTENSION_FIELD_NAME],"XML",(string)dline[_CONTEXT_FIELD_NAME],false)
                                        },
                                        null,
                                        null
                                    )
                                }));
                        }
                        ret.Add(new sCallContext(cont, exts.ToArray()));
                    }
                }
                return ret.ToArray();
            }
        }
    }
}
