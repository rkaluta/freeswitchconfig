using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/Extensions.js")]
    [ModelRoute("/core/models/phones/Voicemail")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.Collection | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class VoiceMail : ExtensionNumber
    {

        private const string VM_DB = "voicemail_default.db";
        private const string VM_DB_PASS_SELECT = "SELECT password FROM voicemail_prefs WHERE username='{0}';";

        private string _password;
        [Field(50, false)]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _email;
        [Field(500, true)]
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private int? _maxMessages;
        [Field(true)]
        public int? MaxMessage
        {
            get { return _maxMessages; }
            set { _maxMessages = value; }
        }

        private Extension _copyTo;
        [ForeignField(true,ForeignField.UpdateDeleteAction.CASCADE,ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelPropertyLazyLoadExternalModel()]
        public Extension CopyTo{
            get { return _copyTo; }
            set { _copyTo = value; }
        }

        private bool _attachToEmail;
        [Field(false)]
        public bool AttachToEmail
        {
            get { return _attachToEmail; }
            set { _attachToEmail = value; }
        }

        //must set true when first creating item
        private bool _resetVmPassword=false;
        [Field(false)]
        public bool ResetVMPassword
        {
            get { return _resetVmPassword; }
            set { _resetVmPassword = value; }
        }

        [ModelLoadMethod()]
        public new static VoiceMail Load(string number)
        {
            return Load(number, Context.Current);
        }

        internal static VoiceMail Load(string number, Context context)
        {
            VoiceMail ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(VoiceMail)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(VoiceMail),
                new SelectParameter[] { new EqualParameter("Number", number),
                new EqualParameter("Context",context)});
            if (tmp.Count > 0)
                ret = (VoiceMail)tmp[0];
            conn.CloseConnection();
            if ((ret != null) && (!ret.ResetVMPassword))
            {
                List<Dictionary<string, string>> tpass = Utility.SelectFromFreeswitchDB(VM_DB, string.Format(VM_DB_PASS_SELECT, number));
                if (tpass.Count > 0)
                {
                    ret.Password = tpass[0]["password"];
                    ret.Update();
                }
            }
            return ret;
        }

        [ModelDeleteMethod()]
        public new bool Delete()
        {
            bool ret = true;
            try
            {
                base.Delete();
                ConfigurationController.RegisterExtensionChangeCall(
                        Number,
                        Domain.Current.Name,
                        typeof(CallExtensionPlan),
                        new ADialPlan.sUpdateConfigurationsCall(
                            "RemoveVoicemail",
                            new NameValuePair[]{
                                new NameValuePair("context",Context.Name),
                                new NameValuePair("extension",Number)
                            }),
                        new IEvent[]{
                            new GenericEvent("VoicemailRemovedFromExtension",
                                    new NameValuePair[]{
                                        new NameValuePair("Number",Number),
                                        new NameValuePair("Domain",Domain.Current.Name),
                                        new NameValuePair("ContextName",Context.Name)
                                    }
                                )
                        });
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
                ConfigurationController.RegisterExtensionChangeCall(
                        Number,
                        Domain.Current.Name,
                        typeof(CallExtensionPlan),
                        new ADialPlan.sUpdateConfigurationsCall[0],
                        new IEvent[]{
                            new GenericEvent("VoicemailInformationUpdated",
                                    new NameValuePair[]{
                                        new NameValuePair("Number",Number),
                                        new NameValuePair("Domain",Domain.Current.Name),
                                        new NameValuePair("ContextName",Context.Name)
                                    }
                                )
                        });
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
            bool ret = true;
            try
            {
                base.Save();
                ConfigurationController.RegisterExtensionChangeCall(
                        Number,
                        Domain.Current.Name,
                        typeof(CallExtensionPlan),
                        new ADialPlan.sUpdateConfigurationsCall(
                            "AddVoicemail",
                            new NameValuePair[]{
                                new NameValuePair("context",Context.Name),
                                new NameValuePair("extension",Number)
                            }),
                        new IEvent[]{
                            new GenericEvent("VoicemailAttachedToExtension",
                                    new NameValuePair[]{
                                        new NameValuePair("Number",Number),
                                        new NameValuePair("Domain",Domain.Current.Name),
                                        new NameValuePair("ContextName",Context.Name)
                                    }
                                )
                        });
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }
    }
}
