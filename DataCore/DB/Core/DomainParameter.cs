using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core
{
    [Table()]
    public class DomainParameter : Org.Reddragonit.Dbpro.Structure.Table
    {
        private Domain _domain;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public Domain Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        private string _name;
        [PrimaryKeyField(false, 250)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _value;
        [Field(2000, false)]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
