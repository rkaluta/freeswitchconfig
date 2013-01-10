using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Models.Core
{
    [ModelJSFilePath("/resources/scripts/Core/OperatingSystem.js")]
    [ModelJSFilePath("/resources/scripts/Core/SystemConfig/Setup.js")]
    [ModelRoute("/core/models/core/OperatingSystem")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.CollectionView|ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.View)]
    [ModelNamespace("FreeswitchConfig.System")]
    public class OperatingSystem : IModel
    {
        private IOSDefinition _os;

        [ReadOnlyModelProperty()]
        public string OsName
        {
            get { return _os.OsName; }
        }

        [ReadOnlyModelProperty()]
        public bool IsCurrentOS
        {
            get { return _os.IsCurrentOS; }
        }

        [ReadOnlyModelProperty()]
        public OSVersion Version
        {
            get { return _os.Version; }
        }

        [ReadOnlyModelProperty()]
        public bool UsesMappedSockets
        {
            get { return _os.UsesMappedSockets; }
        }

        [ReadOnlyModelProperty()]
        public bool CanUseSysLog
        {
            get { return _os.CanUseSysLog; }
        }

        [ReadOnlyModelProperty()]
        public bool UsesSudo
        {
            get { return _os.UsesSudo; }
        }

        private OperatingSystem(IOSDefinition os)
        {
            _os = os;
        }

        [ModelLoadMethod()]
        public static OperatingSystem Load(string typeName)
        {
            IOSDefinition os = (IOSDefinition)Utility.LocateType(typeName).GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            return new OperatingSystem(os);
        }

        [ModelLoadAllMethod()]
        public static List<OperatingSystem> LoadAll()
        {
            List<OperatingSystem> ret = new List<OperatingSystem>();
            foreach (Type t in Utility.LocateTypeInstances(typeof(IOSDefinition)))
                ret.Add(new OperatingSystem((IOSDefinition)t.GetConstructor(Type.EmptyTypes).Invoke(new object[0])));
            return ret;
        }

        #region IModel Members

        public string id
        {
            get { return _os.GetType().FullName; }
        }

        #endregion
    }
}
