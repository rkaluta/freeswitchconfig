using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.Stringtemplate;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks.SIP;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using System.Threading;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.Events;


namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators
{
    public static class CoreGenerator
    {
        private static IConfigDeployer _deployer;

        public static Type CurrentDeploymentMethod
        {
            get
            {
                Monitor.Enter(_lock);
                Type t = _deployer.GetType();
                Monitor.Exit(_lock);
                return t;
            }
        }

        private const string TRUNKS_DIRECTORY = "/sip_profiles/external";
        private const string DEPLOYER_TYPE_SETTING_NAME = "ConfigurationDeploymentClass";

        private static object _lock = new object();

        public static void Lock()
        {
            Monitor.Enter(_lock);
        }

        public static void UnLock()
        {
            Monitor.Exit(_lock);
        }

        private static List<sDeployedDomain> domains
        {
            get { return (List<sDeployedDomain>)Settings.Current["CoreGenerator", "domains"]; }
            set { Settings.Current["CoreGenerator", "domains"] = value; }
        }

        public static List<sDeployedDomain> Domains
        {
            get { return domains; }
        }

        private static List<sDeployedExtension> extensions
        {
            get { return (List<sDeployedExtension>)Settings.Current["CoreGenerator", "extensions"]; }
            set { Settings.Current["CoreGenerator", "extensions"] = value; }
        }

        public static List<sDeployedExtension> Extensions
        {
            get { return extensions; }
        }

        private static List<sDeployedContext> contexts
        {
            get { return (List<sDeployedContext>)Settings.Current["CoreGenerator", "contexts"]; }
            set { Settings.Current["CoreGenerator", "contexts"] = value; }
        }

        public static List<sDeployedContext> Contexts
        {
            get { return contexts; }
        }

        private static List<sDeployedModule> modules
        {
            get { return (List<sDeployedModule>)Settings.Current["CoreGenerator", "modules"]; }
            set { Settings.Current["CoreGenerator", "modules"] = value; }
        }

        public static List<sDeployedModule> Modules
        {
            get { return modules; }
        }

        private static List<sDeployedProfile> profiles
        {
            get { return (List<sDeployedProfile>)Settings.Current["CoreGenerator", "profiles"]; }
            set { Settings.Current["CoreGenerator", "profiles"] = value; }
        }

        public static List<sDeployedProfile> Profiles
        {
            get { return profiles; }
        }

        private static string varsContent
        {
            get { return (string)Settings.Current["CoreGenerator", "varsContent"]; }
            set { Settings.Current["CoreGenerator", "varsContent"] = value; }
        }

        public static string VarsContent
        {
            get { return varsContent; }
        }

        private static List<sDeployedIncomingSIPTrunk> itrunks
        {
            get { return (List<sDeployedIncomingSIPTrunk>)Settings.Current["CoreGenerator", "itrunks"]; }
            set { Settings.Current["CoreGenerator", "itrunks"] = value; }
        }

        public static List<sDeployedIncomingSIPTrunk> ITrunks
        {
            get { return itrunks; }
        }

        private static List<sDeployedOutgoingSipTrunk> trunks
        {
            get { return (List<sDeployedOutgoingSipTrunk>)Settings.Current["CoreGenerator", "trunks"]; }
            set { Settings.Current["CoreGenerator", "trunks"] = value; }
        }

        public static List<sDeployedOutgoingSipTrunk> Trunks
        {
            get { return trunks; }
        }

        static CoreGenerator()
        {
            StreamReader sr;
            if (Settings.Current[Constants.BASE_PATH_NAME] == null)
                Settings.Current[Constants.BASE_PATH_NAME] = Constants.DEFAULT_BASE_BATH;
            if (Settings.Current["CoreGenerator", DEPLOYER_TYPE_SETTING_NAME] == null)
                Settings.Current["CoreGenerator", DEPLOYER_TYPE_SETTING_NAME] = typeof(DefaultFileConfigDeployer).FullName;
            if (domains == null)
                domains = new List<sDeployedDomain>();
            if (extensions == null)
                extensions = new List<sDeployedExtension>();
            if (contexts == null)
                contexts = new List<sDeployedContext>();
            if (extensions == null)
                extensions = new List<sDeployedExtension>();
            if (trunks == null)
                trunks = new List<sDeployedOutgoingSipTrunk>();
            if (itrunks == null)
                itrunks = new List<sDeployedIncomingSIPTrunk>();
            if (profiles == null)
                profiles = new List<sDeployedProfile>();
            if (modules == null)
            {
                DirectoryInfo di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR + Path.DirectorySeparatorChar + Constants.DEFAULT_AUTOLOAD_CONF_DIR);
                if (di.Exists)
                {
                    List<sDeployedModule> mods = new List<sDeployedModule>();
                    foreach (FileInfo fi in di.GetFiles("*.xml"))
                    {
                        sr = fi.OpenText();
                        string cont = sr.ReadToEnd();
                        sr.Close();
                        XmlDocument doc = new XmlDocument();
                        while (cont.Contains("<!--"))
                        {
                            string pre = cont.Substring(0, cont.IndexOf("<!--"));
                            cont = cont.Substring(cont.IndexOf("<!--") + 4);
                            cont = pre + cont.Substring(cont.IndexOf("-->") + 3);
                        }
                        cont = cont.Replace('\n', ' ').Replace('\r', ' ');
                        try
                        {
                            doc.LoadXml(cont);
                            mods.Add(new sDeployedModule(fi.Name, new sFreeSwitchModuleFile(doc.ChildNodes[0].Attributes["name"].Value,
                                    (doc.ChildNodes[0].Attributes["description"] == null ? "" : doc.ChildNodes[0].Attributes["description"].Value),
                                    doc.ChildNodes[0].OuterXml)));
                        }
                        catch (Exception e)
                        {
                            throw new Exception("An error occured attempting to load the configuration file " + fi.Name, e);
                        }
                    }
                    modules = mods;
                }
            }
            if (varsContent == null)
            {
                FileInfo fi = new FileInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_CONF_DIR + Path.DirectorySeparatorChar + "vars.xml");
                if (fi.Exists)
                {
                    sr = fi.OpenText();
                    varsContent = sr.ReadToEnd();
                    sr.Close();
                }
            }
            _deployer = (IConfigDeployer)Utility.LocateType(Settings.Current["CoreGenerator", DEPLOYER_TYPE_SETTING_NAME].ToString()).GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            _deployer.Init(domains, extensions, itrunks, profiles, trunks, contexts, modules, varsContent);
        }

        public static void ChangeDeploymentMethod(Type type)
        {
            if (_deployer.GetType().FullName != type.FullName)
            {
                Lock();
                _deployer.Shutdown();
                EventController.TriggerEvent(new ConfigurationDeployerChangedEvent(_deployer.GetType(), type));
                Settings.Current["CoreGenerator", DEPLOYER_TYPE_SETTING_NAME] = type.FullName;
                _deployer = (IConfigDeployer)type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                _deployer.Init(domains, extensions, itrunks, profiles, trunks, contexts, modules, varsContent);
                UnLock();
            }
        }

        public static void RegenerateDomainFile(Domain domain)
        {
            sDeployedDomain dom = new sDeployedDomain(domain);
            Lock();
            List<sDeployedDomain> doms = domains;
            bool add = true;
            for (int x = 0; x < doms.Count; x++)
            {
                if (doms[x].Name == dom.Name)
                {
                    doms[x] = dom;
                    add = false;
                    break;
                }
            }
            if (add)
                doms.Add(dom);
            domains = doms;
            UnLock();
            _deployer.DeployDomain(dom);
            EventController.TriggerEvent(new DomainDeploymentEvent(dom));
        }

        public static void DestroyDomain(string domainName)
        {
            Lock();
            List<sDeployedDomain> doms = domains;
            for (int x = 0; x < doms.Count; x++)
            {
                if (doms[x].Name == domainName)
                {
                    doms.RemoveAt(x);
                    break;
                }
            }
            domains = doms;
            UnLock();
            _deployer.DestroyDomain(domainName);
            EventController.TriggerEvent(new DomainDestroyedEvent(domainName));
        }

        internal static List<sDeployedDomain> GetDomainsForProfile(sDeployedProfile profile)
        {
            List<sDeployedDomain> ret = new List<sDeployedDomain>();
            Lock();
            foreach (sDeployedDomain dom in domains)
            {
                if ((dom.InternalProfile == profile.Name) || (dom.ExternalProfile == profile.Name))
                    ret.Add(dom);
            }
            UnLock();
            return ret;
        }

        internal static List<string> GetExtensionNumbersForDomain(sDeployedDomain domain)
        {
            List<string> ret = new List<string>();
            Lock();
            foreach (sDeployedExtension ext in extensions)
            {
                if (ext.DomainName == domain.Name)
                    ret.Add(ext.Number);
            }
            foreach (sDeployedIncomingSIPTrunk ist in itrunks)
            {
                if (ist.DomainName == domain.Name)
                    ret.Add(ist.Number);
            }
            UnLock();
            return ret;
        }

        public static void RegenerateSIPProfile(SipProfile profile)
        {
            sDeployedProfile prof = new sDeployedProfile(profile);
            Lock();
            List<sDeployedProfile> profs = profiles;
            bool add = true;
            for (int x = 0; x < profs.Count; x++)
            {
                if (profs[x].Name == prof.Name)
                {
                    profs[x] = prof;
                    add = false;
                    break;
                }
            }
            if (add)
                profs.Add(prof);
            profiles = profs;
            UnLock();
            _deployer.DeploySipProfile(prof);
            EventController.TriggerEvent(new SipProfileDeploymentEvent(prof));
        }

        public static void DestroySIPProfile(string profileName)
        {
            List<string> doms = new List<string>();
            Lock();
            List<sDeployedProfile> profs = profiles;
            for (int x = 0; x < profs.Count; x++)
            {
                if (profs[x].Name == profileName)
                {
                    profs.RemoveAt(x);
                    foreach (sDeployedDomain dom in profs[x].Domains)
                        doms.Add(dom.Name);
                    break;
                }
            }
            profiles = profs;
            UnLock();
            foreach (string str in doms)
                DestroyDomain(str);
            _deployer.DestroySipProfile(profileName);
            EventController.TriggerEvent(new SipProfileDestroyedEvent(profileName));
        }

        public static bool IsDomainDeployed(string domainName)
        {
            bool ret = false;
            Lock();
            foreach (sDeployedDomain dom in domains)
            {
                if (dom.Name == domainName)
                {
                    ret = true;
                    break;
                }
            }
            UnLock();
            return ret;
        }

        public static void RegenerateExtensionFiles(string extensionNumber, Domain domain)
        {
            if (!IsDomainDeployed(domain.Name))
            {
                Log.Trace("Creating domain " + domain.Name + " to produce an extension");
                RegenerateDomainFile(domain);
            }
            Extension ext = Extension.Load(extensionNumber, domain);
            if (ext == null)
            {
                Lock();
                List<sDeployedExtension> exts = extensions;
                for (int x = 0; x < exts.Count; x++)
                {
                    if ((exts[x].Number == extensionNumber) || (exts[x].DomainName == domain.Name))
                    {
                        exts.RemoveAt(x);
                        break;
                    }
                }
                extensions = exts;
                UnLock();
                _deployer.DestroyExtension(domain.Name, extensionNumber);
                EventController.TriggerEvent(new ExtensionDestroyedEvent(extensionNumber,domain.Name));
            }
            else
            {
                sDeployedExtension sext = new sDeployedExtension(ext);
                Lock();
                List<sDeployedExtension> exts = extensions;
                bool add = true;
                for (int x = 0; x < exts.Count; x++)
                {
                    if ((exts[x].Number == extensionNumber) && (exts[x].DomainName == domain.Name))
                    {
                        exts[x] = sext;
                        add = false;
                        break;
                    }
                }
                if (add)
                    exts.Add(sext);
                extensions = exts;
                UnLock();
                _deployer.DeployExtension(sext);
                EventController.TriggerEvent(new ExtensionDeploymentEvent(sext));
                Log.Trace("Config file for extension " + extensionNumber + " has been successfully created");
                Extension ex = Extension.Load(extensionNumber, domain);
                VoiceMail vm = VoiceMail.Load(ex.Number,ex.Context);
                if ((vm != null) && vm.ResetVMPassword)
                {
                    vm.ResetVMPassword = false;
                    vm.Update();
                }
            }
        }

        public static void RegenerateIncomingSIPTrunkFile(string extensionNumber, Domain domain)
        {
            if (!IsDomainDeployed(domain.Name))
            {
                Log.Trace("Creating domain " + domain.Name + " to produce an extension");
                RegenerateDomainFile(domain);
            }
            sDeployedIncomingSIPTrunk dit = new sDeployedIncomingSIPTrunk(IncomingSIPTrunk.Load(extensionNumber, domain));
            Lock();
            bool add = true;
            List<sDeployedIncomingSIPTrunk> its = itrunks;
            for (int x = 0; x < its.Count; x++)
            {
                if ((its[x].Number == dit.Number) && (its[x].DomainName == dit.DomainName))
                {
                    its[x] = dit;
                    add = false;
                    break;
                }
            }
            if (add)
                its.Add(dit);
            itrunks = its;
            UnLock();
            _deployer.DeployIncomingSipTrunk(dit);
            EventController.TriggerEvent(new IncomingSIPTrunkDeploymentEvent(dit));
            Log.Trace("Config file for Internal SIP Trunk " + dit.Number + " has been successfully created");
        }

        public static void DestroyIncomingSIPTrunkFile(string extensionNumber, string domainName)
        {
            Lock();
            List<sDeployedIncomingSIPTrunk> its = itrunks;
            for (int x = 0; x < its.Count; x++)
            {
                if ((its[x].Number == extensionNumber) && (its[x].DomainName == domainName))
                {
                    its.RemoveAt(x);
                    break;
                }
            }
            itrunks = its;
            UnLock();
            _deployer.DestroyIncomingSipTrunk(domainName, extensionNumber);
            EventController.TriggerEvent(new IncomingSIPTrunkDestroyedEvent(extensionNumber,domainName));
        }

        internal static void RegenerateContexts(string[] scontexts)
        {
            Lock();
            List<string> tmp = new List<string>(scontexts);
            foreach (sDeployedContext context in contexts)
            {
                if (tmp.Contains(context.Name))
                    _deployer.DeployContext(context);
            }
            UnLock();
        }

        public static void RegenerateContextFile(string contextName)
        {
            Context ct = Context.LoadByName(contextName);
            sDeployedContext dc = new sDeployedContext(ct);
            Lock();
            bool add = true;
            List<sDeployedContext> conts = contexts;
            for (int x = 0; x < conts.Count; x++)
            {
                if (conts[x].Name == dc.Name)
                {
                    conts[x] = dc;
                    add = false;
                    break;
                }
            }
            if (add)
                conts.Add(dc);
            contexts = conts;
            UnLock();
            _deployer.DeployContext(dc);
            EventController.TriggerEvent(new ContextDeploymentEvent(dc));
        }

        public static void DestroyContext(string contextName)
        {
            List<string> spros = new List<string>();
            sDeployedContext context=null;
            Lock();
            List<sDeployedContext> conts = contexts;
            for (int x = 0; x < conts.Count; x++)
            {
                if (conts[x].Name == contextName)
                {
                    context = conts[x];
                    conts.RemoveAt(x);
                    break;
                }
            }
            contexts = conts;
            List<sDeployedProfile> profs = Profiles;
            for (int x = 0; x < profs.Count; x++)
            {
                if (profs[x].ContextName == contextName)
                {
                    profs.RemoveAt(x);
                    spros.Add(profs[x].Name);
                }
            }
            profiles = profs;
            UnLock();
            foreach (string str in spros)
                DestroySIPProfile(str);
            _deployer.DestroyContext(contextName);
            if (context!=null)
                EventController.TriggerEvent(new ContextDestroyedEvent(context));
        }

        public static void RegenerateVarsFile()
        {
            string str = Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.DataCore.Generators.resources.BaseConfigurations.vars.st");
            Template st = new Template(str);
            Lock();
            varsContent = st.ToString();
            UnLock();
            _deployer.DeployVarsFile(st.ToString());
            EventController.TriggerEvent(new VarsFileDeploymentEvent());
        }

        public static void RegenerateModuleFile(string fileName)
        {
            FreeSwitchModuleFile fsmf = FreeSwitchModuleFile.Load(fileName);
            Lock();
            List<sDeployedModule> mods = modules;
            if (fsmf == null)
            {
                for (int x = 0; x < mods.Count; x++)
                {
                    if (mods[x].FileName == fileName)
                    {
                        mods.RemoveAt(x);
                        break;
                    }
                }
            }
            else
            {
                bool add = true;
                for (int x = 0; x < mods.Count; x++)
                {
                    if (mods[x].FileName == fileName)
                    {
                        mods[x] = new sDeployedModule(fileName, fsmf.File);
                        add = false;
                        break;
                    }
                }
                if (add)
                    mods.Add(new sDeployedModule(fileName, fsmf.File));
            }
            modules = mods;
            UnLock();
            _deployer.DeployModuleFile(fileName, (fsmf == null ? null : fsmf.File));
            EventController.TriggerEvent(new ModuleDeploymentEvent(fileName, fsmf == null));
        }

        public static sFreeSwitchModuleFile ReadCurrentModuleSettings(string moduleName)
        {
            sFreeSwitchModuleFile ret = null;
            Lock();
            foreach (sDeployedModule dm in modules)
            {
                if (dm.File.Name == moduleName)
                {
                    ret = dm.File;
                    break;
                }
            }
            UnLock();
            return ret;
        }

        public static void GenerateOutgoingSIPTrunkConfiguration(string name)
        {
            OutgoingSIPTrunk ost = OutgoingSIPTrunk.Load(name);
            sDeployedOutgoingSipTrunk trunk = new sDeployedOutgoingSipTrunk(ost);
            Lock();
            bool add = true;
            List<sDeployedOutgoingSipTrunk> tks = trunks;
            for (int x = 0; x < tks.Count; x++)
            {
                if (tks[x].Name == trunk.Name)
                {
                    add = false;
                    tks[x] = trunk;
                    break;
                }
            }
            if (add)
                tks.Add(trunk);
            trunks = tks;
            UnLock();
            _deployer.DeployOutgoingSipTrunk(trunk);
            EventController.TriggerEvent(new OutgoingSIPTrunkDeploymentEvent(trunk));
            Log.Trace("Config file for outgoing SIP trunk " + ost.Name + " has been successfully created");
        }

        public static void DestroyOutgoingSIPTrunk(string name, string profileName)
        {
            Lock();
            List<sDeployedOutgoingSipTrunk> tks = trunks;
            for (int x = 0; x < tks.Count; x++)
            {
                if (tks[x].Name == name)
                {
                    tks.RemoveAt(x);
                    break;
                }
            }
            trunks = tks;
            UnLock();
            _deployer.DestroyOutgoingSipTrunk(profileName, name);
            EventController.TriggerEvent(new OutgoingSIPTrunkDestroyedEvent(name, profileName));
        }

        internal static List<sDeployedOutgoingSipTrunk> GetOutgoingTrunksForProfile(sDeployedProfile profile)
        {
            List<sDeployedOutgoingSipTrunk> ret = new List<sDeployedOutgoingSipTrunk>();
            Lock();
            foreach (sDeployedOutgoingSipTrunk trk in trunks)
            {
                if (trk.ProfileName == profile.Name)
                    ret.Add(trk);
            }
            UnLock();
            return ret;
        }
    }
}
