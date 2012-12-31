using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig
{
    [ModelJSFilePath("/resources/scripts/Core/SystemConfig/SiteModules.js")]
    [ModelRoute("/core/models/sysconfig/SiteModule")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.CollectionView | ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.EditForm)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class SiteModule : IModel
    {
        private IModule _module;

        private bool _enabled;
        [ViewIgnoreField()]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        [ReadOnlyModelProperty()]
        public string ModuleName {
            get { return _module.ModuleName; }
        }

        [ReadOnlyModelProperty()]
        public string Description {
            get { return _module.Description; }
        }

        private SiteModule(IModule module)
        {
            _module = module;
            _enabled = ModuleController.Current.IsModuleEnabled(_module.ModuleName);
        }

        [ModelLoadMethod()]
        public static SiteModule Load(string name)
        {
            IModule mod = null;
            foreach (IModule m in ModuleController.CurrentModules)
            {
                if (m.ModuleName == name)
                {
                    mod = m;
                    break;
                }
            }
            if (mod != null)
                return new SiteModule(mod);
            return null;
        }

        [ModelLoadAllMethod()]
        public static List<SiteModule> LoadAll()
        {
            List<SiteModule> ret = new List<SiteModule>();
            foreach (IModule mod in ModuleController.CurrentModules)
                ret.Add(new SiteModule(mod));
            return ret;
        }

        [ModelUpdateMethod()]
        public bool Update()
        {
            if (_enabled)
                ModuleController.Current.EnableModule(_module.ModuleName);
            else
                ModuleController.Current.DisableModule(_module.ModuleName);
            return true;
        }

        #region IModel Members

        public string id
        {
            get { return _module.ModuleName; }
        }
        #endregion
    }
}
