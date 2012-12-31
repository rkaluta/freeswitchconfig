using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks.SIP
{
    [Table(Table.TableSettings.AutoDeleteParent)]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/IncomingSIPTrunk.js")]
    [ModelRoute("/core/models/pbxconfig/IncomingSIPTrunk")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelNamespace("FreeswitchConfig.Trunks")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.View|ModelBlockJavascriptGenerations.CollectionView)]
    public class IncomingSIPTrunk : ExtensionNumber,IModel
    {

        private string _name;
        [Field(100,false)]
        [ModelRequiredField()]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _password;
        [Field(50, false)]
        [ModelRequiredField()]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _internalCallerIDName;
        [Field(50, true)]
        public string InternalCallerIDName
        {
            get { return _internalCallerIDName; }
            set { _internalCallerIDName = value; }
        }

        private string _internalCallerID;
        [Field(25, true)]
        public string InternalCallerID
        {
            get { return _internalCallerID; }
            set { _internalCallerID = value; }
        }

        private string _externalCallerIDName;
        [Field(50, true)]
        public string ExternalCallerIDName
        {
            get { return _externalCallerIDName; }
            set { _externalCallerIDName = value; }
        }

        private string _externalCallerID;
        [Field(25, true)]
        public string ExternalCallerID
        {
            get { return _externalCallerID; }
            set { _externalCallerID = value; }
        }

        private int _maxLines=5;
        [Field(false)]
        [ModelRequiredField()]
        public int MaxLines
        {
            get { return _maxLines; }
            set { _maxLines = value; }
        }

        private Domain _domain;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        [ModelPropertyLazyLoadExternalModel()]
        public Domain Domain
        {
            get { return _domain; }
            set
            {
                _domain = value;
            }
        }

        protected override bool _ignoreExtensionRegex
        {
            get
            {
                return true;
            }
        }

        protected override string _overriddenRegexValidation
        {
            get
            {
                return "^[A-Za-z\\d_\\.]{1,50}$";
            }
        }

        protected bool _isValid
        {
            get
            {
                return Domain.InternalProfile.Context.Name == Context.Name || Domain.ExternalProfile.Context.Name == Context.Name;
            }
        }

        [ModelSaveMethod()]
        public new bool Save()
        {
            bool ret = _isValid;
            if (ret)
            {
                try
                {
                    base.Save();
                    ConfigurationController.RegisterDeployIncomingTrunkRedployment(Number, Domain.Name);
                }
                catch (Exception e)
                {
                    ret = false;
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                }
            }
            return ret;
        }

        [ModelDeleteMethod()]
        public new bool Delete()
        {
            bool ret = false;
            try
            {
                base.Delete();
                ConfigurationController.RegisterDestroyIncomingTrunkRedeployment(Number, Domain.Name);
            }
            catch (Exception e)
            {
                ret = false;
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
            }
            return ret;
        }

        [ModelUpdateMethod()]
        public new bool Update()
        {
            bool ret = _isValid;
            if (ret)
            {
                try
                {
                    base.Update();
                    if (base.OriginalNumber != Number)
                        ConfigurationController.RegisterDestroyIncomingTrunkRedeployment(base.OriginalNumber, Domain.Name);
                    ConfigurationController.RegisterDeployIncomingTrunkRedployment(Number, Domain.Name);
                }
                catch (Exception e)
                {
                    ret = false;
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                }
            }
            return ret;
        }

        [ModelLoadMethod()]
        public new static IncomingSIPTrunk Load(string number)
        {
            IncomingSIPTrunk ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(IncomingSIPTrunk)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(IncomingSIPTrunk),
                new SelectParameter[] { new EqualParameter("Number", number),
                new EqualParameter("Context",Context.Current)});
            if (tmp.Count > 0)
                ret = (IncomingSIPTrunk)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public new static List<IncomingSIPTrunk> LoadAll()
        {
            List<IncomingSIPTrunk> ret = new List<IncomingSIPTrunk>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(IncomingSIPTrunk)).getConnection();
            foreach (IncomingSIPTrunk ist in conn.SelectAll(typeof(IncomingSIPTrunk)))
                ret.Add(ist);
            conn.CloseConnection();
            return ret;
        }

        [ModelListMethod("/core/models/search/pbxconfig/IncominSIPTrunk/Available")]
        public new static List<IncomingSIPTrunk> LoadAllAvailable()
        {
            List<IncomingSIPTrunk> ret = new List<IncomingSIPTrunk>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(IncomingSIPTrunk)).getConnection();
            foreach (IncomingSIPTrunk ist in conn.Select(typeof(IncomingSIPTrunk),
                new SelectParameter[] { new EqualParameter("Domain",Domain.Current),
                new EqualParameter("Context",Context.Current)}))
                ret.Add(ist);
            conn.CloseConnection();
            return ret;
        }

        public static IncomingSIPTrunk Load(string number,Domain domain)
        {
            IncomingSIPTrunk ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(IncomingSIPTrunk)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(IncomingSIPTrunk),
                new SelectParameter[] { new EqualParameter("Number", number),
                new EqualParameter("Domain",domain)});
            if (tmp.Count > 0)
                ret = (IncomingSIPTrunk)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        #region IModel Members

        public string id
        {
            get { return Number; }
        }

        #endregion
    }
}
