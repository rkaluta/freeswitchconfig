using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces
{
    public interface ICallAction
    {
        CallActionTypes Command { get; }
        bool WaitUntilDone { get; }
        NameValuePair[] ActionXMLAttributes { get; }
    }
}
