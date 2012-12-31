using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Diagnostics;
using System.Net;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.EmbeddedWebServer.Minifiers;
using System.Collections;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Security.Cryptography;

using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using Org.Reddragonit.EmbeddedWebServer.Components;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore
{
    internal class SetupHandler : IEventHandler
    {
        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            return Event.Name == "Setup Complete";
        }

        public void ProcessEvent(IEvent Event)
        {
            Utility._isSiteSetup = Domain.AllDomainNames.Count > 0 && User.SelectList().Count > 0;
            if (Utility.IsSiteSetup)
                EventController.UnRegisterEventHandler(this);
        }

        #endregion
    }

    public class Utility : IDiagnosable, IBackgroundOperationContainer
    {
        private static string basePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.Substring(0, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.LastIndexOf(Path.DirectorySeparatorChar));
        private static Regex _regIP = new Regex("^(((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static Dictionary<string, CachedItemContainer> _TYPE_CACHE;
        private static Dictionary<string, CachedItemContainer> _INSTANCES_CACHE;
        private static Dictionary<string, Dictionary<string, CachedItemContainer>> _ASSEMBLY_INSTANCES_CACHE;

        [BackgroundOperationCall(0, -1, -1, -1, BackgroundOperationDaysOfWeek.All, 60000)]
        public static void CleanupTypeCaches()
        {
            string[] keys;
            lock (_TYPE_CACHE)
            {
                keys = new string[_TYPE_CACHE.Count];
                _TYPE_CACHE.Keys.CopyTo(keys, 0);
                foreach (string str in keys)
                {
                    if (DateTime.Now.Subtract(_TYPE_CACHE[str].LastAccess).TotalMinutes > 60)
                        _TYPE_CACHE.Remove(str);
                }
            }
            lock (_INSTANCES_CACHE)
            {
                keys = new string[_INSTANCES_CACHE.Count];
                _INSTANCES_CACHE.Keys.CopyTo(keys, 0);
                foreach (string str in keys)
                {
                    if (DateTime.Now.Subtract(_INSTANCES_CACHE[str].LastAccess).TotalMinutes > 60)
                        _INSTANCES_CACHE.Remove(str);
                }
            }
            lock (_ASSEMBLY_INSTANCES_CACHE)
            {
                keys = new string[_ASSEMBLY_INSTANCES_CACHE.Count];
                _ASSEMBLY_INSTANCES_CACHE.Keys.CopyTo(keys, 0);
                foreach (string str in keys)
                {
                    string[] tkeys = new string[_ASSEMBLY_INSTANCES_CACHE[str].Count];
                    _ASSEMBLY_INSTANCES_CACHE[str].Keys.CopyTo(tkeys, 0);
                    foreach (string key in tkeys)
                    {
                        if (DateTime.Now.Subtract(_ASSEMBLY_INSTANCES_CACHE[str][key].LastAccess).TotalMinutes > 60)
                            _ASSEMBLY_INSTANCES_CACHE[str].Remove(key);
                    }
                }
            }
        }

        [DiagnosticFunctionAttribute("Operating System")]
        public static List<string> RunDefaultDiagnostics()
        {
            List<string> ret = new List<string>();
            if (OperatingSystem == null)
                ret.Add("The server is unable to determine the operating system that it is running on.  Assuming using default configurations to properly run.");
            else
                ret.Add("The server determined successfully to be running on " + OperatingSystem.OsName + " " + OperatingSystem.Version.ToString());
            bool found = false;
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName.Contains("freeswitch") || (p.ProcessName.Contains("fs_cli")))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                ret.Add("Unable to locate running freeswitch VoIP server process.");
            else
                ret.Add("Located a running freeswitch VoIP server process.");
            return ret;
        }

        private static object _lock = new object();
        private static IOSDefinition _os = null;
        public static IOSDefinition OperatingSystem
        {
            get
            {
                Monitor.Enter(_lock);
                if (_os == null)
                {
                    try
                    {
                        foreach (Type t in Utility.LocateTypeInstances(typeof(IOSDefinition)))
                        {
                            _os = (IOSDefinition)t.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                            if (_os.IsCurrentOS)
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        EventController.TriggerEvent(new ErrorOccuredEvent(e));
                        Log.Error(e);
                    }
                }
                Monitor.Exit(_lock);
                if (!_os.IsCurrentOS)
                    _os = null;
                return _os;
            }
        }

        public static new DirectoryInfo LocateDirectory(string name)
        {
            Log.Trace("Locating Directory " + name);
            if (name == "/")
                return new DirectoryInfo(basePath);
            else
                return recurLocateDirectory(name, new DirectoryInfo(basePath));
        }

        private static DirectoryInfo recurLocateDirectory(string name, DirectoryInfo curDirectory)
        {
            Log.Trace("Recursively Locating Directory " + name);
            Log.Trace("Starting in Location " + curDirectory.FullName);
            if (curDirectory.Name.ToUpper() == name.ToUpper())
                return curDirectory;
            else
            {
                foreach (DirectoryInfo di in curDirectory.GetDirectories())
                {
                    DirectoryInfo ret = recurLocateDirectory(name, di);
                    if (ret != null)
                        return ret;
                }
            }
            return null;
        }

        public static string MergeRelativeURLs(string baseURL, string extendedURL)
        {
            Log.Trace("Joining realtive urls, baseURL: " + baseURL + ", extendedURL: " + extendedURL);
            if (baseURL.EndsWith("/"))
                baseURL = baseURL.Substring(0, baseURL.Length - 1);
            if (extendedURL.StartsWith("."))
            {
                while (extendedURL.StartsWith("."))
                {
                    if (extendedURL.StartsWith(".."))
                    {
                        Log.Trace("Located .. in " + extendedURL + " dropping back 1 directory.");
                        baseURL = baseURL.Substring(0, baseURL.LastIndexOf("/"));
                        extendedURL = extendedURL.Substring(3);
                    }
                    else
                    {
                        Log.Trace("Located . in " + extendedURL + " trimming off excess characters.");
                        extendedURL = extendedURL.Substring(2);
                    }
                }
            }
            if (!baseURL.EndsWith("/") && !extendedURL.StartsWith("/"))
                baseURL += "/";
            return baseURL + extendedURL;
        }

        public static string FormatDialPlanEnumName(uint enumIndex, string dialPlanName)
        {
            return enumIndex.ToString("00000") + "_" + dialPlanName.Replace(".", "_").Replace(" ", "_") + ".xml";
        }

        public static void DeployLuaScript(string fileName, Stream stream)
        {
            string path = Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_SCRIPTS_DIRECTORY + Path.DirectorySeparatorChar + fileName;
            BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None));
            BinaryReader br = new BinaryReader(stream);
            byte[] buffer = new byte[1024];
            int len = 0;
            while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
            {
                bw.Write(buffer, 0, len);
            }
            bw.Flush();
            bw.Close();
        }

        public static void DeleteLuaScript(string fileName)
        {
            new FileInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_SCRIPTS_DIRECTORY + Path.DirectorySeparatorChar + fileName).Delete();
        }

        private static Version _monoVersion = null;
        public static Version MonoVersion
        {
            get { return _monoVersion; }
        }

        static Utility()
        {
            Type type = Type.GetType("Mono.Runtime", false);
            if (type != null)
            {
                MethodInfo mi = type.GetMethod("GetDisplayName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                string str = mi.Invoke(null, new object[] { }).ToString();
                _monoVersion = new Version(str.Substring(0, str.IndexOf(" ")));
            }
            if (Settings.Current == null)
                Log.Trace("Current settings object is null.....");
            if (Settings.Current[Constants.BASE_PATH_NAME] == null)
                Settings.Current[Constants.BASE_PATH_NAME] = Constants.DEFAULT_BASE_BATH;
            if (Settings.Current[Constants.FREESWITCH_DBS_PATH_NAME] == null)
                Settings.Current[Constants.FREESWITCH_DBS_PATH_NAME] = Constants.DEFAULT_DBS_PATH;
            if (Settings.Current[Constants.SERVER_NAME_SETTING_NAME] == null)
                Settings.Current[Constants.SERVER_NAME_SETTING_NAME] = Constants.DEFAULT_SERVER_NAME;
            _TYPE_CACHE = new Dictionary<string, CachedItemContainer>();
            _INSTANCES_CACHE = new Dictionary<string, CachedItemContainer>();
            _ASSEMBLY_INSTANCES_CACHE = new Dictionary<string, Dictionary<string, CachedItemContainer>>();
            _isSiteSetup = Domain.AllDomainNames.Count > 0 && User.SelectList().Count > 0;
            if (!_isSiteSetup)
                EventController.RegisterEventHandler(new SetupHandler());
        }

        public static bool IsValidIPAddress(string value)
        {
            return (value != null) && _regIP.IsMatch(value);
        }

        public static List<Dictionary<string, string>> SelectFromFreeswitchDB(string database, string query)
        {
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();
            List<string> headers = new List<string>();
            if (!database.EndsWith(".db"))
                database += ".db";
            if (!query.EndsWith(";"))
                query += ";";
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(OperatingSystemPaths.Current.SQLLITECommand, " -list -header " +
            Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Settings.Current[Constants.FREESWITCH_DBS_PATH_NAME].ToString() + Path.DirectorySeparatorChar + database);
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.StandardInput.WriteLine(query);
            p.StandardInput.WriteLine(".exit");
            p.StandardInput.Flush();
            p.WaitForExit();
            string results = new StreamReader(p.StandardOutput.BaseStream).ReadToEnd().Trim();
            if (results.Length > 0)
            {
                string[] tmp = results.Split('\n');
                foreach (string str in tmp[0].Split('|'))
                {
                    headers.Add(str);
                }
                for (int x = 1; x < tmp.Length; x++)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    string[] split = tmp[x].Split('|');
                    for (int y = 0; y < split.Length; y++)
                        res.Add(headers[y], split[y]);
                    ret.Add(res);
                }
            }
            return ret;
        }

        public static void ExecuteCommandToFreeswitchDB(string database, string query)
        {
            if (!database.EndsWith(".db"))
                database += ".db";
            if (!query.EndsWith(";"))
                query += ";";
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(OperatingSystemPaths.Current.SQLLITECommand, " -list -header " +
            Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Settings.Current[Constants.FREESWITCH_DBS_PATH_NAME].ToString() + Path.DirectorySeparatorChar + database);
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.StandardInput.WriteLine(query);
            p.StandardInput.WriteLine(".exit");
            p.StandardInput.Flush();
            p.WaitForExit();
        }

        public static string ConstructFileDownlodURL(Type clazz, string methodName)
        {
            MethodInfo mi = null;
            foreach (MethodInfo m in clazz.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (m.Name == methodName && m.GetParameters().Length == 0)
                {
                    mi = m;
                    break;
                }
            }
            if (mi == null)
                throw new Exception("Unable to register File Download method that is not public static with no parameters");
            return "/FileDownloader.ashx?clazz=" + clazz.FullName + "&MethodName=" + methodName;
        }

        public static string ExecuteProgram(string program, string arguements, bool readOutput)
        {
            Process p = new Process();
            if (program != "/bin/cat")
            {
                if (program.EndsWith("sudo") && !arguements.StartsWith("-"))
                {
                    if (!ProcessSecurityControl.Current.SecurityClosed)
                    {
                        program = arguements.Split(' ')[0];
                        arguements = arguements.Substring(arguements.IndexOf(" ") + 1);
                    }
                }
            }
            p.StartInfo = new ProcessStartInfo(program, arguements);
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            string ret = null;
            if (readOutput)
                ret = p.StandardOutput.ReadToEnd();
            return ret;
        }

        //Called to locate a type by its name, this scans through all assemblies 
        //which by default Type.Load does not perform.
        public static Type LocateType(string typeName)
        {
            Type t = null;
            lock (_TYPE_CACHE)
            {
                if (_TYPE_CACHE.ContainsKey(typeName))
                    t = (Type)_TYPE_CACHE[typeName].Value;
            }
            if (t == null)
            {
                t = Type.GetType(typeName, false, true);
                if (t == null)
                {
                    foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        try
                        {
                            if (ass.GetName().Name != "mscorlib" && !ass.GetName().Name.StartsWith("System.") && ass.GetName().Name != "System" && !ass.GetName().Name.StartsWith("Microsoft"))
                            {
                                t = ass.GetType(typeName, false, true);
                                if (t != null)
                                    break;
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
                lock (_TYPE_CACHE)
                {
                    if (!_TYPE_CACHE.ContainsKey(typeName))
                        _TYPE_CACHE.Add(typeName, new CachedItemContainer(t));
                }
            }
            return t;
        }

        //Called to locate all child classes of a given parent type
        public static List<Type> LocateTypeInstances(Type parent)
        {
            List<Type> ret = null;
            lock (_INSTANCES_CACHE)
            {
                if (_INSTANCES_CACHE.ContainsKey(parent.FullName))
                    ret = (List<Type>)_INSTANCES_CACHE[parent.FullName].Value;
            }
            if (ret == null)
            {
                ret = new List<Type>();
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (ass.GetName().Name!="mscorlib" && !ass.GetName().Name.StartsWith("System.") && ass.GetName().Name!="System" && !ass.GetName().Name.StartsWith("Microsoft"))
                        {
                            foreach (Type t in ass.GetTypes())
                            {
                                if (t.IsSubclassOf(parent) || (parent.IsInterface && new List<Type>(t.GetInterfaces()).Contains(parent)))
                                    ret.Add(t);
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
                lock (_INSTANCES_CACHE)
                {
                    if (!_INSTANCES_CACHE.ContainsKey(parent.FullName))
                        _INSTANCES_CACHE.Add(parent.FullName, new CachedItemContainer(ret));
                }
            }
            return ret;
        }

        //Called to locate all child classes of a given parent type for a specific assembly
        public static List<Type> LocateTypeInstances(Type parent, Assembly ass)
        {
            List<Type> ret = null;
            lock (_ASSEMBLY_INSTANCES_CACHE)
            {
                if (_ASSEMBLY_INSTANCES_CACHE.ContainsKey(ass.FullName))
                {
                    if (_ASSEMBLY_INSTANCES_CACHE[ass.FullName].ContainsKey(parent.FullName))
                    {
                        ret = (List<Type>)_ASSEMBLY_INSTANCES_CACHE[ass.FullName][parent.FullName].Value;
                    }
                }
            }
            if (ret == null)
            {
                ret = new List<Type>();
                try
                {
                    foreach (Type t in ass.GetTypes())
                    {
                        if (t.IsSubclassOf(parent) || (parent.IsInterface && new List<Type>(t.GetInterfaces()).Contains(parent)))
                            ret.Add(t);
                    }
                }
                catch (Exception e)
                {
                    if (e.Message != "The invoked member is not supported in a dynamic assembly.")
                    {
                        throw e;
                    }
                }
                lock (_ASSEMBLY_INSTANCES_CACHE)
                {
                    if (!_ASSEMBLY_INSTANCES_CACHE.ContainsKey(ass.FullName))
                        _ASSEMBLY_INSTANCES_CACHE.Add(ass.FullName, new Dictionary<string, CachedItemContainer>());
                    Dictionary<string, CachedItemContainer> tmp = _ASSEMBLY_INSTANCES_CACHE[ass.FullName];
                    _ASSEMBLY_INSTANCES_CACHE.Remove(ass.FullName);
                    if (!tmp.ContainsKey(parent.FullName))
                        tmp.Add(parent.FullName, new CachedItemContainer(ret));
                    _ASSEMBLY_INSTANCES_CACHE.Add(ass.FullName, tmp);
                }
            }
            return ret;
        }

        public static string ConvertObjectToXML(object obj)
        {
            return ConvertObjectToXML(obj, false);
        }
        //called to convert an object to an XML string
        public static string ConvertObjectToXML(object obj, bool stripXmlTag)
        {
            return XmlObjectConverter.ConvertObjectToXML(obj, stripXmlTag);
        }

        //called to convert an objet from and XML string
        public static object ConvertObjectFromXML(string xmlCode)
        {
            return XmlObjectConverter.LoadFromXML(xmlCode);
        }

        //called to open a stream of a given embedded resource, again searches through all assemblies
        public static Stream LocateEmbededResource(string name)
        {
            Stream ret = typeof(Utility).Assembly.GetManifestResourceStream(name);
            if (ret == null)
            {
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (ass.GetName().Name != "mscorlib" && !ass.GetName().Name.StartsWith("System.") && ass.GetName().Name != "System" && !ass.GetName().Name.StartsWith("Microsoft"))
                        {
                            ret = ass.GetManifestResourceStream(name);
                            if (ret != null)
                                break;
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
            return ret;
        }

        //returns a string containing the contents of an embedded resource
        public static string ReadEmbeddedResource(string name)
        {
            Stream s = LocateEmbededResource(name);
            string ret = "";
            if (s != null)
            {
                TextReader tr = new StreamReader(s);
                ret = tr.ReadToEnd();
                tr.Close();
            }
            return ret;
        }

        //returns the MD5 calculation of a binary chunk
        public static string CalculateMD5Sum(byte[] data)
        {
            byte[] output = MD5.Create().ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < output.Length; i++)
            {
                sb.Append(output[i].ToString("X2"));
            }

            return sb.ToString().ToLower();
        }

        public static string CalculateMD5Sum(string str)
        {
            return CalculateMD5Sum(ASCIIEncoding.ASCII.GetBytes(str));
        }

        //returns the unix timestamp of a date
        private static readonly DateTime _UTC = new DateTime(1970, 1, 1, 00, 00, 00);

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (long)dateTime.Subtract(_UTC).TotalMilliseconds;
        }

        public static DateTime UnixTimestampToDateTime(long timestamp)
        {
            return _UTC.AddMilliseconds(timestamp);
        }

        internal static bool _isSiteSetup;
        public static bool IsSiteSetup
        {
            get
            {
                return _isSiteSetup;
            }
        }

        //called to compare to string whill checkign for nulls
        public static bool StringsEqual(string str1, string str2)
        {
            if (str1 == null)
            {
                if (str2 != null)
                    return false;
                return true;
            }
            else if (str2 == null)
            {
                return false;
            }
            return str1.Equals(str2);
        }
    }
}
