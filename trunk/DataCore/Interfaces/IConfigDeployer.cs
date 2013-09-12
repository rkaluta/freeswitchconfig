using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks.SIP;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces
{
    public class sDeployedVoicemail : IXmlConvertableObject
    {
        private string _number;
        private string _domain;

        private string _password;
        public string Password
        {
            get { return _password; }
        }

        public string A1Hash
        {
            get { return Utility.CalculateMD5Sum(_number + ":" + _domain + ":" + Password); }
        }

        private int? _maxMessages;
        public int? MaxMessages
        {
            get { return _maxMessages; }
        }

        private string _copyTo;
        public string CopyTo
        {
            get { return _copyTo; }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
        }

        private bool _attachToEmail;
        public bool AttachToEmail
        {
            get { return _attachToEmail; }
        }

        public sDeployedVoicemail(VoiceMail vm,string number,string domain)
        {
            _password = vm.Password;
            _maxMessages = vm.MaxMessage;
            _copyTo = null;
            if (vm.CopyTo != null)
                _copyTo = vm.CopyTo.Number + "@" + vm.CopyTo.Domain.Name;
            _email = vm.Email;
            _attachToEmail = vm.AttachToEmail;
            _number = number;
            _domain = domain;
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("password", _password);
            if (_maxMessages.HasValue)
                writer.WriteAttributeString("maxMessages", _maxMessages.ToString());
            if (_copyTo!=null)
                writer.WriteAttributeString("copyTo", _copyTo);
            if (_email!=null)
                writer.WriteAttributeString("email", _email);
            writer.WriteAttributeString("attachToEmail", _attachToEmail.ToString());
            writer.WriteAttributeString("number", _number);
            writer.WriteAttributeString("domain", _domain);
        }

        public void LoadFromElement(XmlElement element)
        {
            _password = element.Attributes["password"].Value;
            if (element.Attributes["maxMessages"] != null)
                _maxMessages = int.Parse(element.Attributes["maxMessage"].Value);
            if (element.Attributes["copyTo"] != null)
                _copyTo = element.Attributes["copyTo"].Value;
            if (element.Attributes["email"] != null)
                _email = element.Attributes["email"].Value;
            _attachToEmail = bool.Parse(element.Attributes["attachToEmail"].Value);
            _number = element.Attributes["number"].Value;
            _domain = element.Attributes["domain"].Value;
        }

        #endregion
    }

    public class sDeployedIncomingSIPTrunk : IXmlConvertableObject
    {

        private string _domainName;
        public string DomainName
        {
            get { return _domainName; }
        }

        private string _number;
        public string Number
        {
            get { return _number; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
        }

        public string A1Hash
        {
            get { return Utility.CalculateMD5Sum(Number + ":" + DomainName + ":" + Password); }
        }

        private string _context;
        public string Context
        {
            get { return _context; }
        }

        private string _internalCallerIDName;
        public string InternalCallerIDName
        {
            get { return _internalCallerIDName; }
        }

        private string _internalCallerID;
        public string InternalCallerID
        {
            get { return _internalCallerID; }
        }

        private string _externalCallerIDName;
        public string ExternalCallerIDName
        {
            get { return _externalCallerIDName; }
        }

        private string _externalCallerID;
        public string ExternalCallerID
        {
            get { return _externalCallerID; }
        }

        public sDeployedIncomingSIPTrunk(IncomingSIPTrunk trunk)
        {
            _domainName = trunk.Domain.Name;
            _number = trunk.Number;
            _password = trunk.Password;
            _context = trunk.Context.Name;
            _internalCallerID = trunk.InternalCallerID;
            _internalCallerIDName = trunk.InternalCallerIDName;
            _externalCallerID = trunk.ExternalCallerID;
            _externalCallerIDName = trunk.ExternalCallerIDName;
        }

        public sDeployedIncomingSIPTrunk() { }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("domainName", _domainName);
            writer.WriteAttributeString("number", _number);
            writer.WriteAttributeString("password", _password);
            writer.WriteAttributeString("context", _context);
            if (_internalCallerID != null)
                writer.WriteAttributeString("internalCallerID", _internalCallerID);
            if (_internalCallerIDName != null)
                writer.WriteAttributeString("internalCallerIDName", _internalCallerIDName);
            if (_externalCallerID != null)
                writer.WriteAttributeString("externalCallerID", _externalCallerID);
            if (_externalCallerIDName != null)
                writer.WriteAttributeString("externalCallerIDName", _externalCallerIDName);
        }

        public void LoadFromElement(XmlElement element)
        {
            _domainName = element.Attributes["domainName"].Value;
            _number = element.Attributes["number"].Value;
            _password = element.Attributes["password"].Value;
            _context = element.Attributes["context"].Value;
            if (element.Attributes["internalCallerID"] != null)
                _internalCallerID = element.Attributes["internalCallerID"].Value;
            if (element.Attributes["externalCallerID"] != null)
                _externalCallerID = element.Attributes["externalCallerID"].Value;
            if (element.Attributes["internalCallerIDName"] != null)
                _internalCallerIDName = element.Attributes["internalCallerIDName"].Value;
            if (element.Attributes["externalCallerIDName"] != null)
                _externalCallerIDName = element.Attributes["externalCallerIDName"].Value;
        }

        #endregion
    }

    public class sDeployedExtension : IXmlConvertableObject
    {
        private string _domainName;
        public string DomainName
        {
            get { return _domainName; }
        }

        private string _number;
        public string Number
        {
            get { return _number; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
        }

        public string A1Hash
        {
            get { return Utility.CalculateMD5Sum(Number + ":" + DomainName + ":" + Password); }
        }

        private sDeployedVoicemail _vm;
        public sDeployedVoicemail VM
        {
            get { return _vm; }
        }

        private string _context;
        public string Context
        {
            get { return _context; }
        }

        private string _internalCallerIDName;
        public string InternalCallerIDName
        {
            get { return _internalCallerIDName; }
        }

        private string _internalCallerID;
        public string InternalCallerID
        {
            get { return _internalCallerID; }
        }

        private string _externalCallerIDName;
        public string ExternalCallerIDName
        {
            get { return _externalCallerIDName; }
        }

        private string _externalCallerID;
        public string ExternalCallerID
        {
            get { return _externalCallerID; }
        }

        private int? _voicemailTimeout;
        public int? VoicemailTimeout
        {
            get { return _voicemailTimeout; }
        }

        public sDeployedExtension(Extension ext)
        {
            _domainName = ext.Domain.Name;
            _number = ext.Number;
            _password = ext.Password;
            _vm = null;
            _context = ext.Context.Name;
            _internalCallerID = ext.InternalCallerID;
            _internalCallerIDName = ext.InternalCallerIDName;
            _externalCallerID = ext.ExternalCallerID;
            _externalCallerIDName = ext.ExternalCallerIDName;
            _voicemailTimeout = ext.VoicemailTimeout;
            if (ext.HasVoicemail)
                _vm = new sDeployedVoicemail(VoiceMail.Load(ext.Number,ext.Context), Number,DomainName);
        }

        public sDeployedExtension() { }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("domainName", _domainName);
            writer.WriteAttributeString("number", _number);
            writer.WriteAttributeString("password", _password);
            writer.WriteAttributeString("context", _context);
            if (_internalCallerID != null)
                writer.WriteAttributeString("internalCallerID", _internalCallerID);
            if (_internalCallerIDName != null)
                writer.WriteAttributeString("internalCallerIDName", _internalCallerIDName);
            if (_externalCallerID != null)
                writer.WriteAttributeString("externalCallerID", _externalCallerID);
            if (_externalCallerIDName != null)
                writer.WriteAttributeString("externalCallerIDName", _externalCallerIDName);
            if (_voicemailTimeout.HasValue)
                writer.WriteAttributeString("voicemailTimeout", _voicemailTimeout.Value.ToString());
            if (_vm != null)
                writer.WriteRaw(Utility.ConvertObjectToXML(_vm, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _domainName = element.Attributes["domainName"].Value;
            _number = element.Attributes["number"].Value;
            _password = element.Attributes["password"].Value;
            _context = element.Attributes["context"].Value;
            if (element.Attributes["internalCallerID"] != null)
                _internalCallerID = element.Attributes["internalCallerID"].Value;
            if (element.Attributes["externalCallerID"] != null)
                _externalCallerID = element.Attributes["externalCallerID"].Value;
            if (element.Attributes["internalCallerIDName"] != null)
                _internalCallerIDName = element.Attributes["internalCallerIDName"].Value;
            if (element.Attributes["externalCallerIDName"] != null)
                _externalCallerIDName = element.Attributes["externalCallerIDName"].Value;
            if (element.Attributes["voicemailTimeout"] != null)
                _voicemailTimeout = int.Parse(element.Attributes["voicemailTimeout"].Value);
            string ixml = element.InnerXml.Trim();
            if (ixml != "")
                _vm = (sDeployedVoicemail)Utility.ConvertObjectFromXML(ixml);
            else
                _vm = null;
        }

        #endregion
    }

    public class sDeployedDomainGroup : IXmlConvertableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private List<string> _extensions;
        public List<string> Extensions
        {
            get { return _extensions; }
        }

        public sDeployedDomainGroup(string groupName,string domainName)
        {
            _name = groupName;
            _extensions = new List<string>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones", "SELECT egrp.Extensions.Number as Number FROM ExtensionGroup egrp WHERE egrp.Domain.Name = @domainName and egrp.Name = @groupName");
            IDbDataParameter[] pars = new IDbDataParameter[] { 
                cq.CreateParameter("@domainName", domainName),
                cq.CreateParameter("@groupName",groupName)
            };
            cq.Execute(pars);
            while (cq.Read())
                _extensions.Add(cq[0].ToString());
            cq.Close();
        }

        public sDeployedDomainGroup() { }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("name", _name);
            writer.WriteRaw(Utility.ConvertObjectToXML(_extensions, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _name = element.Attributes["name"].Value;
            _extensions = (List<string>)Utility.ConvertObjectFromXML(element.InnerXml);
        }

        #endregion
    }

    public class sDeployedDomain : IXmlConvertableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _internalProfile;
        public string InternalProfile
        {
            get { return _internalProfile; }
        }

        private string _externalProfile;
        public string ExternalProfile
        {
            get { return _externalProfile; }
        }

        private List<NameValuePair> _params;
        public List<NameValuePair> Params
        {
            get { return _params; }
        }

        private List<NameValuePair> _variables;
        public List<NameValuePair> Variables
        {
            get { return _variables; }
        }

        private List<sDeployedDomainGroup> _groups;
        public List<sDeployedDomainGroup> Groups
        {
            get { return _groups; }
        }

        public List<string> Extensions
        {
            get
            {
                return CoreGenerator.GetExtensionNumbersForDomain(this);
            }
        }


        public sDeployedDomain(Domain domain)
        {
            _name = domain.Name;
            _variables = new List<NameValuePair>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core", "SELECT dv.Name,dv.Value FROM DomainVariable dv WHERE dv.Domain.Name = @domainName");
            IDbDataParameter[] pars = new IDbDataParameter[] { cq.CreateParameter("@domainName", domain.Name) };
            cq.Execute(pars);
            while (cq.Read())
                _variables.Add(new NameValuePair(cq[0].ToString(), cq[1].ToString()));
            cq.Close();
            _params = new List<NameValuePair>();
            cq.NewQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core", "SELECT dp.Name,dp.Value FROM DomainParameter dp WHERE dp.Domain.Name = @domainName");
            cq.Execute(pars);
            while (cq.Read())
                _params.Add(new NameValuePair(cq[0].ToString(), cq[1].ToString()));
            cq.Close();
            _groups = new List<sDeployedDomainGroup>();
            cq.NewQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones", "SELECT egrp.Name FROM ExtensionGroup egrp WHERE egrp.Domain.Name = @domainName");
            cq.Execute(pars);
            while (cq.Read())
                _groups.Add(new sDeployedDomainGroup(cq[0].ToString(),_name));
            cq.Close();
            _internalProfile = domain.InternalProfile.Name;
            _externalProfile = domain.ExternalProfile.Name;
        }

        public sDeployedDomain()
        {
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("internalProfile", _internalProfile);
            writer.WriteAttributeString("externalProfile", _externalProfile);
            writer.WriteStartElement("variables");
            writer.WriteRaw(Utility.ConvertObjectToXML(_variables, true));
            writer.WriteEndElement();
            writer.WriteStartElement("params");
            writer.WriteRaw(Utility.ConvertObjectToXML(_params, true));
            writer.WriteEndElement();
            writer.WriteStartElement("groups");
            writer.WriteRaw(Utility.ConvertObjectToXML(_groups, true));
            writer.WriteEndElement();
        }

        public void LoadFromElement(XmlElement element)
        {
            _name = element.Attributes["name"].Value;
            _internalProfile = element.Attributes["internalProfile"].Value;
            _externalProfile = element.Attributes["externalProfile"].Value;
            foreach (XmlElement elem in element.ChildNodes)
            {
                switch (elem.Name)
                {
                    case "variables":
                        _variables = (List<NameValuePair>)Utility.ConvertObjectFromXML(elem.InnerXml);
                        break;
                    case "params":
                        _params = (List<NameValuePair>)Utility.ConvertObjectFromXML(elem.InnerXml);
                        break;
                    case "groups":
                        _groups = (List<sDeployedDomainGroup>)Utility.ConvertObjectFromXML(elem.InnerXml);
                        break;
                }
            }
        }

        #endregion
    }

    public class sDeployedProfile : IXmlConvertableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public List<sDeployedDomain> Domains
        {
            get { return CoreGenerator.GetDomainsForProfile(this); }
        }

        public List<sDeployedOutgoingSipTrunk> Trunks
        {
            get { return CoreGenerator.GetOutgoingTrunksForProfile(this); }
        }

        private List<NameValuePair> _settings;
        public List<NameValuePair> Settings
        {
            get { return _settings; }
        }

        private string _contextName;
        public string ContextName
        {
            get { return _contextName; }
        }

        public sDeployedContext Context
        {
            get
            {
                sDeployedContext ret = null;
                CoreGenerator.Lock();
                for (int x = 0; x < CoreGenerator.Contexts.Count; x++)
                {
                    if (CoreGenerator.Contexts[x].Name == _contextName)
                    {
                        ret = CoreGenerator.Contexts[x];
                        break;
                    }
                }
                CoreGenerator.UnLock();
                return ret;
            }
        }

        public sDeployedProfile(SipProfile profile)
        {
            _name = profile.Name;
            _contextName = profile.Context.Name;
            _settings = new List<NameValuePair>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core",
                "Select sps.SettingType,sps.Value from SipProfileSetting sps WHERE sps.Profile.Name = @profileName");
            IDbDataParameter[] pars = new IDbDataParameter[]{
                cq.CreateParameter("@profileName",profile.Name)
            };
            cq.Execute(pars);
            while (cq.Read())
                _settings.Add(new NameValuePair(cq[0].ToString(), cq[1].ToString()));
            cq.Close();
        }

        public sDeployedProfile()
        {
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("context", _contextName);
            writer.WriteRaw(Utility.ConvertObjectToXML(_settings, true));
        }

        public void LoadFromElement(XmlElement element)
        {
            _name = element.Attributes["name"].Value;
            _contextName = element.Attributes["context"].Value;
            _settings = (List<NameValuePair>)Utility.ConvertObjectFromXML(element.InnerXml);
        }

        #endregion
    }

    public class sDeployedOutgoingSipTrunk : IXmlConvertableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _realm;
        public string Realm
        {
            get { return _realm; }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
        }

        private OutgoingSIPTrunk.RegistrationTypes _regType;
        public OutgoingSIPTrunk.RegistrationTypes RegistrationType
        {
            get { return _regType; }
        }

        private bool _register;
        public bool Register
        {
            get { return _register; }
        }

        private string _context;
        public string Context
        {
            get { return _context; }
            set { _context = value; }
        }

        private int _pingInterval;
        public int PingInterval
        {
            get { return _pingInterval; }
            set { _pingInterval = value; }
        }

        private int _retrySeconds;
        public int RetrySeconds
        {
            get { return _retrySeconds; }
            set { _retrySeconds = value; }
        }

        private string _fromUser;
        public string FromUser
        {
            get { return _fromUser; }
            set { _fromUser = value; }
        }

        private string _fromDomain;
        public string FromDomain
        {
            get { return _fromDomain; }
            set { _fromDomain = value; }
        }

        private string _extension;
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        private string _proxy;
        public string Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        private string _registerProxy;
        public string RegisterProxy
        {
            get { return _registerProxy; }
            set { _registerProxy = value; }
        }

        private int? _expireSeconds;
        public int? ExpireSeconds
        {
            get { return _expireSeconds; }
            set { _expireSeconds = value; }
        }

        private string _profileName;
        public string ProfileName
        {
            get { return _profileName; }
        }

        public sDeployedOutgoingSipTrunk(OutgoingSIPTrunk trunk)
        {
            _name = trunk.Name;
            _userName = trunk.UserName;
            _password = trunk.Password;
            _register = trunk.Register;
            _regType = trunk.RegistrationType;
            _context = trunk.Profile.Context.Name;
            _pingInterval = trunk.PingInterval;
            _retrySeconds = trunk.RetrySeconds;
            _fromUser = trunk.FromUser;
            _fromDomain = trunk.FromDomain;
            _extension = trunk.Extension;
            _proxy = trunk.Proxy;
            _registerProxy = trunk.RegisterProxy;
            _expireSeconds = trunk.ExpireSeconds;
            _realm = trunk.Realm;
            _profileName = trunk.Profile.Name;
        }

        public sDeployedOutgoingSipTrunk()
        {
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("username", _userName);
            writer.WriteAttributeString("password", _password);
            writer.WriteAttributeString("register", _register.ToString());
            writer.WriteAttributeString("regType", _regType.ToString());
            writer.WriteAttributeString("context", _context);
            writer.WriteAttributeString("pingInterval", _pingInterval.ToString());
            writer.WriteAttributeString("retryseconds", _retrySeconds.ToString());
            writer.WriteAttributeString("realm", _realm);
            writer.WriteAttributeString("profileName", _profileName);
            if (_fromUser != null)
                writer.WriteAttributeString("fromUser", _fromUser);
            if (_fromDomain != null)
                writer.WriteAttributeString("fromDoamin", _fromDomain);
            if (_extension != null)
                writer.WriteAttributeString("extension", _extension);
            if (_proxy != null)
                writer.WriteAttributeString("proxy", _proxy);
            if (_registerProxy != null)
                writer.WriteAttributeString("registerProxy", _registerProxy);
            if (_expireSeconds != null)
                writer.WriteAttributeString("expireSeconds", _expireSeconds.ToString());
        }

        public void LoadFromElement(XmlElement element)
        {
            _name = element.Attributes["name"].Value;
            _userName = element.Attributes["username"].Value;
            _password = element.Attributes["password"].Value;
            _register = bool.Parse(element.Attributes["register"].Value);
            _regType = (OutgoingSIPTrunk.RegistrationTypes)Enum.Parse(typeof(OutgoingSIPTrunk.RegistrationTypes), element.Attributes["regType"].Value);
            _context = element.Attributes["context"].Value;
            _pingInterval = int.Parse(element.Attributes["pingInterval"].Value);
            _retrySeconds = int.Parse(element.Attributes["retryseconds"].Value);
            _realm = element.Attributes["realm"].Value;
            _profileName = element.Attributes["profileName"].Value;
            if (element.Attributes["fromUser"] != null)
                _fromUser = element.Attributes["fromUser"].Value;
            if (element.Attributes["fromDomain"] != null)
                _fromDomain = element.Attributes["fromDomain"].Value;
            if (element.Attributes["extension"] != null)
                _extension = element.Attributes["extension"].Value;
            if (element.Attributes["proxy"] != null)
                _proxy = element.Attributes["proxy"].Value;
            if (element.Attributes["registerProxy"] != null)
                _registerProxy = element.Attributes["registerProxy"].Value;
            if (element.Attributes["expireSeconds"] != null)
                _expireSeconds = int.Parse(element.Attributes["expireSeconds"].Value);
        }

        #endregion
    }

    public class sDeployedContext : IXmlConvertableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public sDeployedContext(Context cont)
        {
            _name = cont.Name;
        }

        public sDeployedContext()
        {
            _name = "";
        }

        public XmlContextFile ContextFile
        {
            get { return CallControlManager.GenerateContextFile(this); }
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("name", _name);
        }

        public void LoadFromElement(XmlElement element)
        {
            _name = element.Attributes["name"].Value;
        }

        #endregion
    }

    public class sDeployedModule : IXmlConvertableObject
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
        }

        private sFreeSwitchModuleFile _file;
        public sFreeSwitchModuleFile File
        {
            get { return _file; }
        }

        public sDeployedModule(string fileName, sFreeSwitchModuleFile file)
        {
            _fileName = fileName;
            _file = file;
        }

        public sDeployedModule() {
        }

        #region IXmlConvertableObject Members

        public void SaveToStream(XmlWriter writer)
        {
            writer.WriteAttributeString("filename", FileName);
            _file.WriteXml(writer);
        }

        public void LoadFromElement(XmlElement element)
        {
            _fileName = element.Attributes["filename"].Value;
            _file = new sFreeSwitchModuleFile((XmlElement)element.ChildNodes[0]);
        }

        #endregion
    }

    public interface IConfigDeployer
    {
        void DeployDomain(sDeployedDomain domain);
        void DestroyDomain(string domainName);
        void DeploySipProfile(sDeployedProfile profile);
        void DestroySipProfile(string profileName);
        void DeployExtension(sDeployedExtension ext);
        void DestroyExtension(string domainName, string extensionNumber);
        void DeployIncomingSipTrunk(sDeployedIncomingSIPTrunk trunk);
        void DestroyIncomingSipTrunk(string domainName, string extensionNumber);
        void DeployContext(sDeployedContext context);
        void DestroyContext(string contextName);
        void DeployVarsFile(string content);
        void DeployModuleFile(string fileName, sFreeSwitchModuleFile file);
        void DeployOutgoingSipTrunk(sDeployedOutgoingSipTrunk trunk);
        void DestroyOutgoingSipTrunk(string profileName, string name);
        void Init(List<sDeployedDomain> domains,List<sDeployedExtension> extensions, 
            List<sDeployedIncomingSIPTrunk> itrunks,List<sDeployedProfile> profiles, 
            List<sDeployedOutgoingSipTrunk> trunks,List<sDeployedContext> contexts, 
            List<sDeployedModule> modules, string varsContent);
        void Shutdown();
        string Name { get; }
        string Description { get; }
        bool IsValidToUse { get; }
    }
}
