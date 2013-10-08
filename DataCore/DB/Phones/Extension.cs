/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 08/11/2009
 * Time: 2:39 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using System.Text.RegularExpressions;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using System.Collections.Generic;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.BackBoneDotNet;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using Org.Reddragonit.Dbpro;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones
{
	/// <summary>
	/// Description of Extension.
	/// </summary>
	[Table(Table.TableSettings.AutoDeleteParent)]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/Extensions.js")]
    [ModelRoute("/core/models/phones/Extension")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View|ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Core")]
	public class Extension : ExtensionNumber
	{

		public Extension()
		{
		}
		
		private string _password;
		[Field(50,false)]
		public string Password{
			get{return _password;}
			set{_password=value;}
		}

        private Domain _domain;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE,ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelPropertyLazyLoadExternalModel()]
        public Domain Domain
        {
            get { return _domain; }
            set {
                if (Context != null)
                {
                    if (value.InternalProfile.Context.Name != Context.Name && value.ExternalProfile.Context.Name != Context.Name)
                        throw new Exception("Unable to move an extension to a domain that does not contain the extensions context.");
                }
                _domain = value;}
        }
		
		private string _internalCallerIDName;
		[Field(50,true)]
		public string InternalCallerIDName{
			get{return _internalCallerIDName;}
			set{_internalCallerIDName=value;}
		}
		
		private string _internalCallerID;
		[Field(25,true)]
		public string InternalCallerID{
			get{return _internalCallerID;}
			set{_internalCallerID=value;}
		}
		
		private string _externalCallerIDName;
		[Field(50,true)]
		public string ExternalCallerIDName{
			get{return _externalCallerIDName;}
			set{_externalCallerIDName=value;}
		}
		
		private string _externalCallerID;
		[Field(25,true)]
		public string ExternalCallerID{
			get{return _externalCallerID;}
			set{_externalCallerID=value;}
		}

        private int? _voicemailTimeout;
        [Field(true)]
        public int? VoicemailTimeout
        {
            get { return _voicemailTimeout; }
            set { _voicemailTimeout = value; }
        }

        private VoiceMail _mail = null;
        [ModelIgnoreProperty()]
        public VoiceMail Mail
        {
            get{
                if (_mail == null)
                    _mail = VoiceMail.Load(this.Number,this.Context);
                return _mail;
            }
        }

        [ReadOnlyModelProperty()]
        public bool HasVoicemail
        {
            get { return Mail != null; }
        }

        [ModelLoadMethod()]
        public static new Extension Load(string number)
        {
            Extension ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Extension));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = null;
            if (number.Contains("@"))
                tmp = conn.Select(typeof(Extension),
                    new SelectParameter[] { new EqualParameter("Number", number.Substring(0,number.IndexOf('@'))),
                    new EqualParameter("Domain.Name",number.Substring(number.IndexOf('@')+1))});
            else
                tmp = conn.Select(typeof(Extension),
                    new SelectParameter[] { new EqualParameter("Number", number),
                    new EqualParameter("Context",Context.Current)});
            if (tmp.Count > 0)
                ret = (Extension)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static new List<Extension> LoadAll()
        {
            List<Extension> ret = new List<Extension>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Extension));
            foreach (Extension ext in conn.Select(typeof(Extension),
                new SelectParameter[] { new EqualParameter("Context", Context.Current) }))
                ret.Add(ext);
            conn.CloseConnection();
            return ret;
        }

        [ModelSelectListMethod()]
        public new static List<sModelSelectOptionValue> SelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones",
                "SELECT ext.Number,ext.Domain.Name FROM Extension ext WHERE ext.Domain.Name = @domainName");
            cq.Execute(new IDbDataParameter[] { cq.CreateParameter("@domainName", Domain.Current.Name) });
            while (cq.Read())
                ret.Add(new sModelSelectOptionValue(cq[0].ToString() + "@" + cq[1].ToString(), cq[0].ToString() + "@" + cq[1].ToString()));
            cq.Close();
            return ret;
        }

        [ModelDeleteMethod()]
        public new bool Delete()
        {
            bool ret = true;
            try
            {
                base.Delete();
                ConfigurationController.RegisterChangeCall(
                    typeof(CallExtensionPlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "DeleteExtension",
                        new NameValuePair[]{
                            new NameValuePair("context",Context.Name),
                            new NameValuePair("extension",Number)
                        }),
                new IEvent[]{
                            new GenericEvent("ExtensionDeleted",
                                    new NameValuePair[]{
                                        new NameValuePair("Number",Number),
                                        new NameValuePair("Domain",Domain.Name),
                                        new NameValuePair("ContextName",Context.Name)
                                    }
                                )
                        }
                        );
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }

        [ModelUpdateMethod()]
        public new bool Update()
        {
            bool ret = true;
            try
            {
                base.Update();
                ConfigurationController.RegisterChangeCall(
                    typeof(CallExtensionPlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                    "RedeployExtension",
                    new NameValuePair[]{
                        new NameValuePair("context",Context.Name),
                        new NameValuePair("extension",OriginalNumber),
                        new NameValuePair("newExtension",Number),
                        new NameValuePair("domain",Domain.Name),
                        new NameValuePair("hasVoicemail",HasVoicemail),
                        new NameValuePair("timeout",VoicemailTimeout)
                    }),
                    new IEvent[]{
                        new GenericEvent("ExtensionUpdated",
                                new NameValuePair[]{
                                    new NameValuePair("Number",Number),
                                    new NameValuePair("Domain",Domain.Name),
                                    new NameValuePair("ContextName",Context.Name)
                                }
                            )
                    }
                    );
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }

        [ModelSaveMethod()]
        public new bool Save()
        {
            if (Domain == null)
                Domain = Domain.Current;
            bool ret = true;
            try
            {
                base.Save();
                ConfigurationController.RegisterChangeCall(
                    typeof(CallExtensionPlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                    "AddExtension",
                    new NameValuePair[]{
                        new NameValuePair("context",Context.Name),
                        new NameValuePair("extension",Number),
                        new NameValuePair("domain",Domain.Name),
                        new NameValuePair("hasVoicemail",HasVoicemail),
                        new NameValuePair("timeout",VoicemailTimeout)
                    }),
                    new IEvent[]{
                        new GenericEvent("ExtensionCreated",
                                new NameValuePair[]{
                                    new NameValuePair("Number",Number),
                                    new NameValuePair("Domain",Domain.Name),
                                    new NameValuePair("ContextName",Context.Name)
                                }
                            )
                    }
                    );
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }

        public static Extension Load(string number,Domain domain)
        {
            Extension ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Extension));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(Extension),
                new SelectParameter[] { new EqualParameter("Number", number),
                new EqualParameter("Domain",domain)});
            if (tmp.Count > 0)
                ret = (Extension)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        public new string id
        {
            get { return Number + "@" + this.Domain.Name; }
        }
	}
}
