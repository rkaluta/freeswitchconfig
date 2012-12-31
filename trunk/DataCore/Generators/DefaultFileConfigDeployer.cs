using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System;
using Org.Reddragonit.Stringtemplate;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators
{
    internal class DefaultFileConfigDeployer : IConfigDeployer
    {
        #region IConfigDeployer Members

        public string Name
        {
            get { return "Default Deployment Method"; }
        }

        public string Description
        {
            get { return "This is the Default Deployment Method, it uses the file system as per standard freeswitch deployments."; }
        }

        public bool IsValidToUse
        {
            get { return true; }
        }

        public void DeployDomain(sDeployedDomain domain)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_EXTENSIONS_DIRECTORY + Path.DirectorySeparatorChar + domain.Name);
            if (!di.Exists)
            {
                Log.Trace("Creating domain directory " + domain.Name + " for extensions");
                di.Create();
            }
            FileInfo fi = new FileInfo(di.FullName + ".xml");
            if (fi.Exists)
                fi.Delete();
            Log.Trace("Deploying domain configuration file for domain " + domain.Name);
            string str = Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.resources.BaseConfigurations.domain.st");
            Template st = new Template(str);
            st.SetAttribute("domain", domain);
            FileStream fs = fi.OpenWrite();
            st.WriteToStream(fs);
            fs.Close();
        }

        public void DestroyDomain(string domainName)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_EXTENSIONS_DIRECTORY + Path.DirectorySeparatorChar + domainName);
            if (di.Exists)
            {
                Log.Trace("Dropping domain directory " + domainName);
                di.Delete();
            }
            FileInfo fi = new FileInfo(di.FullName + ".xml");
            if (fi.Exists)
                fi.Delete();
        }

        public void DeploySipProfile(sDeployedProfile profile)
        {
            string str = Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.resources.BaseConfigurations.sip_profile.st");
            Template st = new Template(str);
            st.SetAttribute("profile", profile);
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_SIP_PROFILES_PATH + Path.DirectorySeparatorChar + profile.Name);
            if (!di.Exists)
            {
                Log.Trace("Creating SIP profile directory " + profile.Name + " for gateways");
                di.Create();
            }
            FileInfo fi = new FileInfo(di.FullName + ".xml");
            if (fi.Exists)
                fi.Delete();
            Log.Trace("Deploying SIP profile configuration file for sip_profile " + profile.Name);
            FileStream fs = fi.OpenWrite();
            st.WriteToStream(fs);
            fs.Close();
        }

        public void DestroySipProfile(string profileName)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
               Path.DirectorySeparatorChar + Constants.DEFAULT_SIP_PROFILES_PATH + Path.DirectorySeparatorChar + profileName);
            if (di.Exists)
            {
                Log.Trace("Droping SIP profile directory " + profileName);
                di.Delete();
            }
            FileInfo fi = new FileInfo(di.FullName + ".xml");
            if (fi.Exists)
                fi.Delete();
        }

        public void DeployExtension(sDeployedExtension extension)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_EXTENSIONS_DIRECTORY + Path.DirectorySeparatorChar + extension.DomainName);
            string str = Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.resources.Extension.st");
            Log.Trace("Generating config file for extension " + extension.Number);
            Template st = new Template(str);
            st.SetAttribute("ext", extension);
            FileStream fs = new FileStream(di.FullName + Path.DirectorySeparatorChar + extension.Number + ".xml", FileMode.Create, FileAccess.Write, FileShare.None);
            st.WriteToStream(fs);
            fs.Close();
        }

        public void DestroyExtension(string domainName, string extensionNumber)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_EXTENSIONS_DIRECTORY + Path.DirectorySeparatorChar + domainName);
            FileInfo fi = new FileInfo(di.FullName + Path.DirectorySeparatorChar + extensionNumber + ".xml");
            if (fi.Exists)
                fi.Delete();
        }

        public void DeployIncomingSipTrunk(sDeployedIncomingSIPTrunk trunk)
        {
            string intTrunkTemplate = Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.resources.SIPTrunk.Internal.st");
            Template st = new Template(intTrunkTemplate);
            st.SetAttribute("trunk", trunk);
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_EXTENSIONS_DIRECTORY + Path.DirectorySeparatorChar + trunk.DomainName);
            FileStream fs = new FileStream(di.FullName + Path.DirectorySeparatorChar + trunk.Number + ".xml", FileMode.Create, FileAccess.Write, FileShare.None);
            st.WriteToStream(fs);
            fs.Close();
        }

        public void DestroyIncomingSipTrunk(string domainName, string extensionNumber)
        {
            FileInfo fi = new FileInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_EXTENSIONS_DIRECTORY + Path.DirectorySeparatorChar + domainName + Path.DirectorySeparatorChar + extensionNumber + ".xml");
            if (fi.Exists)
                fi.Delete();
        }

        public void DeployContext(sDeployedContext context)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_DIALPLAN_DIR);
            if (!di.Exists)
            {
                Log.Trace("Creating context directory to deploy context");
                di.Create();
            }
            XmlContextFile cfile = context.ContextFile;
            StreamWriter sw = new StreamWriter(di.FullName+Path.DirectorySeparatorChar+context.Name+".xml");
            sw.Write(cfile.ToXMLContent(false));
            sw.Flush();
            sw.Close();
            di = new DirectoryInfo(di.FullName + Path.DirectorySeparatorChar + context.Name);
            if (!di.Exists)
            {
                Log.Trace("Creating context directory");
                di.Create();
            }
            else
            {
                Log.Trace("Cleaning out files for context directory");
                foreach (FileInfo fi in di.GetFiles())
                    fi.Delete();
            }
            int index = 0;
            foreach (XmlContextFile xcf in cfile.Includes)
            {
                sw = new StreamWriter(di.FullName + Path.DirectorySeparatorChar+index.ToString("00000")+"_" + xcf.FileName + ".xml");
                sw.Write(xcf.ToXMLContent(false));
                sw.Flush();
                sw.Close();
                index++;
            }
        }

        public void DestroyContext(string contextName)
        {
            FileInfo fi = new FileInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_DIALPLAN_DIR + Path.DirectorySeparatorChar + contextName + ".xml");
            if (fi.Exists)
                fi.Delete();
            DirectoryInfo di = new DirectoryInfo(fi.FullName.Substring(0, fi.FullName.Length - 4));
            if (di.Exists)
                di.Delete();
        }

        public void DeployVarsFile(string content)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR);
            if (!di.Exists)
            {
                Log.Trace("Creating conf directory to deploy vars.xml");
                di.Create();
            }
            FileInfo fi = new FileInfo(di.FullName+Path.DirectorySeparatorChar+"vars.xml");
            if (fi.Exists)
                fi.Delete();
            Log.Trace("Deploying vars.xml");
            StreamWriter sw = new StreamWriter(fi.OpenWrite());
            sw.Write(content);
            sw.Flush();
            sw.Close();
        }

        public void DeployModuleFile(string fileName, sFreeSwitchModuleFile file)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR + Path.DirectorySeparatorChar + Constants.DEFAULT_AUTOLOAD_CONF_DIR);
            FileInfo fi = new FileInfo(di.FullName + Path.DirectorySeparatorChar + fileName);
            if (fi.Exists)
                fi.Delete();
            if (file != null)
            {
                StreamWriter sw = new StreamWriter(fi.OpenWrite());
                sw.Write(file.ConfigurationSectionString);
                sw.Flush();
                sw.Close();
            }
        }

        public sFreeSwitchModuleFile ReadCurrentModuleSettings(string moduleName)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR + Path.DirectorySeparatorChar + Constants.DEFAULT_AUTOLOAD_CONF_DIR);
            FileInfo fi = new FileInfo(di.FullName + Path.DirectorySeparatorChar + moduleName+".conf.xml");
            if (fi.Exists)
            {
                StreamReader sr = fi.OpenText();
                string cont = sr.ReadToEnd();
                sr.Close();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(cont);
                return new sFreeSwitchModuleFile(doc.ChildNodes[1].Attributes["name"].Value,
                                doc.ChildNodes[1].Attributes["description"].Value,
                                doc.ChildNodes[1].OuterXml);
            }
            return null;
        }

        public void DeployOutgoingSipTrunk(sDeployedOutgoingSipTrunk trunk)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_SIP_PROFILES_PATH + Path.DirectorySeparatorChar + trunk.ProfileName);
            string str = Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.resources.SIPTrunk.External.st");
            Log.Trace("Generating config file for outgoing sip trunk " + trunk.Name);
            Template st = new Template(str);
            st.SetAttribute("trunk", trunk);
            FileStream fs = new FileStream(di.FullName + Path.DirectorySeparatorChar + trunk.Name + ".xml", FileMode.Create, FileAccess.Write, FileShare.None);
            st.WriteToStream(fs);
            fs.Close();
        }

        public void DestroyOutgoingSipTrunk(string profileName, string name)
        {
            FileInfo fi = new FileInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_SIP_PROFILES_PATH + Path.DirectorySeparatorChar + profileName + Path.DirectorySeparatorChar + name + ".xml");
            if (fi.Exists)
                fi.Delete();
        }

        public void Init(List<sDeployedDomain> domains, List<sDeployedExtension> extensions,
            List<sDeployedIncomingSIPTrunk> itrunks,List<sDeployedProfile> profiles, 
            List<sDeployedOutgoingSipTrunk> trunks,List<sDeployedContext> contexts, 
            List<sDeployedModule> modules, string varsContent)
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_EXTENSIONS_DIRECTORY);
            foreach (DirectoryInfo d in di.GetDirectories())
                d.Delete(true);
            foreach (FileInfo f in di.GetFiles())
                f.Delete();
            foreach (sDeployedDomain dd in domains)
                DeployDomain(dd);
            foreach (sDeployedExtension ext in extensions)
                DeployExtension(ext);
            foreach (sDeployedIncomingSIPTrunk trunk in itrunks)
                DeployIncomingSipTrunk(trunk);
            DeployVarsFile(varsContent);
            di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR + Path.DirectorySeparatorChar + Constants.DEFAULT_AUTOLOAD_CONF_DIR);
            foreach (FileInfo f in di.GetFiles())
                f.Delete();
            foreach (sDeployedModule mod in modules)
                DeployModuleFile(mod.FileName, mod.File);
            di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_DIALPLAN_DIR);
            foreach (FileInfo fi in di.GetFiles())
                fi.Delete();
            foreach (sDeployedContext cont in contexts)
                DeployContext(cont);
        }

        public void Shutdown()
        {
            DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_EXTENSIONS_DIRECTORY);
            foreach (DirectoryInfo d in di.GetDirectories())
                d.Delete(true);
            foreach (FileInfo f in di.GetFiles())
                f.Delete();
            di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR + Path.DirectorySeparatorChar + Constants.DEFAULT_AUTOLOAD_CONF_DIR);
            foreach (FileInfo f in di.GetFiles())
                f.Delete();
            di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR +
                Path.DirectorySeparatorChar + Constants.DEFAULT_DIALPLAN_DIR);
            foreach (FileInfo fi in di.GetFiles())
                fi.Delete();
        }

        #endregion
    }
}
