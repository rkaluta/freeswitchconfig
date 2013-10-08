using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using System.Text.RegularExpressions;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/ExtensionNumber.js")]
    [ModelRoute("/core/models/core/ExtensionNumber")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class ExtensionNumber : Org.Reddragonit.Dbpro.Structure.Table,IModel
    {

        private Context _context;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelPropertyLazyLoadExternalModel()]
        public Context Context
        {
            get { return _context; }
            set { _context = value; }
        }

        private string _originalNumber = null;
        protected string OriginalNumber
        {
            get { return _originalNumber; }
        }

        private string _number;
        [PrimaryKeyField(50, false)]
        public string Number
        {
            get { return _number; }
            set
            {
                _originalNumber = (_originalNumber == null ? value : _originalNumber);
                if (value != null)
                {
                    value = value.Trim();
                    if (!_ignoreExtensionRegex)
                    {
                        if (!new Regex(Constants.EXTENSION_REGEX).IsMatch(value))
                            throw new InvalidExtension();
                    }
                    else if (_overriddenRegexValidation != null)
                    {
                        if (!new Regex(_overriddenRegexValidation).IsMatch(value))
                            throw new InvalidExtension();
                    }
                }
                _number = value;
            }
        }

        protected virtual bool _ignoreExtensionRegex
        {
            get { return false; }
        }

        protected virtual string _overriddenRegexValidation
        {
            get { return null; }
        }

        public override sealed string ToString()
        {
            return Number;
        }

        [ModelLoadMethod()]
        public static ExtensionNumber Load(string number)
        {
            ExtensionNumber ret = null;
            List<SelectParameter> pars = new List<SelectParameter>();
            if (number.Contains("@"))
            {
                pars.Add(new EqualParameter("Number", number.Substring(0, number.LastIndexOf('@'))));
                pars.Add(new EqualParameter("Context.Name", number.Substring(number.LastIndexOf('@') + 1)));
            }
            else
            {
                pars.Add(new EqualParameter("Number", number));
                pars.Add(new EqualParameter("Context.Name", Context.Current.Name));
            }
            Connection conn = ConnectionPoolManager.GetConnection(typeof(ExtensionNumber));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(ExtensionNumber), pars.ToArray());
            if (tmp.Count > 0)
                ret = (ExtensionNumber)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<ExtensionNumber> LoadAll()
        {
            List<ExtensionNumber> ret = new List<ExtensionNumber>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(ExtensionNumber));
            foreach (ExtensionNumber ext in conn.Select(typeof(ExtensionNumber),new SelectParameter[]{
                new OrParameter(new SelectParameter[]{
                    new EqualParameter("Context.Name", Domain.Current.InternalProfile.Context.Name),
                    new EqualParameter("Context.Name",Domain.Current.ExternalProfile.Context.Name)})
            }))
                ret.Add(ext);
            conn.CloseConnection();
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> SelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core",
                "SELECT ext.Number,ext.Context.Name FROM ExtensionNumber ext WHERE ext.Context.Name = @internalName OR ext.Context.Name = @externalName");
            cq.Execute(new IDbDataParameter[] { cq.CreateParameter("@internalName", Domain.Current.InternalProfile.Context.Name),
            cq.CreateParameter("@externalName", Domain.Current.ExternalProfile.Context.Name)});
            while (cq.Read())
                ret.Add(new sModelSelectOptionValue(cq[0].ToString() + "@" + cq[1].ToString(), cq[0].ToString() + "@" + cq[1].ToString()));
            cq.Close();
            return ret;
        }

        public static bool ExtensionExists(string number)
        {
            Connection conn = ConnectionPoolManager.GetConnection(typeof(ExtensionNumber));
            bool ret = conn.SelectCount(typeof(ExtensionNumber), new SelectParameter[] { new EqualParameter("Number", number), new EqualParameter("Context", Context.Current) }) > 0;
            conn.CloseConnection();
            return ret;
        }

        public static ExtensionNumber Load(string number,string context)
        {
            ExtensionNumber ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(ExtensionNumber));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(ExtensionNumber), new SelectParameter[] { new EqualParameter("Number", number), new EqualParameter("Context.Name",context ) });
            if (tmp.Count > 0)
                ret = (ExtensionNumber)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        #region IModel Members

        public string id
        {
            get { return Number + "@" + Context.Name; }
        }

        #endregion
    }
}
