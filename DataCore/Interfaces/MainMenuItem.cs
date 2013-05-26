using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;
using System.Collections;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces
{
    [ModelJSFilePath("/resources/scripts/Menus.js")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.View|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Site")]
    [ModelRoute("/core/site/Menu")]
    public class MainMenuItem : IModel
    {
        private static readonly MainMenuItem[] _CORE_MENU = new MainMenuItem[]{
            new MainMenuItem("Home","Home",null,"Org.Reddragonit.FreeSwitchConfig.Site.Home.GeneratePage",null,null,null,null,true),
            new MainMenuItem("PBXConfiguration","PBX Config",null,null,null,null,null,new SubMenuItem[]{
                new SubMenuItem("CDR Search","CDRsAccess",new string[]{"Core.PBXConfiguration.CDRSearch"},null,null,"PBXConfiguration","FreeswitchConfig.PBX.CDR.GeneratePage"),
                new SubMenuItem("PhoneBooks",null,new string[]{"Core","Core.PBXConfiguration.PhoneBooks"},null,null,"PBXConfiguration","FreeswitchConfig.PBX.PhoneBook.GeneratePage"),
                new SubMenuItem("PinSets",null,new string[]{"Core.PBXConfiguration.PinSets"},null,null,"PBXConfiguration","FreeswitchConfig.PBX.PinSet.GeneratePage"),
                new SubMenuItem("Extensions",null,new string[]{"Core.PBXConfiguration.Extensions"},null,null,"PBXConfiguration","FreeswitchConfig.Core.Extension.GeneratePage"),
                new SubMenuItem("Pinned Routes",null,new string[]{"Core.PBXConfiguration.PinSets","Core.PBXConfiguration.PinnedRoute"},null,null,"PBXConfiguration","FreeswitchConfig.Routes.PinnedRoute.GeneratePage"),
                new SubMenuItem("Vacation",null,new string[]{"Core.PBXConfiguration.Vacation"},null,null,"PBXConfiguration","FreeswitchConfig.Routes.VacationRoute.GeneratePage"),
                new SubMenuItem("Outgoing SIP Trunk",null,new string[]{"Core.PBXConfiguration.OutgoingSIPTrunk"},null,null,"PBXConfiguration","FreeswitchConfig.Trunks.OutgoingSIPTrunk.GeneratePage"),
                new SubMenuItem("Gateway Route",null,new string[]{"Core.PBXConfiguration.GatewayRoute"},null,null,"PBXConfiguration","FreeswitchConfig.Routes.GatewayRoute.GeneratePage"),
                new SubMenuItem("Intercom",null,new string[]{"Core","Core.PBXConfiguration.Intercom"},null,null,"PBXConfiguration","FreeswitchConfig.Routes.Intercom.GeneratePage"),
                new SubMenuItem("Timed Route",null,new string[]{"Core.PBXConfiguration.TimedRoute"},null,null,"PBXConfiguration","FreeswitchConfig.Routes.TimedRoute.GeneratePage"),
                new SubMenuItem("Hunt Group",null,new string[]{"Core.PBXConfiguration.HuntGroup"},null,null,"PBXConfiguration","FreeswitchConfig.Routes.HuntGroup.GeneratePage"),
                new SubMenuItem("Incoming SIP Trunks",null,new string[]{"Core.PBXConfiguration.IncomingSIPTrunk"},null,null,"PBXConfiguration","FreeswitchConfig.Trunks.IncomingSIPTrunk.GeneratePage"),
                new SubMenuItem("Direct Lines",null,new string[]{"Core.PBXConfiguration.DirectLine"},null,null,"PBXConfiguration","FreeswitchConfig.PBX.DirectLine.GeneratePage")
            },false),
            new MainMenuItem("SystemConfiguration","System Config",null,null,null,null,null,new SubMenuItem[]{
                new SubMenuItem("Deployment Method",null,new string[]{"Core.SystemConfig.DeploymentControl"},null,null,"System Configuration","FreeswitchConfig.Core.DeploymentMethod.GeneratePage"),
                new SubMenuItem("Core Setup","DomainProfileSetup",new string[]{"Core.SystemConfig.Setup"},null,null,"System Configuration","FreeswitchConfig.Web.Setup.GeneratePage"),
                new SubMenuItem("File Access",null,new string[]{"Core.SystemConfig.FileAccess"},null,null,"System Configuration","Org.Reddragonit.FreeswitchConfig.Site.Core.SystemConfig.FileAccess.GeneratePage"),
                new SubMenuItem("User Management","SystemControl",new string[]{"Core.SystemConfig.UserManagement","TYPE=FreeswitchConfig.Services.UserManagementService"},null,null,"System Configuration","FreeswitchConfig.Services.UserManagement.GeneratePage"),
                new SubMenuItem("Freeswitch Modules","AlterFreeswitchModules",new string[]{"Core.SystemConfig.FreeswitchModules"},null,null,"System Configuration","FreeswitchConfig.Core.FreeswitchModuleConfiguration.GeneratePage"),
                new SubMenuItem("Network","SystemControl",new string[]{"Core.SystemConfig.NetworkConfig"},null,null,"System Configuration","FreeswitchConfig.System.sNetworkCard.GeneratePage"),
                new SubMenuItem("Firewall","SystemControl",new string[]{"Core.SystemConfig.Firewall"},null,null,"System Configuration","FreeswitchConfig.System.mFirewallRule.GeneratePage"),
                new SubMenuItem("System Settings",null,new string[]{"Core.SystemConfig.SystemSettings"},null,null,"System Configuration","FreeswitchConfig.Core.SystemSetting.GeneratePage"),
                new SubMenuItem("Site Modules",null,new string[]{"Core.SystemConfig.SiteModules"},null,null,"System Configuration","FreeswitchConfig.Core.SiteModule.GeneratePage"),
                new SubMenuItem("Backup/Restore",null,new string[]{"Core.SystemConfig.BackupRestore","TYPE=Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemConfig.BackupRestoreService"},null,null,"System Configuration","FreeswitchConfig.Services.BackupRestoreService.GeneratePage"),
            },false),
            new MainMenuItem("SystemDiagnostics","Diagnostics",null,"FreeswitchConfig.Services.DiagnosticsService.GeneratePage",new string[]{
                "TYPE=FreeswitchConfig.Services.DiagnosticsService",
                "Core.Diagnostics"
            },null,null,null,false),
            new MainMenuItem("ReloadMenus","Reload Menus",null,"FreeswitchConfig.Site.MainMenuItem.SetupMenu",null,null,null,null,false),
            new MainMenuItem("ChangePassword","Change Password",null,"Org.Reddragonit.FreeSwitchConfig.Site.Password.GeneratePage",new string[]{
                "Core.Password"
            },null,null,null,false),
            new MainMenuItem("Logout","Logout",null,"Logout",null,null,null,null,true)
        };

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

        private string[] _combinedURLs;
        [ModelIgnoreProperty()]
        public string[] CombinedURLs
        {
            get { return _combinedURLs; }
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

        public MainMenuItem(string name,string title, string requiredRights,string generateFunction,string[] combinedURLs,string[] javascriptURLs,string[] cssURLs,SubMenuItem[] subMenus,bool clearMainWindow)
        {
            _name = name;
            _requiredRights = requiredRights;
            _javascriptURLs = javascriptURLs;
            _cssURLs = cssURLs;
            _subMenuItems = (subMenus==null ? new SubMenuItem[0] : subMenus);
            _title = title;
            _clearMainWindow = clearMainWindow;
            _generateFunction = generateFunction;
            _combinedURLs = combinedURLs;
        }

        private const string _SESSION_ID = "MENU";

        [ModelLoadAllMethod()]
        public static List<MainMenuItem> LoadAll()
        {
            if (User.Current == null)
                return null;
            if (HttpRequest.CurrentRequest != null)
            {
                if (HttpRequest.CurrentRequest.Session != null)
                {
                    if (HttpRequest.CurrentRequest.Session[_SESSION_ID] != null)
                        return (List<MainMenuItem>)HttpRequest.CurrentRequest.Session[_SESSION_ID];
                }
            }
            List<MainMenuItem> ret = new List<MainMenuItem>();
            foreach (MainMenuItem mmi in _CORE_MENU)
            {
                if (User.Current.HasRight(mmi.RequiredRights))
                {
                    List<SubMenuItem> subMenu = new List<SubMenuItem>();
                    if (mmi.SubMenuItems != null)
                        subMenu.AddRange(mmi.SubMenuItems);
                    if (mmi.Name == SubMenuItemTypes.PBXConfiguration.ToString())
                    {
                        foreach (IModule mod in ModuleController.CurrentActiveModules)
                        {
                            SubMenuItem[] tmp = mod.MenuItemsForParent(SubMenuItemTypes.PBXConfiguration);
                            if (tmp != null)
                                subMenu.AddRange(tmp);
                        }
                    }
                    else if (mmi.Name == SubMenuItemTypes.SystemConfiguration.ToString())
                    {
                        foreach (IModule mod in ModuleController.CurrentActiveModules)
                        {
                            SubMenuItem[] tmp = mod.MenuItemsForParent(SubMenuItemTypes.SystemConfiguration);
                            if (tmp != null)
                                subMenu.AddRange(tmp);
                        }
                    }
                    for (int x = 0; x < subMenu.Count; x++)
                    {
                        if (!User.Current.HasRight(subMenu[x].RequiredRights))
                        {
                            subMenu.RemoveAt(x);
                            x--;
                        }
                    }
                    subMenu.Sort();
                    ret.Add(new MainMenuItem(mmi.Name, mmi.Title, mmi.RequiredRights, mmi.GenerateFunction,mmi.CombinedURLs, mmi.JavascriptURLs, mmi.CssURLs, (subMenu.Count > 0 ? subMenu.ToArray() : (SubMenuItem[])null), mmi.ClearMainWindow));
                }
            }
            if (HttpRequest.CurrentRequest != null)
            {
                if (HttpRequest.CurrentRequest.Session != null)
                    HttpRequest.CurrentRequest.Session[_SESSION_ID] = ret;
            }
            return ret;
        }

        [ModelLoadMethod()]
        public static MainMenuItem Load(string id)
        {
            return null;
        }

        #region IModel Members

        public string id
        {
            get { return _name; }
        }

        #endregion
    }
}
