using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.Collections;
using System.Net;
using System.Reflection;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore
{
    internal static class XmlObjectConverter
    {
        private static readonly Type _LIST_TYPE = typeof(List<>);
        private static readonly Type _DICTIONARY_TYPE = typeof(Dictionary<,>);
        private static readonly Type _NULLABLE_TYPE = typeof(Nullable<>);

        public static object LoadFromXML(string xmlCode)
        {
            object ret = null;
            if (xmlCode == null)
                return ret;
            if (xmlCode == "")
                return ret;
            if (!xmlCode.StartsWith("<"))
                xmlCode = xmlCode.Substring(xmlCode.IndexOf("<"));
            if (!xmlCode.StartsWith("<?xml"))
                xmlCode = "<?xml version=\"1.0\"?>" + xmlCode;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlCode);
            XmlElement elem = (XmlElement)doc.ChildNodes[1];
            switch (elem.Name)
            {
                case "System.String":
                    ret = elem.InnerText;
                    break;
                case "System.Boolean":
                    ret = bool.Parse(elem.InnerText);
                    break;
                case "System.Byte":
                    ret = byte.Parse(elem.InnerText);
                    break;
                case "System.SByte":
                    ret = sbyte.Parse(elem.InnerText);
                    break;
                case "System.Char":
                    ret = elem.InnerText[0];
                    break;
                case "System.Decimal":
                    ret = decimal.Parse(elem.InnerText);
                    break;
                case "System.Double":
                    ret = double.Parse(elem.InnerText);
                    break;
                case "System.Single":
                    ret = Single.Parse(elem.InnerText);
                    break;
                case "System.Int32":
                    ret = int.Parse(elem.InnerText);
                    break;
                case "System.UInt32":
                    ret = uint.Parse(elem.InnerText);
                    break;
                case "System.Int64":
                    ret = long.Parse(elem.InnerText);
                    break;
                case "System.UInt64":
                    ret = ulong.Parse(elem.InnerText);
                    break;
                case "System.Int16":
                    ret = Int16.Parse(elem.InnerText);
                    break;
                case "System.UInt16":
                    ret = UInt16.Parse(elem.InnerText);
                    break;
                case "System.DateTime":
                    ret = DateTime.Parse(elem.InnerText);
                    break;
                case "System.Net.IPAddress":
                    ret = IPAddress.Parse(elem.InnerText);
                    break;
                case "System.Collections.Hashtable":
                    ret = DeserializeHashtable(elem);
                    break;
                case "System.Array":
                case "System.Collections.Generic.List":
                    Array ar = Array.CreateInstance(Utility.LocateType(elem.Attributes["ElementType"].Value), elem.ChildNodes.Count);
                    for (int x = 0; x < elem.ChildNodes.Count; x++)
                    {
                        ar.SetValue(LoadFromXML(elem.ChildNodes[x].OuterXml), x);
                    }
                    if (elem.Name == "System.Collections.Generic.List")
                    {
                        Type ltype = _LIST_TYPE.MakeGenericType(Utility.LocateType(elem.Attributes["ElementType"].Value));
                        ret = ltype.GetConstructor(new Type[] { Utility.LocateType(elem.Attributes["ElementType"].Value).MakeArrayType() }).Invoke(new object[] { ar });
                    }
                    else
                        ret = ar;
                    break;
                case "System.Collections.ArrayList":
                    ArrayList al = new ArrayList();
                    for (int x = 0; x < elem.ChildNodes.Count; x++)
                        al.Add(LoadFromXML(elem.ChildNodes[x].OuterXml));
                    ret = al;
                    break;
                case "System.Enum":
                    ret = Enum.Parse(Utility.LocateType(elem.Attributes["EnumType"].Value), elem.Value);
                    break;
                case "System.Nullable":
                    if (elem.Attributes["IsNull"].Value == "true")
                        ret = null;
                    else
                        ret = LoadFromXML(elem.InnerXml);
                    break;
                case "System.Collections.Generic.Dictionary":
                    Type dtype = _DICTIONARY_TYPE.MakeGenericType(new Type[] { Utility.LocateType(elem.Attributes["KeyType"].Value), Utility.LocateType(elem.Attributes["ValueType"].Value) });
                    IDictionary idic = (IDictionary)dtype.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                    foreach (XmlElement e in elem.ChildNodes)
                    {
                        idic.Add(LoadFromXML(e.ChildNodes[0].InnerXml), LoadFromXML(e.ChildNodes[1].InnerXml));
                    }
                    ret = idic;
                    break;
                case "Org.Reddragonit.FreeSwitchSockets.Inbound.sDomainExtensionPair":
                    ret = (sDomainExtensionPair)elem.InnerText;
                    break;
                case "Org.Reddragonit.FreeSwitchSockets.Inbound.sGatewayNumberPair":
                    ret = (sGatewayNumberPair)elem.InnerText;
                    break;
                default:
                    Type t = Utility.LocateType(elem.Name.Replace("..","+"));
                    IXmlConvertableObject ixco = null;
                    try
                    {
                         ixco = (IXmlConvertableObject)Activator.CreateInstance(t,true);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    ixco.LoadFromElement(elem);
                    ret = ixco;
                    break;
            }
            return ret;
        }

        public static string ConvertObjectToXML(object obj, bool stripXmlTag)
        {
            if (obj == null)
                return null;
            MemoryStream ms = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(ms);
            writer.WriteStartDocument();
            switch (obj.GetType().FullName)
            {
                case "System.String":
                case "System.Boolean":
                case "System.Byte":
                case "System.SByte":
                case "System.Char":
                case "System.Decimal":
                case "System.Double":
                case "System.Single":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                case "System.Int16":
                case "System.UInt16":
                case "System.DateTime":
                case "System.Net.IPAddress":
                case "Org.Reddragonit.FreeSwitchSockets.Inbound.sDomainExtensionPair":
                case "Org.Reddragonit.FreeSwitchSockets.Inbound.sGatewayNumberPair":
                    writer.WriteStartElement(obj.GetType().FullName);
                    writer.WriteValue(obj.ToString());
                    writer.WriteEndElement();
                    break;
                default:
                    if (obj.GetType().IsEnum)
                    {
                        writer.WriteStartElement("System.Enum");
                        writer.WriteAttributeString("EnumType", obj.GetType().FullName);
                        writer.WriteValue(obj.ToString());
                        writer.WriteEndElement();
                    }
                    else if (obj.GetType().IsArray)
                    {
                        writer.WriteStartElement("System.Array");
                        writer.WriteAttributeString("ElementType", obj.GetType().GetElementType().FullName);
                        for (int x = 0; x < ((Array)obj).GetLength(0); x++)
                            writer.WriteRaw(ConvertObjectToXML(((Array)obj).GetValue(x), true));
                        writer.WriteEndElement();
                    }
                    else if (obj is ArrayList)
                    {
                        writer.WriteStartElement("System.Collections.ArrayList");
                        foreach (object o in (ArrayList)obj)
                            writer.WriteRaw(ConvertObjectToXML(o, true));
                        writer.WriteEndElement();
                    }
                    else if (obj is Hashtable)
                    {
                        SerializeHashTable((Hashtable)obj, writer);
                    }
                    else if (new List<Type>(obj.GetType().GetInterfaces()).Contains(typeof(IXmlConvertableObject)))
                    {
                        writer.WriteStartElement(obj.GetType().FullName.Replace("+",".."));
                        ((IXmlConvertableObject)obj).SaveToStream(writer);
                        writer.WriteEndElement();
                    }
                    else if (obj.GetType().IsGenericType)
                        SerializeGenericType(obj, writer);
                    else
                        throw new Exception("Unable to serialize the type " + obj.GetType().FullName);
                    break;
            }
            writer.Flush();
            string ret = ASCIIEncoding.ASCII.GetString(ms.ToArray());
            if (!ret.StartsWith("<"))
                ret = ret.Substring(ret.IndexOf("<"));
            if (stripXmlTag)
                ret = ret.Substring(ret.IndexOf(">") + 1);
            return ret;
        }

        private static void SerializeGenericType(object obj,XmlWriter writer)
        {
            Type gtype = obj.GetType().GetGenericTypeDefinition();
            if (gtype == _LIST_TYPE)
            {
                writer.WriteStartElement("System.Collections.Generic.List");
                writer.WriteAttributeString("ElementType", obj.GetType().GetGenericArguments()[0].FullName);
                foreach (object o in (IEnumerable)obj)
                    writer.WriteRaw(ConvertObjectToXML(o, true));
                writer.WriteEndElement();
            }
            else if (gtype == _DICTIONARY_TYPE)
            {
                writer.WriteStartElement("System.Collections.Generic.Dictionary");
                writer.WriteAttributeString("KeyType", obj.GetType().GetGenericArguments()[0].FullName);
                writer.WriteAttributeString("ValueType", obj.GetType().GetGenericArguments()[1].FullName);
                IDictionaryEnumerator ienum = ((IDictionary)obj).GetEnumerator();
                while (ienum.MoveNext())
                {
                    writer.WriteStartElement("item");
                    writer.WriteStartElement("key");
                    writer.WriteRaw(ConvertObjectToXML(ienum.Key, true));
                    writer.WriteEndElement();
                    writer.WriteStartElement("value");
                    writer.WriteRaw(ConvertObjectToXML(ienum.Value, true));
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            else if (gtype == _NULLABLE_TYPE)
            {
                writer.WriteStartElement("System.Nullable");
                if (obj == null)
                    writer.WriteAttributeString("IsNull", "true");
                else
                {
                    writer.WriteAttributeString("IsNull", "false");
                    writer.WriteRaw(ConvertObjectToXML(Convert.ChangeType(obj, obj.GetType().GetGenericArguments()[0]), true));
                }
                writer.WriteEndElement();
            }
            else
                throw new Exception("Unable to serialize the type " + obj.GetType().FullName);
        }


        private static Hashtable DeserializeHashtable(XmlElement elem)
        {
            Hashtable ret = new Hashtable();
            foreach (XmlElement e in elem.ChildNodes)
            {
                if (e.ChildNodes[0].Attributes["IsNull"] != null)
                    ret.Add(null, (e.ChildNodes[1].Attributes["IsNull"] != null ? LoadFromXML(e.ChildNodes[1].InnerXml) : null));
                else
                    ret.Add(LoadFromXML(e.ChildNodes[0].InnerXml), (e.ChildNodes[1].Attributes["IsNull"] != null ? null : LoadFromXML(e.ChildNodes[1].InnerXml)));
            }
            return ret;
        }

        private static void SerializeHashTable(Hashtable hashtable, XmlWriter writer)
        {
            writer.WriteStartElement("System.Collections.Hashtable");
            if (hashtable != null)
            {
                IDictionaryEnumerator ienum = hashtable.GetEnumerator();
                while (ienum.MoveNext())
                {
                    writer.WriteStartElement("item");
                    writer.WriteStartElement("key");
                    if (ienum.Key == null)
                        writer.WriteAttributeString("IsNull", "true");
                    else
                        writer.WriteRaw(ConvertObjectToXML(ienum.Key, true));
                    writer.WriteEndElement();
                    writer.WriteStartElement("value");
                    if (ienum.Value == null)
                        writer.WriteAttributeString("IsNull", "true");
                    else
                        writer.WriteRaw(ConvertObjectToXML(ienum.Value, true));
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
    }
}
