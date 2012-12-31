using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using sSite = Org.Reddragonit.EmbeddedWebServer.Interfaces.Site;
using Utility = Org.Reddragonit.FreeSwitchConfig.DataCore.Utility;
using System.Configuration;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class ModelHandler : IRequestHandler
    {
        private const string _SETUP_CORE_PATH = "Org.Reddragonit.FreeSwitchConfig.Site.Handlers.resources.desktop.scripts.SetupCore.js";

        private bool CompressJS
        {
            get
            {
                return bool.Parse(ConfigurationSettings.AppSettings["Org.Reddragonit.FreeSwitchConfig.Site.Handlers.ModelHandler.CompressJS"]);
            }
        }

        #region IRequestHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public bool CanProcessRequest(HttpRequest request, sSite site)
        {
            return RequestHandler.HandlesURL(request.URL, request.Method);
        }

        public void ProcessRequest(HttpRequest request, sSite site)
        {
            if (site.EmbeddedFiles != null)
            {
                if (site.EmbeddedFiles.ContainsKey(request.URL.AbsolutePath))
                {
                    if (request.URL.AbsolutePath.EndsWith(".min.js") || CompressJS)
                        request.ResponseWriter.Write(JSMinifier.Minify(Utility.ReadEmbeddedResource(site.EmbeddedFiles[request.URL.AbsolutePath].DLLPath)));
                    else
                        request.ResponseWriter.Write(Utility.ReadEmbeddedResource(site.EmbeddedFiles[request.URL.AbsolutePath].DLLPath));
                }
            }
            if (request.URL.AbsolutePath == "/resources/scripts/Core/SystemConfig/Setup.js")
            {
                if (request.URL.AbsolutePath.EndsWith(".min.js") || CompressJS)
                    request.ResponseWriter.Write(JSMinifier.Minify(Utility.ReadEmbeddedResource(_SETUP_CORE_PATH)));
                else
                    request.ResponseWriter.Write(Utility.ReadEmbeddedResource(_SETUP_CORE_PATH));
            }
            RequestHandler.HandleRequest(new MappedRequest(request));
        }

        public void Init()
        {
            RequestHandler.Start(RequestHandler.StartTypes.ThrowInvalidExceptions, null, null, null,new BackBoneLogger());
        }

        public void DeInit()
        {
            RequestHandler.Stop();
        }

        public bool RequiresSessionForRequest(HttpRequest request, sSite site)
        {
            return true;
        }

        #endregion
    }
}
