using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DiagnosticFunctionAttribute : Attribute
    {
        public delegate List<string> DiagnosticsDelegate();

        private string _groupname;
        public string GroupName
        {
            get { return _groupname; }
        }

        private string[] _requiredRights;
        public string[] RequiredRights
        {
            get { return _requiredRights; }
        }

        public DiagnosticFunctionAttribute(string groupname)
            : this(groupname,null)
        {
        }

        public DiagnosticFunctionAttribute(string groupname, string[] requiredRights)
        {
            _groupname = groupname;
            _requiredRights = requiredRights;
        }
    }
}
