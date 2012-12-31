using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using System.Threading;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using System.IO;
using Org.Reddragonit.Stringtemplate;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;

namespace Org.Reddragonit.FreeSwitchConfig.UserModules.XmlCurl
{
    public class VirtualConfigDeployer : IConfigDeployer
    {

        private const string CONFIGURATION_BINDING_TEMPLATE = "<binding name=\"curl_{0}\">\n<param name=\"gateway-url\" value=\"http://{1}:{2}{3}\" bindings=\"{0}\"/></binding>";

        private void DeployCurlFile(sFreeSwitchModuleFile file)
        {
            string confSection = file.ConfigurationSectionString;
            string ip = (string)Settings.Current[Constants.SERVER_IP_SETTING_NAME];
            int port = (int)Settings.Current[Constants.SERVER_PORT_SETTING_NAME];
            ip = (ip == null ? "127.0.0.1" : (ip == "0.0.0.0" ? "127.0.0.1" : ip));
            confSection = confSection.Replace("<bindings>", "<bindings>" + String.Format(CONFIGURATION_BINDING_TEMPLATE, new object[]{
                "configuration",
                ip,
                port,
                EmbeddedHandlerFactory.BASE_PATH+ConfigurationWebHandler.BASE_PATH+ConfigurationWebHandler.CONFIGURATION_PATH
            }) + String.Format(CONFIGURATION_BINDING_TEMPLATE, new object[]{
                "directory",
                ip,
                port,
                EmbeddedHandlerFactory.BASE_PATH+ConfigurationWebHandler.BASE_PATH+ConfigurationWebHandler.DIRECTORY_PATH
            }) + String.Format(CONFIGURATION_BINDING_TEMPLATE, new object[]{
                "dialplan",
                ip,
                port,
                EmbeddedHandlerFactory.BASE_PATH+ConfigurationWebHandler.BASE_PATH+ConfigurationWebHandler.DIALPLAN_PATH
            }));
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR + Path.DirectorySeparatorChar + Constants.DEFAULT_AUTOLOAD_CONF_DIR);
            if (!di.Exists)
                di.Create();
            FileInfo fi = new FileInfo(di.FullName + Path.DirectorySeparatorChar + file.Name + ".xml");
            StreamWriter sw = fi.CreateText();
            sw.Write(confSection);
            sw.Flush();
            sw.Close();
        }

