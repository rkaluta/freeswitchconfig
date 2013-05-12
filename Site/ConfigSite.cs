using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using System.Net;
using Org.Reddragonit.EmbeddedWebServer;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using System.Threading;
using System.Reflection;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Utility = Org.Reddragonit.FreeSwitchConfig.DataCore.Utility;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.EmbeddedWebServer.BasicHandlers;
using System.Configuration;
using Org.Reddragonit.EmbeddedWebServer.Diagnostics;

using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.Site.Handlers;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.Site.Services;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.EmbeddedWebServer.Components;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;

namespace Org.Reddragonit.FreeSwitchConfig.Site
{
    public class ConfigSite : Org.Reddragonit.EmbeddedWebServer.Interfaces.Site,IEventHandler
    {
        public static void TurnOnLoggingSocket()
        {
            Logger.OverrideDiagnosticsLevel(DiagnosticsLevels.TRACE);
            Logger.OverrideOutputLevel(DiagnosticsOutputs.SOCKET);
        }

        public static void TurnOffLoggingSocket()
        {
            Logger.ClearOverrideDiagnosticsLevel();
            Logger.ClearOverrideOutput();
        }

        public static void TurnOnConsoleLogging()
        {
            Logger.OverrideDiagnosticsLevel(DiagnosticsLevels.TRACE);
            Logger.OverrideOutputLevel(DiagnosticsOutputs.CONSOLE);
        }

        private static DiagnosticsLevels? _logLevel = null;

        public override DiagnosticsLevels DiagnosticsLevel
        {
            get
            {
                if (_logLevel == null)
                    _logLevel = (DiagnosticsLevels)Enum.Parse(typeof(DiagnosticsLevels), ConfigurationSettings.AppSettings["Org.Reddragonit.FreeSwitchConfig.Site.ConfigSite.DiagnosticsLevel"]);
                return _logLevel.Value;
            }
        }

        private static DiagnosticsOutputs? _output = null;

        public override DiagnosticsOutputs DiagnosticsOutput
        {
            get
            {
                if (_output == null)
                    _output = (DiagnosticsOutputs)Enum.Parse(typeof(DiagnosticsOutputs), ConfigurationSettings.AppSettings["Org.Reddragonit.FreeSwitchConfig.Site.ConfigSite.DiagnosticsOutput"]);
                return _output.Value;
            }
        }

        public override bool AddJqueryJavascript
        {
            get
            {
                return false;
            }
        }

        public override bool AddJsonJavascript
        {
            get
            {
                return false;
            }
        }

        public override string TMPPath
        {
            get
            {
                return "/tmp";
            }
        }

        public override string ServerName
        {
            get
            {
                if (Settings.Current[Constants.SERVER_NAME_SETTING_NAME] != null)
                    return Settings.Current[Constants.SERVER_NAME_SETTING_NAME].ToString();
                return null;
            }
        }

        public override Org.Reddragonit.EmbeddedWebServer.Components.SiteSessionTypes SessionStateType
        {
            get
            {
                return Org.Reddragonit.EmbeddedWebServer.Components.SiteSessionTypes.ThreadState;
            }
        }

        public override string DefaultPage(bool isMobile)
        {
            return "/index.html";
        }

        private const string SSL_SETTINGS_ID = "Org.Reddragonit.FreeSwitchConfig.Site.ConfigSite.SSLCertificatePath";
        private string SSLCertificatePath
        {
            get
            {
                if (ConfigurationSettings.AppSettings[SSL_SETTINGS_ID] != null)
                    return (ConfigurationSettings.AppSettings[SSL_SETTINGS_ID] == "" ? null : ConfigurationSettings.AppSettings[SSL_SETTINGS_ID]);
                return null;
            }
        }

        public override sIPPortPair[] ListenOn
        {
            get
            {
                if (Settings.Current[Constants.SERVER_PORT_SETTING_NAME] == null)
                    Settings.Current[Constants.SERVER_PORT_SETTING_NAME] = Constants.DEFAULT_SERVER_PORT_NUMBER;
                if (Settings.Current[Constants.SERVER_IP_SETTING_NAME] == null)
                    Settings.Current[Constants.SERVER_IP_SETTING_NAME] = IPAddress.Any.ToString();
                return new sIPPortPair[]{
                    new sIPPortPair(IPAddress.Parse(Settings.Current[Constants.SERVER_IP_SETTING_NAME].ToString()),(int)Settings.Current[Constants.SERVER_PORT_SETTING_NAME],SSLCertificatePath!=null)
                };
            }
        }

        public override X509Certificate GetCertificateForEndpoint(sIPPortPair pair)
        {
            if (SSLCertificatePath == null)
                return null;
            return X509Certificate.CreateFromCertFile(SSLCertificatePath);
        }

