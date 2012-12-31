using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;

using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.Stringtemplate;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.EmbeddedWebServer.Components;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class HomePageHandler : IRequestHandler
    {

        private List<IHomePageComponent> parts = new List<IHomePageComponent>();

        #region IRequestHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public bool CanProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            return request.URL.AbsolutePath == "/resources/scripts/Core/home.js";
        }

        public void ProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            Template st = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.Site.Deployments.home.js"));
            st.SetAttribute("components", parts);
            request.ClearResponse();
            request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension("js");
            request.ResponseWriter.Write(st.ToString());
        }

        public void Init()
        {
            List<string> sorts = new List<string>();
            foreach (Type t in Utility.LocateTypeInstances(typeof(IHomePageComponent)))
            {
                parts.Add((IHomePageComponent)t.GetConstructor(Type.EmptyTypes).Invoke(new object[0]));
                sorts.Add(parts[parts.Count - 1].Title);
            }
            sorts.Sort();
            IHomePageComponent[] tparts = new IHomePageComponent[parts.Count];
            for(int x=0;x<sorts.Count;x++){
                for(int y=0;y<parts.Count;y++)
                {
                    if (parts[y].Title == sorts[x])
                    {
                        tparts[x] = parts[y];
                        parts.RemoveAt(y);
                        break;
                    }
                }
            }
            parts = new List<IHomePageComponent>(tparts);
        }

        public void DeInit()
        {
        }

        public bool RequiresSessionForRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            return true;
        }

        #endregion
    }
}
