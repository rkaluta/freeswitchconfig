/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 09/11/2009
 * Time: 8:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Procurios.Public;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System
{
	/// <summary>
	/// Description of SystemSetting.
	/// </summary>
	[Table()]
	public class SystemSetting : Org.Reddragonit.Dbpro.Structure.Table
	{
		
		public SystemSetting()
		{
		}
		
		private string _name;
		[PrimaryKeyField(false,500)]
		public string Name{
			get{return _name;}
			set{_name=value;}
		}

        private string _valueType;
        [Field(2000, false)]
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
