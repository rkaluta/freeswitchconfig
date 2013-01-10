using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OperatingSystemOverridablePropertyAttribute : Attribute
    {

        private string _description;
        public string Description
        {
            get { return _description; }
        }

        public OperatingSystemOverridablePropertyAttribute(string description)
        {
            _description = description;
        }

        public OperatingSystemOverridablePropertyAttribute()
        {
            _description = "NOT SET";
        }

    }
}
