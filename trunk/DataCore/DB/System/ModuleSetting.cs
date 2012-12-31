using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System
{
    [Table()]
    public class ModuleSetting : Org.Reddragonit.Dbpro.Structure.Table 
    {
        public ModuleSetting()
		{
		}

        private string _moduleName;
        [PrimaryKeyField(false, 250)]
        public string ModuleName
        {
            get { return _moduleName; }
            set { _moduleName = value; }
        }

		private string _name;
		[PrimaryKeyField(false,500)]
		public string Name{
			get{return _name;}
			set{_name=value;}
		}

        private string _valueType;
        [Field(2000,false)]
        public string ValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
        }
		
		private string _value;
		[Field(-1,true)]
		public string Value{
			get{return _value;}
			set{_value=value;}
		}
    }
}
