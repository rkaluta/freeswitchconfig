using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using System.Collections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Procurios.Public;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem
{
    public static class CallControlManager
    {
        internal const string _MODULE_NAME = "CallControlManager";
        private const string _PLANS_LIST_ID = "DeployedPlans";
        private const string _CONTROLLER_TYPE_ID = "CallController";
        private const string _DEFAULT_CONTROLLER_VALUE = "Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.DefaultCallController";

        private static List<ADialPlan> _plans;

        internal static Type ControllerType
        {
            get
            {
                if (_controller != null)
                    return _controller.GetType();
                else if (Settings.Current[_MODULE_NAME, _CONTROLLER_TYPE_ID] != null)
                    return (Type)Settings.Current[_MODULE_NAME, _CONTROLLER_TYPE_ID];
                return Utility.LocateType(_DEFAULT_CONTROLLER_VALUE);
            }
            set
            {
                if (_controller != null)
                    _controller.Stop();
                _controller = (ICallController)value.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                lock (_plans)
                {
                    _controller.Start(_plans.ToArray());
                    List<string> contexts = new List<string>();
                    foreach (ADialPlan adp in _plans)
                    {
                        foreach (string str in adp.ContextsUsed)
                        {
                            if (!contexts.Contains(str))
                                contexts.Add(str);
                        }
                    }
                    CoreGenerator.RegenerateContexts(contexts.ToArray());
                }
                Settings.Current[_MODULE_NAME, _CONTROLLER_TYPE_ID] = value;
            }
        }

        static CallControlManager()
        {
            _plans = new List<ADialPlan>();
            lock (_plans)
            {
                if (Settings.Current[_MODULE_NAME, _PLANS_LIST_ID] != null)
                {
                    foreach (string str in (List<string>)Settings.Current[_MODULE_NAME, _PLANS_LIST_ID])
                        _plans.Add((ADialPlan)Utility.LocateType(str).GetConstructor(Type.EmptyTypes).Invoke(new object[0]));
                }
                _plans.Sort();
            }
            _controller = (ICallController)ControllerType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            _controller.Start(_plans.ToArray());
        }

        private static ICallController _controller;

        public static DialedNumberTester TestDialNumber(Extension originatingCall, string dialNumber, string pin, DateTime? currentTime)
        {
            DialedNumberTester tester = new DialedNumberTester(originatingCall, dialNumber, pin, currentTime);
            ADialPlan[] plans = new ADialPlan[0];
            lock (_plans)
            {
                plans = _plans.ToArray();
            }
            bool exit=false;
            while(!exit){
                bool startFromTop=false;
                foreach (ADialPlan plan in plans)
                {
                    //build code here to test dialplans
                    //plan.RunDialPlanWithConditions(tester, out exit, out startFromTop);
                    if (exit || startFromTop)
                        break;
                }
            }
            return tester;
        }

        public static void DeployDialPlan(ADialPlan plan)
        {
            lock(_plans){
                if (!_plans.Contains(plan))
                {
                    plan.LoadInitialConfiguration();
                    _plans.Add(plan);
                    _plans.Sort();
                    List<string> planNames = new List<string>();
                    foreach (ADialPlan p in _plans)
                        planNames.Add(p.GetType().FullName);
                    Settings.Current[_MODULE_NAME, _PLANS_LIST_ID] = planNames;
                    _controller.DialplanDeployed(plan, _plans.IndexOf(plan));
                    List<string> contexts = plan.ContextsUsed;
                    CoreGenerator.RegenerateContexts(contexts.ToArray());
                }
            }
        }

        public static void UpdateDialPlan(Type type,ADialPlan.sUpdateConfigurationsCall call)
        {
            lock(_plans){
                for (int x = 0; x < _plans.Count; x++)
                {
                    if (_plans[x].GetType().FullName == type.FullName)
                    {
                        _plans[x].UpdateConfiguration(call);
                        _controller.DialplanUpdated(_plans[x]);
                        List<string> contexts = _plans[x].ContextsUsed;
                        CoreGenerator.RegenerateContexts(contexts.ToArray());
                        break;
                    }
                }
            }
        }

        public static void UnDeployPlan(Type type)
        {
            lock(_plans)
            {
                for (int x = 0; x < _plans.Count; x++)
                {
                    if (_plans[x].GetType().FullName == type.FullName)
                    {
                        _controller.DialplanUnDeployed(_plans[x]);
                        List<string> contexts = _plans[x].ContextsUsed;
                        _plans.RemoveAt(x);
                        _plans.Sort();
                        List<string> planNames = new List<string>();
                        foreach (ADialPlan p in _plans)
                            planNames.Add(p.GetType().FullName);
                        Settings.Current[_MODULE_NAME, _PLANS_LIST_ID] = planNames;
                        CoreGenerator.RegenerateContexts(contexts.ToArray());
                        break;
                    }
                }
            }
        }

        public static void UnDeployPlan(ADialPlan plan)
        {
            lock (_plans)
            {
                _controller.DialplanUnDeployed(plan);
                _plans.Remove(plan);
                _plans.Sort();
                List<string> planNames = new List<string>();
                foreach (ADialPlan p in _plans)
                    planNames.Add(p.GetType().FullName);
                Settings.Current[_MODULE_NAME, _PLANS_LIST_ID] = planNames;
                List<string> contexts = plan.ContextsUsed;
                CoreGenerator.RegenerateContexts(contexts.ToArray());
            }
        }

        internal static XmlContextFile GenerateContextFile(sDeployedContext context)
        {
            return _controller.GenerateContextFile(context);
        }

        public static sCallExtensionReference[] CallExtensions
        {
            get
            {
                List<sCallExtensionReference> ret = new List<sCallExtensionReference>();
                lock (_plans)
                {
                    foreach (ADialPlan pln in _plans)
                        ret.AddRange(pln.DefinedCallExtensions);
                }
                return ret.ToArray();
            }
        }
    }
}
