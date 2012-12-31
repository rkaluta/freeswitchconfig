using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using System.IO;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using System.Threading;
using System.Collections;
using Procurios.Public;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans
{
    public class PinnedRoutePlan : ADialPlan
    {
        public struct sAdvancedPin
        {
            private string _extension;
            public string Extension
            {
                get { return _extension; }
            }

            private string _domain;
            public string Domain
            {
                get { return _domain; }
            }

            private string _pin;
            public string Pin
            {
                get { return _pin; }
            }

            public sAdvancedPin(string extension,string domain, string pin)
            {
                _extension = extension;
                _domain = domain;
                _pin = pin;
            }

            public override string ToString()
            {
                return _extension + "@" + Domain + ":" + _pin;
            }
        }

        private const string _NPA_FIELD_ID = "npa";
        private const string _PIN_SET_NAME_FIELD_ID = "pinset";
        private const string _NAME_FIELD_ID = "name";

        private const string _PINSET_ADVANCED_FIELD_ID = "advanced";
        private const string _PINSET_PINS_FIELD_ID = "pins";

        private const string _SCONFIG_ROUTES_NAME = "Routes";
        private const string _SCONFIG_PINS_NAME = "Pins";

        public override DialPlanPriorities Priority
        {
            get { return DialPlanPriorities.First; }
        }

        public override DialPlanPriorities SubPriority
        {
            get { return DialPlanPriorities.First; }
        }

        public override bool Movable
        {
            get {return false; }
        }

        public override string Name
        {
            get { return "Pinned Route"; }
        }

        public override string Description
        {
            get { return "This dialplan handles the dialed routes that were secured using pins."; }
        }

        private const string PIN_ASK_PATH = "conference/8000/conf-pin.wav";
        private const string PIN_INVALID_PATH = "conference/8000/conf-bad-pin.wav";
        private const string PIN_FAILED_PATH = "conference/8000/conf-goodbye.wav";
        private const string INPUTTED_PIN_VARIABLE_NAME = "inputted_validation_pin";
        private const string PIN_TRY_COUNT_VARIABLE_NAME = "pin_attempts";
        private const string PIN_VALID_VARIABLE_NAME = "is_pin_valid";
        private const string ORIGINAL_DESTINATION_VARIABLE_NAME = "original_destination_number";
        private const string PIN_VALIDATION_NAME_FORMAT = "pin_validation_{0}";

        public PinnedRoutePlan(){
        }

        public override List<string> ContextsUsed
        {
            get {
                List<string> ret = new List<string>();
                lock (_lock)
                {
                    foreach (string cont in ((Hashtable)StoredConfiguration[_SCONFIG_ROUTES_NAME]).Keys)
                        ret.Add(cont);
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
                    Hashtable ht = (Hashtable)StoredConfiguration[_SCONFIG_ROUTES_NAME];
                    foreach (string cont in ht.Keys)
                    {
                        List<string> pins = new List<string>();
                        foreach (Hashtable pr in (ArrayList)ht[cont])
                        {
                            ret.Add(new sCallExtensionReference("pinned_route_" + (string)pr[_NAME_FIELD_ID], cont));
                            if (!pins.Contains((string)pr[_PIN_SET_NAME_FIELD_ID]))
                                pins.Add((string)pr[_PIN_SET_NAME_FIELD_ID]);
                        }
                        foreach (string pin in pins)
                            ret.Add(new sCallExtensionReference(string.Format(PIN_VALIDATION_NAME_FORMAT, pin), cont));
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
                    Hashtable routes = (Hashtable)StoredConfiguration[_SCONFIG_ROUTES_NAME];
                    Hashtable pins = (Hashtable)StoredConfiguration[_SCONFIG_PINS_NAME];
                    foreach (string cont in routes.Keys)
                    {
                        List<string> pinSets = new List<string>();
                        List<sCallExtension> exts = new List<sCallExtension>();
                        foreach (Hashtable pr in (ArrayList)routes[cont])
                        {
                            if (!pinSets.Contains((string)pr[_PIN_SET_NAME_FIELD_ID]))
                                pinSets.Add((string)pr[_PIN_SET_NAME_FIELD_ID]);
                            exts.Add(new sCallExtension("pinned_route_" + (string)pr[_NAME_FIELD_ID], true, true,
                                new ICallCondition[]{
                                    new sCallFieldCondition("destination_number",((NPANXXValue)pr[_NPA_FIELD_ID]).ToRegexString(),true,null,null,null),
                                    new sCallFieldCondition("${"+PIN_VALID_VARIABLE_NAME+"}", "true", true,
                                    new ICallAction[]{
                                        new Actions.Log(LogLevels.Info,"pin is valid for route "+(string)pr[_NAME_FIELD_ID])
                                    },
                                    new ICallAction[]{
                                        new Actions.Set(ORIGINAL_DESTINATION_VARIABLE_NAME,"${destination_number}"),
                                        new Actions.Answer(),
                                        new Actions.Log(LogLevels.Info,"Attempting to obtain pin"),
                                        new Actions.PlayAndGetDigits(4, 10, 3, 3000, "#", PIN_ASK_PATH, null,INPUTTED_PIN_VARIABLE_NAME, "\\d+", null),
                                        new Actions.Set(PIN_TRY_COUNT_VARIABLE_NAME,"${expr(${"+PIN_TRY_COUNT_VARIABLE_NAME+"}+1)}"),
                                        new Actions.Transfer(string.Format(PIN_VALIDATION_NAME_FORMAT,(string)pr[_PIN_SET_NAME_FIELD_ID]),"XML","${context}",false)
                                    },
                                    null)
                                }
                                ));
                        }
                        foreach (string str in pinSets)
                        {
                            List<ICallCondition> conds = new List<ICallCondition>();
                            conds.Add(new sCallFieldCondition("destination_number", string.Format(PIN_VALIDATION_NAME_FORMAT, str), false, null, null, null));
                            Hashtable pset = (Hashtable)((Hashtable)pins[cont])[str];
                            if ((bool)pset[_PINSET_ADVANCED_FIELD_ID])
                                conds.Add(new sCallFieldCondition("${accountcode}@$${domain}:${" + INPUTTED_PIN_VARIABLE_NAME + "}", _PinSetToRegex((ArrayList)pset[_PINSET_PINS_FIELD_ID]), true,
                                    new ICallAction[]{
                                        new Actions.Set(PIN_VALID_VARIABLE_NAME,"true"),
                                        new Actions.Transfer("${"+ORIGINAL_DESTINATION_VARIABLE_NAME+"}","XML","${context}",false)
                                    },
                                    null, CallConditionBreakTypes.OnTrue));
                            else
                                conds.Add(new sCallFieldCondition("${" + INPUTTED_PIN_VARIABLE_NAME + "}", _PinSetToRegex((ArrayList)pset[_PINSET_PINS_FIELD_ID]), true,
                                    new ICallAction[]{
                                        new Actions.Set(PIN_VALID_VARIABLE_NAME,"true"),
                                        new Actions.Transfer("${"+ORIGINAL_DESTINATION_VARIABLE_NAME+"}","XML","${context}",false)
                                    },
                                    null, CallConditionBreakTypes.OnTrue));
                            conds.Add(new sCallFieldCondition("${cond(${" + PIN_TRY_COUNT_VARIABLE_NAME + "}>2 ? YES : NO)}", "YES", false,
                                new ICallAction[]{
                                    new Actions.PlayAudioFile(PIN_FAILED_PATH,true),
                                    new Actions.Hangup()
                                },
                                new ICallAction[]{
                                    new Actions.PlayAudioFile(PIN_INVALID_PATH,true),
                                    new Actions.Transfer("${"+ORIGINAL_DESTINATION_VARIABLE_NAME+"}","XML","${context}",false)
                                },
                                null));
                            exts.Add(new sCallExtension(string.Format(PIN_VALIDATION_NAME_FORMAT, str), true, true, conds.ToArray()));
                        }
                        ret.Add(new sCallContext(cont, exts.ToArray()));
                    }
                }
                return ret.ToArray();
            }
        }

        private string _PinSetToRegex(ArrayList pins)
        {
            string ret = "^(";
            foreach (object obj in pins)
                ret += obj.ToString() + "|";
            return ret.Substring(0,ret.Length-1) + ")$";
        }

        public override void LoadInitialConfiguration()
        {
            lock(_lock){
                Hashtable ht = new Hashtable();
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents",
                    "SELECT pr.RouteContext.Name,pr.Name,pr.DestinationCondition,pr.PinFile.Name FROM PinnedRoute pr ORDER BY pr.RouteContext.Name,pr.Index");
                cq.Execute();
                Hashtable routes = new Hashtable();
                string curContext = "";
                ArrayList prs = new ArrayList();
                while (cq.Read())
                {
                    if (curContext != cq[0].ToString())
                    {
                        if (prs.Count > 0)
                            routes.Add(curContext, prs);
                        prs = new ArrayList();
                        curContext = cq[0].ToString();
                    }
                    Hashtable pr = new Hashtable();
                    pr.Add(_NAME_FIELD_ID, cq[1].ToString());
                    pr.Add(_NPA_FIELD_ID, cq[2].ToString());
                    pr.Add(_PIN_SET_NAME_FIELD_ID, cq[3].ToString());
                    prs.Add(pr);
                }
                if (prs.Count > 0)
                    routes.Add(curContext, prs);
                cq.Close();

                Hashtable pins = new Hashtable();
                cq.NewQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones",
                                "SELECT ps.Context.Name,ps.Advanced,ps.Name FROM PinSet ps");
                cq.Execute();
                curContext = "";
                Hashtable pinSets = new Hashtable();
                while (cq.Read())
                {
                    if (curContext!=cq[0].ToString()){
                        if (pinSets.Count>0)
                            pins.Add(curContext,pinSets);
                        pinSets = new Hashtable();
                        curContext = cq[0].ToString();
                    }
                    bool adv = cq.GetBoolean(1);
                    ClassQuery pinq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones",
                        "SELECT pn.PinNumber" + (adv ? ",pn.Extension.Number,pn.Extension.Domain.Name" : "") + " Pin pn WHERE pn.OwningSet.Name = @name AND pn.OwningSet.Context.Name = @context");
                    pinq.Execute(new IDbDataParameter[]{
                        pinq.CreateParameter("@name",cq[1].ToString()),
                        pinq.CreateParameter("@context",cq[0].ToString())
                    });
                    ArrayList pinValues = new ArrayList();
                    while (pinq.Read())
                    {
                        if (adv)
                            pinValues.Add(cq[1].ToString()+"@"+cq[2].ToString() + ":" + cq[0].ToString());
                        else
                            pinValues.Add(cq[0].ToString());
                    }
                    pinq.Close();
                    Hashtable pset = new Hashtable();
                    pset.Add(_PINSET_ADVANCED_FIELD_ID, adv);
                    pset.Add(_PINSET_PINS_FIELD_ID, pinValues);
                    pinSets.Add(cq[2].ToString(), pset);
                }
                cq.Close();
                if (pinSets.Count>0)
                    pins.Add(curContext,pinSets);
                ht.Add(_SCONFIG_ROUTES_NAME, routes);
                ht.Add(_SCONFIG_PINS_NAME, pins);
                StoredConfiguration = ht;
            }
        }

        #region pinsets
        protected void DeletePinset(string context, string name)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable pins = (Hashtable)ht[_SCONFIG_PINS_NAME];
                if (pins.ContainsKey(context))
                {
                    Hashtable cont = (Hashtable)pins[context];
                    if (cont.ContainsKey(name))
                        cont.Remove(name);
                    pins.Remove(context);
                    pins.Add(context, cont);
                }
                ht.Remove(_SCONFIG_PINS_NAME);
                ht.Add(_SCONFIG_PINS_NAME, pins);
                StoredConfiguration = ht;
            }
        }

        protected void AddBasicPinset(string context, string name, string[] pins)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                }
                if (cont.ContainsKey(name))
                    cont.Remove(name);
                Hashtable pset = new Hashtable();
                pset.Add(_PINSET_ADVANCED_FIELD_ID, false);
                pset.Add(_PINSET_PINS_FIELD_ID, new ArrayList(pins));
                cont.Add(name, pset);
                psets.Add(context, cont);

                ht.Remove(_SCONFIG_PINS_NAME);
                ht.Add(_SCONFIG_PINS_NAME, psets);
                StoredConfiguration = ht;
            }
        }

        protected void AddAdvancedPinset(string context, string name, sAdvancedPin[] pins)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                }
                if (cont.ContainsKey(name))
                    cont.Remove(name);
                Hashtable pset = new Hashtable();
                pset.Add(_PINSET_ADVANCED_FIELD_ID, true);
                ArrayList apins = new ArrayList();
                foreach (sAdvancedPin pin in pins)
                    apins.Add(pin.ToString());
                pset.Add(_PINSET_PINS_FIELD_ID, apins);
                cont.Add(name, pset);
                psets.Add(context, cont);

                ht.Remove(_SCONFIG_PINS_NAME);
                ht.Add(_SCONFIG_PINS_NAME, psets);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateBasicPinset(string context, string name, string newName, string[] pins)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                }
                if (cont.ContainsKey(name))
                    cont.Remove(name);
                Hashtable pset = new Hashtable();
                pset.Add(_PINSET_ADVANCED_FIELD_ID, false);
                pset.Add(_PINSET_PINS_FIELD_ID, new ArrayList(pins));
                cont.Add(newName, pset);
                psets.Add(context, cont);

                ht.Remove(_SCONFIG_PINS_NAME);
                ht.Add(_SCONFIG_PINS_NAME, psets);
                StoredConfiguration = ht;
            }
        }

        protected void UpdateAdvancedPinset(string context, string name,string newName, sAdvancedPin[] pins)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                }
                if (cont.ContainsKey(name))
                    cont.Remove(name);
                Hashtable pset = new Hashtable();
                pset.Add(_PINSET_ADVANCED_FIELD_ID, true);
                ArrayList apins = new ArrayList();
                foreach (sAdvancedPin pin in pins)
                    apins.Add(pin.ToString());
                pset.Add(_PINSET_PINS_FIELD_ID, apins);
                cont.Add(newName, pset);
                psets.Add(context, cont);

                ht.Remove(_SCONFIG_PINS_NAME);
                ht.Add(_SCONFIG_PINS_NAME, psets);
                StoredConfiguration = ht;
            }
        }
        #endregion

        #region pins
        protected void UpdateAdvancedPin(string context, string name, sAdvancedPin oldPin, sAdvancedPin newPin)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                    if (cont.ContainsKey(name))
                    {
                        Hashtable pset = (Hashtable)cont[name];
                        cont.Remove(name);
                        ArrayList pins = (ArrayList)pset[_PIN_SET_NAME_FIELD_ID];
                        pins.Remove(oldPin.ToString());
                        pins.Add(newPin.ToString());
                        pset.Remove(_PIN_SET_NAME_FIELD_ID);
                        pset.Add(_PIN_SET_NAME_FIELD_ID, pins);
                        cont.Add(name, pset);
                    }
                    psets.Add(context, cont);
                    ht.Remove(_SCONFIG_PINS_NAME);
                    ht.Add(_SCONFIG_PINS_NAME, psets);
                    StoredConfiguration = ht;
                }
            }
        }

        protected void UpdateBasicPin(string context, string name, string oldPin, string newPin)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                    if (cont.ContainsKey(name))
                    {
                        Hashtable pset = (Hashtable)cont[name];
                        cont.Remove(name);
                        ArrayList pins = (ArrayList)pset[_PIN_SET_NAME_FIELD_ID];
                        pins.Remove(oldPin);
                        pins.Add(newPin);
                        pset.Remove(_PIN_SET_NAME_FIELD_ID);
                        pset.Add(_PIN_SET_NAME_FIELD_ID, pins);
                        cont.Add(name, pset);
                    }
                    psets.Add(context, cont);
                    ht.Remove(_SCONFIG_PINS_NAME);
                    ht.Add(_SCONFIG_PINS_NAME, psets);
                    StoredConfiguration = ht;
                }
            }
        }

        protected void AddAdvancedPin(string context, string name, sAdvancedPin pin)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                    if (cont.ContainsKey(name))
                    {
                        Hashtable pset = (Hashtable)cont[name];
                        cont.Remove(name);
                        ArrayList pins = (ArrayList)pset[_PIN_SET_NAME_FIELD_ID];
                        pins.Add(pin.ToString());
                        pset.Remove(_PIN_SET_NAME_FIELD_ID);
                        pset.Add(_PIN_SET_NAME_FIELD_ID, pins);
                        cont.Add(name, pset);
                    }
                    psets.Add(context, cont);
                    ht.Remove(_SCONFIG_PINS_NAME);
                    ht.Add(_SCONFIG_PINS_NAME, psets);
                    StoredConfiguration = ht;
                }
            }
        }

        protected void AddBasicPin(string context, string name, string pin)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                    if (cont.ContainsKey(name))
                    {
                        Hashtable pset = (Hashtable)cont[name];
                        cont.Remove(name);
                        ArrayList pins = (ArrayList)pset[_PIN_SET_NAME_FIELD_ID];
                        pins.Add(pin);
                        pset.Remove(_PIN_SET_NAME_FIELD_ID);
                        pset.Add(_PIN_SET_NAME_FIELD_ID, pins);
                        cont.Add(name, pset);
                    }
                    psets.Add(context, cont);
                    ht.Remove(_SCONFIG_PINS_NAME);
                    ht.Add(_SCONFIG_PINS_NAME, psets);
                    StoredConfiguration = ht;
                }
            }
        }

        protected void DeleteAdvancedPin(string context, string name, sAdvancedPin pin)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                    if (cont.ContainsKey(name))
                    {
                        Hashtable pset = (Hashtable)cont[name];
                        cont.Remove(name);
                        ArrayList pins = (ArrayList)pset[_PIN_SET_NAME_FIELD_ID];
                        pins.Remove(pin.ToString());
                        pset.Remove(_PIN_SET_NAME_FIELD_ID);
                        pset.Add(_PIN_SET_NAME_FIELD_ID, pins);
                        cont.Add(name, pset);
                    }
                    psets.Add(context, cont);
                    ht.Remove(_SCONFIG_PINS_NAME);
                    ht.Add(_SCONFIG_PINS_NAME, psets);
                    StoredConfiguration = ht;
                }
            }
        }

        protected void DeleteBasicPin(string context, string name, string pin)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable psets = (Hashtable)ht[_SCONFIG_PINS_NAME];
                Hashtable cont = new Hashtable();
                if (psets.ContainsKey(context))
                {
                    cont = (Hashtable)psets[context];
                    psets.Remove(context);
                    if (cont.ContainsKey(name))
                    {
                        Hashtable pset = (Hashtable)cont[name];
                        cont.Remove(name);
                        ArrayList pins = (ArrayList)pset[_PIN_SET_NAME_FIELD_ID];
                        pins.Remove(pin);
                        pset.Remove(_PIN_SET_NAME_FIELD_ID);
                        pset.Add(_PIN_SET_NAME_FIELD_ID, pins);
                        cont.Add(name, pset);
                    }
                    psets.Add(context, cont);
                    ht.Remove(_SCONFIG_PINS_NAME);
                    ht.Add(_SCONFIG_PINS_NAME, psets);
                    StoredConfiguration = ht;
                }
            }
        }
        #endregion

        #region pinRoutes
        protected void DeletePinnedRoute(string context, string name)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable routes = (Hashtable)ht[_SCONFIG_ROUTES_NAME];
                ArrayList cont = new ArrayList();
                if (routes.ContainsKey(context))
                {
                    cont = (ArrayList)routes[context];
                    routes.Remove(context);
                }
                for (int x = 0; x < cont.Count; x++)
                {
                    Hashtable pr = (Hashtable)cont[x];
                    if ((string)pr[_NAME_FIELD_ID] == name)
                    {
                        cont.RemoveAt(x);
                        break;
                    }
                }
                routes.Add(context, cont);
                ht.Remove(_SCONFIG_ROUTES_NAME);
                ht.Add(_SCONFIG_ROUTES_NAME, routes);
                StoredConfiguration = ht;
            }
        }

        protected void AddPinnedRoute(string context, string name, NPANXXValue condition, string pinsetName)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable routes = (Hashtable)ht[_SCONFIG_ROUTES_NAME];
                ArrayList cont = new ArrayList();
                if (routes.ContainsKey(context))
                {
                    cont = (ArrayList)routes[context];
                    routes.Remove(context);
                }
                Hashtable pr = new Hashtable();
                pr.Add(_NAME_FIELD_ID, name);
                pr.Add(_NPA_FIELD_ID, condition.Value);
                pr.Add(_PIN_SET_NAME_FIELD_ID, pinsetName);
                cont.Add(pr);
                routes.Add(context, cont);
                ht.Remove(_SCONFIG_ROUTES_NAME);
                ht.Add(_SCONFIG_ROUTES_NAME, routes);
                StoredConfiguration = ht;
            }
        }

        protected void UpdatePinnedRoute(string context, string name, NPANXXValue condition, string pinsetName)
        {
            lock (_lock)
            {
                Hashtable ht = StoredConfiguration;
                Hashtable routes = (Hashtable)ht[_SCONFIG_ROUTES_NAME];
                ArrayList cont = new ArrayList();
                if (routes.ContainsKey(context))
                {
                    cont = (ArrayList)routes[context];
                    routes.Remove(context);
                }
                Hashtable pr;
                int index = cont.Count;
                for (int x = 0; x < cont.Count; x++)
                {
                    pr = (Hashtable)cont[x];
                    if ((string)pr[_NAME_FIELD_ID] == name)
                    {
                        index = x;
                        cont.RemoveAt(x);
                        break;
                    }
                }
                pr = new Hashtable();
                pr.Add(_NAME_FIELD_ID, name);
                pr.Add(_NPA_FIELD_ID, condition.Value);
                pr.Add(_PIN_SET_NAME_FIELD_ID, pinsetName);
                cont.Insert(index, pr);
                routes.Add(context, cont);
                ht.Remove(_SCONFIG_ROUTES_NAME);
                ht.Add(_SCONFIG_ROUTES_NAME, routes);
                StoredConfiguration = ht;
            }
        }
        #endregion
    }
}
