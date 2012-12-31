using System;
using System.Data;
using System.Configuration;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;

using ISite = Org.Reddragonit.EmbeddedWebServer.Interfaces.Site;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files;
using System.IO;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;

/// <summary>
/// Summary description for FileUpload
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class FileUpload : IRequestHandler
    {
        public FileUpload()
        {
        }

        #region IRequestHandler Members

        public bool CanProcessRequest(HttpRequest request, ISite site)
        {
            return request.URL.AbsolutePath == "/FileUpload.ashx";
        }

        public void DeInit()
        {
        }

        public void Init()
        {
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpRequest request, ISite site)
        {
            string[] tmp = new string[request.UploadedFiles.Count];
            request.UploadedFiles.Keys.CopyTo(tmp, 0);
            BinaryReader br = new BinaryReader(request.UploadedFiles[tmp[0]].Stream);
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                bw.Write(br.ReadBytes(1024));
            }
            br.Close();
            bw.Flush();
            request.ResponseWriter.Write(FileCache.CacheFile(ms.ToArray()));
            bw.Close();
        }

        public bool RequiresSessionForRequest(HttpRequest request, ISite site)
        {
            return false;
        }

        #endregion
    }
}