using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files;
using System.Collections;
using Procurios.Public;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data;

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans
{
    public class VacationRoutePlan : ADialPlan
    {
        private const string _END_WITH_VM_FIELD_ID = "endWithVoicemail";
        private const string _START_DATE_FIELD_ID = "startDate";
        private const string _END_DATE_FIELD_ID = "endDate";
        private const string _TYPE_FIELD_ID = "type";
        private const string _AUDIO_FILE_FIELD_ID = "audioFile";
        private const string _BRIDGE_EXTENSION_FIELD_ID = "bridgeExtension";
        private const string _GATEWAY_NUMBER_FIELD_ID = "gateway";
        private const string _CALL_EXTENSION_FIELD_ID = "callExtension";
        private const string _VOICEMAIL_TIMEOUT_FIELD_ID = "timeout";
        private const string _OWNING_EXTENSION_FIELD_ID = "owning_extension";

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
            get {return true; }
        }

        public override string Name
        {
            get { return "VacationRoute"; }
        }

        public override string Description
        {
            get { return "This plan handles the routing of calls based on vacation route rules supplied."; }
        }

        public VacationRoutePlan()
        {
        }

        public override sCallExtensionReference[] DefinedCallExtensions
        {
            get {
                List<sCallExtensionReference> ret = new List<sCallExtensionReference>();
                lock (_lock)
                {
                    foreach (string cont in StoredConfiguration.Keys)
                    {
                        foreach (Hashtable vcr in (ArrayList)StoredConfiguration[cont])
                            ret.Add(new sCallExtensionReference("Vacation_Route_"+((sDomainExtensionPair)vcr[_OWNING_EXTENSION_FIELD_ID]).Extension+"_"+((DateTime)vcr[_START_DATE_FIELD_ID]).ToString("YYYYMMddmmss"),cont));
                    }
                }
                return ret.ToArray();
            }
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
                    foreach (string cont in StoredConfiguration.Keys)
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
                                    if ((bool)ext[_END_WITH_VM_FIELD_ID])
                                        actions.Add(new Actions.Set("call_timeout", ext[_VOICEMAIL_TIMEOUT_FIELD_ID].ToString()));
                                    actions.Add(new Actions.BridgeToExtension((sDomainExtensionPair)ext[_BRIDGE_EXTENSION_FIELD_ID], false));
                                    break;
                                case VacationRouteRedirectTypes.OutGateway:
                                    if ((bool)ext[_END_WITH_VM_FIELD_ID])
                                        actions.Add(new Actions.Set("call_timeout", ext[_VOICEMAIL_TIMEOUT_FIELD_ID].ToString()));
                                    actions.Add(new Actions.BridgeOutGateway((sGatewayNumberPair)ext[_GATEWAY_NUMBER_FIELD_ID], false));
                                    break;
                            }
                            if (!(bool)ext[_END_WITH_VM_FIELD_ID])
                                actions.Add(new Actions.Hangup());
                            else
                            {
                                actions.Add(new Actions.Answer());
                                actions.Add(new Actions.Sleep(1000));
                                actions.Add(new Actions.Voicemail(cont,(sDomainExtensionPair)ext[_OWNING_EXTENSION_FIELD_ID]));
                            }
                            exts.Add(new sCallExtension("Vacation_Route_" + ((sDomainExtensionPair)ext[_OWNING_EXTENSION_FIELD_ID]).Extension + "_" + ((DateTime)ext[_START_DATE_FIELD_ID]).ToString("YYYYMMddmmss"), false, true,
                                new ICallCondition[]{
                                        new sCallFieldCondition("destination_number",((sDomainExtensionPair)ext[_OWNING_EXTENSION_FIELD_ID]).Extension,false,null,null,null),
                                        new sCallTimeOfDayCondition(new sCallTimeDateRange((DateTime)ext[_START_DATE_FIELD_ID],(DateTime)ext[_END_DATE_FIELD_ID],true),
                                            actions.ToArray(),
                                            null,
                                            CallConditionBreakTypes.OnFalse)
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
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents",
                    "SELECT vr.AttachedExtension.Context.Name, vr.AttachedExtension.Domain.Name, vr.AttachedExtension.Number, " +
                    "vr.Type,vr.AttachedExtension.Domain.VoicemailTimeout, vr.EndWithVoicemail, vr.StartDate, " +
                    "vr.EndDate, vr.BridgeExtension.Number, vr.BridgeExtension.Domain.Name, " +
                    "vr.GatewayNumber, vr.OutGateway.Name, vr.AudioFileString, vr.ExtensionReference " +
                " FROM VacationRoute vr " +
                " ORDER BY vr.AttachedExtension.Context.Name");
                cq.Execute();
                string curContext = "";
                ArrayList vcrs = new ArrayList();
                while (cq.Read())
                {
                    if (curContext != cq[0].ToString())
                    {
                        if (vcrs.Count > 0)
                            ht.Add(curContext, vcrs);
                        vcrs = new ArrayList();
                        curContext = cq[0].ToString();
                    }
                    Hashtable vcr = new Hashtable();
                    vcr.Add(_OWNING_EXTENSION_FIELD_ID, new sDomainExtensionPair(cq[2].ToString(), cq[1].ToString()).ToString());
                    vcr.Add(_TYPE_FIELD_ID, (VacationRouteRedirectTypes)cq[3]);
                    vcr.Add(_VOICEMAIL_TIMEOUT_FIELD_ID, cq.GetInt32(4));
                    vcr.Add(_END_WITH_VM_FIELD_ID, cq.GetBoolean(5));
                    vcr.Add(_START_DATE_FIELD_ID, cq.GetDateTime(6));
                    vcr.Add(_END_DATE_FIELD_ID, cq.GetDateTime(7));
                    switch ((VacationRouteRedirectTypes)cq[3])
                    {
                        case VacationRouteRedirectTypes.PhoneExtension:
                            vcr.Add(_BRIDGE_EXTENSION_FIELD_ID, new sDomainExtensionPair(cq[8].ToString(), cq[9].ToString()).ToString());
                            break;
                        case VacationRouteRedirectTypes.OutGateway:
                            vcr.Add(_GATEWAY_NUMBER_FIELD_ID, new sGatewayNumberPair(cq[10].ToString(), cq[11].ToString()).ToString());
                            break;
                        case VacationRouteRedirectTypes.PlayFile:
                            vcr.Add(_AUDIO_FILE_FIELD_ID, cq[12].ToString());
                            break;
                        case VacationRouteRedirectTypes.TransferToExtension:
                            vcr.Add(_CALL_EXTENSION_FIELD_ID, cq[13].ToString());
                            break;
                    }
                    vcrs.Add(vcr);
                }
                if (vcrs.Count > 0)
                    ht.Add(curContext, vcrs);
                StoredConfiguration = ht;
            }
        }

        protected void AddVacationRoute(string context, string extension, DateTime startDate, DateTime endDate, bool endWithVoicemail, sDomainExtensionPair bridgeExtension)
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
                Hashtable vcr = new Hashtable();
                vcr.Add(_OWNING_EXTENSION_FIELD_ID, extension);
                vcr.Add(_START_DATE_FIELD_ID, startDate);
                vcr.Add(_END_DATE_FIELD_ID, endDate);
                vcr.Add(_END_WITH_VM_FIELD_ID, endWithVoicemail);
                vcr.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.PhoneExtension);
                vcr.Add(_BRIDGE_EXTENSION_FIELD_ID, extension);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void AddVacationRoute(string context, string extension, DateTime startDate, DateTime endDate, bool endWithVoicemail, sCallExtensionReference callExtension)
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
                Hashtable vcr = new Hashtable();
                vcr.Add(_OWNING_EXTENSION_FIELD_ID, extension);
                vcr.Add(_START_DATE_FIELD_ID, startDate);
                vcr.Add(_END_DATE_FIELD_ID, endDate);
                vcr.Add(_END_WITH_VM_FIELD_ID, endWithVoicemail);
                vcr.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.TransferToExtension);
                vcr.Add(_CALL_EXTENSION_FIELD_ID, extension);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void AddVacationRoute(string context, string extension, DateTime startDate, DateTime endDate, bool endWithVoicemail, sGatewayNumberPair outGateway)
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
                Hashtable vcr = new Hashtable();
                vcr.Add(_OWNING_EXTENSION_FIELD_ID, extension);
                vcr.Add(_START_DATE_FIELD_ID, startDate);
                vcr.Add(_END_DATE_FIELD_ID, endDate);
                vcr.Add(_END_WITH_VM_FIELD_ID, endWithVoicemail);
                vcr.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.OutGateway);
                vcr.Add(_GATEWAY_NUMBER_FIELD_ID,outGateway);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void AddVacationRoute(string context, string extension, DateTime startDate, DateTime endDate, bool endWithVoicemail, File audioFile)
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
                Hashtable vcr = new Hashtable();
                vcr.Add(_OWNING_EXTENSION_FIELD_ID, extension);
                vcr.Add(_START_DATE_FIELD_ID, startDate);
                vcr.Add(_END_DATE_FIELD_ID, endDate);
                vcr.Add(_END_WITH_VM_FIELD_ID, endWithVoicemail);
                vcr.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.PlayFile);
                vcr.Add(_AUDIO_FILE_FIELD_ID,audioFile.ToString());
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void DeleteVacationRoute(string context, string extension,DateTime startDate)
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
                    Hashtable vcr = (Hashtable)cont[x];
                    if (((string)vcr[_OWNING_EXTENSION_FIELD_ID] == extension)
                        && (((DateTime)vcr[_OWNING_EXTENSION_FIELD_ID]).Ticks== startDate.Ticks)){
                            cont.RemoveAt(x);
                            break;
                    }
                }
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateVacationRoute(string context, string extension,DateTime originalStartDate, DateTime startDate, DateTime endDate, bool endWithVoicemail, sDomainExtensionPair bridgeExtension)
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
                Hashtable vcr;
                for (int x = 0; x < cont.Count; x++)
                {
                    vcr = (Hashtable)cont[x];
                    if (((string)vcr[_OWNING_EXTENSION_FIELD_ID] == extension)
                        && (((DateTime)vcr[_OWNING_EXTENSION_FIELD_ID]).Ticks == originalStartDate.Ticks))
                    {
                        cont.RemoveAt(x);
                        break;
                    }
                }
                vcr = new Hashtable();
                vcr.Add(_OWNING_EXTENSION_FIELD_ID, extension);
                vcr.Add(_START_DATE_FIELD_ID, startDate);
                vcr.Add(_END_DATE_FIELD_ID, endDate);
                vcr.Add(_END_WITH_VM_FIELD_ID, endWithVoicemail);
                vcr.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.PhoneExtension);
                vcr.Add(_BRIDGE_EXTENSION_FIELD_ID, extension);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateVacationRoute(string context, string extension, DateTime originalStartDate, DateTime startDate, DateTime endDate, bool endWithVoicemail, sCallExtensionReference callExtension)
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
                Hashtable vcr;
                for (int x = 0; x < cont.Count; x++)
                {
                    vcr = (Hashtable)cont[x];
                    if (((string)vcr[_OWNING_EXTENSION_FIELD_ID] == extension)
                        && (((DateTime)vcr[_OWNING_EXTENSION_FIELD_ID]).Ticks == originalStartDate.Ticks))
                    {
                        cont.RemoveAt(x);
                        break;
                    }
                }
                vcr = new Hashtable();
                vcr.Add(_OWNING_EXTENSION_FIELD_ID, extension);
                vcr.Add(_START_DATE_FIELD_ID, startDate);
                vcr.Add(_END_DATE_FIELD_ID, endDate);
                vcr.Add(_END_WITH_VM_FIELD_ID, endWithVoicemail);
                vcr.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.TransferToExtension);
                vcr.Add(_CALL_EXTENSION_FIELD_ID, extension);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateVacationRoute(string context, string extension, DateTime originalStartDate, DateTime startDate, DateTime endDate, bool endWithVoicemail, sGatewayNumberPair outGateway)
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
                Hashtable vcr;
                for (int x = 0; x < cont.Count; x++)
                {
                    vcr = (Hashtable)cont[x];
                    if (((string)vcr[_OWNING_EXTENSION_FIELD_ID] == extension)
                        && (((DateTime)vcr[_OWNING_EXTENSION_FIELD_ID]).Ticks == originalStartDate.Ticks))
                    {
                        cont.RemoveAt(x);
                        break;
                    }
                }
                vcr = new Hashtable();
                vcr.Add(_OWNING_EXTENSION_FIELD_ID, extension);
                vcr.Add(_START_DATE_FIELD_ID, startDate);
                vcr.Add(_END_DATE_FIELD_ID, endDate);
                vcr.Add(_END_WITH_VM_FIELD_ID, endWithVoicemail);
                vcr.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.OutGateway);
                vcr.Add(_GATEWAY_NUMBER_FIELD_ID, outGateway);
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateVacationRoute(string context, string extension, DateTime originalStartDate, DateTime startDate, DateTime endDate, bool endWithVoicemail, File audioFile)
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
                Hashtable vcr;
                for (int x = 0; x < cont.Count; x++)
                {
                    vcr = (Hashtable)cont[x];
                    if (((string)vcr[_OWNING_EXTENSION_FIELD_ID] == extension)
                        && (((DateTime)vcr[_OWNING_EXTENSION_FIELD_ID]).Ticks == originalStartDate.Ticks))
                    {
                        cont.RemoveAt(x);
                        break;
                    }
                }
                vcr = new Hashtable();
                vcr.Add(_OWNING_EXTENSION_FIELD_ID, extension);
                vcr.Add(_START_DATE_FIELD_ID, startDate);
                vcr.Add(_END_DATE_FIELD_ID, endDate);
                vcr.Add(_END_WITH_VM_FIELD_ID, endWithVoicemail);
                vcr.Add(_TYPE_FIELD_ID, VacationRouteRedirectTypes.PlayFile);
                vcr.Add(_AUDIO_FILE_FIELD_ID, audioFile.ToString());
                ht.Add(context, cont);
                StoredConfiguration = ht;
            }
        }
    }
}
