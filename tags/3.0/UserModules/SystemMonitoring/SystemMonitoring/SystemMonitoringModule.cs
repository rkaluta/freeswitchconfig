using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Services;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring
{
    public class SystemMonitoringModule : IModule
    {
        #region IModule Members

        public string ModuleName
        {
            get { return "SystemMonitoring"; }
        }

        public string Description
        {
            get { return "This module implements a system performance monitoring component that allows you to monitor the status of the phone system."; }
        }

        public List<sEmbeddedFile> EmbeddedFiles
        {
            get { return new List<sEmbeddedFile>(new sEmbeddedFile[] {
                new sEmbeddedFile("Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.resources.SystemPerformance.js","/resources/scripts/Core/SystemConfig/SystemPerformance.js",EmbeddedFileTypes.Javascript,null) ,
                new sEmbeddedFile("Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.resources.excanvas.min.js","/resources/scripts/jquery/excanvas.min.js",EmbeddedFileTypes.Compressed_Javascript,null),
                new sEmbeddedFile("Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.resources.jquery-flot.min.js","/resources/scripts/jquery/jquery-flot.min.js",EmbeddedFileTypes.Compressed_Javascript,null)
            }); }
        }

        public List<Type> EmbeddedServices
        {
            get { return new List<Type>(new Type[] { typeof(SystemMonitorService) }); }
        }

        public void Init()
        {
            
        }

        public void DeInit()
        {
            
        }

        public ExposedModuleFunction[] Functions
        {
            get { return null; }
        }

        public SubMenuItem[] MenuItemsForParent(SubMenuItemTypes type)
        {
            if (type == SubMenuItemTypes.SystemConfiguration)
                return new SubMenuItem[]{
                    new SubMenuItem("System Performance",
                        null,
                        new string[]{
                            "/resources/scripts/jquery/excanvas.min.js",
                            "/resources/scripts/jquery/jquery-flot.min.js",
                            "/resources/scripts/Core/SystemConfig/SystemPerformance.js",
                            "/EmbeddedJSGenerator.js?TYPE=Org.Reddragonit.FreeSwitchConfig.UserModules.SystemMonitoring.Services"
                        },
                        null,
                        null,
                        "Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.SystemPerformance.GeneratePage"
                        )
                };
            return null;
        }

        public bool DefaultEnabled
        {
            get { return false; }
        }

        public List<FirewallRule> FirewallRules
        {
            get { return null; ; }
        }

        #endregion
    }
}
