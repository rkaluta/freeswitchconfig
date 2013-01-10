using System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Threading;
using System.Collections.Generic;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using System.IO;
using System.Reflection;
using Org.Reddragonit.EmbeddedWebServer.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security;
using Org.Reddragonit.EmbeddedWebServer;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using System.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.Site.Services;
using System.Net;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.Site;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;

namespace Org.Reddragonit.FreeSwitchConfig.ConfigurationServer
{
    internal class ServerService : IEventHandler
    {

        private static string basePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.Substring(0, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.LastIndexOf(Path.DirectorySeparatorChar));
        private bool consoleMessages = false;
        private bool perfomanceMessages = false;

        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private ManualResetEvent _internalShutdownEvent = new ManualResetEvent(false);
        private string pidfile;
        private Thread _runner;
        private string[] args;
        private List<NameValuePair> _runningThreads = null;
        private Timer _runningThreadsOutputter = null;
        private ServerControl.delPreBackgroundCall _preCall;
        private ServerControl.delPostBackgroundCall _postCall;

        public ServerService(string[] Args)
        {
            _preCall = new ServerControl.delPreBackgroundCall(OnBackgroundStart);
            _postCall = new ServerControl.delPostBackgroundCall(OnBackgroundEnd);
            _runningThreads = new List<NameValuePair>();
            args = Args;
            _runner = new Thread(new ThreadStart(_Start));
            _runner.IsBackground = true;
            _runner.Name = "ServerServiceThread";
            _runner.Start();
        }

