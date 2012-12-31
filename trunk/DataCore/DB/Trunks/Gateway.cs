using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/loggedIn.js")]
    [ModelJSFilePath("/mobile/resources/scripts/loggedIn.js")]
    [ModelRoute("/core/models/core/Trunks")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.Collection | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Trunks")]
    public class Gateway : Org.Reddragonit.Dbpro.Structure.Table,IModel
    {
        private string _originalName;
        [ModelIgnoreProperty()]
        protected string OriginalName
        {
            get { return _originalName; }
        }

        private string _name;
        [PrimaryKeyField(false, 350)]
        [ReadOnlyModelProperty()]
        public string Name
        {
            get { return _name; }
            set {
                _originalName = (_originalName == null ? value : _originalName);
                _name = value; 
            }
        }

        [ModelLoadAllMethod()]
        public static List<Gateway> LoadAll()
        {
            List<Gateway> ret = new List<Gateway>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Gateway)).getConnection();
            foreach (Gateway gt in conn.SelectAll(typeof(Gateway)))
                ret.Add(gt);
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadMethod()]
        public static Gateway Load(string name)
        {
            Gateway ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Gateway)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(Gateway),
                new SelectParameter[] { new EqualParameter("Name", name) });
            if (tmp.Count > 0)
                ret = (Gateway)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> GetSelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks.SIP",
                "SELECT ist.Name FROM IncomingSIPTrunk ist where ist.Domain.Name = @domainName UNION SELECT ost.Name FROM OutgoingSIPTrunk ost WHERE ost.Profile.Name = @internalProfileName OR ost.Profile.Name = @externalProfileName");
            cq.Execute(new IDbDataParameter[] { cq.CreateParameter("@domainName", Domain.Current.Name),
            cq.CreateParameter("@internalProfileName",Domain.Current.InternalProfile.Name),
            cq.CreateParameter("@externalProfileName",Domain.Current.ExternalProfile.Name)
            });
            while(cq.Read())
                ret.Add(new sModelSelectOptionValue(cq[0].ToString(),cq[0].ToString()));
            cq.Close();
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
