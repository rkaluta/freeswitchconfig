/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 09/11/2009
 * Time: 8:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;

using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.EmbeddedWebServer.Components;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System
{
	/// <summary>
	/// Description of Settings.
	/// </summary>
	public class Settings : IBackgroundOperationContainer
	{
        private const string MODULE_STATUS = "{0}_ENABLED";
        private const int CACHE_MINUTES = 60;
		private Connection _conn=null;
        private object _lock = new object();
		private static Settings _current;
        private static object Lock = new object();
        private static Dictionary<string, CachedItemContainer> _cache;

        [BackgroundOperationCall(0,-1,-1,-1,BackgroundOperationDaysOfWeek.All)]
        [BackgroundOperationCall(10,-1,-1,-1,BackgroundOperationDaysOfWeek.All)]
        [BackgroundOperationCall(20,-1,-1,-1,BackgroundOperationDaysOfWeek.All)]
        [BackgroundOperationCall(30,-1,-1,-1,BackgroundOperationDaysOfWeek.All)]
        [BackgroundOperationCall(40,-1,-1,-1,BackgroundOperationDaysOfWeek.All)]
        [BackgroundOperationCall(50,-1,-1,-1,BackgroundOperationDaysOfWeek.All)]
        public static void CleanCache()
        {
            Monitor.Enter(Lock);
            try
            {
                string[] keys = new string[_cache.Count];
                _cache.Keys.CopyTo(keys, 0);
                DateTime now = DateTime.Now;
                foreach (string str in keys)
                {
                    if (now.Subtract(_cache[str].LastAccess).TotalMinutes > CACHE_MINUTES)
                        _cache.Remove(str);
                }
            }
            finally
            {
                Monitor.Exit(Lock);
            }
        }
		
		private Settings()
		{
            _cache = new Dictionary<string, CachedItemContainer>();
            ConnectionPool pool = ConnectionPoolManager.GetPool(typeof(SystemSetting));
            if (pool==null)
                Log.Trace("Unable to obtain a pool for the type " + typeof(SystemSetting).FullName);
			_conn = pool.GetConnection();
		}
		
		~Settings(){
            if (_conn != null)
            {
                try
                {
                    _conn.CloseConnection();
                }
                catch (Exception e) {
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                    Log.Error(e);
                }
            }
		}
		
		public static Settings Current{
			get{
                Monitor.Enter(Lock);
				if (_current==null)
					_current=new Settings();
                Monitor.Exit(Lock);
				return _current;
			}
		}

        public bool SetupComplete
        {
            get
            {
                if (this["IsSetupComplete"] == null)
                    return false;
                return (bool)this["IsSetupComplete"];
            }
            set { this["IsSetupComplete"] = value; }
        }

        public void EnableModule(string name,bool callEnable)
        {
            Log.Trace("Enabling " + name + " module in system settings.");
            Current[string.Format(MODULE_STATUS, name)] = true;
            if (callEnable)
            {
                Log.Trace("Making call to enable " + name + " module in module controller");
                ModuleController.Current.EnableModule(name);
            }
        }

        public void DisableModule(string name)
        {
            Log.Trace("Disabling " + name + " module in system settings.");
            Current[string.Format(MODULE_STATUS, name)] = false;
            ModuleController.Current.DisableModule(name);
        }

        public bool IsModuleEnabled(string name)
        {
            Log.Trace("Checking if module " + name + " is enabled.");
            object obj = this[string.Format(MODULE_STATUS, name)];
            if (obj == null)
                return false;
            return (bool)obj;
        }

        public bool IsModuleEnabledSet(string name)
        {
            Log.Trace("Checking if module " + name + " has enabled set in the database");
            object obj = this[string.Format(MODULE_STATUS, name)];
            if (obj == null)
                return false;
            return true;
        }

        public List<string> EditableSettings
        {
            get
            {
                List<string> ret = new List<string>();
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System",
                    "SELECT st.Name FROM SystemSetting st WHERE " +
                    "st.Name NOT LIKE '%_ENABLED' AND " +
                    "st.ValueType IN ('System.String','System.Boolean')");
                cq.Execute();
                while (cq.Read())
                {
                    ret.Add(cq[0].ToString());
                }
                cq.Close();
                return ret;
            }
        }

		public object this[string name]{
			get{
                Log.Trace("Getting system setting " + name);
                object ret = null;
                bool search = true;
                Monitor.Enter(Lock);
                if (_cache.ContainsKey(name))
                {
                    ret = _cache[name].Value;
                    search = false;
                }
                Monitor.Exit(Lock);
                if (search)
                {
                    Monitor.Enter(_lock);
                    List<Org.Reddragonit.Dbpro.Structure.Table> tmp = _conn.Select(typeof(SystemSetting),
                                                                                   new SelectParameter[] { new EqualParameter("Name", name) });
                    _conn.Close();
                    Monitor.Exit(_lock);
                        if (tmp.Count > 0)
                        {
                            if (((SystemSetting)tmp[0]).Value != null)
                                ret = Utility.ConvertObjectFromXML(((SystemSetting)tmp[0]).Value);
                        }
                    Monitor.Enter(Lock);
                    if (_cache.ContainsKey(name))
                        _cache.Remove(name);
                    _cache.Add(name, new CachedItemContainer(ret));
                    Monitor.Exit(Lock);
                }
                return ret;
			}
            set
            {
                Log.Trace("Setting system setting " + name);
                Monitor.Enter(_lock);
                List<Org.Reddragonit.Dbpro.Structure.Table> tmp = _conn.Select(typeof(SystemSetting),
                                                                               new SelectParameter[] { new EqualParameter("Name", name) });
                _conn.Close();
                if ((tmp.Count > 0) && (value == null))
                {
                    _conn.Delete(tmp[0]);
                }
                else if (value != null)
                {
                    string val = Utility.ConvertObjectToXML(value);
                    if (tmp.Count > 0)
                    {
                        ((SystemSetting)tmp[0]).Value = val;
                        ((SystemSetting)tmp[0]).ValueType = value.GetType().FullName;
                        tmp[0].Update();
                    }
                    else
                    {
                        SystemSetting ss = new SystemSetting();
                        ss.Name = name;
                        ss.Value = val;
                        ss.ValueType = value.GetType().FullName;
                        _conn.Save(ss);
                    }
                }
                _conn.Commit();
                Monitor.Enter(Lock);
                if (_cache.ContainsKey(name))
                    _cache.Remove(name);
                _cache.Add(name, new CachedItemContainer(value));
                Monitor.Exit(Lock);
                Monitor.Exit(_lock);
            }
		}

        public object this[string moduleName,string name]
        {
            get
            {
                Log.Trace("Getting module setting " + name + " for module " + moduleName);
                object ret = null;
                bool search = true;
                Monitor.Enter(Lock);
                if (_cache.ContainsKey(moduleName))
                {
                    if (((Dictionary<string, object>)_cache[moduleName].Value).ContainsKey(name))
                    {
                        ret = ((Dictionary<string, object>)_cache[moduleName].Value)[name];
                        search = false;
                    }
                }
                Monitor.Exit(Lock);
                if (search)
                {
                    Monitor.Enter(_lock);
                    List<Org.Reddragonit.Dbpro.Structure.Table> tmp = _conn.Select(typeof(ModuleSetting),
                                                                                   new SelectParameter[] { new EqualParameter("Name", name),
                                                                               new EqualParameter("ModuleName",moduleName)});
                    _conn.Close();
                    Monitor.Exit(_lock);
                        if (tmp.Count > 0)
                        {
                            if (((ModuleSetting)tmp[0]).Value != null)
                                ret = Utility.ConvertObjectFromXML(((ModuleSetting)tmp[0]).Value);
                        }
                    Monitor.Enter(Lock);
                    if (!_cache.ContainsKey(moduleName))
                        _cache.Add(moduleName, new CachedItemContainer(new Dictionary<string, object>()));
                    Dictionary<string, object> tmpd = (Dictionary<string, object>)_cache[moduleName].Value;
                    _cache.Remove(moduleName);
                    if (tmpd.ContainsKey(name))
                        tmpd.Remove(name);
                    tmpd.Add(name,ret);
                    _cache.Add(moduleName, new CachedItemContainer(tmpd));
                    Monitor.Exit(Lock);
                }
                return ret;
            }
            set
            {
                Log.Trace("Setting module setting " + name + " for module " + moduleName);
                Monitor.Enter(_lock);
                List<Org.Reddragonit.Dbpro.Structure.Table> tmp = _conn.Select(typeof(ModuleSetting),
                                                                               new SelectParameter[] { new EqualParameter("Name", name),
                                                                               new EqualParameter("ModuleName",moduleName)});
                _conn.Close();
                if ((tmp.Count > 0) && (value == null))
                {
                    _conn.Delete(tmp[0]);
                }
                else if (value != null)
                {
                    string val = Utility.ConvertObjectToXML(value);
                    if (tmp.Count > 0)
                    {
                        ((ModuleSetting)tmp[0]).Value = val;
                        ((ModuleSetting)tmp[0]).ValueType = value.GetType().FullName;
                        tmp[0].Update();
                    }
                    else
                    {
                        ModuleSetting ss = new ModuleSetting();
                        ss.Name = name;
                        ss.Value = val;
                        ss.ValueType = value.GetType().FullName;
                        ss.ModuleName = moduleName;
                        _conn.Save(ss);
                    }
                }
                _conn.Commit();
                Monitor.Enter(Lock);
                if (!_cache.ContainsKey(moduleName))
                    _cache.Add(moduleName, new CachedItemContainer(new Dictionary<string, object>()));
                Dictionary<string, object> tmpd = (Dictionary<string, object>)_cache[moduleName].Value;
                _cache.Remove(moduleName);
                if (tmpd.ContainsKey(name))
                    tmpd.Remove(name);
                tmpd.Add(name,value);
                _cache.Add(moduleName, new CachedItemContainer(tmpd));
                Monitor.Exit(Lock);
                Monitor.Exit(_lock);
            }
        }

        public List<string> GetSettingsForModule(string moduleName)
        {
            List<string> ret = new List<string>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System",
                "SELECT ms.Name FROM ModuleSetting ms WHERE ms.ModuleName = @moduleName");
            cq.Execute(new IDbDataParameter[] { cq.CreateParameter("@moduleName", moduleName) });
            while (cq.Read())
                ret.Add(cq[0].ToString());
            cq.Close();
            return ret;
        }
	}
}
