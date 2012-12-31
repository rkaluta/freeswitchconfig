using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using System.Net;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.BackBoneDotNet;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/loggedIn.js")]
    [ModelJSFilePath("/mobile/resources/scripts/loggedIn.js")]
    [ModelRoute("/core/models/core/SipProfile")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class SipProfile : Org.Reddragonit.Dbpro.Structure.Table,IModel
    {
        public static SipProfile Current
        {
            get
            {
                if (Domain.Current != null)
                {
                    return (Domain.Current.InternalProfile.Context.Name == Context.Current.Name ? Domain.Current.InternalProfile : Domain.Current.ExternalProfile);
                }
                return null;
            }
        }

        private string _name;
        [PrimaryKeyField(false,250)]
        [ModelRequiredField()]
        [ReadOnlyModelProperty()]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Context _context;
        [ForeignField(false, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        [ModelPropertyLazyLoadExternalModel()]
        public Context Context
        {
            get { return _context; }
            set { _context = value; }
        }

        private int _sipPort;
        [Field(false)]
        [ModelRequiredField()]
        [ModelFieldValidationRegex(Constants.PORT_RANGE_REGEX)]
        public int SIPPort
        {
            get { return _sipPort; }
            set { _sipPort = value;}
        }

        private sNetworkCard _sipInterface;
        [Field("SIP_INTERFACE",FieldType.STRING,false,250)]
        [ModelPropertyLazyLoadExternalModel()]
        public sNetworkCard SIPInterface
        {
            get { return _sipInterface; }
            set { _sipInterface = value;}
        }

        private sNetworkCard _rtpInterface;
        [Field("RTP_INTERFACE", FieldType.STRING, false, 250)]
        [ModelPropertyLazyLoadExternalModel()]
        public sNetworkCard RTPInterface
        {
            get { return _rtpInterface; }
            set { _rtpInterface = value;}
        }

        [ModelIgnoreProperty()]
        public string this[SipProfileSettingTypes type]
        {
            get { return SipProfileSetting.GetSettingValue(type, this); }
            set { SipProfileSetting.SetSettingValue(type, value, this); }
        }

        [ModelSaveMethod()]
        public new bool Save()
        {
            if (User.Current != null)
            {
                if (!User.Current.HasRight(Constants.DOMAIN_PROFILE_SETUP_RIGHT))
                    throw new UnauthorizedAccessException();
            }
            else if (Utility.IsSiteSetup)
                throw new UnauthorizedAccessException();
            bool ret = true;
            try
            {
                base.Save();
                this[SipProfileSettingTypes.sip_port] = SIPPort.ToString();
                this[SipProfileSettingTypes.rtp_ip] = RTPInterface.IPAddress;
                this[SipProfileSettingTypes.sip_ip] = SIPInterface.IPAddress;
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
            if (User.Current != null)
            {
                if (!User.Current.HasRight(Constants.DOMAIN_PROFILE_SETUP_RIGHT))
                    throw new UnauthorizedAccessException();
            }
            else if (Utility.IsSiteSetup)
                throw new UnauthorizedAccessException();
            bool ret = true;
            try
            {
                base.Update();
                this[SipProfileSettingTypes.sip_port] = SIPPort.ToString();
                this[SipProfileSettingTypes.rtp_ip] = RTPInterface.IPAddress;
                this[SipProfileSettingTypes.sip_ip] = SIPInterface.IPAddress;
            }
            catch (Exception e)
            {
                ret = false;
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
            }
            return ret;
        }

        [ModelDeleteMethod()]
        public new bool Delete(){
            if (User.Current != null)
            {
                if (!User.Current.HasRight(Constants.DOMAIN_PROFILE_SETUP_RIGHT))
                    throw new UnauthorizedAccessException();
            }
            else if (Utility.IsSiteSetup)
                throw new UnauthorizedAccessException();
            bool ret = true;
            try
            {
                base.Delete();
            }
            catch (Exception e)
            {
                ret = false;
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
            }
            return ret;
        }

        public SipProfile() { }

        [ModelLoadMethod()]
        public static SipProfile Load(string name)
        {
            SipProfile ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(SipProfile)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(SipProfile),
                new SelectParameter[] { new EqualParameter("Name", name) });
            if (tmp.Count > 0)
                ret = (SipProfile)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<SipProfile> LoadAll()
        {
            List<SipProfile> ret = new List<SipProfile>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(SipProfile)).getConnection();
            foreach (SipProfile con in conn.SelectAll(typeof(SipProfile)))
                ret.Add(con);
            conn.CloseConnection();
            return ret;
        }

        [ModelListMethod("/core/models/search/core/CurrentSipProfile")]
        public static List<SipProfile> LoadCurrent()
        {
            List<SipProfile> ret = new List<SipProfile>();
            ret.Add(SipProfile.Current);
            return ret;
        }

        [ModelListMethod("/core/models/search/core/CurrentlyAvailableSipProfiles")]
        public static List<SipProfile> LoadCurrentlyAvailable()
        {
            List<SipProfile> ret = new List<SipProfile>();
            ret.Add(Domain.Current.InternalProfile);
            ret.Add(Domain.Current.ExternalProfile);
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> GetSelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            foreach (string str in SipProfile.AllSipProfileNames)
                ret.Add(new sModelSelectOptionValue(str, str));
            return ret;
        }

        public static List<string> AllSipProfileNames
        {
            get
            {
                List<string> ret = new List<string>();
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core",
                    "SELECT s.Name FROM SipProfile s");
                cq.Execute();
                while (cq.Read())
                    ret.Add(cq[0].ToString());
                cq.Close();
                return ret;
            }
        }

        #region IModel Members

        public string id
        {
            get { return Name; }
        }

        #endregion
    }
}
