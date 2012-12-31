using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events
{
    public interface IEventHandler
    {
        bool HandlesEvent(IEvent Event);
        void ProcessEvent(IEvent Event);
    }
}
