using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;
using System.Collections;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces
{
    [ModelJSFilePath("/resources/scripts/Menus.js")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.View|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Site")]
    [ModelRoute("/core/site/Menu")]
    public class MainMenuItem : IModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
        }

        private string _requiredRights;
        [ModelIgnoreProperty()]
        public string RequiredRights
        {
            get { return _requiredRights; }
            set { _requiredRights = value; }
        }

        private string[] _javascriptURLs;
        [ModelIgnoreProperty()]
        public string[] JavascriptURLs
        {
            get { return _javascriptURLs; }
        }

        private string[] _cssURLs;
        [ModelIgnoreProperty()]
        public string[] CssURLs
        {
            get { return _cssURLs; }
        }

        private SubMenuItem[] _subMenuItems;
        [ModelIgnoreProperty()]
        public SubMenuItem[] SubMenuItems{
            get{return _subMenuItems;}
            set { _subMenuItems = value; }
        }

        public ArrayList SubMenus
        {
            get
            {
                ArrayList ret = new ArrayList();
                if (_subMenuItems != null)
                {
                    foreach (SubMenuItem smi in _subMenuItems)
                    {
                        Hashtable ht = new Hashtable();
                        ht.Add("Name", smi.Name);
                        ht.Add("GenerateFunction", smi.GenerateFunction);
                        ret.Add(ht);
                    }
                }
                return (ret.Count == 0 ? null : ret);
            }
        }

        private string _generateFunction;
        public string GenerateFunction
        {
            get { return _generateFunction; }
            set { _generateFunction = value; }
        }

        private bool _clearMainWindow;
        public bool ClearMainWindow
        {
            get { return _clearMainWindow; }
        }

        public MainMenuItem(string name,string title, string requiredRights,string[] javascriptURLs,string[] cssURLs,SubMenuItem[] subMenus,bool clearMainWindow)
        {
            _name = name;
            _requiredRights = requiredRights;
            _javascriptURLs = javascriptURLs;
            _cssURLs = cssURLs;
            _subMenuItems = subMenus;
            _title = title;
            _clearMainWindow = clearMainWindow;
        }

        private MainMenuItem(XmlNode node)
        {
            if (node["Name"] == null)
                throw new Exception("Unable to load Main Menu Items because the Name element is missing.");
            _name = node["Name"].InnerText;
            _title = node["Title"].InnerText;
            if (node["GenerateFunction"]!=null)
                _generateFunction = node["GenerateFunction"].InnerText;
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
            if (node["SubMenus"] != null)
            {
                List<SubMenuItem> tmp = new List<SubMenuItem>();
                for (int x = 0; x < node["SubMenus"].ChildNodes.Count; x++)
                {
                    if (node["SubMenus"].ChildNodes[x].Name == "SubMenu")
                    {
                        bool canAdd = true;
                        if (node["SubMenus"].ChildNodes[x]["RequiredRights"] != null)
                        {
                            foreach (XmlNode n in node["SubMenus"].ChildNodes[x]["RequiredRights"].ChildNodes)
                            {
                                if (!User.Current.HasRight(n.InnerText))
                                {
                                    canAdd = false;
                                    break;
                                }
                            }
                        }
                        if (canAdd)
                            tmp.Add(SubMenuItem.LoadFromXml(node["SubMenus"].ChildNodes[x], this));
                    }
                }
                _subMenuItems = tmp.ToArray();
            }
            if (node["ClearWindow"] != null)
                _clearMainWindow = bool.Parse(node["ClearWindow"].InnerText);
            else
                _clearMainWindow = true;
            if (Name == SubMenuItemTypes.PBXConfiguration.ToString())
            {
                foreach (IModule mod in ModuleController.CurrentModules)
                {
                    if (ModuleController.Current.IsModuleEnabled(mod.ModuleName))
                    {
                        SubMenuItem[] sitems = mod.MenuItemsForParent(SubMenuItemTypes.PBXConfiguration);
                        if (sitems!=null){
                            List<SubMenuItem> titems = new List<SubMenuItem>(sitems);
                            for (int x = 0; x < titems.Count; x++)
                            {
                                if (titems[x].RequiredRights != null)
                                {
                                    bool remove = false;
                                    foreach (string str in titems[x].RequiredRights)
                                    {
                                        if (!User.Current.HasRight(str))
                                        {
                                            remove = true;
                                            break;
                                        }
                                    }
                                    if (remove)
                                    {
                                        titems.RemoveAt(x);
                                        x--;
                                    }
                                }
                            }
                            if (titems.Count > 0)
                                _subMenuItems = MergeArrays(_subMenuItems, titems.ToArray());
                        }
                    }
                }
            }
            else if (Name == SubMenuItemTypes.SystemConfiguration.ToString())
            {
                foreach (IModule mod in ModuleController.CurrentModules)
                {
                    if (ModuleController.Current.IsModuleEnabled(mod.ModuleName))
                    {
                        SubMenuItem[] sitems = mod.MenuItemsForParent(SubMenuItemTypes.SystemConfiguration);
                        if (sitems != null)
                        {
                            List<SubMenuItem> titems = new List<SubMenuItem>(sitems);
                            for (int x = 0; x < titems.Count; x++)
                            {
                                if (titems[x].RequiredRights != null)
                                {
                                    bool remove = false;
                                    foreach (string str in titems[x].RequiredRights)
                                    {
                                        if (!User.Current.HasRight(str))
                                        {
                                            remove = true;
                                            break;
                                        }
                                    }
                                    if (remove)
                                    {
                                        titems.RemoveAt(x);
                                        x--;
                                    }
                                }
                            }
                            if (titems.Count > 0)
                                _subMenuItems = MergeArrays(_subMenuItems, titems.ToArray());
                        }
                    }
                }
            }
        }

        private SubMenuItem[] MergeArrays(SubMenuItem[] left, SubMenuItem[] right)
        {
            List<SubMenuItem> ret = new List<SubMenuItem>();
            if (left != null)
                ret.AddRange(left);
            if (right != null)
            {
                foreach (SubMenuItem smi in right)
                {
                    SubMenuItem newsmi = null;
                    foreach (SubMenuItem s in ret)
                    {
                        if (s.Name == smi.Name)
                        {
                            string[] css = MergeArrays(s.CssURLs, smi.CssURLs);
                            string[] jscript = MergeArrays(s.JavascriptURLs, smi.JavascriptURLs);
                            string[] rights = MergeArrays(s.RequiredRights, smi.RequiredRights);
                            newsmi = new SubMenuItem(s.Name, rights, jscript, css, s.ParentName,
                            (smi.GenerateFunction == null ? s.GenerateFunction : smi.GenerateFunction));
                            ret.Remove(s);
                            break;
                        }
                    }
                    if (newsmi == null)
                        newsmi = smi;
                    ret.Add(newsmi);
                }
            }
            ret.Sort();
            return (ret.Count == 0 ? null : ret.ToArray());
        }

        private string[] MergeArrays(string[] left, string[] right)
        {
            List<string> ret = new List<string>();
            if (left != null)
                ret.AddRange(left);
            if (right != null)
            {
                foreach (string str in right)
                {
                    if (!ret.Contains(str))
                        ret.Add(str);
                }
            }
            return (ret.Count == 0 ? null : ret.ToArray());
        }

        private const string MAIN_FILE_NAME = "Org.Reddragonit.FreeSwitchConfig.Site.Deployments.DefaultMenu.xml";

        [ModelLoadAllMethod()]
        public static List<MainMenuItem> LoadAll()
        {
            if (User.Current == null)
                return null;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Utility.ReadEmbeddedResource(MAIN_FILE_NAME));
            List<MainMenuItem> ret = new List<MainMenuItem>();
            foreach (XmlNode node in doc.GetElementsByTagName("MainMenuItem"))
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (node["RequiredRights"] != null)
                    {
                        bool canAdd=true;
                        foreach (XmlNode rt in node["RequiredRights"].ChildNodes)
                        {
                            if (!User.Current.HasRight(rt.InnerText))
                            {
                                canAdd = false;
                                break;
                            }
                        }
                        if (canAdd)
                            ret.Add(new MainMenuItem(node));
                    }else
                        ret.Add(new MainMenuItem(node));
                }
            }
            foreach (MainMenuItem mmi in ret)
                mmi.SortSubmenus();
            return ret;
        }

        private void SortSubmenus()
        {
            if (SubMenus!=null)
                Array.Sort(_subMenuItems);
        }

        [ModelLoadMethod()]
        public static MainMenuItem Load(string id)
        {
            MainMenuItem ret = null;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Utility.ReadEmbeddedResource(MAIN_FILE_NAME));
            foreach (XmlNode node in doc.GetElementsByTagName("MainMenuItem"))
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (node["Name"].Value == id)
                    {
                        bool canGet = true;
                        if (node["RequiredRights"] != null)
                        {
                            foreach (XmlNode rt in node["RequiredRights"].ChildNodes)
                            {
                                if (!User.Current.HasRight(rt.InnerText))
                                {
                                    canGet = false;
                                    break;
                                }
                            }
                        }
                        if (canGet)
                            ret = new MainMenuItem(node);
                        break;
                    }
                }
            }
            return ret;
        }

        #region IModel Members

        public string id
        {
            get { return _name; }
        }

        #endregion
    }
}
