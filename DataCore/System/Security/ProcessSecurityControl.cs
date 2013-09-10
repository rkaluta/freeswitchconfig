using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using System.Threading;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using System.Text.RegularExpressions;
using System.Reflection;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security
{
    [RequiredSudoPath("id",IDCommand,"Used to obtain the id of a given user for securing the process")]
    public class ProcessSecurityControl : MarshalByRefObject,IDiagnosable
    {
        private const string ASSEMBLY_NAME = "Mono.Posix, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756";
        private const string ECHO_PATH = "/bin/echo";
        private const string CAT_PATH = "/bin/cat";
        private const string SUDOERS_FILE = "/etc/sudoers";
        public const string IDCommand = "/usr/bin/id";

        private object _lock = new object();
        private static ProcessSecurityControl _current;

        public static ProcessSecurityControl Current
        {
            get { return _current; }
        }

        [DiagnosticFunctionAttribute("System Security")]
        public static List<string> RunDefaultDiagnostics()
        {
            return Current.RunDiagnostics();
        }

        [OperatingSystemOverridableFunctionAttribute("Returns the user ID that the application is current running as.")]
        public List<string> RunDiagnostics()
        {
            List<string> results = _current.SudoCommands;
            List<string> ret = new List<string>();
            if (!results.Contains(IDCommand))
                ret.Add("Running user does NOT have access to " + IDCommand);
            else
                ret.Add("Running user DOES have access to " + IDCommand); 
            return ret;
        }

        static ProcessSecurityControl(){
            _current = (ProcessSecurityControl)OSClassProxy.Instance(new ProcessSecurityControl());
        }

        private ProcessSecurityControl(){
            _sudoCommands = new List<string>();
            if (Utility.OperatingSystem.UsesSudo)
            {
                string coms = Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand, "-l -U " + Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME].ToString(), true);
                if (coms.Contains("NOPASSWD:"))
                {
                    coms = coms.Substring(coms.IndexOf("NOPASSWD:") + "NOPASSWD:".Length).Replace(" ", "").Replace("\n","").Replace("\r","").Replace("\t","");
                    foreach (string str in coms.Split(','))
                    {
                        if (str.StartsWith("/"))
                            _sudoCommands.Add(str);
                    }
                }
            }
        }

        public int CurrentProcessID
        {
            get
            {
                return Process.GetCurrentProcess().Id;
            }
        }

        [OperatingSystemOverridablePropertyAttribute("Returns the user ID that the application is current running as.")]
        public uint CurrentProcessUserID
        {
            get
            {
                Log.Trace("Locating user ID for current process, with Process ID of " + CurrentProcessID.ToString());
                Assembly.Load(ASSEMBLY_NAME);
                Type t = Utility.LocateType("Mono.Unix.Native.Syscall");
                if (t == null)
                    throw new Exception("Unable to locate the Syscall type in Mono.Unix.Native");
                MethodInfo mi = t.GetMethod("getuid");
                return (uint)mi.Invoke(null, new object[0]);
            }
        }

        [OperatingSystemOverridableFunctionAttribute("Returns the user ID for a user with the given username.Parameters(string username)")]
        public uint GetUIDForUser(string username)
        {
            Log.Trace("Locating UserID for user " + username);
            uint ret = 0;
            ret = uint.Parse(Utility.ExecuteProgram(OperatingSystemPaths.Current.SUDOCommand,IDCommand+" -u "+username,true));
            Log.Trace("Returning UserID " + ret.ToString() + " for user " + username);
            return ret;
        }

        [OperatingSystemOverridablePropertyAttribute("Returns the running user ID according to the system configuration settings.")]
        public uint RunningUserID
        {
            get
            {
                return GetUIDForUser(Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME].ToString());
            }
        }

        public void CloseProcessSecurity()
        {
            if (!_securityClosed)
            {
                _CloseProcessSecurity();
                _securityClosed = true;
            }
        }

        [OperatingSystemOverridableFunctionAttribute("The function closes off the process capabilities to allow root access to certain items, like opening sockets.")]
        internal void _CloseProcessSecurity(){
            Monitor.Enter(_lock);
            Log.Trace("Closing up process security");
            Assembly.Load(ASSEMBLY_NAME);
            Type t = Utility.LocateType("Mono.Unix.Native.Syscall");
            if (t == null)
                throw new Exception("Unable to locate the Syscall type in Mono.Unix.Native");
            MethodInfo mi = t.GetMethod("seteuid");
            mi.Invoke(null, new object[] { RunningUserID });
            Monitor.Exit(_lock);
        }

        private bool _securityClosed=false;
        public bool SecurityClosed
        {
            get { return _securityClosed; }
        }

        #region SUDO
        internal void SetSuperUserInfo(string username, string password)
        {
            if (HttpRequest.CurrentRequest != null)
            {
                if (HttpRequest.CurrentRequest.Session != null)
                {
                    HttpRequest.CurrentRequest.Session[_SUPER_USER_NAME_SESSION_ID] = username;
                    HttpRequest.CurrentRequest.Session[_SUPER_USER_PASSWORD_SESSION_ID] = password;
                }
            }
        }

        internal bool IsSuperUser
        {
            get
            {
                if (SecurityClosed)
                {
                    if (HttpRequest.CurrentRequest != null)
                    {
                        if (HttpRequest.CurrentRequest.Session != null)
                            return HttpRequest.CurrentRequest.Session[_SUPER_USER_PASSWORD_SESSION_ID] != null;
                    }
                    return false;
                }
                return true;
            }
        }

        private const string _SUPER_USER_NAME_SESSION_ID = "Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.ProcessSecurityControl.SuperUserName";
        private static string SuperUserName
        {
            get
            {
                if (HttpRequest.CurrentRequest != null)
                {
                    if (HttpRequest.CurrentRequest.Session != null)
                        return (string)HttpRequest.CurrentRequest.Session[_SUPER_USER_NAME_SESSION_ID];
                }
                return null;
            }
        }
        private const string _SUPER_USER_PASSWORD_SESSION_ID = "Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.ProcessSecurityControl.SuperUserPassword";
        private static string SuperUserPassword
        {
            get
            {
                if (HttpRequest.CurrentRequest != null)
                {
                    if (HttpRequest.CurrentRequest.Session != null)
                        return (string)HttpRequest.CurrentRequest.Session[_SUPER_USER_PASSWORD_SESSION_ID];
                }
                return null;
            }
        }

        private List<string> _sudoCommands;
        private List<string> _tmpCommands = new List<string>();
        public List<string> SudoCommands
        {
            get { return _sudoCommands; }
        }

        public bool RequiresSuperUserToRun(string commandName)
        {
            if (!Utility.OperatingSystem.UsesSudo)
                return false;
            return _sudoCommands.Contains(commandName);
        }

        public void AddSudoCommands(string[] commands,bool permanent)
        {
            if (Utility.OperatingSystem.UsesSudo)
            {
                lock (_sudoCommands)
                {
                    if (permanent)
                    {
                        foreach (string str in commands)
                        {
                            if (!_sudoCommands.Contains(str))
                                _sudoCommands.Add(str);
                        }
                        UpdateSudoersFile(_sudoCommands, SuperUserName, SuperUserPassword);
                    }
                    else
                    {
                        List<string> tmp = new List<string>(commands);
                        _tmpCommands.AddRange(tmp);
                        tmp.AddRange(_sudoCommands);
                        foreach (string str in commands)
                        {
                            if (!tmp.Contains(str))
                                tmp.Add(str);
                        }
                        UpdateSudoersFile(tmp, SuperUserName, SuperUserPassword);
                    }
                }
            }
        }

        public void RemoveSudoCommands(string[] commands,bool permanent)
        {
            if (Utility.OperatingSystem.UsesSudo)
            {
                lock (_sudoCommands)
                {
                    if (permanent)
                    {
                        foreach (string str in commands)
                        {
                            if (_sudoCommands.Contains(str))
                                _sudoCommands.Remove(str);
                        }
                        UpdateSudoersFile(_sudoCommands, SuperUserName, SuperUserPassword);
                    }
                    else
                    {
                        List<string> tmp = new List<string>();
                        foreach (string str in commands)
                        {
                            for (int x = 0; x < _tmpCommands.Count; x++)
                            {
                                if (_tmpCommands[x] == str)
                                {
                                    _tmpCommands.RemoveAt(x);
                                    break;
                                }
                            }
                        }
                        tmp.AddRange(_sudoCommands);
                        foreach (string str in _tmpCommands)
                        {
                            if (!tmp.Contains(str))
                                tmp.Add(str);
                        }
                        UpdateSudoersFile(tmp, SuperUserName, SuperUserPassword);
                    }
                }
            }
        }

        private static readonly string[] _SHELLS = new string[] {"/bin/bash","/bin/sh" };

        private void UpdateSudoersFile(List<string> commands, string superUserName, string superUserPassword)
        {
            List<string> content;
            Process p = new Process();
            if (_securityClosed)
            {
                string shell = _SHELLS[0];
                int x = 1;
                while (!new FileInfo(shell).Exists)
                {
                    if (x > _SHELLS.Length)
                        throw new Exception("Unable to locate a viable shell to use.");
                    else
                        shell = _SHELLS[x];
                    x++;
                }
                p.StartInfo = new ProcessStartInfo(shell);
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.StandardOutput.ReadToEnd();
                if (superUserName != null)
                    p.StandardInput.WriteLine("su " + superUserName);
                else
                    p.StandardInput.WriteLine("su");
                p.StandardInput.WriteLine(superUserPassword);
                p.StandardOutput.ReadToEnd();
                p.StandardInput.WriteLine(CAT_PATH+" "+SUDOERS_FILE);
                content = new List<string>(p.StandardOutput.ReadToEnd().Split('\n'));
            }
            else
                content = new List<string>(Utility.ExecuteProgram(CAT_PATH, SUDOERS_FILE, true).Split('\n'));
            bool addCommandLine = true;
            bool addUserLine = true;
            bool deleteEntries = false;
            bool update = false;
            string commandLine = "Cmnd_Alias FREESWITCH_CONFIG =";
            string userLine = string.Format("{0} ALL = NOPASSWD: FREESWITCH_CONFIG", Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME]);
            foreach (string str in commands)
                commandLine += str + ", ";
            if (commandLine.EndsWith(", "))
            {
                commandLine = commandLine.Substring(0, commandLine.Length - 2);
                deleteEntries = !content.Contains(commandLine);
            }
            else
            {
                deleteEntries = true;
                update = true;
            }
            for (int x = 0; x < content.Count; x++)
            {
                if (content[x].StartsWith("Cmd_Alias FREESWITCH_CONFIG ="))
                {
                    if (deleteEntries)
                    {
                        content.RemoveAt(x);
                        x--;
                    }
                    else if (content[x] != commandLine)
                    {
                        content[x] = commandLine;
                        addCommandLine = false;
                        update = true;
                    }
                }
                else if (content[x].StartsWith(Settings.Current[Constants.RUNNING_USERNAME_SETTING_NAME].ToString() + " ALL = "))
                {
                    if (deleteEntries)
                    {
                        content.RemoveAt(x);
                        x--;
                    }
                    else if (content[x] != userLine)
                    {
                        content[x] = userLine;
                        addUserLine = false;
                        update = true;
                    }
                }
            }
            if (!deleteEntries)
            {
                update = addCommandLine || addUserLine;
                if (addCommandLine)
                    content.Add(commandLine);
                if (addUserLine)
                    content.Add(userLine);
            }
            if (update)
            {
                if (_securityClosed)
                {
                    p.StandardInput.WriteLine(ECHO_PATH+" \"\" > "+SUDOERS_FILE);
                    foreach (string str in content)
                        p.StandardInput.WriteLine(ECHO_PATH + " \""+str+"\" >> "+SUDOERS_FILE);
                    p.StandardInput.WriteLine("exit");
                    p.StandardInput.WriteLine("exit");
                    p.WaitForExit();
                }
                else
                {
                    StreamWriter sw = new StreamWriter(new FileStream(SUDOERS_FILE, FileMode.Create, FileAccess.Write));
                    foreach (string str in content)
                        sw.WriteLine(str);
                    sw.Flush();
                    sw.Close();
                }
            }
        }
        #endregion
    }
}
