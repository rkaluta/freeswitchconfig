using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.BackBoneDotNet;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig
{
    [ModelRoute("/core/models/sysconfig/FreeswitchModuleConfiguration")]
    [ModelJSFilePath("/resources/scripts/Core/SystemConfig/FreeswitchModules.js")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.Collection|ModelBlockJavascriptGenerations.CollectionView|ModelBlockJavascriptGenerations.View)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class FreeswitchModuleConfiguration : IModel
    {
        private FreeSwitchModuleFile _fsmf;

        [ReadOnlyModelProperty()]
        public string Name
        {
            get { return _fsmf.FileName; }
        }

        private string _configurationSection;
        public string ConfigurationSection
        {
            get { return _configurationSection; }
            set { _configurationSection = value; }
        }

        private FreeswitchModuleConfiguration(string filename)
        {
            _fsmf = FreeSwitchModuleFile.Load(filename);
            if (_fsmf.Data == null)
                _configurationSection = null;
            else
                _configurationSection = _fsmf.File.ConfigurationSectionString;
        }

        [ModelLoadMethod()]
        public static FreeswitchModuleConfiguration Load(string filename)
        {
            if (!User.Current.HasRight(Constants.CHANGE_FREESWITCH_MODULE_SETTINGS_RIGHT))
                return null;
            return new FreeswitchModuleConfiguration(filename);
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> GetFilesList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            foreach (string str in FreeSwitchModuleFile.FileNames)
                ret.Add(new sModelSelectOptionValue(str, str));
            return ret;
        }

        [ModelUpdateMethod()]
        public bool Update()
        {
            if (!User.Current.HasRight(Constants.CHANGE_FREESWITCH_MODULE_SETTINGS_RIGHT))
                return false;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ConfigurationSection);
            _fsmf.File = new sFreeSwitchModuleFile(doc.ChildNodes[1].Attributes["name"].Value,
                                doc.ChildNodes[1].Attributes["description"].Value,
                                doc.ChildNodes[1].OuterXml);
            _fsmf.Update();
            ConfigurationController.RegisterModuleFileRedeployment(id);
            return true;
        }


        #region IModel Members

        public string id
        {
            get { return _fsmf.FileName; }
        }
        #endregion
    }
}
