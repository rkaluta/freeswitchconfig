using System;
using System.Collections.Generic;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.Reflection;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using System.Collections;


/// <summary>
/// Summary description for DiagnosticsService
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.Services
{
    [EmbeddedServiceNamespace("FreeswitchConfig.Services")]
    public class DiagnosticsService : EmbeddedService
    {
        public DiagnosticsService()
        {
        }

        [WebMethod(true)]
        public Hashtable ProcessDiagnostics()
        {
            Hashtable ret = new Hashtable();
            foreach (Type t in Utility.LocateTypeInstances(typeof(IDiagnosable)))
            {
                Object obj = null;
                foreach (MethodInfo mi in t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                {
                    if (mi.GetCustomAttributes(typeof(DiagnosticFunctionAttribute),false).Length > 0)
                    {
                        DiagnosticFunctionAttribute att = ((DiagnosticFunctionAttribute)mi.GetCustomAttributes(typeof(DiagnosticFunctionAttribute), false)[0]);
                        bool run = true;
                        if (att.RequiredRights != null)
                        {
                            foreach (string str in att.RequiredRights)
                            {
                                if (!User.Current.HasRight(str))
                                {
                                    run = false;
                                    break;
                                }
                            }
                        }
                        if (run)
                        {
                            Delegate del = null;
                            if (mi.IsStatic)
                            {
                                del = Delegate.CreateDelegate(typeof(DiagnosticFunctionAttribute.DiagnosticsDelegate), mi);
                            }
                            else
                            {
                                if (obj == null)
                                    obj = t.GetConstructor(Type.EmptyTypes).Invoke(null);
                                del = DiagnosticFunctionAttribute.DiagnosticsDelegate.CreateDelegate(t, obj, mi.Name);
                            }
                            List<string> tmp = ((DiagnosticFunctionAttribute.DiagnosticsDelegate)del).Invoke();
                            if (tmp != null)
                            {
                                string name = att.GroupName;
                                if (ret.ContainsKey(name))
                                {
                                    tmp.AddRange((List<string>)ret[name]);
                                    ret.Remove(name);
                                    ret.Add(name, tmp);
                                }
                                else
                                    ret.Add(name, tmp);
                            }
                        }
                    }
                }
            }
            return ret;
        }
    }
}