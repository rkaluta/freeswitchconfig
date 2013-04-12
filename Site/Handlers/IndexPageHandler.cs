using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class IndexPageHandler : IRequestHandler
    {
        private const string _INDEX_PAGE_CODE = @"<html>
<head>
    <title>FreeSwitch Config</title>
    <script src=""/resources/scripts/core.js"" type=""text/javascript""></script>
    <link type=""text/css"" href=""/resources/styles/core.css"" rel=""Stylesheet"" />
    <script type=""text/javascript"">
    {1}
    </script>
    {0}
</head><body></body></html>";

        #region IRequestHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public bool CanProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            return request.URL.AbsolutePath == "/" || request.URL.AbsolutePath == "/index.html";
        }

        public void ProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            request.ResponseHeaders.ContentType = "text/html";
            request.ResponseWriter.WriteLine(string.Format(_INDEX_PAGE_CODE,
                (!Utility.IsSiteSetup ?
                "<script src=\"/resources/scripts/setup.js\" type=\"text/javascript\"/></script>\n<link type=\"text/css\" href=\"/resources/styles/setup.css\" rel=\"Stylesheet\" />" :
                "<link type=\"text/css\" href=\"/resources/styles/user.css\" rel=\"Stylesheet\" />\n<script src=\"/resources/scripts/user.js\" type=\"text/javascript\"></script>"),
                (Utility.IsSiteSetup ? @"function Logout() {
                    FreeswitchConfig.Services.UserService.Logout(
                        function(msg) {
                            location.href = msg;
                        }
                    );
                }" : "")+"\nvar ready=false;"+
                @"$(document).on('pageinit ready',function(){
                if (!ready){
                ready=true;
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Site.InitPage();
                if (FreeswitchConfig.Site.PreloadImages().length > 0) {
                    var images = [];
                    for (i = 0, length = FreeswitchConfig.Site.PreloadImages().length; i < length; ++i) {
                        images[i] = new Image();
                        images[i].src = FreeswitchConfig.Site.PreloadImages()[i];
                    }
                }" +"\n"+
                (!Utility.IsSiteSetup ? @"FreeswitchConfig.Site.TitleContainer().html('Initial Setup');
                FreeswitchConfig.Web.Setup.GeneratePage(FreeswitchConfig.Site.MainContainer());"
                :
                "FreeswitchConfig.Site.MainMenuItem.SetupMenu();")+
                "}});"));
        }

        public void Init()
        {
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
