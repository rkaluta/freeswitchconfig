using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using System.Collections;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System
{
    public class JavascriptConstants
    {
        public static JavascriptConstants Current
        {
            get
            {
                if (Site.CurrentSite != null)
                {
                    if (Site.CurrentSite["Org.Reddragonit.FreeSwitchConfig.DataCore.System.JavascriptConstants.Current"] != null)
                    {
                        return (JavascriptConstants)Site.CurrentSite["Org.Reddragonit.FreeSwitchConfig.DataCore.System.JavascriptConstants.Current"];
                    }
                    else
                    {
                        Site.CurrentSite["Org.Reddragonit.FreeSwitchConfig.DataCore.System.JavascriptConstants.Current"] = new JavascriptConstants();
                    }
                }
                return new JavascriptConstants();
            }
        }

        private JavascriptConstants()
        {
            if (Settings.Current["JavascriptConstants", "Values"] != null)
                _vals = (Hashtable)Settings.Current["JavascriptConstants", "Values"];
            else
                _vals = new Hashtable();
        }

        private Hashtable _vals;
        public string this[string name]
        {
            get
            {
                if (_vals.ContainsKey(name))
                    return (string)_vals[name];
                return null;
            }
            set
            {
                _vals.Remove(name);
                if (value != null)
                    _vals.Add(name, value);
                Settings.Current["JavascriptConstants", "Values"] = _vals;
            }
        }

        public List<string> Keys {
            get
            {
                List<string> ret = new List<string>();
                foreach (string str in _vals.Keys)
                    ret.Add(str);
                return ret;
            }
        }

        public int Count
        {
            get { return _vals.Count; }
        }
    }
}
