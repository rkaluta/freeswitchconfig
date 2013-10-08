using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.BackBoneDotNet;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/Context.js")]
    [ModelRoute("/core/models/core/Context")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View|ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class Context : Org.Reddragonit.Dbpro.Structure.Table,IModel 
    {
        internal const string SESSION_ID = "Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core.Context.Current";
        public static Context Current
        {
            get
            {
                if (HttpRequest.CurrentRequest != null)
                {
                    if (HttpRequest.CurrentRequest.Session != null)
                    {
                        if (HttpRequest.CurrentRequest.Session[SESSION_ID] != null)
                            return (Context)HttpRequest.CurrentRequest.Session[SESSION_ID];
                    }
                }
                return null;
            }
        }

        internal static void SetCurrentContext(Context context)
        {
            if (HttpRequest.CurrentRequest != null)
            {
                if (HttpRequest.CurrentRequest.Session != null)
                {
                    HttpRequest.CurrentRequest.Session[SESSION_ID] = context;
                }
            }
        }

        private string _name;
        [PrimaryKeyField(false, 50)]
        [ModelRequiredField()]
        [ReadOnlyModelProperty()]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _description;
        [Field(500,true)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private ContextTypes _type;
        [Field(false)]
        [ModelRequiredField()]
        public ContextTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Context() { }

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
        public static Context LoadByName(string name)
        {
            Context ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Context));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(Context),
                new SelectParameter[] { new EqualParameter("Name", name) });
            if (tmp.Count > 0)
                ret=(Context)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<Context> LoadAll()
        {
            List<Context> ret = new List<Context>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Context));
            foreach (Context con in conn.SelectAll(typeof(Context)))
                ret.Add(con);
            conn.CloseConnection();
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> GetSelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            foreach (string str in Context.AllContextNames)
                ret.Add(new sModelSelectOptionValue(str, str));
            return ret;
        }

        [ModelListMethod("/core/models/search/core/CurrentContext")]
        public static List<Context> LoadCurrent()
        {
            List<Context> ret = new List<Context>();
            ret.Add(SipProfile.Current.Context);
            return ret;
        }

        [ModelListMethod("/core/models/search/core/CurrentlyAvailableContexts")]
        public static List<Context> LoadCurrentlyAvailable()
        {
            List<Context> ret = new List<Context>();
            ret.Add(Domain.Current.InternalProfile.Context);
            ret.Add(Domain.Current.ExternalProfile.Context);
            return ret;
        }

        public static List<string> AllContextNames
        {
            get
            {
                List<string> ret = new List<string>();
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core",
                    "SELECT c.Name FROM Context c");
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
            get { return this.Name; }
        }

        #endregion
    }
}
