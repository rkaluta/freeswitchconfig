using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
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
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data;

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans
{
    public class TimedRoutePlan : ADialPlan
    {
        private const string _NAME_FIELD_ID = "name";
        private const string _CONDITION_FIELD_ID = "condition";
        private const string _PERFORM_ON_FAIL_FIElD_ID = "performOnFail";
        private const string _START_FIELD_ID = "start";
        private const string _END_FIELD_ID = "end";
        private const string _TYPE_FIELD_ID = "type";
        private const string _AUDIO_FILE_FIELD_ID = "audioFile";
        private const string _EXTENSION_FIELD_ID = "extension";
        private const string _GATEWAY_NUMBER_FIELD_ID = "gateway";
        private const string _CALL_EXTENSION_FIELD_ID = "callExtension";

        public override DialPlanPriorities Priority
        {
            get { return DialPlanPriorities.Normal; }
        }

        public override DialPlanPriorities SubPriority
        {
            get { return DialPlanPriorities.High; }
        }

        public override bool Movable
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "TimedRoute"; }
        }

        public override string Description
        {
            get { return "This dialplan handles the timed based routing set in the interface."; }
        }

        public TimedRoutePlan()
        {
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
                        foreach (Hashtable ext in (ArrayList)StoredConfiguration[cont])
                        {
                            List<ICallAction> actions = new List<ICallAction>();
                            switch ((VacationRouteRedirectTypes)ext[_TYPE_FIELD_ID])
                            {
                                case VacationRouteRedirectTypes.TransferToExtension:
                                    actions.Add(new Actions.TransferToCallExtension((sCallExtensionReference)ext[_CALL_EXTENSION_FIELD_ID]));
                                    break;
                                case VacationRouteRedirectTypes.PlayFile:
                                    actions.Add(new Actions.PlayAudioFile((string)ext[_AUDIO_FILE_FIELD_ID], false));
                                    break;
                                case VacationRouteRedirectTypes.PhoneExtension:
                                    actions.Add(new Actions.BridgeToExtension((sDomainExtensionPair)ext[_EXTENSION_FIELD_ID], false));
                                    break;
                                case VacationRouteRedirectTypes.OutGateway:
                                    actions.Add(new Actions.BridgeOutGateway((sGatewayNumberPair)ext[_GATEWAY_NUMBER_FIELD_ID], false));
                                    break;
                            }
                            actions.Add(new Actions.Hangup());
                            exts.Add(new sCallExtension((string)ext[_NAME_FIELD_ID], false, true,
                                new ICallCondition[]{
                                    new sCallTimeOfDayCondition((sCallTime)ext[_START_FIELD_ID],(ext.ContainsKey(_END_FIELD_ID) ? (sCallTime?)ext[_END_FIELD_ID] : (sCallTime?)null),
                                        ((bool)ext[_PERFORM_ON_FAIL_FIElD_ID] ? new ICallAction[0] : actions.ToArray()),
                                        ((bool)ext[_PERFORM_ON_FAIL_FIElD_ID] ? actions.ToArray() : new ICallAction[0]),
                                        null)
                                }));
                        }
                        ret.Add(new sCallContext(cont, exts.ToArray()));
                    }
                }
                return ret.ToArray();
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
                        foreach (Hashtable trp in (ArrayList)StoredConfiguration[cont])
                            ret.Add(new sCallExtensionReference((string)trp[_NAME_FIELD_ID], cont));
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
                    "SELECT tr.RouteContext.Name,tr.Start,tr.End,tr.DestinationCondition,tr.PerformOnFail," +
                    "tr.Type,tr.AudioFileString,tr.GatewayNumber,tr.OutGateway.Name,tr.BridgeExtension.Number,tr.BridgeExtension.Domain.Name, " +
                    "tr.ExtensionReference "+
                    "FROM TimedRoute tr ORDER BY tr.Context.Name");
                cq.Execute();
                string curContext = "";
                ArrayList trs = new ArrayList();
                while (cq.Read())
                {
                    if (curContext != cq[0].ToString())
                    {
                        if (trs.Count > 0)
                            ht.Add(curContext, trs);
                        trs = new ArrayList();
                        curContext = cq[0].ToString();
                    }
                    Hashtable tr = new Hashtable();
                    tr.Add(_START_FIELD_ID, (sCallTime)cq[1].ToString());
                    if (!cq.IsDBNull(2))
                        tr.Add(_END_FIELD_ID, (sCallTime)cq[2].ToString());
                    tr.Add(_CONDITION_FIELD_ID, cq[3].ToString());
                    tr.Add(_PERFORM_ON_FAIL_FIElD_ID, cq.GetBoolean(4));
                    tr.Add(_TYPE_FIELD_ID, (VacationRouteRedirectTypes)cq[5]);
                    switch ((VacationRouteRedirectTypes)cq[5])
                    {
                        case VacationRouteRedirectTypes.PlayFile:
                            tr.Add(_AUDIO_FILE_FIELD_ID, cq[6].ToString());
                            break;
                        case VacationRouteRedirectTypes.OutGateway:
                            tr.Add(_GATEWAY_NUMBER_FIELD_ID, new sGatewayNumberPair(cq[7].ToString(), cq[8].ToString()).ToString());
                            break;
                        case VacationRouteRedirectTypes.PhoneExtension:
                            tr.Add(_EXTENSION_FIELD_ID, new sDomainExtensionPair(cq[9].ToString(), cq[10].ToString()).ToString());
                            break;
                        case VacationRouteRedirectTypes.TransferToExtension:
                            tr.Add(_CALL_EXTENSION_FIELD_ID, cq[11].ToString());
                            break;
                    }
                    trs.Add(tr);
                }
                if (trs.Count > 0)
                    ht.Add(curContext, trs);
                StoredConfiguration = ht;
            }
        }

        public void DeleteTimedRoute(string context, string name)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                if (ht.Contains(context))
                {
                    ArrayList cont = (ArrayList)ht[context];
                    ht.Remove(context);
                    for (int x = 0; x < cont.Count; x++)
                    {
                        Hashtable ext = (Hashtable)cont[x];
                        if (ext[_NAME_FIELD_ID].ToString() == name)
                        {
                            cont.RemoveAt(x);
                            break;
                        }
                    }
                    ht.Add(context, cont);
                }
                StoredConfiguration = ht;
            }
        }

        public void AddTimedRoute(string context, string name, string condition, bool performOnFail, sCallTime start, sCallTime? end,
            string audioFile)
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
                Hashtable pln = new Hashtable();
                pln.Add(_NAME_FIELD_ID, name);
                pln.Add(_CONDITION_FIELD_ID, condition);
                pln.Add(_PERFORM_ON_FAIL_FIElD_ID, performOnFail);
                pln.Add(_START_FIELD_ID, start.ToString());
                if (end.HasValue)
                    pln.Add(_END_FIELD_ID, end.Value.ToString());
                pln.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.PlayFile);
                pln.Add(_AUDIO_FILE_FIELD_ID, audioFile);
                cont.Add(pln);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        public void AddTimedRoute(string context, string name, string condition, bool performOnFail, sCallTime start, sCallTime? end,
            sCallExtensionReference callExtension)
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
                Hashtable pln = new Hashtable();
                pln.Add(_NAME_FIELD_ID, name);
                pln.Add(_CONDITION_FIELD_ID, condition);
                pln.Add(_PERFORM_ON_FAIL_FIElD_ID, performOnFail);
                pln.Add(_START_FIELD_ID, start.ToString());
                if (end.HasValue)
                    pln.Add(_END_FIELD_ID, end.Value.ToString());
                pln.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.TransferToExtension);
                pln.Add(_CALL_EXTENSION_FIELD_ID, callExtension.ToString());
                cont.Add(pln);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        public void AddTimedRoute(string context, string name, string condition, bool performOnFail, sCallTime start, sCallTime? end,
            sGatewayNumberPair gateway)
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
                Hashtable pln = new Hashtable();
                pln.Add(_NAME_FIELD_ID, name);
                pln.Add(_CONDITION_FIELD_ID, condition);
                pln.Add(_PERFORM_ON_FAIL_FIElD_ID, performOnFail);
                pln.Add(_START_FIELD_ID, start.ToString());
                if (end.HasValue)
                    pln.Add(_END_FIELD_ID, end.Value.ToString());
                pln.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.OutGateway);
                pln.Add(_GATEWAY_NUMBER_FIELD_ID, gateway.ToString());
                cont.Add(pln);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        public void AddTimedRoute(string context, string name, string condition, bool performOnFail, sCallTime start, sCallTime? end,
            sDomainExtensionPair extensionNumber)
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
                Hashtable pln = new Hashtable();
                pln.Add(_NAME_FIELD_ID, name);
                pln.Add(_CONDITION_FIELD_ID, condition);
                pln.Add(_PERFORM_ON_FAIL_FIElD_ID, performOnFail);
                pln.Add(_START_FIELD_ID, start.ToString());
                if (end.HasValue)
                    pln.Add(_END_FIELD_ID, end.Value.ToString());
                pln.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.PhoneExtension);
                pln.Add(_EXTENSION_FIELD_ID, extensionNumber.ToString());
                cont.Add(pln);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        public void UpdateTimedRoute(string context, string originalName, string name, string condition, bool performOnFail, sCallTime start, sCallTime? end,
            string audioFile)
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
                    Hashtable ext = (Hashtable)cont[x];
                    if (ext[_NAME_FIELD_ID].ToString() == originalName)
                    {
                        cont.RemoveAt(x);
                        break;
                    }
                }
                ht.Add(context, cont);
                Hashtable pln = new Hashtable();
                pln.Add(_NAME_FIELD_ID, name);
                pln.Add(_CONDITION_FIELD_ID, condition);
                pln.Add(_PERFORM_ON_FAIL_FIElD_ID, performOnFail);
                pln.Add(_START_FIELD_ID, start.ToString());
                if (end.HasValue)
                    pln.Add(_END_FIELD_ID, end.Value.ToString());
                pln.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.PlayFile);
                pln.Add(_AUDIO_FILE_FIELD_ID, audioFile);
                cont.Add(pln);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        public void UpdateTimedRoute(string context, string originalName, string name, string condition, bool performOnFail, sCallTime start, sCallTime? end,
            sCallExtensionReference callExtension)
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
                    Hashtable ext = (Hashtable)cont[x];
                    if (ext[_NAME_FIELD_ID].ToString() == originalName)
                    {
                        cont.RemoveAt(x);
                        break;
                    }
                }
                Hashtable pln = new Hashtable();
                pln.Add(_NAME_FIELD_ID, name);
                pln.Add(_CONDITION_FIELD_ID, condition);
                pln.Add(_PERFORM_ON_FAIL_FIElD_ID, performOnFail);
                pln.Add(_START_FIELD_ID, start.ToString());
                if (end.HasValue)
                    pln.Add(_END_FIELD_ID, end.Value.ToString());
                pln.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.TransferToExtension);
                pln.Add(_CALL_EXTENSION_FIELD_ID, callExtension.ToString());
                cont.Add(pln);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        public void UpdateTimedRoute(string context,string originalName, string name, string condition, bool performOnFail, sCallTime start, sCallTime? end,
            sGatewayNumberPair gateway)
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
                    Hashtable ext = (Hashtable)cont[x];
                    if (ext[_NAME_FIELD_ID].ToString() == originalName)
                    {
                        cont.RemoveAt(x);
                        break;
                    }
                }
                Hashtable pln = new Hashtable();
                pln.Add(_NAME_FIELD_ID, name);
                pln.Add(_CONDITION_FIELD_ID, condition);
                pln.Add(_PERFORM_ON_FAIL_FIElD_ID, performOnFail);
                pln.Add(_START_FIELD_ID, start.ToString());
                if (end.HasValue)
                    pln.Add(_END_FIELD_ID, end.Value.ToString());
                pln.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.OutGateway);
                pln.Add(_GATEWAY_NUMBER_FIELD_ID, gateway.ToString());
                cont.Add(pln);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        public void UpdateTimedRoute(string context,string originalName, string name, string condition, bool performOnFail, sCallTime start, sCallTime? end,
            sDomainExtensionPair extensionNumber)
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
                    Hashtable ext = (Hashtable)cont[x];
                    if (ext[_NAME_FIELD_ID].ToString() == originalName)
                    {
                        cont.RemoveAt(x);
                        break;
                    }
                }
                Hashtable pln = new Hashtable();
                pln.Add(_NAME_FIELD_ID, name);
                pln.Add(_CONDITION_FIELD_ID,condition);
                pln.Add(_PERFORM_ON_FAIL_FIElD_ID, performOnFail);
                pln.Add(_START_FIELD_ID, start.ToString());
                if (end.HasValue)
                    pln.Add(_END_FIELD_ID, end.Value.ToString());
                pln.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.PhoneExtension);
                pln.Add(_EXTENSION_FIELD_ID, extensionNumber.ToString());
                cont.Add(pln);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }
    }
}
