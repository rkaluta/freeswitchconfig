using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig
{
    [ModelJSFilePath("/resources/scripts/Core/SystemConfig/Setup.js")]
    [ModelJSFilePath("/resources/scripts/Core/SystemConfig/SystemSettings.js")]
    [ModelRoute("/core/models/sysconfig/SystemSetting")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [EditButtonDefinition("span",new string[]{"pencil","button"},null)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class SystemSetting : IModel
    {
        private string _name;

        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _valueType;
        [ReadOnlyModelProperty()]
        [ViewIgnoreField()]
        public string ValueType
        {
            get { return _valueType; }
        }

        private SystemSetting(string name)
        {
            _name = name;
            _value = Settings.Current[name].ToString();
            _valueType = Settings.Current[name].GetType().FullName;
        }

        [ModelLoadMethod()]
        public static SystemSetting Load(string name)
        {
            if (User.Current != null)
            {
                if (!User.Current.HasRight(Constants.SYSTEM_CONTROL_RIGHT))
                    return null;
            }
            else if (Utility.IsSiteSetup)
                return null;
            return new SystemSetting(name);
        }

        [ModelLoadAllMethod()]
        public static List<SystemSetting> LoadAll()
        {
            if (User.Current != null)
            {
                if (!User.Current.HasRight(Constants.SYSTEM_CONTROL_RIGHT))
                    return null;
            }
            else if (Utility.IsSiteSetup)
                return null;
            List<SystemSetting> ret = new List<SystemSetting>();
            foreach (string str in Settings.Current.EditableSettings)
                ret.Add(new SystemSetting(str));
            return ret;
        }

        [ModelUpdateMethod()]
        public bool Update()
        {
            if (User.Current != null)
            {
                if (!User.Current.HasRight(Constants.SYSTEM_CONTROL_RIGHT))
                    throw new UnauthorizedAccessException();
            }
            else if (Utility.IsSiteSetup)
                throw new UnauthorizedAccessException();
            try
            {
                switch (_valueType)
                {
                    case "System.String":
                        Settings.Current[_name] = _value;
                        break;
                    case "System.Boolean":
                        Settings.Current[_name] = bool.Parse(_value);
                        break;
                    default:
                        throw new Exception("Unusable system setting type for editing");
                        break;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        #region IModel Members

        public string id
        {
            get { return _name; }
        }

        #endregion
    }
}