        protected override void PreStart()
        {
            EventController.RegisterEventHandler(this);

            _embeddedFiles = new Dictionary<string,sEmbeddedFile>();
            _embeddedFiles.Add("/resources/icons.png", new sEmbeddedFile("Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.icons.png", "/resources/icons.png", EmbeddedFileTypes.Image, ImageTypes.png));
            Assembly ass = this.GetType().Assembly;
            foreach (string str in ass.GetManifestResourceNames()){
                if (str.StartsWith("Org.Reddragonit.FreeSwitchConfig.Site.Web.resources.images"))
                {
                    string url = str.Substring("Org.Reddragonit.FreeSwitchConfig.Site.Web".Length);
                    url = url.Replace(".", "/");
                    url = url.Substring(0, url.LastIndexOf("/")) + "." + url.Substring(url.LastIndexOf("/") + 1);
                    EmbeddedFileTypes ftype = EmbeddedFileTypes.Text;
                    ImageTypes? itype = null;
                    if (str.EndsWith(".min.js"))
                    {
                        ftype = EmbeddedFileTypes.Compressed_Javascript;
                        url = url.Substring(0, url.Length - "/min.js".Length) + ".min.js";
                    }
                    else if (str.EndsWith(".js"))
                        ftype = EmbeddedFileTypes.Javascript;
                    else if (str.EndsWith(".min.css"))
                    {
                        ftype = EmbeddedFileTypes.Compressed_Css;
                        url = url.Substring(0, url.Length - "/min.css".Length) + ".min.css";
                    }
                    else if (str.EndsWith(".css"))
                        ftype = EmbeddedFileTypes.Css;
                    else if (str.EndsWith(".png"))
                    {
                        ftype = EmbeddedFileTypes.Image;
                        itype = ImageTypes.png;
                    }
                    else if (str.EndsWith(".gif"))
                    {
                        ftype = EmbeddedFileTypes.Image;
                        itype = ImageTypes.gif;
                    }
                    Log.Trace("Assigning embedded file " + str + " to the path " + url);
                    _embeddedFiles.Add(url,new sEmbeddedFile(str, url, ftype, itype));
                }
            }

            _embeddedServiceTypes = new List<Type>();
            _embeddedServiceTypes.Add(typeof(UserManagementService));
            _embeddedServiceTypes.Add(typeof(UserService));
            foreach (Type t in Utility.LocateTypeInstances(typeof(EmbeddedService), this.GetType().Assembly))
                _embeddedServiceTypes.Add(t);

            try
            {
                Initializer.Initialize();
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
            }
        }

        protected override void PreStop()
        {
            Initializer.DeInit();
            EventController.UnRegisterEventHandler(this);
        }

        public override IPEndPoint RemoteLoggingServer
        {
            get
            {
                return new IPEndPoint(IPAddress.Parse(ConfigurationSettings.AppSettings["Org.Reddragonit.FreeSwitchConfig.Site.ConfigSite.RemoteIP"]),
                    int.Parse(ConfigurationSettings.AppSettings["Org.Reddragonit.FreeSwitchConfig.Site.ConfigSite.RemotePort"]));
            }
        }

        private Dictionary<string, sEmbeddedFile> _embeddedFiles;
        private List<Type> _embeddedServiceTypes;

        public override Dictionary<string,sEmbeddedFile> EmbeddedFiles
        {
            get
            {
                return _embeddedFiles;
            }
        }

        public override List<Type> EmbeddedServiceTypes
        {
            get
            {
                return _embeddedServiceTypes;
            }
        }

        private static readonly List<IRequestHandler> _handlers = 
            new List<IRequestHandler>(
            new IRequestHandler[]{
                new IndexPageHandler(),
                new CoreJSCSSHandler(),
                new FileUpload(),
                new FileDownloader(),
                new SetupCompleteHandler(),
                new ModelHandler(),
                new EmbeddedResourceHandler(),
                new EmbeddedServiceHandler(),
                new EmbeddedHandlerFactory()
            });

        public override List<IRequestHandler> Handlers
        {
            get
            {
                return _handlers;
            }
        }

