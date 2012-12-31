using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;


namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces
{
    public interface IEmbeddedHandler
    {
        void HandleRequest(HttpRequest request,Site site);
        bool RequiresSessionForRequest(HttpRequest request,Site site);
    }
}
