using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OperatingSystemPropertyOverrideAttribute : Attribute
    {
        private string _type;
        public Type ObjectType
        {
            get { return Utility.LocateType(_type); }
        }

        private string _propertyName;
        public string PropertyName
        {
            get { return _propertyName; }
        }

        private string _osName;
        public string OsName
        {
            get
            {
                return _osName;
            }
        }


        private OSVersion _minVersion=null;
        public OSVersion MinVersion {
            get { return _minVersion; }
        }

        private OSVersion _maxVersion=null;
        public OSVersion MaxVersion
        {
            get { return _maxVersion; }
        }

        private OSVersion _specificVersion=null;
        public OSVersion SpecificVersion
        {
            get { return _specificVersion; }
        }

        private OSVersion[] _specificVersions=null;
        public OSVersion[] SpecificVersions
        {
            get { return _specificVersions; }
        }

        public OperatingSystemPropertyOverrideAttribute(string objectType, string propertyName, string OSName, string minVersion, string maxVersion)
        {
            _type = objectType;
            _propertyName = propertyName;
            _osName = OSName;
            _minVersion = new OSVersion(minVersion);
            if (maxVersion != null)
                _maxVersion = new OSVersion(maxVersion);
        }

        public OperatingSystemPropertyOverrideAttribute(string objectType, string propertyName, string OSName, string specificVersion)
        {
            _type = objectType;
            _propertyName = propertyName;
            _osName = OsName;
            _specificVersion = new OSVersion(specificVersion);
        }

        public OperatingSystemPropertyOverrideAttribute(string objectType, string propertyName, string OSName, string[] specificVersions)
        {
            _type = objectType;
            _propertyName = propertyName;
            _osName = OSName;
            _specificVersions=new OSVersion[specificVersions.Length];
            for (int x=0;x<specificVersions.Length;x++){
                _specificVersions[x] = new OSVersion(specificVersions[x]);
            }
        }

        public int GetMatchLevelForOperatingSystem(IOSDefinition os)
        {
            if (os == null)
                return -1;
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
