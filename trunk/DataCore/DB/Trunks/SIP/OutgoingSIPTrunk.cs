using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using System.Text.RegularExpressions;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks.SIP
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/OutgoingSIPTrunk.js")]
    [ModelRoute("/core/models/pbxconfig/OutgoingSIPTrunk")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Trunks")]
    public class OutgoingSIPTrunk : Gateway,IModel
    {
        private static Regex _regIP = new Regex("^"+Constants.IPADDRESS_REGEX_STRING+"$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static Regex _regDomain = new Regex(Constants.URL_REGEX_STRING, RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static Mutex _mut = new Mutex(false);

        public enum RegistrationTypes
        {
            tcp,
            udp
        }

        private string _realm;
        [Field(350, false)]
        [ModelFieldValidationRegex("((^"+Constants.IPADDRESS_REGEX_STRING+"$)|("+Constants.URL_REGEX_STRING+"))")]
        public string Realm
        {
            get { return _realm; }
            set {
                if (!_regIP.IsMatch(value) && !_regDomain.IsMatch(value))
                    throw new InvalidRealmException(value);
                _realm = value; 
            }
        }

        private string _userName;
        [Field(100,false)]
        [ModelRequiredField()]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password;
        [Field(250, false)]
        [ModelRequiredField()]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private RegistrationTypes _regType = RegistrationTypes.tcp;
        [Field(false)]
        [ModelRequiredField()]
        public RegistrationTypes RegistrationType
        {
            get { return _regType; }
            set { _regType = value; }
        }

        private bool _register = true;
        [Field(false)]
        [ModelRequiredField()]
        public bool Register
        {
            get { return _register; }
            set { _register = value; }
        }

        private string _originalProfileName;

        private SipProfile _profile;
        [ForeignField(false, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        public SipProfile Profile
        {
            get { return _profile; }
            set {
                _originalProfileName = (_originalProfileName == null ? value.Name : _originalProfileName);
                _profile = value; 
            }
        }

        private int _pingInterval = 25;
        [Field(false)]
        [ModelRequiredField()]
        public int PingInterval
        {
            get { return _pingInterval; }
            set { _pingInterval = value; }
        }

        private int _retrySeconds;
        [Field(false)]
        [ModelRequiredField()]
        public int RetrySeconds {
            get { return _retrySeconds; }
            set { _retrySeconds = value; }
        }

        private string _fromUser;
        [Field(500, true)]
        public string FromUser
        {
            get { return _fromUser; }
            set { _fromUser = value; }
        }

        private string _fromDomain;
        [Field(500,true)]
        public string FromDomain
        {
            get { return _fromDomain; }
            set { _fromDomain = value; }
        }

        private string _extension;
        [Field(500, true)]
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        private string _proxy;
        [Field(350, true)]
        public string Proxy
        {
            get { return _proxy; }
            set {
                if (value != null)
                {
                    if (!_regIP.IsMatch(value) && !_regDomain.IsMatch(value))
                        throw new InvalidProxyException(value);
                }
                _proxy = value; 
            }
        }

        private string _registerProxy;
        [Field(350, true)]
        public string RegisterProxy
        {
            get { return _registerProxy; }
            set
            {
                if (value != null)
                {
                    if (!_regIP.IsMatch(value) && !_regDomain.IsMatch(value))
                        throw new InvalidProxyException(value);
                }
                _registerProxy= value; 
            }
        }

        private int? _expireSeconds;
        [Field(true)]
        public int? ExpireSeconds
        {
            get { return _expireSeconds; }
            set { _expireSeconds = value; }
        }

        [ModelLoadMethod()]
        public static new OutgoingSIPTrunk Load(string name)
        {
            OutgoingSIPTrunk ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Gateway)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(Gateway),
                new SelectParameter[] { new EqualParameter("Name", name) });
            if (tmp.Count > 0)
                ret = (OutgoingSIPTrunk)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static new List<OutgoingSIPTrunk> LoadAll()
        {
            List<OutgoingSIPTrunk> ret = new List<OutgoingSIPTrunk>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(OutgoingSIPTrunk)).getConnection();
            foreach (OutgoingSIPTrunk ost in conn.SelectAll(typeof(OutgoingSIPTrunk)))
                ret.Add(ost);
            conn.CloseConnection();
            return ret;
        }

        [ModelSaveMethod()]
        public new bool Save()
        {
            bool ret = true;
            try
            {
                base.Save();
                ConfigurationController.RegisterDeployTrunkRedployment(Name);
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }

        [ModelDeleteMethod()]
        public new bool Delete()
        {
            bool ret = true;
            try
            {
                base.Delete();
                ConfigurationController.RegisterDestroyTrunkRedeployment(Name,Profile.Name);
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }

        [ModelUpdateMethod()]
        public new bool Update()
        {
            bool ret = true;
            try
            {
                base.Delete();
                ConfigurationController.RegisterDestroyTrunkRedeployment(OriginalName, _originalProfileName);
                ConfigurationController.RegisterDeployTrunkRedployment(Name);
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }

        #region IModel Members

        public string id
        {
            get { return Name; }
        }

        #endregion
    }
}
