using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules
{
    public class ModuleDisabledEvent: IEvent
    {
        public IModule Module
        {
            get { return (IModule)this["Module"]; }
        }

        public ModuleDisabledEvent(IModule module)
        {
            _pars = new Dictionary<string, object>();
            _pars.Add("Module", module);
        }

        #region IEvent Members

        public string Name
        {
            get { return "ModuleDisabledEvent"; }
        }

        private Dictionary<string, object> _pars = new Dictionary<string,object>();

        public object this[string name]{ get { if (_pars.ContainsKey(name)) { return _pars[name]; } return null; } }

        public Dictionary<string, object>.KeyCollection Keys { get { return _pars.Keys; } }

        #endregion

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("ModuleName", Module.ModuleName);
        }

        public void LoadFromElement(XmlElement element)
        {
            _pars.Remove("Module");
            foreach (IModule mod in ModuleController.CurrentModules)
            {
                if (mod.ModuleName == element.Attributes["ModuleName"].Value)
                {
                    _pars.Add("Module", mod);
                    break;
                }
            }
        }

        #endregion
    }
}
