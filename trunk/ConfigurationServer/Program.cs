using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer;
using System.Reflection;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using System.Diagnostics;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.Site.Services;
using Org.Reddragonit.EmbeddedWebServer.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using System.Collections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;

namespace Org.Reddragonit.FreeSwitchConfig.ConfigurationServer
{
    public class Program
    {
        internal static ServerService _service;

        static void Main(string[] args)
        {
            if (new List<string>(args).Contains("--RunDiagnostics"))
            {
                Console.Clear();
                Dictionary<string, List<string>> diags = new Dictionary<string, List<string>>();
                foreach (Type t in Utility.LocateTypeInstances(typeof(IDiagnosable)))
                {
                    Object obj = null;
                    foreach (MethodInfo mi in t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                    {
                        if (mi.GetCustomAttributes(typeof(DiagnosticFunctionAttribute), false).Length > 0)
                        {
                            DiagnosticFunctionAttribute att = ((DiagnosticFunctionAttribute)mi.GetCustomAttributes(typeof(DiagnosticFunctionAttribute), false)[0]);
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
                                if (diags.ContainsKey(name))
                                {
                                    tmp.AddRange((List<string>)diags[name]);
                                    diags.Remove(name);
                                    diags.Add(name, tmp);
                                }
                                else
                                    diags.Add(name, tmp);
                            }
                        }
                    }
                }
                Console.WriteLine("Diagnostics Results:");
                foreach (string key in diags.Keys)
                {
                    Console.WriteLine(key + ":");
                    foreach (string line in diags[key])
                        Console.WriteLine(string.Format("\t{0}", line));
                }
                return;
            }
            Thread.CurrentThread.Name = "MainThread";
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            _service = new ServerService(args);
            _service.WaitForExit();
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Interupt called, shutting down Free Switch Config...");
            _service.Stop();
            e.Cancel = true;
        }
    }
}
