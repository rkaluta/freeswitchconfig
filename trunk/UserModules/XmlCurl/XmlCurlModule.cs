using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.XmlCurl
{
    public class XmlCurlModule : IModule,IEventHandler
    {
        private Type _previousType
        {
            get { return Utility.LocateType((string)Settings.Current["XmlCurl", "previousType"]); }
            set {
                if (value == null)
                    Settings.Current["XmlCurl", "previousType"] = null;
                else
                    Settings.Current["XmlCurl", "previousType"] = value.FullName;
            }
        }

        #region IModule Members

        public string ModuleName
        {
            get { return "XmlCurl"; }
        }

        public string Description
        {
            get { return "This module allows freeswitch to use mod_xml_curl to get the basic configuration files for the system, it reduces writing to the disk"; }
        }

        public List<sEmbeddedFile> EmbeddedFiles
        {
            get { return null; }
        }

        public List<Type> EmbeddedServices
        {
            get { return null; }
        }

        public void Init()
        {
            IEmbeddedHandler handler = (IEmbeddedHandler)new ConfigurationWebHandler();
            EmbeddedHandlerFactory.RegisterHandler(ConfigurationWebHandler.BASE_PATH+ConfigurationWebHandler.CONFIGURATION_PATH, handler);
            EmbeddedHandlerFactory.RegisterHandler(ConfigurationWebHandler.BASE_PATH + ConfigurationWebHandler.DIALPLAN_PATH, handler);
            EmbeddedHandlerFactory.RegisterHandler(ConfigurationWebHandler.BASE_PATH + ConfigurationWebHandler.DIRECTORY_PATH, handler);
            EventController.RegisterEventHandler(this);
        }

        public void DeInit()
        {
            EventController.UnRegisterEventHandler(this);
            EmbeddedHandlerFactory.DeregisterHandler(ConfigurationWebHandler.BASE_PATH + ConfigurationWebHandler.CONFIGURATION_PATH);
            EmbeddedHandlerFactory.DeregisterHandler(ConfigurationWebHandler.BASE_PATH + ConfigurationWebHandler.DIALPLAN_PATH);
            EmbeddedHandlerFactory.DeregisterHandler(ConfigurationWebHandler.BASE_PATH + ConfigurationWebHandler.DIRECTORY_PATH);
            if (_previousType != null)
                CoreGenerator.ChangeDeploymentMethod(_previousType);
        }

        public ExposedModuleFunction[] Functions
        {
            get { return null; }
        }

        public SubMenuItem[] MenuItemsForParent(SubMenuItemTypes type)
        {
            return null;
        }

        public bool DefaultEnabled
        {
            get { return false; }
        }

        public List<FirewallRule> FirewallRules
        {
            get { return null; }
        }

        #endregion

        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            return Event is ConfigurationDeployerChangedEvent;
        }

        public void ProcessEvent(IEvent Event)
        {
            ConfigurationDeployerChangedEvent evt = (ConfigurationDeployerChangedEvent)Event;
            if (evt.NewType.FullName == typeof(VirtualConfigDeployer).FullName)
                _previousType = evt.PreviousType;
            else
                _previousType = null;
        }

        #endregion
    }
}
