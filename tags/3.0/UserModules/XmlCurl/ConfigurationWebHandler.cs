using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Template = Org.Reddragonit.Stringtemplate.Template;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.XmlCurl
{
    public class ConfigurationWebHandler : IEmbeddedHandler
    {
        internal const string BASE_PATH = "conf/";
        internal const string CONFIGURATION_PATH = "configurations";
        internal const string DIRECTORY_PATH = "directory";
        internal const string DIALPLAN_PATH = "dialplans";

        private const string CONFIGURATION_FILE_TEMPLATE = "<?xml version=\"1.0\"?>\n<document type=\"freeswitch/xml\">\n<section name=\"{0}\">\n{1}\n</section></document>";
        private const string NOT_FOUND_RESPONSE = "<?xml version=\"1.0\" ?>\n<document type=\"freeswitch/xml\">\n<section name=\"result\">\n<result status=\"not found\" />\n</section>\n</document>";
        private const int CACHE_SIZE = 10;

        #region IEmbeddedHandler Members

        public void HandleRequest(HttpRequest request, Site site)
        {
            bool found = false;
            request.ResponseHeaders.ContentType = "text/xml";
            string confPath = request.URL.AbsolutePath.Substring((EmbeddedHandlerFactory.BASE_PATH + BASE_PATH).Length);
            if (confPath.Contains("/"))
                confPath = confPath.Substring(0, confPath.IndexOf('/'));
            switch (confPath)
            {
                case CONFIGURATION_PATH:
                    string fileName = request.Parameters["key_value"] + ".xml";
                    CoreGenerator.Lock();
                    if (CoreGenerator.Modules != null)
                    {
                        for(int x=0;x<CoreGenerator.Modules.Count;x++)
                        {
                            if (CoreGenerator.Modules[x].FileName == fileName)
                            {
                                sFreeSwitchModuleFile mod = CoreGenerator.Modules[x].File;
                                if (fileName == "sofia.conf.xml")
                                    mod = VirtualConfigDeployer.FixSofiaFile(mod);
                                request.ResponseWriter.Write(String.Format(CONFIGURATION_FILE_TEMPLATE, request.Parameters["section"], mod.ToConfigurationString(request.Parameters["tag_name"])));
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                        request.ResponseWriter.WriteLine(NOT_FOUND_RESPONSE);
                    CoreGenerator.UnLock();
                    break;
                case DIRECTORY_PATH:
                    ProcessDirectoryRequest(request);
                    break;
                case DIALPLAN_PATH:
                    string cont = VirtualConfigDeployer.GetCachedProfile(request.Parameters["variable_sofia_profile_name"]);
                    if (cont != null)
                    {
                        request.ResponseWriter.Write(cont);
                    }
                    else
                    {
                        CoreGenerator.Lock();
                        Template st = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.UserModules.XmlCurl.resources.Context.st"));
                        for (int y = 0; y < CoreGenerator.Profiles.Count; y++)
                        {
                            if (CoreGenerator.Profiles[y].Name == request.Parameters["variable_sofia_profile_name"])
                            {
                                st.SetAttribute("context", CoreGenerator.Profiles[y].Context);
                                st.SetAttribute("contextName", request.Parameters["Caller-Context"]);
                                st.SetAttribute("destinationNumber", request.Parameters["Caller-Destination-Number"]);
                                System.Diagnostics.Debug.WriteLine(st.ToString());
                                request.ResponseWriter.Write(st.ToString());
                                VirtualConfigDeployer.CacheProfile(request.Parameters["variable_sofia_profile_name"], st.ToString());
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            request.ResponseWriter.WriteLine(NOT_FOUND_RESPONSE);
                        CoreGenerator.UnLock();
                    }
                    break;
            }
        }

        private void ProcessDirectoryRequest(HttpRequest request)
        {
            switch (request.Parameters["Event-Name"])
            {
                case "REQUEST_PARAMS":
                    if (request.Parameters["purpose"] != null)
                    {
                        switch (request.Parameters["purpose"])
                        {
                            case "gateways":
                                Template st = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.UserModules.XmlCurl.resources.domain.st"));
                                CoreGenerator.Lock();
                                st.SetAttribute("domains", CoreGenerator.Domains);
                                st.SetAttribute("section", request.Parameters["section"]);
                                request.ResponseWriter.Write(st.ToString());
                                CoreGenerator.UnLock();
                                break;
                            default:
                                request.ResponseWriter.WriteLine(NOT_FOUND_RESPONSE);
                                break;
                        }
                    }
                    else
                    {
                        sDeployedExtension ext = null;
                        sDeployedDomain dom = null;
                        switch (request.Parameters["action"])
                        {
                            case "sip_auth": 
                                CoreGenerator.Lock();
                                for (var x = 0; x < CoreGenerator.Extensions.Count; x++)
                                {
                                    if ((CoreGenerator.Extensions[x].Number == request.Parameters["user"])
                                        && (CoreGenerator.Extensions[x].DomainName == request.Parameters["domain"]))
                                    {
                                        ext = CoreGenerator.Extensions[x];
                                        break;
                                    }
                                }
                                if (ext != null)
                                {
                                    for (int x = 0; x < CoreGenerator.Domains.Count; x++)
                                    {
                                        if (CoreGenerator.Domains[x].Name == ext.DomainName)
                                        {
                                            dom = CoreGenerator.Domains[x];
                                            break;
                                        }
                                    }
                                }
                                CoreGenerator.UnLock();
                                if (ext == null)
                                    request.ResponseWriter.WriteLine(NOT_FOUND_RESPONSE);
                                else
                                {
                                    Template st = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.UserModules.XmlCurl.resources.Extension.st"));
                                    st.SetAttribute("ext", ext);
                                    st.SetAttribute("domain", dom);
                                    st.SetAttribute("section", request.Parameters["section"]);
                                    request.ResponseWriter.Write(st.ToString());
                                }        
                                break;
                            case "reverse-auth-lookup":
                                CoreGenerator.Lock();
                                for (var x = 0; x < CoreGenerator.Extensions.Count; x++)
                                {
                                    if ((CoreGenerator.Extensions[x].Number == request.Parameters["user"])
                                        && (CoreGenerator.Extensions[x].DomainName == request.Parameters["domain"]))
                                    {
                                        ext = CoreGenerator.Extensions[x];
                                        break;
                                    }
                                }
                                CoreGenerator.UnLock();
                                if (ext == null)
                                    request.ResponseWriter.WriteLine(NOT_FOUND_RESPONSE);
                                else
                                {
                                    Template st = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.UserModules.XmlCurl.resources.ReverseAuth.st"));
                                    st.SetAttribute("extension", ext);
                                    st.SetAttribute("section", request.Parameters["section"]);
                                    request.ResponseWriter.Write(st.ToString());
                                }
                                break;
                            default:
                                request.ResponseWriter.WriteLine(NOT_FOUND_RESPONSE);
                                break;
                        }
                    }
                    break;
                case "GENERAL":
                    if (request.Parameters["action"] == "message-count")
                    {
                        string mcont = VirtualConfigDeployer.GetCachedMessageCount(request.Parameters["user"], request.Parameters["domain"]);
                        if (mcont != null)
                        {
                            request.ResponseWriter.Write(mcont);
                        }
                        else
                        {
                            sDeployedExtension ext = null;
                            sDeployedDomain dom = null;
                            CoreGenerator.Lock();
                            for (var x = 0; x < CoreGenerator.Extensions.Count; x++)
                            {
                                if ((CoreGenerator.Extensions[x].Number == request.Parameters["user"])
                                    && (CoreGenerator.Extensions[x].DomainName == request.Parameters["domain"]))
                                {
                                    ext = CoreGenerator.Extensions[x];
                                    break;
                                }
                            }
                            if (ext != null)
                            {
                                for (int x = 0; x < CoreGenerator.Domains.Count; x++)
                                {
                                    if (CoreGenerator.Domains[x].Name == ext.DomainName)
                                    {
                                        dom = CoreGenerator.Domains[x];
                                        break;
                                    }
                                }
                            }
                            CoreGenerator.UnLock();
                            if (ext == null)
                                request.ResponseWriter.WriteLine(NOT_FOUND_RESPONSE);
                            else
                            {
                                Template st = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.UserModules.XmlCurl.resources.MessageCount.st"));
                                st.SetAttribute("ext", ext);
                                st.SetAttribute("domain", dom);
                                st.SetAttribute("section", request.Parameters["section"]);
                                request.ResponseWriter.Write(st.ToString());
                                VirtualConfigDeployer.CacheMessageCount(ext.Number, dom.Name, st.ToString());
                            }
                        }
                    }
                    else
                        request.ResponseWriter.WriteLine(NOT_FOUND_RESPONSE);
                    break;
                default:
                    request.ResponseWriter.WriteLine(NOT_FOUND_RESPONSE);
                    break;
            }
        }

        public bool RequiresSessionForRequest(HttpRequest request, Site site)
        {
            return false;
        }

        #endregion
    }
}
