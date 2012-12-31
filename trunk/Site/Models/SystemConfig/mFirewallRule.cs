using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig
{
    [ModelJSFilePath("/resources/scripts/Core/SystemConfig/Firewall.js")]
    [ModelRoute("/core/models/sysconfig/Firewall")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView | ModelBlockJavascriptGenerations.EditForm)]
    [ModelNamespace("FreeswitchConfig.System")]
    public class mFirewallRule : IModel
    {
        private FirewallRule _rule;

        private string _moduleName;
        public string ModuleName
        {
            get { return _moduleName; }
        }

        public FireWallChains Chain
        {
            get { return _rule.Chain; }
        }

        public Protocols Protocol
        {
            get { return _rule.Protocol; }
        }

        public ICMPTypes? ICMPType
        {
            get { return _rule.ICMPType; }
        }

        public string Interface
        {
            get { return _rule.Interface; }
        }

        public string SourceIP
        {
            get { return (_rule.SourceIP == null ? null : _rule.SourceIP.ToString()); }
        }

        public string SourceNetworkMask
        {
            get { return (_rule.SourceNetworkMask == null ? null : _rule.SourceNetworkMask.ToString()); }
        }

        public string SourcePort
        {
            get { return (_rule.SourcePort == null ? null : _rule.SourcePort.ToString()); }
        }

        public string DestinationIP
        {
            get { return (_rule.DestinationIP == null ? null : _rule.DestinationIP.ToString()); }
        }

        public string DestinationNetworkMask
        {
            get { return (_rule.DestinationNetworkMask == null ? null : _rule.DestinationNetworkMask.ToString()); }
        }

        public string DestinationPort
        {
            get { return (_rule.DestinationPort == null ? null : _rule.DestinationPort.ToString()); }
        }

        public ConnectionStateTypes[] ConnectionStates
        {
            get { return _rule.ConnectionStates; }
        }

        public string Note
        {
            get { return _rule.Note; }
        }

        public string AdditionalDisplayInformation
        {
            get
            {
                return _rule.AdditionalDisplayInformation;
            }
        }

        public string TextDescription
        {
            get { return _rule.TextDescription; }
        }

        public string AddRuleCommand
        {
            get{
                return _rule.AddRulesString;
            }
        }

        public string DeleteRuleCommand
        {
            get { return _rule.DeleteRulesString; }
        }

        public override string ToString()
        {
            string ret="";
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (pi.Name!="ICMPType" && pi.Name!="ConnectionStates" &&pi.Name!="id"){
                    object tmp = pi.GetValue(this, new object[0]);
                    if (tmp!=null)
                        ret += pi.Name + ":" + tmp.ToString()+",";
                    else
                        ret += pi.Name + ":NULL,";
                }
            }
            if (ICMPType.HasValue)
                ret += "ICMPType:" + ICMPType.Value.ToString() + ",";
            else
                ret += "ICMPType:NULL,";
            ret += "ConnectionStates:";
            if (ConnectionStates != null)
            {
                foreach (ConnectionStateTypes cst in ConnectionStates)
                    ret += cst.ToString() + "&";
                ret = ret.Substring(0, ret.Length - 1)+",";
            }
            else
                ret += "NULL,";
            return ret;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        private mFirewallRule(string moduleName,FirewallRule rule)
        {
            _moduleName = moduleName;
            _rule = rule;
        }

        [ModelLoadMethod()]
        public static mFirewallRule Load(string id)
        {
            if (User.Current == null)
                return null;
            else if (!User.Current.HasRight(Constants.SYSTEM_CONTROL_RIGHT))
                return null;
            mFirewallRule ret = null;
            foreach (mFirewallRule mfr in LoadAll())
            {
                if (mfr.id == id)
                {
                    ret = mfr;
                    break;
                }
            }
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<mFirewallRule> LoadAll()
        {
            if (User.Current == null)
                return null;
            else if (!User.Current.HasRight(Constants.SYSTEM_CONTROL_RIGHT))
                return null;
            List<mFirewallRule> ret = new List<mFirewallRule>();
            if (ModuleController.Current.IsModuleEnabled("System Security"))
            {
                List<FirewallRule> rules = (List<FirewallRule>)ModuleController.Current.InvokeModuleMethod("System Security", "GetAllRules");
                foreach (FirewallRule fr in rules)
                    ret.Add(new mFirewallRule(null,fr));
            }
            else
            {
                foreach (FirewallRule fr in FirewallRule.DefaultForwardRules)
                    ret.Add(new mFirewallRule(null, fr));
                foreach (FirewallRule fr in FirewallRule.DefaultInputRules)
                    ret.Add(new mFirewallRule(null, fr));
                foreach (FirewallRule fr in FirewallRule.DefaultOutputRules)
                    ret.Add(new mFirewallRule(null, fr));
                foreach (IModule mod in ModuleController.CurrentModules)
                {
                    if (ModuleController.Current.IsModuleEnabled(mod.ModuleName))
                    {
                        List<FirewallRule> rules = mod.FirewallRules;
                        if (rules != null)
                        {
                            foreach (FirewallRule fr in rules)
                                ret.Add(new mFirewallRule(mod.ModuleName, fr));
                        }
                    }
                }
            }
            return ret;
        }

        [ModelListMethod("/search/core/sysconfig/Firewall/Chain/{0}/{1}")]
        public static List<mFirewallRule> LoadAllForChain(FireWallChains chain,bool moduleOnly)
        {
            if (User.Current == null)
                return null;
            else if (!User.Current.HasRight(Constants.SYSTEM_CONTROL_RIGHT))
                return null;
            List<mFirewallRule> ret = new List<mFirewallRule>();
            if (ModuleController.Current.IsModuleEnabled("System Security"))
            {
                if (!moduleOnly)
                {
                    List<FirewallRule> rules = (List<FirewallRule>)ModuleController.Current.InvokeModuleMethod("System Security", "GetAllRules");
                    foreach (FirewallRule fr in rules)
                    {
                        if (fr.Chain==chain)
                            ret.Add(new mFirewallRule(null, fr));
                    }
                }
            }
            else
            {
                if (moduleOnly)
                {
                    foreach (IModule mod in ModuleController.CurrentModules)
                    {
                        if (ModuleController.Current.IsModuleEnabled(mod.ModuleName))
                        {
                            List<FirewallRule> rules = mod.FirewallRules;
                            if (rules != null)
                            {
                                foreach (FirewallRule fr in rules)
                                {
                                    if (fr.Chain == chain)
                                        ret.Add(new mFirewallRule(mod.ModuleName, fr));
                                }
                            }
                        }
                    }
                }
                else
                {
                    switch (chain)
                    {
                        case FireWallChains.Input:
                            foreach (FirewallRule fr in FirewallRule.DefaultInputRules)
                                ret.Add(new mFirewallRule(null, fr));
                            break;
                        case FireWallChains.Output:
                            foreach (FirewallRule fr in FirewallRule.DefaultOutputRules)
                                ret.Add(new mFirewallRule(null, fr));
                            break;
                        case FireWallChains.Forward:
                            foreach (FirewallRule fr in FirewallRule.DefaultForwardRules)
                                ret.Add(new mFirewallRule(null, fr));
                            break;
                    }
                }
            }
            int index = 0;
            foreach (mFirewallRule mfr in ret)
            {
                try
                {
                    System.Diagnostics.Debug.Write(index.ToString() + ". ");
                    index++;
                    System.Diagnostics.Debug.WriteLine(mfr.AddRuleCommand);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            return ret;
        }


        #region IModel Members

        public string id
        {
            get { return Convert.ToBase64String(new SHA512Managed().ComputeHash(ASCIIEncoding.ASCII.GetBytes(ToString()))); }
        }

        #endregion
    }
}
