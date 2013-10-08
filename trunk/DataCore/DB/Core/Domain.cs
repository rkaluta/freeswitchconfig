using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using System.Text.RegularExpressions;
using Org.Reddragonit.BackBoneDotNet;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;


namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core
{
    [Table()]
    [TableIndex("ind_domain_name",new string[]{"Name"},true,true)]
    [ModelJSFilePath("/resources/scripts/Core/Domain.js")]
    [ModelRoute("/core/models/core/Domain")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class Domain : Org.Reddragonit.Dbpro.Structure.Table,IModel
    {
        internal const string SESSION_ID = "Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core.Domain.Current";
        public static Domain Current
        {
            get
            {
                if (HttpRequest.CurrentRequest != null)
                {
                    if (HttpRequest.CurrentRequest.Session != null)
                    {
                        if (HttpRequest.CurrentRequest.Session[SESSION_ID] != null)
                            return (Domain)HttpRequest.CurrentRequest.Session[SESSION_ID];
                    }
                }
                return null;
            }
        }

        internal static void SetCurrentDomain(Domain domain)
        {
            if (HttpRequest.CurrentRequest != null)
            {
                if (HttpRequest.CurrentRequest.Session != null)
                {
                    HttpRequest.CurrentRequest.Session[SESSION_ID] = domain;
                }
            }
        }

        private int _id;
        [PrimaryKeyField(true)]
        [ModelIgnoreProperty()]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;
        [Field(500, false)]
        [ModelRequiredField()]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _voicemailTimeout = 10;
        [Field(false)]
        [ModelRequiredField()]
        public int VoicemailTimeout{
            get { return _voicemailTimeout; }
            set { _voicemailTimeout = value; }
        }

        private SipProfile _internalProfile;
        [ForeignField(false, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        [ModelPropertyLazyLoadExternalModel()]
        public SipProfile InternalProfile
        {
            get { return _internalProfile; }
            set { _internalProfile = value; }
        }

        private SipProfile _externalProfile;
        [ForeignField(false, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        [ModelPropertyLazyLoadExternalModel()]
        public SipProfile ExternalProfile
        {
            get { return _externalProfile; }
            set { _externalProfile = value; }
        }

        private const string DIAL_STRING_NAME = "dial-string";
        private const string DIAL_STRING_VALUE = "{sip_invite_domain=${dialed_domain},presence_id=${dialed_user}@${dialed_domain}}${sofia_contact(${dialed_user}@${dialed_domain})}";
        private static readonly Regex _RegNumber = new Regex("^(\\d+)$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        public Domain() { }

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
            }
            catch (Exception e)
            {
                ret = false;
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
            }
            return ret;
        }

        [ModelDeleteMethod()]
        public new bool Delete()
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
                base.Delete();
            }
            catch (Exception e)
            {
                ret = false;
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
            }
            return ret;
        }

        [ModelLoadMethod()]
        public static Domain Load(string name)
        {
            Domain ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Domain));
            
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp;
            if (_RegNumber.IsMatch(name))
            {
                tmp = conn.Select(typeof(Domain),
                new SelectParameter[] { new EqualParameter("ID", int.Parse(name)) });
            }
            else
            {
                tmp = conn.Select(typeof(Domain),
                new SelectParameter[] { new EqualParameter("Name", name) });
            }
            if (tmp.Count > 0)
                ret = (Domain)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<Domain> LoadAll()
        {
            List<Domain> ret = new List<Domain>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Domain));
            foreach (Domain con in conn.SelectAll(typeof(Domain)))
                ret.Add(con);
            conn.CloseConnection();
            return ret;
        }

        [ModelListMethod("/core/models/search/core/CurrentDomain")]
        public static List<Domain> CurrentDomain()
        {
            List<Domain> ret = new List<Domain>();
            ret.Add(Domain.Current);
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> GetSelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            foreach (Domain d in Domain.LoadAll())
                ret.Add(new sModelSelectOptionValue(d.ID.ToString(), d.Name));
            return ret;
        }

        public static List<string> AllDomainNames
        {
            get
            {
                List<string> ret = new List<string>();
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core",
                    "SELECT d.Name FROM Domain d");
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
            get { return ID.ToString(); }
        }

        #endregion
    }
}