        private void _Start()
        {
            if (Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME] == null)
                Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME] = "www";
            pidfile = null;
            consoleMessages = false;
            bool _doNotPromptSudo = false;
            if (args.Length > 0)
            {
                for (int x = 0; x < args.Length; x++)
                {
                    switch (args[x])
                    {
                        case "--runningUser":
                            Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME] = args[x + 1];
                            x += 1;
                            break;
                        case "--pidfile":
                            pidfile = args[x + 1];
                            x += 1;
                            break;
                        case "--showConsoleMessages":
                            consoleMessages = true;
                            break;
                        case "--showRunningThreads":
                            _runningThreads = new List<NameValuePair>();
                            _runningThreadsOutputter = new Timer(new TimerCallback(_OutputRunningThreads), null, 30000, 30000);
                            break;
                        case "--showPerformanceData":
                            perfomanceMessages = true;
                            if (!ModuleController.Current.IsModuleEnabled("SystemMonitoring"))
                                ModuleController.Current.EnableModule("SystemMonitoring");
                            EventController.RegisterEventHandler(this);
                            break;
                        case "--doNotPromptforSudoChanges":
                            _doNotPromptSudo = true;
                            break;
                        default:
                            Console.WriteLine("Invalid command line arguements specified " + args[x] + ", ignoring.");
                            break;
                    }
                }
            }
            if (Utility.OperatingSystem == null)
            {
                Console.WriteLine("Unknown operating system, exiting...");
                _shutdownEvent.Set();
                return;
            }
            if (consoleMessages)
                ConfigSite.TurnOnConsoleLogging();
            foreach (FileInfo fi in new DirectoryInfo(basePath).GetFiles("*.dll"))
            {
                if (consoleMessages)
                    Console.WriteLine("Loading assembly from dll " + fi.Name + " ...");
                Assembly.LoadFile(fi.FullName);
                if (consoleMessages)
                    Console.WriteLine("Loaded assembly from dll " + fi.Name + ".");
            }
            if (consoleMessages)
            {
                Console.WriteLine("Loaded assemblies: ");
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (ass.GetName().Name!="mscorlib" && !ass.GetName().Name.StartsWith("System.") && ass.GetName().Name!="System" && !ass.GetName().Name.StartsWith("Microsoft"))
                        {
                            Console.WriteLine(ass.GetName().FullName);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e);
                    }
                }
            }
            if (ProcessSecurityControl.Current.CurrentProcessUserID != ProcessSecurityControl.Current.GetUIDForUser("root"))
            {
                Console.WriteLine("You must run this application as the root user, it will secure itself afterwards.");
                _shutdownEvent.Set();
                return;
            }
            else
            {
                bool _updateSudo = false;
                List<RequiredSudoPathAttribute> paths = new List<RequiredSudoPathAttribute>();
                if (Utility.OperatingSystem.UsesSudo)
                {
                    paths = _RequiredSudoPaths;
                    for (int x = 0; x < paths.Count; x++)
                    {
                        if (ProcessSecurityControl.Current.SudoCommands.Contains(paths[x].Path))
                        {
                            paths.RemoveAt(x);
                            x--;
                        }
                    }
                }
                if (_doNotPromptSudo)
                    _updateSudo = true;
                else if (Utility.OperatingSystem.UsesSudo)
                {
                    if (paths.Count > 0)
                    {
                        Console.WriteLine("This system requires access to the following (through sudo):");
                        foreach (RequiredSudoPathAttribute rspa in paths)
                            Console.WriteLine(rspa.Path + ": " + rspa.Reason);
                        Console.WriteLine("Is it allowed to update /etc/sudoers accordingly? (Y or N)");
                        _updateSudo = Console.ReadLine()=="Y";
                        if (!_updateSudo)
                        {
                            Console.WriteLine("System cannot operate without sudo access...");
                            _shutdownEvent.Set();
                            return;
                        }
                    }
                }
                if (paths.Count > 0 && _updateSudo)
                {
                    if (consoleMessages)
                        Console.WriteLine("Updating /etc/sudoers...");
                    List<string> tmp = new List<string>();
                    foreach (RequiredSudoPathAttribute rspa in paths)
                        tmp.Add(rspa.Path);
                    ProcessSecurityControl.Current.AddSudoCommands(tmp.ToArray(), true);
                    if (consoleMessages)
                        Console.WriteLine("Updated /etc/sudoers successfully");
                }
                if (consoleMessages)
                    Console.WriteLine("Starting up control socket...");
                if (consoleMessages)
                    Console.WriteLine("Control Socket started.");
                if (consoleMessages)
                    Console.WriteLine("Starting up Website through Server Control...");
                ServerControl.Start();
                if (_runningThreads != null)
                {
                    ServerControl.RegisterBackgroundOperationPreCall(_preCall);
                    ServerControl.RegisterBackgrounOperationPostCall(_postCall);
                }
                if (consoleMessages)
                    Console.WriteLine("Website started with Server Control.");
                try
                {
                    if (consoleMessages)
                        Console.WriteLine("Closing up Process security...");
                    ProcessSecurityControl.Current.CloseProcessSecurity();
                    if (consoleMessages)
                        Console.WriteLine("Process Security Closed.");
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                    Console.WriteLine("WARNING!!! An error occured changing the security role of the current process away from root.  It is still running as the root user.");
                }
                if (pidfile != null)
                {
                    if (consoleMessages)
                        Console.WriteLine("Writing process ID to pidile " + pidfile + "...");
                    SetPidFile(pidfile);
                    if (consoleMessages)
                        Console.WriteLine("Process ID written to file.");
                }
            }
            _internalShutdownEvent.WaitOne();
            OnStop();
        }

        private List<RequiredSudoPathAttribute> _RequiredSudoPaths
        {
            get
            {
                List<RequiredSudoPathAttribute> paths = new List<RequiredSudoPathAttribute>();
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (ass.GetName().Name!="mscorlib" && !ass.GetName().Name.StartsWith("System.") && ass.GetName().Name!="System" && !ass.GetName().Name.StartsWith("Microsoft"))
                    {
                        try
                        {
                            foreach (Type t in ass.GetTypes())
                            {
                                object[] props = t.GetCustomAttributes(typeof(RequiredSudoPathAttribute), false);
                                if (props.Length > 0)
                                {
                                    foreach (RequiredSudoPathAttribute rspa in props)
                                    {
                                        if (rspa.IsValid)
                                            paths.Add(rspa);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (e.Message != "The invoked member is not supported in a dynamic assembly.")
                            {
                                throw e;
                            }
                        }
                    }
                }
                //clear out duplicates
                for (int x = 0; x < paths.Count; x++)
                {
                    for (int y = x + 1; y < paths.Count; y++)
                    {
                        if (paths[x].Name == paths[y].Name)
                        {
                            if (paths[x].GetMatchLevelForOperatingSystem(Utility.OperatingSystem) < paths[y].GetMatchLevelForOperatingSystem(Utility.OperatingSystem))
                            {
                                paths.RemoveAt(x);
                                x--;
                                break;
                            }
                            else if (paths[x].GetMatchLevelForOperatingSystem(Utility.OperatingSystem) > paths[y].GetMatchLevelForOperatingSystem(Utility.OperatingSystem))
                            {
                                paths.RemoveAt(y);
                                y--;
                                break;
                            }
                        }
                    }
                }
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (ass.GetName().Name!="mscorlib" && !ass.GetName().Name.StartsWith("System.") && ass.GetName().Name!="System" && !ass.GetName().Name.StartsWith("Microsoft"))
                    {
                        try
                        {
                            foreach (Type t in ass.GetTypes())
                            {
                                object[] props = t.GetCustomAttributes(typeof(BlockedSudoPathAttribute), false);
                                if (props.Length > 0)
                                {
                                    foreach (BlockedSudoPathAttribute bspa in props)
                                    {
                                        if (bspa.IsBlocked)
                                        {
                                            for (int x = 0; x < paths.Count; x++)
                                            {
                                                if (paths[x].Path == bspa.Path)
                                                {
                                                    paths.RemoveAt(x);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (e.Message != "The invoked member is not supported in a dynamic assembly.")
                            {
                                throw e;
                            }
                        }
                    }
                }
                return paths;
            }
        }

        private void OnStop()
        {
            if (consoleMessages)
                Console.WriteLine("Stopping Website with ServerControl...");
            if (_runningThreads != null)
                {
                    ServerControl.UnRegisterBackgroundOperationPreCall(_preCall);
                    ServerControl.UnRegisterBackgroundOperationPostCall(_postCall);
            }
            ServerControl.Stop();
            if (consoleMessages)
                Console.WriteLine("Website stoppped with ServerControl.");
            if (pidfile != null)
            {
                if (consoleMessages)
                    Console.WriteLine("Deleting pidfile...");
                new FileInfo(pidfile).Delete();
                if (consoleMessages)
                    Console.WriteLine("Pidfile deleted.");
            }
            _shutdownEvent.Set();
        }

        public void Stop()
        {
            if (_runningThreadsOutputter != null)
                _runningThreadsOutputter.Dispose();
            if (perfomanceMessages)
                EventController.UnRegisterEventHandler(this);
            _internalShutdownEvent.Set();
        }

        public void WaitForExit()
        {
            _shutdownEvent.WaitOne();
        }

        private static void SetPidFile(string p)
        {
            string path = "";
            string[] split = p.Split(Path.DirectorySeparatorChar);
            for (int x = 0; x < split.Length - 1; x++)
            {
                if (split[x].Length > 0)
                {
                    path += Path.DirectorySeparatorChar + split[x];
                    new DirectoryInfo(path).Create();
                }
            }
            StreamWriter sw = new StreamWriter(new FileStream(p, FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
            sw.Write(Process.GetCurrentProcess().Id.ToString());
            sw.Flush();
            sw.Close();
        }

        private void OnBackgroundStart(sCall call, out bool abort)
        {
            abort = false;
            lock (_runningThreads)
            {
                _runningThreads.Add(new NameValuePair(call.type.FullName + "." + call.Method.Name, Thread.CurrentThread));
            }
        }

        private void OnBackgroundEnd(sCall call, double milliseconds,Exception error,bool timedOut)
        {
            lock(_runningThreads){
                for(int x=0;x<_runningThreads.Count;x++){
                    if (((Thread)_runningThreads[x].Value).ManagedThreadId==Thread.CurrentThread.ManagedThreadId){
                        _runningThreads.RemoveAt(x);
                        break;
                    }
                }
            }
        }

        private void _OutputRunningThreads(object pars)
        {
            Console.Clear();
            Console.WriteLine("Method Name[Status]");
            lock (_runningThreads)
            {
                for (int x = 0; x < _runningThreads.Count; x++)
                {
                    if ((int)(((Thread)_runningThreads[x].Value).ThreadState & System.Threading.ThreadState.Stopped) == (int)System.Threading.ThreadState.Stopped)
                    {
                        _runningThreads.RemoveAt(x);
                        x--;
                    }else
                        Console.WriteLine(_runningThreads[x].Name + "[" + ((Thread)_runningThreads[x].Value).ThreadState.ToString() + "]");
                }
            }
        }

        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            return Event.Name=="MetricsUpdatedEvent";
        }

        public void ProcessEvent(IEvent Event)
        {
            Console.Clear();
            Console.WriteLine("Sample Time: " + DateTime.Now.ToString());
            Console.WriteLine("Metric\tValue");
            foreach (string str in Event.Keys)
                Console.WriteLine(str + "\t" + Event[str].ToString());
        }

        #endregion
    }
}
