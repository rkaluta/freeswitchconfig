using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true)]
    public class RequiredSudoPathAttribute : Attribute
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
        }

        private string _osName=null;
        public string OsName
        {
            get
            {
                return _osName;
            }
        }

        private string _moduleName=null;
        public string ModuleName
        {
            get { return _moduleName; }
        }


        private OSVersion _minVersion = null;
        public OSVersion MinVersion
        {
            get { return _minVersion; }
        }

        private OSVersion _maxVersion = null;
        public OSVersion MaxVersion
        {
            get { return _maxVersion; }
        }

        private OSVersion _specificVersion = null;
        public OSVersion SpecificVersion
        {
            get { return _specificVersion; }
        }

        private OSVersion[] _specificVersions = null;
        public OSVersion[] SpecificVersions
        {
            get { return _specificVersions; }
        }

        private string _reason;
        public string Reason
        {
            get { return _reason; }
        }

        public RequiredSudoPathAttribute(string name,string path,string reason)
        {
            _name = name;
            _path = path;
            _reason = reason;
        }

        public RequiredSudoPathAttribute(string name, string path, string moduleName, string reason)
        {
            _name = name;
            _path = path;
            _moduleName = moduleName;
            _reason = reason;
        }

        public RequiredSudoPathAttribute(string name, string path, string OSName, string minVersion, string maxVersion, string reason)
        {
            _name = name;
            _path = path;
            _osName = OSName;
            _minVersion = new OSVersion(minVersion);
            if (maxVersion != null)
                _maxVersion = new OSVersion(maxVersion);
            _reason = reason;
        }

        public RequiredSudoPathAttribute(string name, string path, string OSName, string specificVersion, string reason)
        {
            _name = name;
            _path = path;
            _osName = OsName;
            _specificVersion = new OSVersion(specificVersion);
            _reason = reason;
        }

        public RequiredSudoPathAttribute(string name, string path, string OSName, string[] specificVersions, string reason)
        {
            _name = name;
            _path = path;
            _osName = OSName;
            _specificVersions = new OSVersion[specificVersions.Length];
            for (int x = 0; x < specificVersions.Length; x++)
            {
                _specificVersions[x] = new OSVersion(specificVersions[x]);
            }
            _reason = reason;
        }

        public bool IsValid
        {
            get{
                if (_moduleName != null)
                    return ModuleController.Current.IsModuleEnabled(_moduleName);
                return true;
            }
        }

        public int GetMatchLevelForOperatingSystem(IOSDefinition os)
        {
            if (os == null)
                return -1;
            if (OsName == null)
            {
                if (_moduleName != null)
                    return (ModuleController.Current.IsModuleEnabled(_moduleName) ? 4 : 0);
                return 0;
            }
            if (OsName == os.OsName)
            {
                if (SpecificVersion != null)
                    return (os.Version == SpecificVersion ? 4 : -1);
                else if (SpecificVersions != null)
                {
                    foreach (OSVersion ver in SpecificVersions)
                    {
                        if (ver == os.Version)
                            return 3;
                    }
                }
                else
                {
                    if ((os.Version >= MinVersion) && (MaxVersion == null))
                        return 1;
                    else if ((os.Version >= MinVersion) && (MaxVersion != null))
                        return (os.Version <= MaxVersion ? 2 : -1);
                }
            }
            return -1;
        }
    }
}
