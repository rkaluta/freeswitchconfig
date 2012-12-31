using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System
{
    public class ReadonlyNetworkCardException : Exception
    {
        public ReadonlyNetworkCardException()
            : base("Unable to change the settings on a readonly network interface") { }
    }
}