        protected override void PreRequest(HttpRequest request)
        {
            EventController.TriggerEvent(new HttpRequestRecievedEvent(request));
            if (!request.IsResponseSent)
            {
                if (!Utility.IsSiteSetup)
                {
                    if ((request.URL.AbsolutePath != "/resources/scripts/core.js") &&
                        (request.URL.AbsolutePath != "/resources/styles/core.css") &&
                        (request.URL.AbsolutePath != "/resources/scripts/setup.js") &&
                        (request.URL.AbsolutePath != "/resources/styles/setup.css") &&
                        !request.URL.AbsolutePath.StartsWith("/resources/images") &&
                        (request.URL.AbsolutePath!="/index.html") &&
                        (request.URL.AbsolutePath != "/") &&
                        !request.URL.AbsolutePath.StartsWith("/resources/icons/icons.png") &&
                        !request.URL.PathAndQuery.StartsWith("/EmbeddedJSGenerator.js?TYPE=Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users.UserService") &&
                        !request.URL.AbsolutePath.StartsWith(EmbeddedServiceHandler.GetPathForType(typeof(UserService)))&&
                        !request.URL.AbsolutePath.StartsWith("/core/models/sysconfig/SystemSetting")&&
                        !request.URL.AbsolutePath.StartsWith("/core/models/core/Domain")&&
                        !request.URL.AbsolutePath.StartsWith("/core/models/core/Context")&&
                        !request.URL.AbsolutePath.StartsWith("/core/models/core/SipProfile")&&
                        !request.URL.AbsolutePath.StartsWith("/core/models/sysconfig/NetworkInterface")&&
                        request.URL.AbsolutePath!="/core/SetupComplete")
                    {
                        request.ResponseHeaders.ContentType = HttpUtility.GetContentTypeForExtension(request.URL.AbsolutePath.Substring(request.URL.AbsolutePath.LastIndexOf(".")));
                        request.ResponseStatus = HttpStatusCodes.Forbidden;
                        request.SendResponse();
                    }
                }
            }
        }

        public override void PreSendResponseHeaders(HttpRequest request)
        {
            //set caching for embedded resources to last min 1 hour
            if (request.URL != null)
            {
                if (request.URL.AbsolutePath.EndsWith(".png")||request.URL.AbsolutePath.EndsWith(".js")||request.URL.AbsolutePath.EndsWith(".gif"))
                    request.ResponseHeaders["Cache-Control"] = "max-age = " + (60 * 60).ToString();
            }
        }

        protected override void PostRequest(HttpRequest request)
        {
            EventController.TriggerEvent(new HttpRequestCompleteEvent(request));
        }

        protected override bool RequestError(HttpRequest request, Exception error)
        {
            EventController.TriggerEvent(new ErrorOccuredEvent(error));
            EventController.TriggerEvent(new HttpRequestErrorEvent(request, error));
            return false;
        }

        public override bool CompressJS
        {
            get
            {
                if (ConfigurationSettings.AppSettings["Org.Reddragonit.FreeSwitchConfig.Site.ConfigSite.RemotePort"] != null)
                    return bool.Parse(ConfigurationSettings.AppSettings["Org.Reddragonit.FreeSwitchConfig.Site.ConfigSite.RemotePort"]);
                return true;
            }
        }

        public override int RequestTimeout
        {
            get
            {
                return int.MaxValue;
            }
        }

        protected override void PreAuthentication(HttpRequest request, out bool loadSession)
        {
            loadSession = Utility.IsSiteSetup;
        }

        protected override HttpAuthTypes GetAuthenticationTypeForUrl(Uri url, out string realm)
        {
            realm = null;
            if (!Utility.IsSiteSetup)
                return HttpAuthTypes.None;
            else if (url.AbsolutePath + url.Query == "/EmbeddedJSGenerator.js?TYPE=FreeswitchConfig.Services.UserService")
                return HttpAuthTypes.None;
            realm = Constants.HTTP_AUTH_REALM;
            return HttpAuthTypes.Digest;
        }

        protected override sHttpAuthUsernamePassword[] GetAuthenticationInformationForUrl(Uri url,string username)
        {
            return UserAuthentication.GetAuthenticationInformationForUrl(url,username);
        }

        protected override void PostAuthenticationFailure(HttpRequest request, string username)
        {
            UserAuthentication.PostAuthenticationFailure(request, username);
        }

        protected override void PostAuthentication(HttpRequest request, sHttpAuthUsernamePassword user)
        {
            UserAuthentication.PostAuthentication(request, user);
        }

        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            return Event is ModuleEnabledEvent || Event is ModuleDisabledEvent;
        }

        public void ProcessEvent(IEvent Event)
        {
            IModule mod=null;
            bool add = true;
            if (Event is ModuleEnabledEvent)
                mod = ((ModuleEnabledEvent)Event).Module;
            else if (Event is ModuleDisabledEvent)
            {
                mod = ((ModuleDisabledEvent)Event).Module;
                add = false;
            }
            if (mod.EmbeddedFiles != null)
            {
                foreach (sEmbeddedFile sef in mod.EmbeddedFiles)
                {
                    if (add)
                        _embeddedFiles.Add(sef.URL, sef);
                    else
                        _embeddedFiles.Remove(sef.URL);
                }
            }
            if (mod.EmbeddedServices != null)
            {
                foreach (Type t in mod.EmbeddedServices)
                {
                    _embeddedServiceTypes.Add(t);
                }
            }
        }

        #endregion
    }
}
