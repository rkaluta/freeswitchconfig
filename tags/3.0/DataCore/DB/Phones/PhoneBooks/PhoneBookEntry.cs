using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones.PhoneBooks
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/PhoneBooks.js")]
    [ModelRoute("/core/models/core/PhoneBookEntry")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.PBX")]
    public class PhoneBookEntry : Org.Reddragonit.Dbpro.Structure.Table,IModel 
    {
        private string _id;
        [PrimaryKeyField(true)]
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private PhoneBookEntryType _type;
        [Field(false)]
        public PhoneBookEntryType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _number;
        [Field(25, false)]
        public string Number
        {
            get { return _number; }
            set { _number = value; }
        }

        private string _firstName;
        [Field(250, false)]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _lastName;
        [Field(250, false)]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        private string _title;
        [Field(250, true)]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _organization;
        [Field(250, true)]
        public string Organization
        {
            get { return _organization; }
            set { _organization = value; }
        }

        private string _email;
        [Field(250, true)]
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private string _note;
        [Field(250, true)]
        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        private DateTime? _birthDay;
        [Field(true)]
        public DateTime? BirthDay
        {
            get { return _birthDay; }
            set { _birthDay = value; }
        }

        private bool _editableByUser;
        [Field(false)]
        public bool EditableByUser
        {
            get { return _editableByUser; }
            set { _editableByUser = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is PhoneBookEntry))
                return false;
            return ((PhoneBookEntry)obj).ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        [ModelLoadMethod()]
        public static PhoneBookEntry Load(string id)
        {
            PhoneBookEntry ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(PhoneBookEntry)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(PhoneBookEntry),
                new SelectParameter[]{
                    new EqualParameter("ID",id)
                });
            if (tmp.Count > 0)
                ret = (PhoneBookEntry)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<PhoneBookEntry> LoadAll()
        {
            List<PhoneBookEntry> ret = new List<PhoneBookEntry>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(PhoneBookEntry)).getConnection();
            foreach (Org.Reddragonit.Dbpro.Structure.Table tbl in conn.SelectAll(typeof(PhoneBookEntry)))
                ret.Add((PhoneBookEntry)tbl);
            conn.CloseConnection();
            return ret;
        }

        public PhoneBookEntry() { }

        [ModelDeleteMethod()]
        public new bool Delete()
        {
            bool ret = false;
            try
            {
                base.Delete();
                ret = true;
            }
            catch (Exception e) {
                Log.Error(e);
            }
            return ret;
        }

        [ModelUpdateMethod()]
        public new bool Update()
        {
            bool ret = false;
            try
            {
                base.Update();
                ret = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return ret;
        }

        [ModelSaveMethod()]
        public new bool Save()
        {
            bool ret = false;
            try
            {
                base.Save();
                ret = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return ret;
        }

        [ModelListMethod("/core/models/search/PhoneBookEntry/{0}/{1}/{2}/{3}/{4}", true)]
        public static List<PhoneBookEntry> GetPagedList(string firstName, string lastName, string number, string organization, string type, ulong startIndex, ulong pageSize, out int totalPages)
        {
            List<PhoneBookEntry> ret = new List<PhoneBookEntry>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(PhoneBookEntry)).getConnection();
            List<SelectParameter> pars = new List<SelectParameter>();
            if (firstName!=null)
                pars.Add(new LikeParameter("FirstName",firstName));
            if (lastName != null)
                pars.Add(new LikeParameter("LastName", lastName));
            if (number != null)
                pars.Add(new LikeParameter("Number", number));
            if (organization != null)
                pars.Add(new LikeParameter("Organization", organization));
            if (type != null)
                pars.Add(new EqualParameter("Type", (PhoneBookEntryType)Enum.Parse(typeof(PhoneBookEntryType), type)));
            totalPages = (int)Math.Ceiling((decimal)conn.SelectCount(typeof(PhoneBookEntry), pars.ToArray()) / (decimal)pageSize);
            foreach (PhoneBookEntry pbe in conn.SelectPaged(typeof(PhoneBookEntry),pars.ToArray(), startIndex, pageSize))
                ret.Add(pbe);
            conn.CloseConnection();
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> SelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones.PhoneBooks",
                "SELECT DISTINCT pbe.ID,pbe.FirstName,pbe.LastName,pbe.Number,pbe.Type FROM PhoneBookEntry pbe");
            cq.Execute();
            while (cq.Read())
                ret.Add(new sModelSelectOptionValue(cq[0].ToString(), cq[2].ToString()+", "+cq[1].ToString()+"["+cq[3].ToString()+"("+cq[4].ToString()+")]"));
            cq.Close();
            return ret;
        }

        #region IModel Members

        public string id
        {
            get { return ID; }
        }

        #endregion
    }
}
