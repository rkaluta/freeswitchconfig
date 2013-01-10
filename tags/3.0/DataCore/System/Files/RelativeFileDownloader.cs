using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.IO;
using System.Text.RegularExpressions;

using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.EmbeddedWebServer.Components;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files
{
    public class RelativeFileDownloader : IEmbeddedHandler,IInitializer
    {

        private static Regex _regAudioFile = new Regex("^.+\\.(wav|mp3)$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        #region IEmbeddedHandler Members

        public void HandleRequest(HttpRequest request, Site site)
        {
            File f = new File(HttpUtility.UrlDecode(request.URL.AbsolutePath.Substring(request.URL.AbsolutePath.IndexOf("RelativeFiles/") + "RelativeFiles/".Length).Replace("/",Path.DirectorySeparatorChar.ToString())));
            request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(f.FileName.Substring(f.FileName.IndexOf(".") + 1));
            request.UseResponseStream(new FileStream(f.ActualPath, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public bool RequiresSessionForRequest(HttpRequest request, Site site)
        {
            return false;
        }

        #endregion

        #region IInitializer Members

        public void Init()
        {
            EmbeddedHandlerFactory.RegisterHandler("RelativeFiles/*", new RelativeFileDownloader());
        }

        public void DeInit()
        {
            EmbeddedHandlerFactory.DeregisterHandler("RelativeFiles/*");
        }

        #endregion
    }
}
