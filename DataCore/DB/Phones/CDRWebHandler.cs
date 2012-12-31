using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones
{
    public class CDRWebHandler : IEmbeddedHandler
    {
        #region IEmbeddedHandler Members

        public void HandleRequest(HttpRequest request, Site site)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(request.Parameters[""]);
            foreach (XmlElement elem in doc.GetElementsByTagName("cdr"))
                EventController.TriggerEvent(new HttpCDREvent(elem));
        }

        public bool RequiresSessionForRequest(HttpRequest request, Site site)
        {
            return false;
        }

        #endregion
    }
}
