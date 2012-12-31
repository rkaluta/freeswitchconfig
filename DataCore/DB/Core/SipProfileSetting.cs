using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using Org.Reddragonit.Dbpro.Connections.Parameters;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core
{
    [Table()]
    public class SipProfileSetting : Org.Reddragonit.Dbpro.Structure.Table
    {
        private SipProfile _profile;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public SipProfile Profile
        {
            get { return _profile; }
            set { _profile = value; }
        }

        private SipProfileSettingTypes _settingType;
        [PrimaryKeyField(false)]
        public SipProfileSettingTypes SettingType
        {
            get { return _settingType; }
            set { _settingType = value; }
        }

        private string _value;
        [Field(2000, false)]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        internal static string GetSettingValue(SipProfileSettingTypes type, SipProfile profile)
        {
            string ret = null;
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core",
                "Select sps.Value from SipProfileSetting sps WHERE sps.Profile.Name = @profileName AND sps.SettingType = @type");
            IDbDataParameter[] pars = new IDbDataParameter[]{
                cq.CreateParameter("@profileName",profile.Name),
                cq.CreateParameter("@type",type)
            };
            cq.Execute(pars);
            if (cq.Read())
                ret = cq.GetString(0);
            cq.Close();
            return ret;
        }

        internal static void SetSettingValue(SipProfileSettingTypes type, string value, SipProfile profile)
        {
            Connection conn = ConnectionPoolManager.GetConnection(typeof(SipProfileSetting)).getConnection();
            if (GetSettingValue(type, profile) != null)
            {
                Dictionary<string, object> fields = new Dictionary<string, object>();
                fields.Add("Value", value);
                conn.Update(typeof(SipProfileSetting),
                    fields,
                    new SelectParameter[]{
                    new EqualParameter("Profile",profile),
                    new EqualParameter("SettingType",type)
                });
            }
            else
            {
                SipProfileSetting sps = new SipProfileSetting();
                sps.Profile = profile;
                sps.SettingType = type;
                sps.Value = value;
                conn.Save(sps);
            }
            conn.CloseConnection();
        }
    }
}
