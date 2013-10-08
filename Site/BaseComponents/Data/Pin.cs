using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data
{
    [Table()]
    public class Pin : Org.Reddragonit.Dbpro.Structure.Table
    {
        public Pin() { }

        private Pin(PinSet owner, Extension number, string pin)
        {
            _owningSet = owner;
            _extension = number;
            _pinNumber = pin;
        }

        private PinSet _owningSet;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public PinSet OwningSet
        {
            get { return _owningSet; }
            set { _owningSet = value; }
        }

        private Extension _extension;
        [ForeignField(true,ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public Extension Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        private string _pinNumber;
        [Field(10,false)]
        public string PinNumber
        {
            get { return _pinNumber; }
            set { _pinNumber = value; }
        }

        private int _id;
        [PrimaryKeyField(true)]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        internal static Pin Load(string setName, string contextName, int id)
        {
            Pin ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Pin));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(Pin),
                new SelectParameter[]{
                    new EqualParameter("OwningSet.Name",setName),
                    new EqualParameter("OwningSet.Context.Name",contextName),
                    new EqualParameter("ID",id)
                });
            if (tmp.Count > 0)
                ret = (Pin)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        internal static List<Pin> LoadAllForSet(PinSet set)
        {
            List<Pin> ret = new List<Pin>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Pin));
            foreach (Pin p in conn.Select(typeof(Pin),
                new SelectParameter[]{
                    new EqualParameter("OwningSet",set)
                }))
                ret.Add(p);
            conn.CloseConnection();
            return ret;
        }
    }
}
