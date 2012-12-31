using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl
{
    public class XmlContextFile
    {
        private const string _INCLUDE_LINE = "<X-PRE-PROCESS cmd=\"include\" data=\"{0}\" />";

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
        }

        private MemoryStream _ms;
        private XmlWriter _writer;

        private List<XmlContextFile> _includes;
        public List<XmlContextFile> Includes
        {
            get { return _includes; }
        }
        private bool _closed = false;
        private XmlContextFile _parent=null;

        private void ChangeParent(XmlContextFile parent)
        {
            string tmp = ToXMLContent(false);
            string newPath = "";
            if (parent == null)
                newPath = "." + Path.DirectorySeparatorChar;
            else
                newPath = parent.BasePath + _fileName + Path.DirectorySeparatorChar;
            foreach (XmlContextFile xcf in _includes)
            {
                tmp = tmp.Replace(string.Format(_INCLUDE_LINE, xcf.BasePath + xcf.FileName + ".xml"),
                    string.Format(_INCLUDE_LINE, newPath + xcf.FileName + ".xml"));
            }
            _writer.Close();
            _ms = new MemoryStream();
            _writer = XmlWriter.Create(_ms);
            _writer.WriteRaw(tmp);
            foreach (XmlContextFile xcf in _includes)
                xcf.ChangeParent(this);
        }

        private string BasePath
        {
            get
            {
                if (_parent == null)
                    return "." + Path.DirectorySeparatorChar;
                else
                    return _parent.BasePath + _fileName + Path.DirectorySeparatorChar;
            }
        }

        public string ToXMLContent(bool mergeIncludes)
        {
            if (!_closed)
            {
                _writer.WriteEndElement();
                _closed = true;
                _writer.Flush();
            }
            string ret = ASCIIEncoding.ASCII.GetString(_ms.ToArray());
            if (mergeIncludes)
            {
                foreach (XmlContextFile xcf in _includes)
                {
                    ret = ret.Replace(string.Format(_INCLUDE_LINE, xcf.BasePath + xcf.FileName + ".xml"),
                        xcf.ToXMLContent(mergeIncludes));
                }
            }
            return ret;
        }

        public XmlContextFile(string fileName)
        {
            _fileName = fileName;
            if (_fileName.ToUpper().Contains(".XML"))
                _fileName = _fileName.Substring(0, _fileName.Length - 4);
            _ms = new MemoryStream();
            _writer = XmlWriter.Create(_ms);
            _writer.WriteStartDocument();
            _writer.WriteStartElement("include");
            _includes = new List<XmlContextFile>();
        }

        private XmlContextFile(string fileName, XmlContextFile parent)
            : this(fileName)
        {
            _parent = parent;
        }

        public XmlContextFile AddInclude(string fileName)
        {
            XmlContextFile ret = new XmlContextFile(fileName, this);
            _includes.Add(ret);
            WriteRaw(string.Format(_INCLUDE_LINE, ret.BasePath + ret._fileName));
            return ret;
        }

        public void AddInclude(XmlContextFile file)
        {
            _includes.Add(file);
            file.ChangeParent(this);
            WriteRaw(string.Format(_INCLUDE_LINE, file.BasePath + file._fileName));
        }

        #region XML Writer
        public void WriteStartElement(string localName)
        {
            _writer.WriteStartElement(localName);
        }

        public void WriteStartElement(string localName,string ns)
        {
            _writer.WriteStartElement(localName,ns);
        }

        public void WriteStartElement(string prefix,string localName,string ns)
        {
            _writer.WriteStartElement(prefix,localName,ns);
        }

        public void WriteEndElement()
        {
            _writer.WriteEndElement();
        }

        public void WriteStartAttribute(string localName)
        {
            _writer.WriteStartAttribute(localName);
        }

        public void WriteStartAttribute(string localName, string ns)
        {
            _writer.WriteStartAttribute(localName, ns);
        }

        public void WriteStartAttribute(string prefix, string localName, string ns)
        {
            _writer.WriteStartAttribute(prefix, localName, ns);
        }

        public void WriteEndAttribute()
        {
            _writer.WriteEndAttribute();
        }

        public void WriteValue(object value)
        {
            _writer.WriteValue(value);
        }

        public void WriteRaw(string data)
        {
            _writer.WriteRaw(data);
        }

        public void WriteRaw(char[] data, int index, int count)
        {
            _writer.WriteRaw(data, index, count);
        }
        #endregion
    }
}
