using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.EmbeddedWebServer.Components;
using Org.Reddragonit.EmbeddedWebServer.Minifiers;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.Stringtemplate;
using Procurios.Public;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.BasicHandlers;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class CoreJSCSSHandler : IRequestHandler
    {
        private const string _BASE_PATH = "Org.Reddragonit.FreeSwitchConfig.Site.Handlers.resources.";

        private static readonly string[] _CORE_PATHS = new string[]{
            "jquery",
            "json",
            "backbone",
            "main",
            "Skin",
            "tables",
            "validations",
            "extensions",
            "form",
            "modals",
            "DateTimePicker",
            "icons"
        };

        private static readonly string[] _SETUP_PATHS = new string[]{
            "Core.SystemConfig.Setup",
            "Core.Domain",
            "Core.Context",
            "Core.SipProfile",
            "Core.ExtensionNumber",
            "Core.Gateway",
            "Core.CallExtensionReference",
            "Core.SystemConfig.NetworkConfig",
            "Core.SetupCore",
            "Core.SystemConfig.SystemSettings"
        };

        private static readonly string[] _USER_PATHS = new string[]{
            "TYPE=FreeswitchConfig.Services.UserService",
            "Menus",
            "Structures",
            "Home",
            "Core.ExtensionNumber"
        };

        private List<IHomePageComponent> parts = new List<IHomePageComponent>();

        #region IRequestHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public bool CanProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            return request.URL.AbsolutePath == "/resources/scripts/core.js"
                || request.URL.AbsolutePath == "/resources/scripts/setup.js"
                || request.URL.AbsolutePath == "/resources/scripts/user.js"
                || request.URL.AbsolutePath == "/resources/styles/core.css"
                || request.URL.AbsolutePath == "/resources/styles/setup.css"
                || request.URL.AbsolutePath == "/resources/styles/user.css";
        }

        public void ProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            request.ResponseHeaders["Cache-Control"] = "max-age = " + (60 * 60).ToString();
            List<string> paths = new List<string>();
            string ext = request.URL.AbsolutePath.Substring(request.URL.AbsolutePath.LastIndexOf("."));
            string bPath = "scripts";
            if (ext == ".css")
                bPath = "styles";
            switch (request.URL.AbsolutePath)
            {
                case "/resources/scripts/core.js":
                case "/resources/styles/core.css":
                    paths.AddRange(_CORE_PATHS);
                    break;
                case "/resources/scripts/setup.js":
                case "/resources/styles/setup.css":
                    paths.AddRange(_SETUP_PATHS);
                    break;
                case "/resources/scripts/user.js":
                    paths.AddRange(_USER_PATHS);
                    foreach (MainMenuItem mmi in MainMenuItem.LoadAll())
                    {
                        if (mmi.JavascriptURLs != null)
                            paths.AddRange(mmi.JavascriptURLs);
                        if (mmi.CombinedURLs != null)
                            paths.AddRange(mmi.CombinedURLs);
                        if (mmi.SubMenuItems != null)
                        {
                            foreach (SubMenuItem smi in mmi.SubMenuItems)
                            {
                                if (smi.JavascriptURLs != null)
                                    paths.AddRange(smi.JavascriptURLs);
                                if (smi.CombinedURLs != null)
                                    paths.AddRange(smi.CombinedURLs);
                            }
                        }
                        foreach (IHomePageComponent ihp in parts)
                        {
                            if (ihp.JSUrls != null)
                                paths.AddRange(ihp.JSUrls);
                        }
                    }
                    break;
                case "/resources/styles/user.css":
                    paths.AddRange(_USER_PATHS);
                    foreach (MainMenuItem mmi in MainMenuItem.LoadAll())
                    {
                        if (mmi.CssURLs != null)
                            paths.AddRange(mmi.CssURLs);
                        if (mmi.CombinedURLs != null)
                            paths.AddRange(mmi.CombinedURLs);
                        if (mmi.SubMenuItems != null)
                        {
                            foreach (SubMenuItem smi in mmi.SubMenuItems)
                            {
                                if (smi.CssURLs != null)
                                    paths.AddRange(smi.CssURLs);
                                if (smi.CombinedURLs != null)
                                    paths.AddRange(smi.CombinedURLs);
                            }
                        }
                        foreach (IHomePageComponent ihp in parts)
                        {
                            if (ihp.CSSUrls != null)
                                paths.AddRange(ihp.CSSUrls);
                        }
                    }
                    break;
            }
            request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(request.URL.AbsolutePath.Substring(request.URL.AbsolutePath.LastIndexOf(".")));
            foreach (string str in paths)
            {
                if (str.StartsWith("TYPE=")||str.StartsWith("/EmbeddedJSGenerator.js?TYPE="))
                {
                    if (ext != ".css")
                    {
                        request.ResponseWriter.WriteLine("/* " + str + " */");
                        foreach (IRequestHandler irh in site.Handlers)
                        {
                            if (irh is EmbeddedServiceHandler)
                            {
                                request.ResponseWriter.WriteLine(((EmbeddedServiceHandler)irh).GenerateJSForServiceType(str.Substring(str.IndexOf("=") + 1)));
                            }
                        }
                    }
                }
                else
                {
                    List<string> tpaths = new List<string>();
                    if (str.StartsWith("/"))
                    {
                        if (str.EndsWith(".min" + ext))
                        {
                            tpaths.Add(str);
                            tpaths.Add(str.Substring(0, str.LastIndexOf(".min")) + ext);
                        }
                        else
                        {
                            tpaths.Add(str);
                            tpaths.Add(str.Substring(0, str.LastIndexOf(ext)) + ".min" + ext);
                        }
                    }
                    else
                    {
                        tpaths.AddRange(new string[]{
                            "/resources/" + bPath + "/" + str.Replace(".", "/") + ext,
                            "/resources/" + bPath + "/base/" + str.Replace(".", "/") + ext,
                            "/resources/" + bPath + "/" + (request.IsMobile ? "mobile" : "desktop") + "/" + str.Replace(".", "/") + ext,
                            "Org.Reddragonit.FreeSwitchConfig.Site." + bPath + ".base." + str + ext,
                            "Org.Reddragonit.FreeSwitchConfig.Site." + bPath + "." + (request.IsMobile ? "mobile" : "desktop") + "." + str + ext,
                            "/resources/" + bPath + "/" + str.Replace(".", "/")+".min" + ext,
                            "/resources/" + bPath + "/base/" + str.Replace(".", "/")+".min" + ext,
                            "/resources/" + bPath + "/" + (request.IsMobile ? "mobile" : "desktop") + "/" + str.Replace(".", "/")+".min" + ext,
                            "Org.Reddragonit.FreeSwitchConfig.Site." + bPath + ".base." + str+".min" + ext,
                            "Org.Reddragonit.FreeSwitchConfig.Site." + bPath + "." + (request.IsMobile ? "mobile" : "desktop") + "." + str+".min" + ext
                        });
                    }
                    foreach (string path in tpaths)
                    {
                        if (path.StartsWith("/"))
                        {
                            request.ResponseWriter.WriteLine("/* " + path + " */");
                            VirtualMappedRequest vmp = new VirtualMappedRequest(new Uri("http://" + request.URL.Host + ":" + request.URL.Port.ToString() + path), request.Headers["Accept-Language"]);
                            Org.Reddragonit.BackBoneDotNet.RequestHandler.HandleRequest(vmp);
                            request.ResponseWriter.WriteLine(vmp.ToString());
                            if (site.EmbeddedFiles != null)
                            {
                                if (site.EmbeddedFiles.ContainsKey(path))
                                {
                                    if (ModelHandler.CompressJS)
                                        request.ResponseWriter.WriteLine((request.URL.AbsolutePath.EndsWith(".js") ? JSMinifier.Minify(Utility.ReadEmbeddedResource(site.EmbeddedFiles[path].DLLPath)) : CSSMinifier.Minify(Utility.ReadEmbeddedResource(site.EmbeddedFiles[path].DLLPath))));
                                    else
                                        request.ResponseWriter.WriteLine(Utility.ReadEmbeddedResource(site.EmbeddedFiles[path].DLLPath));
                                }
                            }
                            string tmpStr = Utility.ReadEmbeddedResource(_ReverseURL(path));
                            if (tmpStr != null)
                            {
                                if (ModelHandler.CompressJS)
                                    request.ResponseWriter.WriteLine((request.URL.AbsolutePath.EndsWith(".js") ? JSMinifier.Minify(tmpStr) : CSSMinifier.Minify(tmpStr)));
                                else
                                    request.ResponseWriter.WriteLine(tmpStr);
                            }
                        }
                        else
                        {
                            request.ResponseWriter.WriteLine("/* " + _ExtractURL(path) + " */");
                            request.ResponseWriter.WriteLine(Utility.ReadEmbeddedResource(path));
                        }
                    }
                }
            }
            if (request.URL.AbsolutePath=="/resources/scripts/core.js")
                _WriteConstants(request);
            else if (request.URL.AbsolutePath == "/resources/scripts/setup.js")
            {
            }
            else if (request.URL.AbsolutePath == "/resources/scripts/user.js")
            {
                Template st = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.Site.Deployments.home.js"));
                st.SetAttribute("components", parts);
                request.ResponseWriter.WriteLine(st.ToString());
            }
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
            for (int x = 0; x < sorts.Count; x++)
            {
                for (int y = 0; y < parts.Count; y++)
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
            return false;
        }

        #endregion

        private string _ExtractURL(string path)
        {
            string url = "/"+path.Replace(".", "/");
            url = url.Substring(0, url.LastIndexOf("/")) + "." + url.Substring(url.LastIndexOf("/") + 1);
            if (path.EndsWith(".min.js"))
                url = url.Substring(0, url.Length - "/min.js".Length) + ".min.js";
            else if (path.EndsWith(".min.css"))
                url = url.Substring(0, url.Length - "/min.css".Length) + ".min.css";
            return url;
        }

        private string _ReverseURL(string path)
        {
            string ret = path.Replace("/",".");
            ret = "Org.Reddragonit.FreeSwitchConfig.Site.Web." + ret.TrimStart('.');
            return ret;
        }

        private string _EscapeCode(string code)
        {
            return code.Replace("\\", "\\\\");
        }

        private void _WriteConstants(HttpRequest request)
        {
            StringBuilder js = new StringBuilder();
            js.AppendLine("var DIRECTORY_SEPERATOR = '" + _EscapeCode(Path.DirectorySeparatorChar.ToString()) + "';");
            js.AppendLine("var HELP_CLASS= 'help';");
            js.AppendLine("var NPANXX_HELP='N = [2-9]<br/>Z = [1-9]<br/>X = [0-9]<br/>. = \\d+<br/>| = ignore on output, bracket in the validation';");
            js.AppendLine("var CURRENT_OS=" + JSON.JsonEncode(Utility.OperatingSystem) + ";");
            js.AppendLine("var PORT_RANGE_REGEX = '" + Constants.PORT_RANGE_REGEX + "';");
            js.AppendLine("var IS_SETUP = " + Utility.IsSiteSetup.ToString().ToLower() + ";");
            if (JavascriptConstants.Current.Count > 0)
            {
                foreach (string str in JavascriptConstants.Current.Keys)
                {
                    js.AppendLine("var " + str + " = '" + _EscapeCode(JavascriptConstants.Current[str]) + "';");
                }
            }
            js.AppendLine("Backbone=_.extend(Backbone,{HasConfigurationChangesToMake:" + ConfigurationController.HasChangesToMake.ToString().ToLower() + "});");
            request.ResponseWriter.WriteLine(JSMinifier.Minify(js.ToString()));
        }
    }
}
