using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;

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
                (request.IsMobile ? "<meta name=\"viewport\" content=\"width=device-width, height=device-height, initial-scale=1.0, user-scalable=no\">" : "")+
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
                FreeswitchConfig.Site.InitPage();
                FreeswitchConfig.Site.Modals.ShowLoading();
                if (FreeswitchConfig.Site.PreloadImages().length > 0) {
                    var images = [];
                    for (i = 0; i < FreeswitchConfig.Site.PreloadImages().length; ++i) {
                        images[i] = new Image();
                        images[i].src = FreeswitchConfig.Site.PreloadImages()[i];
                    }
                }" + "\n"+
                (!Utility.IsSiteSetup ? @"FreeswitchConfig.Site.TitleContainer().html('Initial Setup');
                FreeswitchConfig.Web.Setup.GeneratePage(FreeswitchConfig.Site.MainContainer());"
                :
                @"FreeswitchConfig.Services.UserService.GetAvailbleUserDomains(
                    function(msg) {
                        var sel = $('<select id=""selDomain""></select>');
                        for(var x=0;x<msg.length;x++){
                            sel.append('<option value=""'+msg[x]+'"">'+msg[x]+'</option>');
                        }
                        sel.bind('change',function(){
                            var sel = $('#selDomain');
                            FreeswitchConfig.Site.Modals.ShowLoading();
                            FreeswitchConfig.Services.UserService.ChangeDomain(
                                sel.val(),
                                function(msg){
                                    FreeswitchConfig.Site.Modals.HideLoading();
                                    if (msg){
                                        FreeswitchConfig.Site.triggerDomainChange();
                                    }else{
                                        alert('An error occured attempting to change the current domain.');
                                    }
                                },
                                function(){
                                    FreeswitchConfig.Site.Modals.HideLoading();        
                                    alert('An error occured attempting to change the current domain.');
                                });          
                        });
                        $(document.body).append(sel);" + (Domain.Current == null ? @"
                        FreeswitchConfig.Services.UserService.GetCurrentDomain(
                            function(msg){
                                if (msg==null){         
                                    $('#selDomain').trigger('change');
                                }
                            },
                            null,
                            null,
                            true
                        );" :"")+@"
                    },
                    null,
                    null,
                    true
                );
                FreeswitchConfig.Site.MainMenuItem.SetupMenu();") +
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