        private void DeployModulesFile(sFreeSwitchModuleFile file)
        {
            string confSection = file.ConfigurationSectionString;
            confSection = confSection.Replace("<load module=\"mod_xml_curl\"/>", "");
            confSection = confSection.Replace("<modules>", "<modules><load module=\"mod_xml_curl\"/>");
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR + Path.DirectorySeparatorChar + Constants.DEFAULT_AUTOLOAD_CONF_DIR);
            if (!di.Exists)
                di.Create();
            FileInfo fi = new FileInfo(di.FullName + Path.DirectorySeparatorChar + file.Name + ".xml");
            StreamWriter sw = fi.CreateText();
            sw.Write(confSection);
            sw.Flush();
            sw.Close();
        }

        internal static sFreeSwitchModuleFile FixSofiaFile(sFreeSwitchModuleFile file)
        {
            string confSection = file.ConfigurationSectionString;
            if (confSection.Contains("<profiles>")){
                string pre = confSection.Substring(0,confSection.IndexOf("<profiles>"));
                confSection = pre + confSection.Substring(confSection.IndexOf("</profiles>") + "</profiles>".Length);
            }
            Template st = new Template(Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.UserModules.XmlCurl.resources.sip_profile.st"));
            CoreGenerator.Lock();
            st.SetAttribute("profiles",CoreGenerator.Profiles);
            confSection = confSection.Replace("</configuration>",st.ToString()+"</configuration>");
            CoreGenerator.UnLock();
            return new sFreeSwitchModuleFile(file.Name,file.Description,confSection);
        }

        private static Dictionary<string, string> _cachedDialPlans;
        private static Dictionary<string, string> _cachedMessageCount;

        #region Caching
        internal static string GetCachedProfile(string profileName)
        {
            string ret = null;
            lock (_cachedDialPlans)
            {
                if (_cachedDialPlans.ContainsKey(profileName))
                    ret = _cachedDialPlans[profileName];
            }
            return ret;
        }

        internal static void CacheProfile(string profileName, string content)
        {
            lock (_cachedDialPlans)
            {
                if (_cachedDialPlans.ContainsKey(profileName))
                    _cachedDialPlans.Remove(profileName);
                _cachedDialPlans.Add(profileName, content);
            }
        }

        internal static string GetCachedMessageCount(string extension, string domain)
        {
            string ret = null;
            lock (_cachedMessageCount)
            {
                if (_cachedMessageCount.ContainsKey(extension+"@"+domain))
                    ret = _cachedMessageCount[extension+"@"+domain];
            }
            return ret;
        }

        internal static void CacheMessageCount(string extension, string domain, string content)
        {
            lock (_cachedMessageCount)
            {
                if (_cachedMessageCount.ContainsKey(extension + "@" + domain))
                    _cachedMessageCount.Remove(extension + "@" + domain);
                _cachedMessageCount.Add(extension + "@" + domain, content);
            }
        }
        #endregion

        static VirtualConfigDeployer()
        {
            _cachedDialPlans = new Dictionary<string, string>();
            _cachedMessageCount = new Dictionary<string, string>();
        }

        #region IConfigDeployer Members

        public string Name
        {
            get { return "Xml Curl Deployer"; }
        }

        public string Description
        {
            get { return "This deployment method uses mod_xml_curl to minimize writing data to the hard drive."; }
        }

        public bool IsValidToUse
        {
            get { return ModuleController.Current.IsModuleEnabled("XmlCurl"); }
        }

        public void DeployDomain(sDeployedDomain domain)
        {
            lock (_cachedMessageCount)
            {
                string[] keys = new string[_cachedMessageCount.Count];
                _cachedMessageCount.Keys.CopyTo(keys, 0);
                foreach (string str in keys)
                {
                    if (str.EndsWith("@" + domain.Name))
                        _cachedMessageCount.Remove(str);
                }
            }
        }

        public void DestroyDomain(string domainName)
        {
            lock (_cachedMessageCount)
            {
                string[] keys = new string[_cachedMessageCount.Count];
                _cachedMessageCount.Keys.CopyTo(keys, 0);
                foreach (string str in keys)
                {
                    if (str.EndsWith("@" + domainName))
                        _cachedMessageCount.Remove(str);
                }
            }
        }

        public void DeploySipProfile(sDeployedProfile profile)
        {
            lock (_cachedDialPlans)
            {
                if (_cachedDialPlans.ContainsKey(profile.Name))
                    _cachedDialPlans.Remove(profile.Name);
            }
        }

        public void DestroySipProfile(string profileName)
        {
            lock (_cachedDialPlans)
            {
                if (_cachedDialPlans.ContainsKey(profileName))
                    _cachedDialPlans.Remove(profileName);
            }
        }

        public void DeployExtension(sDeployedExtension ext)
        {
            lock (_cachedMessageCount)
            {
                _cachedMessageCount.Remove(ext.Number + "@" + ext.DomainName);
            }
        }

        public void DestroyExtension(string domainName, string extensionNumber)
        {
            lock (_cachedMessageCount)
            {
                _cachedMessageCount.Remove(extensionNumber + "@" + domainName);
            }
        }

        public void DeployIncomingSipTrunk(sDeployedIncomingSIPTrunk trunk)
        {
        }

        public void DestroyIncomingSipTrunk(string domainName, string extensionNumber)
        {
        }

        public void DeployContext(sDeployedContext context)
        {
            lock (_cachedDialPlans)
            {
                foreach (sDeployedProfile prof in CoreGenerator.Profiles)
                {
                    if (prof.ContextName == context.Name)
                        _cachedDialPlans.Remove(prof.Name);
                }
            }
        }

        public void DestroyContext(string contextName)
        {
            lock (_cachedDialPlans)
            {
                foreach (sDeployedProfile prof in CoreGenerator.Profiles)
                {
                    if (prof.ContextName == contextName)
                        _cachedDialPlans.Remove(prof.Name);
                }
            }
        }

        public void DeployVarsFile(string content)
        {
            CoreGenerator.Lock();
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR);
            if (!di.Exists)
            {
                di.Create();
            }
            FileInfo fi = new FileInfo(di.FullName + Path.DirectorySeparatorChar + "vars.xml");
            if (fi.Exists)
                fi.Delete();
            StreamWriter sw = new StreamWriter(fi.OpenWrite());
            sw.Write(content);
            sw.Flush();
            sw.Close();
            CoreGenerator.UnLock();
        }

        public void DeployModuleFile(string fileName, sFreeSwitchModuleFile file)
        {
            CoreGenerator.Lock();
            if (fileName == "modules.conf.xml")
                DeployModulesFile(file);
            else if (fileName == "xml_curl.conf.xml")
                DeployCurlFile(file);
            CoreGenerator.UnLock();
        }

        public void DeployOutgoingSipTrunk(sDeployedOutgoingSipTrunk trunk)
        {
        }

        public void DestroyOutgoingSipTrunk(string profileName, string name)
        {
        }

        public void Init(List<sDeployedDomain> domains, List<sDeployedExtension> extensions, 
            List<sDeployedIncomingSIPTrunk> itrunks, List<sDeployedProfile> profiles, 
            List<sDeployedOutgoingSipTrunk> trunks, List<sDeployedContext> contexts, 
            List<sDeployedModule> modules, string varsContent)
        {
            foreach (sDeployedModule mod in modules)
                DeployModuleFile(mod.FileName, mod.File);
        }

        public void Shutdown()
        {
            
        }

        #endregion
    }
}
