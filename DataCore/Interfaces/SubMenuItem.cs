using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces
{
    public class SubMenuItem : IComparable
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string[] _requiredRights;
        public string[] RequiredRights
        {
            get { return _requiredRights; }
            set { _requiredRights = value; }
        }

        private string[] _javascriptURLs;
        public string[] JavascriptURLs
        {
            get { return _javascriptURLs; }
        }

        private string[] _cssURLs;
        public string[] CssURLs
        {
            get { return _cssURLs; }
        }

        private string _parentName;
        public string ParentName
        {
            get { return _parentName; }
        }

        private string _generateFunction;
        public string GenerateFunction
        {
            get { return _generateFunction; }
        }

        public SubMenuItem(string name, string[] requiredRights,string[] javascriptURLs,string[] cssURLs,string parentName,string generateFunction)
        {
            _name = name;
            _requiredRights = requiredRights;
            _javascriptURLs = javascriptURLs;
            _cssURLs = cssURLs;
            _parentName = parentName;
            _generateFunction = generateFunction;
        }

        private SubMenuItem(XmlNode node,MainMenuItem parent)
        {
            if (node["Name"] == null)
                throw new Exception("Unable to load Sub Menu Items because the Name element is missing.");
            if (node["GenerateFunction"] == null)
                throw new Exception("Unable to load Sub Menu Items because the GenerateFunction element is missing.");
            _name = node["Name"].InnerText;
                _generateFunction = node["GenerateFunction"].InnerText;
            if (parent != null)
                _parentName = parent.Name;
            else if (node["ParentName"] == null)
                throw new Exception("Unable to load Sub Menu Item because the Parent Name is missing and a Parent Menu was not supplied.");
            else
                _parentName = node["ParentName"].InnerText;
            //if (node["RequiredRights"] != null)
            //{
            //    _requiredRights = new string[node["RequiredRights"].ChildNodes.Count];
            //    for (int x = 0; x < _requiredRights.Length; x++)
            //        _requiredRights[x] = node["RequiredRights"].ChildNodes[x].InnerText;
            //}
            if (node["JavascriptURLs"] != null)
            {
                _javascriptURLs = new string[node["JavascriptURLs"].ChildNodes.Count];
                for (int x = 0; x < _javascriptURLs.Length; x++)
                    _javascriptURLs[x] = node["JavascriptURLs"].ChildNodes[x].InnerText;
            }
            if (node["CssURLs"] != null)
            {
                _cssURLs = new string[node["CssURLs"].ChildNodes.Count];
                for (int x = 0; x < _cssURLs.Length; x++)
                    _cssURLs[x] = node["CssURLs"].ChildNodes[x].InnerText;
            }
        }

        public static SubMenuItem LoadFromXml(XmlNode node,MainMenuItem parent)
        {
            return new SubMenuItem(node,parent);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((SubMenuItem)obj).Name);
        }

        #endregion
    }
}
