using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces;
using System.Collections;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.DialPlans
{
    public class GatewayRoutePlan : ADialPlan
    {
        private const string _GATEWAY_NAME_FIELD_ID = "gateway";
        private const string _NPANXX_FIELD_ID = "npanxx";
        private const string _ROUTE_ID_FIELD = "id";

        public GatewayRoutePlan()
        {
        }

        public override DialPlanPriorities Priority
        {
            get { return DialPlanPriorities.Low; }
        }

        public override DialPlanPriorities SubPriority
        {
            get { return DialPlanPriorities.Low; }
        }

        public override bool Movable
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "Gateway Routing"; }
        }

        public override string Description
        {
            get { return "This dialplan handles routing of a call out specific gateways depending on the rules defined."; }
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
                        foreach (Hashtable gway in (ArrayList)StoredConfiguration[cont])
                            ret.Add(new sCallExtensionReference((string)gway[_GATEWAY_NAME_FIELD_ID] + "_" + gway[_ROUTE_ID_FIELD].ToString(), cont));
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
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks",
                        "SELECT DISTINCT gr.RouteContext.Name,gr.OutGateway.Name,gr.DestinationCondition,gr.ID FROM GatewayRoute gr ORDER BY gr.Context.Name,gr.Index");
                cq.Execute();
                string curContext = "";
                ArrayList gways = new ArrayList();
                while (cq.Read())
                {
                    if (curContext != cq[0].ToString())
                    {
                        if (gways.Count > 0)
                            ht.Add(curContext, gways);
                        gways = new ArrayList();
                        curContext = cq[0].ToString();
                    }
                    Hashtable gw = new Hashtable();
                    gw.Add(_GATEWAY_NAME_FIELD_ID, cq[1].ToString());
                    gw.Add(_NPANXX_FIELD_ID, cq[2].ToString());
                    gw.Add(_ROUTE_ID_FIELD, cq.GetInt32(3));
                    gways.Add(gw);
                }
                cq.Close();
                if (gways.Count > 0)
                    ht.Add(curContext, gways);
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
                    Hashtable ht = StoredConfiguration;
                    foreach (string cont in ht.Keys)
                    {
                        List<sCallExtension> exts = new List<sCallExtension>();
                        foreach (Hashtable gway in (ArrayList)ht[cont])
                        {
                            exts.Add(new sCallExtension((string)gway[_GATEWAY_NAME_FIELD_ID] + "_" + gway[_ROUTE_ID_FIELD].ToString(),
                                true,
                                false,
                                new ICallCondition[]{
                                    new sCallFieldCondition("destination_number",
                                        new NPANXXValue((string)gway[_NPANXX_FIELD_ID]).ToRegexString(),
                                        true,
                                        new ICallAction[]{
                                            new Actions.BridgeOutGateway(
                                                (string)gway[_GATEWAY_NAME_FIELD_ID],
                                                (((string)gway[_NPANXX_FIELD_ID]).Contains("|")||((string)gway[_NPANXX_FIELD_ID]).Contains("\n") ? "$" : "${destination_number}")
                                                ,false)
                                        },
                                        null,
                                        null)
                                }));
                        }
                        ret.Add(new sCallContext(cont, exts.ToArray()));
                    }
                }
                return ret.ToArray();
            }
        }

        protected void DeleteGatewayRoute(string context, int id)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                if (ht.ContainsKey(context))
                {
                    ArrayList gways = (ArrayList)ht[context];
                    for (int x = 0; x < gways.Count; x++)
                    {
                        Hashtable gway = (Hashtable)gways[x];
                        if ((int)gway[_ROUTE_ID_FIELD] == id)
                        {
                            gways.RemoveAt(x);
                            break;
                        }
                    }
                    ht.Remove(context);
                    ht.Add(context, gways);
                }
                StoredConfiguration = ht;
            }
        }

        protected void AddGatewayRoute(string context, string gateway, string condition, int index, int id)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList cont = new ArrayList();
                if (ht.ContainsKey(context))
                    cont = (ArrayList)ht[context];
                Hashtable gway = new Hashtable();
                gway.Add(_GATEWAY_NAME_FIELD_ID, gateway);
                gway.Add(_NPANXX_FIELD_ID, condition);
                gway.Add(_ROUTE_ID_FIELD, id);
                cont.Insert(index, gway);
                ht.Remove(context);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateGatewayRoute(string context, int id, string gateway, string condition)
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
                    Hashtable gw = (Hashtable)cont[x];
                    if ((int)gw[_ROUTE_ID_FIELD] == id)
                    {
                        index = x;
                        cont.RemoveAt(x);
                        break;
                    }
                }
                Hashtable gway = new Hashtable();
                gway.Add(_GATEWAY_NAME_FIELD_ID, gateway);
                gway.Add(_NPANXX_FIELD_ID, condition);
                gway.Add(_ROUTE_ID_FIELD, id);
                cont.Insert(index, gway);
                ht.Remove(context);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateRouteIndex(string context, int id, int index)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                ArrayList cont = new ArrayList();
                if (ht.ContainsKey(context))
                {
                    cont = (ArrayList)ht[context];
                    ht.Remove(context);
                }
                for (int x = 0; x < cont.Count; x++)
                {
                    Hashtable gway = (Hashtable)cont[x];
                    if ((int)gway[_ROUTE_ID_FIELD] == id)
                    {
                        cont.RemoveAt(x);
                        cont.Insert(index, gway);
                    }
                }
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }
    }
}
