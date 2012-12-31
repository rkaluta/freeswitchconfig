using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces
{
    public interface IXmlConvertableObject
    {
        void SaveToStream(XmlWriter writer);
        void LoadFromElement(XmlElement element);
    }
}
