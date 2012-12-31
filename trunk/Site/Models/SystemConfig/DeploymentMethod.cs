using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig
{
    [ModelJSFilePath("/resources/scripts/Core/SystemConfig/DeploymentControl.js")]
    [ModelRoute("/core/models/sysconfig/DeploymentMethod")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.CollectionView|ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.EditForm)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class DeploymentMethod : IModel
    {
        private const string USER_RIGHT = "Change Deployment Method";

        static DeploymentMethod()
        {
            UserRight.CreateRight(USER_RIGHT);
        }

        private IConfigDeployer _dep;

        [ReadOnlyModelProperty()]
        public string Name
        {
            get { return _dep.Name; }
        }

        [ReadOnlyModelProperty()]
        public string Description
        {
            get { return _dep.Description; }
        }

        private bool _isCurrent;
        [ViewIgnoreField()]
        public bool IsCurrent
        {
            get { return _isCurrent; }
            set { _isCurrent = value; }
        }

        private DeploymentMethod(IConfigDeployer deployer)
        {
            _dep = deployer;
            _isCurrent = id == CoreGenerator.CurrentDeploymentMethod.FullName;
        }

        [ModelLoadMethod()]
        public static DeploymentMethod Load(string typeName)
        {
            if (!User.Current.HasRight(USER_RIGHT))
                return null;
            IConfigDeployer dep =(IConfigDeployer)Utility.LocateType(typeName).GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            if (dep.IsValidToUse)
                return new DeploymentMethod(dep);
            return null;
        }

        [ModelLoadAllMethod()]
        public static List<DeploymentMethod> LoadAll()
        {
            if (!User.Current.HasRight(USER_RIGHT))
                return null;
            List<DeploymentMethod> ret = new List<DeploymentMethod>();
            foreach (Type t in Utility.LocateTypeInstances(typeof(IConfigDeployer)))
            {
                IConfigDeployer dep = (IConfigDeployer)t.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                if (dep.IsValidToUse)
                    ret.Add(new DeploymentMethod(dep));
            }
            if (ret.Count == 0)
                return null;
            return ret;
        }

        [ModelUpdateMethod()]
        public bool Update()
        {
            if (!User.Current.HasRight(USER_RIGHT))
                throw new UnauthorizedAccessException();
            if (this.IsCurrent)
            {
                if (CoreGenerator.CurrentDeploymentMethod.FullName != this.id)
                {
                    CoreGenerator.ChangeDeploymentMethod(_dep.GetType());
                    return true;
                }
            }
            return false;
        }

        #region IModel Members

        public string id
        {
            get { return _dep.GetType().FullName; }
        }

        #endregion
    }
}
