using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Initializers
{
    public class CDRs : IInitializer,IEventHandler
    {
        
        #region IInitializer Members

        void IInitializer.Init()
        {
            EventController.RegisterEventHandler(new CdrListener());
            EventController.RegisterEventHandler(this);
            if (ModuleController.Current.IsModuleEnabled("ESL"))
                ModuleController.Current.InvokeModuleMethod("ESL", "RegisterEvent", new NameValuePair[] { new NameValuePair("eventName", CdrListener.CALL_HANGUP_EVENT) }, false);
            else
                EmbeddedHandlerFactory.RegisterHandler("/cdrs/", new CDRWebHandler());
        }

        void IInitializer.DeInit()
        {
        }

        #endregion

        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            return Event.Name=="EventSocketReconnect"
                || Event is ModuleEnabledEvent
                || Event is ModuleDisabledEvent;
        }

        public void ProcessEvent(IEvent Event)
        {
            switch (Event.Name)
            {
                case "ModuleDisabledEvent":
                    if (((ModuleDisabledEvent)Event).Module.ModuleName == "ESL")
                        EmbeddedHandlerFactory.RegisterHandler("/cdrs/", new CDRWebHandler());
                    break;
                case "ModuleEnabledEvent":
                    if (((ModuleEnabledEvent)Event).Module.ModuleName == "ESL")
                    {
                        EmbeddedHandlerFactory.DeregisterHandler("/cdrs/");
                        ModuleController.Current.InvokeModuleMethod("ESL", "RegisterEvent", new NameValuePair[] { new NameValuePair("eventName", CdrListener.CALL_HANGUP_EVENT) }, false);

                    }
                    break;
                case "EventSocketReconnect":
                    ModuleController.Current.InvokeModuleMethod("ESL", "RegisterEvent", new NameValuePair[] { new NameValuePair("eventName", CdrListener.CALL_HANGUP_EVENT) }, false);
                    break;
            }
        }

        #endregion
    }
}
