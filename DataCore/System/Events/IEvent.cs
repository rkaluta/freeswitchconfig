using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events
{
    public interface IEvent : IXmlConvertableObject
    {
        string Name{get;}
        Dictionary<string, object>.KeyCollection Keys{get;}
        object this[string name]{get;}
    }
}
