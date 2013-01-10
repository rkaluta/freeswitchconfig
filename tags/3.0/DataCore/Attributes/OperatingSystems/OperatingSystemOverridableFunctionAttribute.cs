using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OperatingSystemOverridableFunctionAttribute : Attribute
    {
        private string _description;
        public string Description
        {
            get { return _description; }
        }

        public OperatingSystemOverridableFunctionAttribute(string description)
        {
            _description = description;
        }

        public OperatingSystemOverridableFunctionAttribute()
        {
            _description = "NOT SET";
        }

    }
}
