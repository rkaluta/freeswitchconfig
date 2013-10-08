using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using System.Collections;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using Org.Reddragonit.Dbpro;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System
{
    [Table()]
    public class FreeSwitchModuleFile : Org.Reddragonit.Dbpro.Structure.Table 
    {
        private string _fileName;
        [PrimaryKeyField(false, 500)]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private string _data;
        [Field(-1, false)]
        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        [PropertySetChangesField("Data")]
        public sFreeSwitchModuleFile File
        {
            get
            {
                return (sFreeSwitchModuleFile)Utility.ConvertObjectFromXML(Data);
            }
            set
            {
                Data = Utility.ConvertObjectToXML(value);
            }
        }

        public static List<FreeSwitchModuleFile> LoadAll()
        {
            List<FreeSwitchModuleFile> ret = new List<FreeSwitchModuleFile>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(FreeSwitchModuleFile));
            foreach (FreeSwitchModuleFile fsmf in conn.SelectAll(typeof(FreeSwitchModuleFile)))
                ret.Add(fsmf);
            conn.CloseConnection();
            return ret;
        }

        public static List<string> FileNames
        {
            get
            {
                List<string> ret = new List<string>();
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System", 
                    "SELECT fsmf.FileName FROM FreeSwitchModuleFile fsmf");
                cq.Execute();
                while (cq.Read())
                    ret.Add(cq[0].ToString());
                cq.Close();
                return ret;
            }
        }

        public static FreeSwitchModuleFile Create(string name, string content)
        {
            FreeSwitchModuleFile ret = new FreeSwitchModuleFile();
            ret.FileName = name;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            ret.File = new sFreeSwitchModuleFile(name,
                doc.ChildNodes[1].Attributes["description"].Value,
                doc.ChildNodes[1].OuterXml);
            Connection conn = ConnectionPoolManager.GetConnection(typeof(FreeSwitchModuleFile));
            ret = (FreeSwitchModuleFile)conn.Save(ret);
            conn.CloseConnection();
            return ret;
        }

        public static FreeSwitchModuleFile Load(string name)
        {
            FreeSwitchModuleFile ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(FreeSwitchModuleFile));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(FreeSwitchModuleFile),
                new SelectParameter[] { new EqualParameter("FileName", name) });
            conn.CloseConnection();
            if (tmp.Count > 0)
                ret = (FreeSwitchModuleFile)tmp[0];
            return ret;
        }
    }
}
