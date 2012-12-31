using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true)]
    public class BlockedSudoPathAttribute : Attribute
    {

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

        public BlockedSudoPathAttribute(string path, string OSName, string minVersion, string maxVersion)
        {
            _path = path;
            _osName = OSName;
            _minVersion = new OSVersion(minVersion);
            if (maxVersion != null)
                _maxVersion = new OSVersion(maxVersion);
            else
                throw new Exception("Unable to Block a Sudo path without a maximum version");
        }

        public BlockedSudoPathAttribute(string path, string OSName, string specificVersion)
        {
            _osName = OsName;
            _specificVersion = new OSVersion(specificVersion);
        }

        public BlockedSudoPathAttribute(string path, string OSName, string[] specificVersions)
        {
            _path = path;
            _osName = OSName;
            _specificVersions = new OSVersion[specificVersions.Length];
            for (int x = 0; x < specificVersions.Length; x++)
            {
                _specificVersions[x] = new OSVersion(specificVersions[x]);
            }
        }

        public bool IsBlocked
        {
            get
            {
                IOSDefinition os = Utility.OperatingSystem;
                if (os == null)
                    return false;
                if (OsName == os.OsName)
                {
                    if (SpecificVersion != null)
                        return (os.Version == SpecificVersion ? true : false);
                    else if (SpecificVersions != null)
                    {
                        foreach (OSVersion ver in SpecificVersions)
                        {
                            if (ver == os.Version)
                                return true;
                        }
                    }
                    else
                    {
                        if ((os.Version >= MinVersion) && (os.Version <= MaxVersion))
                            return true;
                    }
                }
                return false;
            }
        }
    }
}
