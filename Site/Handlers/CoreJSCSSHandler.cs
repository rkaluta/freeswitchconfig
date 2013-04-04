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

        private static readonly string[] _CORE_JS_LIST = new string[]{
            "Org.Reddragonit.EmbeddedWebServer.resources.jquery.min.js",
            "Org.Reddragonit.EmbeddedWebServer.resources.json2.min.js",
            "Org.Reddragonit.BackBoneDotNet.resources.underscore-min.js",
            "Org.Reddragonit.BackBoneDotNet.resources.backbone.min.js",
            "Org.Reddragonit.BackBoneDotNet.resources.backbone.validate.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.common.main.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.common.tables.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.common.validations.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.common.extensions.js"
        };

        private static readonly string[] _SWITCHABLE_CORE_JS_LIST = new string[]{
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.{0}.main.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.{0}.modals.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.{0}.form.js"
        };

        private static readonly string[] _CORE_CSS_LIST = new string[]{
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.styles.common.main.css"
        };

        private static readonly string[] _SWITCHABLE_CORE_CSS_LIST = new string[]{
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.styles.{0}.main.css",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.styles.{0}.modals.css"
        };

        private static readonly string[] _SETUP_JS_LIST = new string[]{
            "/resources/scripts/Core/SystemConfig/Setup.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.Core.SystemConfig.Setup.js",
            "/resources/scripts/Core/Domain.js",
            "/resources/scripts/Core/Context.js",
            "/resources/scripts/Core/SipProfile.js",
            "/resources/scripts/Core/SystemConfig/NetworkConfig.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.Core.SetupCore.js",
            "/resources/scripts/Core/SystemConfig/SystemSettings.js"
        };
        
        private static readonly string[] _SWITCHABLE_SETUP_JS_LIST = new string[] { };

        private static readonly string[] _SETUP_CSS_LIST = new string[] { };

        private static readonly string[] _SWITCHABLE_SETUP_CSS_LIST = new string[] { 
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.styles.{0}.Core.SystemConfig.Setup.css"
        };

        private static readonly string[] _USER_JS_LIST = new string[] {
            "/EmbeddedJSGenerator.js?TYPE=FreeswitchConfig.Services.UserService",
            "/resources/scripts/Menus.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.common.structures.js"
        };

        private static readonly string[] _SWITCHABLE_USER_JS_LIST = new string[] { 
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.{0}.Menus.js",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.{0}.DateTimePicker.js"
        };


        private static readonly string[] _USER_CSS_LIST = new string[] { };

        private static readonly string[] _SWITCHABLE_USER_CSS_LIST = new string[] { 
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.styles.{0}.Menus.css",
            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.styles.{0}.Home.css"
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
            switch (request.URL.AbsolutePath)
            {
                case "/resources/scripts/core.js":
                    paths.AddRange(_CORE_JS_LIST);
                    if (request.IsMobile)
                        paths.Add("Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.scripts.mobile.jquery.min.js");
                    foreach (string str in _SWITCHABLE_CORE_JS_LIST)
                        paths.Add(string.Format(str,(request.IsMobile ? "mobile" : "desktop")));
                    break;
                case "/resources/scripts/setup.js":
                    paths.AddRange(_SETUP_JS_LIST);
                    foreach (string str in _SWITCHABLE_SETUP_JS_LIST)
                        paths.Add(string.Format(str, (request.IsMobile ? "mobile" : "desktop")));
                    break;
                case "/resources/scripts/user.js":
                    paths.AddRange(_USER_JS_LIST);
                    foreach (string str in _SWITCHABLE_USER_JS_LIST)
                        paths.Add(string.Format(str, (request.IsMobile ? "mobile" : "desktop")));
                    foreach (IHomePageComponent ihpc in parts)
                    {
                        if (ihpc.JSUrls != null)
                            paths.AddRange(ihpc.JSUrls);
                    }
                    foreach (MainMenuItem mmi in MainMenuItem.LoadAll())
                    {
                        if (mmi.JavascriptURLs != null)
                            paths.AddRange(mmi.JavascriptURLs);
                        if (mmi.SubMenuItems != null)
                        {
                            foreach (SubMenuItem smi in mmi.SubMenuItems)
                            {
                                if (smi.JavascriptURLs != null)
                                    paths.AddRange(smi.JavascriptURLs);
                            }
                        }
                    }
                    break;
                case "/resources/styles/core.css":
                    paths.AddRange(_CORE_CSS_LIST);
                    foreach (string str in _SWITCHABLE_CORE_CSS_LIST)
                        paths.Add(string.Format(str, (request.IsMobile ? "mobile" : "desktop")));
                    foreach(IHomePageComponent ihpc in parts){
                        if (ihpc.CSSUrls != null)
                            paths.AddRange(ihpc.CSSUrls);
                    }
                    break;
                case "/resources/styles/setup.css":
                    paths.AddRange(_SETUP_CSS_LIST);
                    foreach (string str in _SWITCHABLE_SETUP_CSS_LIST)
                        paths.Add(string.Format(str, (request.IsMobile ? "mobile" : "desktop")));
                    break;
                case "/resources/styles/user.css":
                    if (request.IsMobile)
                        paths.AddRange(new string[]{
                            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.styles.mobile.jquery.mobile.min.css",
                            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.styles.mobile.jquery.mobile.structure.min.css",
                            "Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.styles.mobile.jquery.mobile.theme.min.css"
                        });
                    paths.AddRange(_USER_CSS_LIST);
                    foreach (string str in _SWITCHABLE_USER_CSS_LIST)
                        paths.Add(string.Format(str, (request.IsMobile ? "mobile" : "desktop")));
                    foreach (MainMenuItem mmi in MainMenuItem.LoadAll())
                    {
                        if (mmi.CssURLs != null)
                            paths.AddRange(mmi.CssURLs);
                        if (mmi.SubMenuItems != null)
                        {
                            foreach (SubMenuItem smi in mmi.SubMenuItems)
                            {
                                if (smi.CssURLs != null)
                                    paths.AddRange(smi.CssURLs);
                            }
                        }
                    }
                    break;
            }
            request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(request.URL.AbsolutePath.Substring(request.URL.AbsolutePath.LastIndexOf(".")));
            foreach (string path in paths)
            {
                if (path.StartsWith("/EmbeddedJSGenerator.js?TYPE=")){
                    request.ResponseWriter.WriteLine("/* " + path + " */");
                    foreach (IRequestHandler irh in site.Handlers)
                    {
                        if (irh is EmbeddedServiceHandler)
                        {
                            request.ResponseWriter.WriteLine(((EmbeddedServiceHandler)irh).GenerateJSForServiceType(path.Substring(path.IndexOf("=") + 1)));
                        }
                    }
                }else if (path.StartsWith("/"))
                {
                    request.ResponseWriter.WriteLine("/* " + path + " */");
                    VirtualMappedRequest vmp = new VirtualMappedRequest(new Uri("http://" + request.URL.Host + ":" + request.URL.Port.ToString() + path), request.Headers["Accept-Language"]);
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
                    Org.Reddragonit.BackBoneDotNet.RequestHandler.HandleRequest(vmp);
                    request.ResponseWriter.WriteLine(vmp.ToString());
                }
                else
                {
                    request.ResponseWriter.WriteLine("/* " + _ExtractURL(path) + " */");
                    request.ResponseWriter.WriteLine(Utility.ReadEmbeddedResource(path));
                }
            }
            if (request.URL.AbsolutePath=="/resources/scripts/core.js")
                _WriteConstants(request);
            else if (request.URL.AbsolutePath == "/resources/styles/core.css")
            {
                foreach (IRequestHandler rh in site.Handlers)
                {
                    if (rh is IconsHandler)
                    {
                        request.ResponseWriter.WriteLine("/* /resources/styles/icons.css */");
                        request.ResponseWriter.WriteLine(((IconsHandler)rh).IconCSS);
                        break;
                    }

                }
            }
            else if (request.URL.AbsolutePath == "/resources/scripts/user.js")
            {
                Template st = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.Site.Deployments.home.js"));
                st.SetAttribute("components", parts);
                request.ResponseWriter.Write(st.ToString());
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
