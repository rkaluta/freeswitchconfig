using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using System.Threading;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using System.Data;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones.PhoneBooks
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/PhoneBooks.js")]
    [ModelRoute("/core/models/core/PhoneBook")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View|ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.PBX")]
    public class PhoneBook : Org.Reddragonit.Dbpro.Structure.Table,IModel
    {
        private string _name;
        [PrimaryKeyField(false, 250)]
        [ModelRequiredField()]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _description;
        [Field(2000, true)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private User _owningUser;
        [ForeignField(true, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelPropertyLazyLoadExternalModel()]
        public User OwningUser
        {
            get { return _owningUser; }
            set { _owningUser = value; }
        }

        private PhoneBookEntry[] _entries;
        [ForeignField(true, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelPropertyLazyLoadExternalModel()]
        public PhoneBookEntry[] Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }

        private User[] _attachedToUsers;
        [ForeignField(true,ForeignField.UpdateDeleteAction.CASCADE,ForeignField.UpdateDeleteAction.CASCADE)]    
        [ModelPropertyLazyLoadExternalModel()]
        public User[] AttachedToUsers
        {
            get { return _attachedToUsers; }
            set { _attachedToUsers = value; }
        }

        private PhoneBookSortTypes[] _order;
        [Field(true)]
        public PhoneBookSortTypes[] Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public PhoneBook() { }

        [ModelLoadMethod()]
        public static PhoneBook Load(string name)
        {
            PhoneBook ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(PhoneBook));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(PhoneBook),
                new SelectParameter[]{
                    new EqualParameter("Name",name)
                });
            if (tmp.Count > 0)
                ret = (PhoneBook)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<PhoneBook> LoadAll()
        {
            List<PhoneBook> ret = new List<PhoneBook>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(PhoneBook));
            foreach (PhoneBook pb in conn.SelectAll(typeof(PhoneBook)))
                ret.Add(pb);
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
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
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
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
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
                base.Update();
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
                ret = false;
            }
            return ret;
        }

        public void Sort(PhoneBookSortTypes[] order)
        {
            if (order != null)
            {
                string OrderBy = "ORDER BY ";
                bool useEntryTypes = false;
                foreach (PhoneBookSortTypes pbst in order)
                {
                    switch (pbst)
                    {
                        case PhoneBookSortTypes.FirstName:
                            OrderBy += "pb.Entries.FirstName, ";
                            break;
                        case PhoneBookSortTypes.LastName:
                            OrderBy += "pb.Entries.LastName, ";
                            break;
                        case PhoneBookSortTypes.EntryType:
                            useEntryTypes = true;
                            OrderBy += "(";
                            foreach (PhoneBookEntryType pbet in Enum.GetValues(typeof(PhoneBookEntryType)))
                                OrderBy += "CASE WHEN pb.Entries.Type = @" + pbet.ToString() + " THEN " + ((int)pbet).ToString() + " ";
                            OrderBy += "END), ";
                            break;
                    }
                }
                OrderBy = OrderBy.Substring(0, OrderBy.Length - 2)+" ASC";
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones.PhoneBooks",
                    "SELECT pb.Entries.ID FROM PhoneBook pb WHERE pb.Name = @Name " + OrderBy);
                List<IDbDataParameter> pars = new List<IDbDataParameter>();
                pars.Add(cq.CreateParameter("@Name", Name));
                if (useEntryTypes){
                    foreach (PhoneBookEntryType pbet in Enum.GetValues(typeof(PhoneBookEntryType)))
                        pars.Add(cq.CreateParameter("@" + pbet.ToString(), pbet));
                }
                cq.Execute(pars.ToArray());
                List<PhoneBookEntry> newEntries = new List<PhoneBookEntry>();
                while (cq.Read())
                    newEntries.Add(PhoneBookEntry.Load(cq[0].ToString()));
                cq.Close();
                Entries = newEntries.ToArray();
                this.Update();
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
