using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using ISite = Org.Reddragonit.EmbeddedWebServer.Interfaces.Site;
using System.IO;
using Org.Reddragonit.EmbeddedWebServer.Components;
using Org.Reddragonit.EmbeddedWebServer.Minifiers;
using System.Reflection;
using System.Drawing;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class IconsHandler : IRequestHandler
    {
        private MemoryStream _iconImages;
        private string _iconCSS;
        public string IconCSS
        {
            get { return _iconCSS; }
        }

        private const string _IMAGES_PATH = "/resources/images/icons.png";
        private const string _CSS_PATH = "/resources/styles/icons.css";

        private const string _CSS_LINE = ".{0} {{background-image:url('"+_IMAGES_PATH+"'); background-position:{1}px {2}px;width: {3}px;height: {4}px;display:inline-block;}}";

        #region IRequestHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public bool CanProcessRequest(HttpRequest request, ISite site)
        {
            return request.URL.AbsolutePath == _IMAGES_PATH || request.URL.AbsolutePath == _CSS_PATH;
        }

        public void ProcessRequest(HttpRequest request, ISite site)
        {
            request.ResponseHeaders["Cache-Control"] = "max-age = " + (60 * 60).ToString();
            switch (request.URL.AbsolutePath)
            {
                case _IMAGES_PATH:
                    request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension("png");
                    request.ResponseStatus = HttpStatusCodes.OK;
                    request.UseResponseStream(new MemoryStream(_iconImages.ToArray()));
                    break;
                case _CSS_PATH:
                    request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension("css");
                    request.ResponseStatus = HttpStatusCodes.OK;
                    request.ResponseWriter.Write(_iconCSS);
                    break;
                default:
                    request.ResponseStatus = HttpStatusCodes.Not_Found;
                    request.SendResponse();
                    break;
            }
        }

        public void Init()
        {
            _iconImages = new MemoryStream();
            Assembly ass = this.GetType().Assembly;
            List<string> paths = new List<string>();

            foreach (string str in ass.GetManifestResourceNames())
            {
                if (str.StartsWith("Org.Reddragonit.FreeSwitchConfig.Site.Handlers.icons"))
                    paths.Add(str);
            }
            int width = 1;
            int height = 1;
            while (width * height < paths.Count)
            {
                width++;
                if (width * height < paths.Count)
                    height++;
            }

            Image[] icons = new Image[paths.Count];
            for (int x = 0; x < paths.Count; x++)
                icons[x] = Image.FromStream(Utility.LocateEmbededResource(paths[x]));
            int pxWidth = 0;
            int pxHeight = 0;
            for (int x = 0; x < height; x++)
            {
                int curWidth = 0;
                int maxHeight = 0;
                for (int y = 0; y < width; y++)
                {
                    if ((x * width) + y >= icons.Length)
                        break;
                    curWidth += icons[(x * width) + y].Width * 2;
                    maxHeight = Math.Max(icons[(x * width) + y].Height * 2, maxHeight);
                }
                pxHeight += maxHeight;
                pxWidth = Math.Max(pxWidth, curWidth);
            }

            StringBuilder sbCss = new StringBuilder();
            Bitmap bmp = new Bitmap(pxWidth, pxHeight);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);

            int curTop = 0;

            for (int x = 0; x < height; x++)
            {
                int maxHeight = 0;
                int curLeft = 0;
                for (int y = 0; y < width; y++)
                {
                    if ((x * width) + y >= icons.Length)
                        break;
                    g.DrawImage(icons[(x * width) + y], curLeft + (int)Math.Floor((decimal)icons[(x * width) + y].Width / (decimal)2), curTop + (int)Math.Floor((decimal)icons[(x * width) + y].Height / (decimal)2));
                    string[] name = paths[(x * width) + y].Split('.');
                    sbCss.AppendLine(string.Format(_CSS_LINE,
                        new object[]{
                            name[name.Length-2],
                            0-(curLeft+(int)Math.Floor((decimal)icons[(x * width) + y].Width/(decimal)2)),
                            0-(curTop+icons[(x * width) + y].Height)+(int)Math.Floor((decimal)icons[(x * width) + y].Height/(decimal)2),
                            icons[(x * width) + y].Width,
                            icons[(x * width) + y].Height
                        }));
                    curLeft += icons[(x * width) + y].Width * 2;
                    maxHeight = Math.Max(icons[(x * width) + y].Height * 2, maxHeight);
                }
                curTop += maxHeight;
            }

            bmp.Save(_iconImages, System.Drawing.Imaging.ImageFormat.Png);
            _iconCSS = CSSMinifier.Minify(sbCss.ToString());
        }

        public void DeInit()
        {
            _iconCSS = null;
            _iconImages = null;
        }

        public bool RequiresSessionForRequest(HttpRequest request, ISite site)
        {
            return false;
        }

        #endregion
    }
}
