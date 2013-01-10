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

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class CoreJSCSSHandler : IRequestHandler
    {
        private const string _BASE_PATH = "Org.Reddragonit.FreeSwitchConfig.Site.Handlers.resources.";
        private const double _PANEL_PERCENTAGE = 0.90;

        private static readonly string[] _BACKBONE_JS_LIST = new string[]{
            "Org.Reddragonit.EmbeddedWebServer.resources.jquery.min.js",
            "Org.Reddragonit.EmbeddedWebServer.resources.json2.min.js",
            "Org.Reddragonit.BackBoneDotNet.resources.underscore-min.js",
            "Org.Reddragonit.BackBoneDotNet.resources.backbone.min.js",
            "Org.Reddragonit.BackBoneDotNet.resources.backbone.validate.js"
        };

        private static readonly string[] _DESKTOP_JS_LIST = new string[]{
            "desktop.scripts.jquery.preloadcssImages.js",
            "desktop.scripts.jquery.jquery-contextMenu.js",
            "desktop.scripts.main.js",
            "desktop.scripts.modals.js",
            "desktop.scripts.form.js",
            "desktop.scripts.tables.js",
            "desktop.scripts.validations.js",
            "desktop.scripts.extensions.js"
        };

        private static readonly string[] _MOBILE_JS_LIST = new string[]{
            "mobile.scripts.jquery.min.js",
            "desktop.scripts.jquery.jquery-contextMenu.js",
            "desktop.scripts.main.js",
            "desktop.scripts.modals.js",
            "desktop.scripts.form.js",
            "desktop.scripts.tables.js",
            "desktop.scripts.validations.js",
            "desktop.scripts.extensions.js"
        };

        private static readonly string[] _MOBILE_CSS_LIST = new string[]{
            "mobile.styles.jquery.min.css",
            "mobile.styles.jquery_structure.min.css",
            "mobile.styles.jquery_theme.min.css",
            "mobile.styles.main.css",
            "desktop.styles.core.css",
            "desktop.styles.icons.css",
            "desktop.styles.Core.home.css",
            "desktop.styles.jquery.jquery-contextMenu.css"
        };

        private static readonly string[] _DESKTOP_CSS_LIST = new string[]{
            "desktop.styles.core.css",
            "desktop.styles.icons.css",
            "desktop.styles.Core.home.css",
            "desktop.styles.cupertino.jquery-ui.css",
            "desktop.styles.anytime.css",
            "desktop.styles.jquery.jquery-contextMenu.css"
        };

        private static readonly string[] _DESKTOP_LOGGED_IN_JS_LIST = new string[]{
            "desktop.scripts.jquery.jquery-ui.min.js",
            "desktop.scripts.jquery.jquery-bt.js",
            "desktop.scripts.jquery.jquery-maskedinput.min.js",
            "desktop.scripts.jquery.ajaxupload.js",
            "desktop.scripts.DateTimePicker.js",
            "desktop.scripts.AudioPlayer.js",
            "desktop.scripts.Menus.js"
        };

        private static readonly string[] _MOBILE_LOGGED_IN_JS_LIST = new string[]{
            "desktop.scripts.jquery.jquery-ui.min.js",
            "desktop.scripts.jquery.jquery-bt.js",
            "desktop.scripts.jquery.jquery-maskedinput.min.js",
            "desktop.scripts.jquery.ajaxupload.js",
            "desktop.scripts.DateTimePicker.js",
            "desktop.scripts.AudioPlayer.js",
            "mobile.scripts.Menus.js"
        };

        private string _desktopJs;
        private string _mobileJs;
        private string _mobileCss;
        private string _desktopCss;
        private string _desktopLoggedInJs;
        private string _mobileLoggedInJs;

        #region IRequestHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public bool CanProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            return request.URL.AbsolutePath == "/resources/scripts/core.js"
                || request.URL.AbsolutePath == "/mobile/resources/scripts/core.js"
                || request.URL.AbsolutePath == "/mobile/resources/styles/core.css"
                || request.URL.AbsolutePath == "/resources/styles/core.css"
                || request.URL.AbsolutePath == "/resources/scripts/loggedIn.js"
                || request.URL.AbsolutePath=="/mobile/resources/scripts/loggedIn.js";
        }

        public void ProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            request.ResponseHeaders["Cache-Control"] = "Private";
            switch (request.URL.AbsolutePath)
            {
                case "/resources/scripts/core.js":
                    request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(".js");
                    WriteConstants(request);
                    request.ResponseWriter.WriteLine(_desktopJs);
                    break;
                case "/mobile/resources/scripts/core.js":
                    request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(".js");
                    WriteConstants(request);
                    request.ResponseWriter.Write(_mobileJs);
                    break;
                case "/mobile/resources/styles/core.css":
                    request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(".css");
                    request.ResponseWriter.Write(_mobileCss);
                    break;
                case "/resources/styles/core.css":
                    request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(".css");
                    request.ResponseWriter.WriteLine(_desktopCss);
                    Template ts = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.Site.Deployments.main.css"));
                    ts.SetAttribute("maxPanelHeight", (double.Parse(request.Parameters["Height"]) < 601 ? 601 : double.Parse(request.Parameters["Height"])) * _PANEL_PERCENTAGE);
                    ts.SetAttribute("maxPanelWidth", (double.Parse(request.Parameters["Width"]) < 990 ? 990 : double.Parse(request.Parameters["Width"])) * _PANEL_PERCENTAGE);
                    request.ResponseWriter.Write("\n/*/resources/styles/main.css*/\n");
                    request.ResponseWriter.Write(CSSMinifier.Minify(ts.ToString()));
                    break;
                case "/resources/scripts/loggedIn.js":
                    request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(".js");
                    request.ResponseWriter.WriteLine(_desktopLoggedInJs);
                    break;
                case "/mobile/resources/scripts/loggedIn.js":
                    request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(".js");
                    request.ResponseWriter.WriteLine(_mobileLoggedInJs);
                    break;
            }
            if (request.URL.AbsolutePath.EndsWith(".js"))
            {
                foreach (IRequestHandler handler in site.Handlers)
                {
                    if (handler is ModelHandler)
                    {
                        if (handler.CanProcessRequest(request, site))
                        {
                            handler.ProcessRequest(request, site);
                        }
                    }
                }
            }
        }

        public void Init()
        {
            _desktopJs = "";
            foreach (string str in _BACKBONE_JS_LIST)
            {
                _desktopJs += "\n//" + ExtractURL(str) + "\n";
                if (!str.EndsWith("min.js"))
                    _desktopJs += JSMinifier.Minify(Utility.ReadEmbeddedResource(str));
                else
                    _desktopJs += Utility.ReadEmbeddedResource(str);
            }
            foreach (string str in _DESKTOP_JS_LIST)
            {
                _desktopJs += "\n//" + ExtractURL(str)+"\n";
                //if (!str.Contains(".min.js"))
                //    _desktopJs += JSMinifier.Minify(Utility.ReadEmbeddedResource(_BASE_PATH + str));
                //else
                    _desktopJs += Utility.ReadEmbeddedResource(_BASE_PATH + str);
            }
            _mobileJs = "";
            foreach (string str in _BACKBONE_JS_LIST)
            {
                _mobileJs += "\n//" + ExtractURL(str) + "\n";
                if (!str.EndsWith("min.js"))
                    _mobileJs += JSMinifier.Minify(Utility.ReadEmbeddedResource(str));
                else
                    _mobileJs += Utility.ReadEmbeddedResource(str);
            }
            foreach (string str in _MOBILE_JS_LIST)
            {
                _mobileJs += "\n//" + ExtractURL(str) + "\n";
                if (!str.Contains(".min.js"))
                    _mobileJs += JSMinifier.Minify(Utility.ReadEmbeddedResource(_BASE_PATH + str));
                else
                    _mobileJs += Utility.ReadEmbeddedResource(_BASE_PATH + str);
            }
            _mobileCss = "";
            foreach (string str in _MOBILE_CSS_LIST)
            {
                _mobileCss += "\n/*" + ExtractURL(str) + "*/\n";
                if (!str.Contains(".min.css"))
                    _mobileCss += CSSMinifier.Minify(Utility.ReadEmbeddedResource(_BASE_PATH + str));
                else
                    _mobileCss += Utility.ReadEmbeddedResource(_BASE_PATH + str);
            }
            _desktopCss = "";
            foreach (string str in _DESKTOP_CSS_LIST)
            {
                _desktopCss += "\n/*" + ExtractURL(str) + "*/\n";
                if (!str.Contains(".min.css"))
                    _desktopCss += CSSMinifier.Minify(Utility.ReadEmbeddedResource(_BASE_PATH + str));
                else
                    _desktopCss += Utility.ReadEmbeddedResource(_BASE_PATH + str);
            }
            _desktopLoggedInJs = "Backbone=_.extend(Backbone,{HasConfigurationChangesToMake:" + ConfigurationController.HasChangesToMake.ToString().ToLower() + "});\n";
            foreach (String str in _DESKTOP_LOGGED_IN_JS_LIST)
            {
                _desktopLoggedInJs += "\n//" + ExtractURL(str) + "\n";
                if (!str.Contains(".min.js"))
                    _desktopLoggedInJs += JSMinifier.Minify(Utility.ReadEmbeddedResource(_BASE_PATH + str));
                else
                    _desktopLoggedInJs += Utility.ReadEmbeddedResource(_BASE_PATH + str);
            }
            _mobileLoggedInJs = "Backbone=_.extend(Backbone,{HasConfigurationChangesToMake:" + ConfigurationController.HasChangesToMake.ToString().ToLower() + "});\n";
            foreach (String str in _MOBILE_LOGGED_IN_JS_LIST)
            {
                _mobileLoggedInJs += "\n//" + ExtractURL(str) + "\n";
                if (!str.Contains(".min.js"))
                    _mobileLoggedInJs += JSMinifier.Minify(Utility.ReadEmbeddedResource(_BASE_PATH + str));
                else
                    _mobileLoggedInJs += Utility.ReadEmbeddedResource(_BASE_PATH + str);
            }
        }

        public void DeInit()
        {
        }

        public bool RequiresSessionForRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            return false;
        }

        #endregion

        private string ExtractURL(string path)
        {
            string url = "/"+path.Replace(".", "/");
            url = url.Substring(0, url.LastIndexOf("/")) + "." + url.Substring(url.LastIndexOf("/") + 1);
            EmbeddedFileTypes ftype = EmbeddedFileTypes.Text;
            ImageTypes? itype = null;
            if (path.EndsWith(".min.js"))
            {
                ftype = EmbeddedFileTypes.Compressed_Javascript;
                url = url.Substring(0, url.Length - "/min.js".Length) + ".min.js";
            }
            else if (path.EndsWith(".min.css"))
            {
                ftype = EmbeddedFileTypes.Compressed_Javascript;
                url = url.Substring(0, url.Length - "/min.css".Length) + ".min.css";
            }
            return url;
        }

        private string EscapeCode(string code)
        {
            return code.Replace("\\", "\\\\");
        }

        private void WriteConstants(HttpRequest request)
        {
            StringBuilder js = new StringBuilder();
            js.AppendLine("var DIRECTORY_SEPERATOR = '" + EscapeCode(Path.DirectorySeparatorChar.ToString()) + "';");
            js.AppendLine("var HELP_CLASS= 'help';");
            js.AppendLine("var NPANXX_HELP='N = [2-9]<br/>Z = [1-9]<br/>X = [0-9]<br/>. = \\d+<br/>| = ignore on output, bracket in the validation';");
            js.AppendLine("var IS_MOBILE=" + (request.URL.AbsolutePath.StartsWith("/mobile") ? "true" : (request.Headers.Browser.IsMobile ? "true" : "false")) + ";");
            js.AppendLine("var CURRENT_OS=" + JSON.JsonEncode(Utility.OperatingSystem) + ";");
            js.AppendLine("var PORT_RANGE_REGEX = '" + Constants.PORT_RANGE_REGEX + "';");
            js.AppendLine("var IS_SETUP = " + Utility.IsSiteSetup.ToString().ToLower() + ";");
            if (JavascriptConstants.Current.Count > 0)
            {
                foreach (string str in JavascriptConstants.Current.Keys)
                {
                    js.AppendLine("var " + str + " = '" + EscapeCode(JavascriptConstants.Current[str]) + "';");
                }
            }
            request.ResponseWriter.WriteLine(JSMinifier.Minify(js.ToString()));
        }
    }
}
