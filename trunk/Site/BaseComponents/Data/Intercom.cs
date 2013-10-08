using System;
using System.Collections.Generic;

using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;

/// <summary>
/// Summary description for Intercom
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data
{
    [Table(Table.TableSettings.AutoDeleteParent)]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/Intercom.js")]
    [ModelRoute("/core/models/pbxConfig/Intercom")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Routes")]
    public class Intercom : ExtensionNumber
    {

        public Intercom()
        {
        }

        private bool _oneWay;
        [Field(false)]
        [ModelRequiredField()]
        public bool OneWay
        {
            get { return _oneWay; }
            set { _oneWay = value; }
        }

        private string _description;
        [Field(500, true)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private Extension[] _extensions;
        [ForeignField(false, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        [ModelPropertyLazyLoadExternalModel()]
        public Extension[] Extensions
        {
            get { return _extensions; }
            set { _extensions = value; }
        }

        [ModelLoadMethod()]
        public static new Intercom Load(string number)
        {
            Intercom ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Intercom));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(Intercom),
                new SelectParameter[] { new EqualParameter("Number", (number.Contains("@") ? number.Substring(0,number.IndexOf("@")) : number)),
                new EqualParameter("Context.Name",(number.Contains("@") ? number.Substring(number.IndexOf("@")+1) : (Context.Current==null ? null : Context.Current.Name)))});
            if (tmp.Count > 0)
                ret = (Intercom)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static new List<Intercom> LoadAll()
        {
            List<Intercom> ret = new List<Intercom>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(Intercom));
            foreach(Intercom icom in  conn.Select(typeof(Intercom),
                new SelectParameter[] { new EqualParameter("Context",Context.LoadByName("Internal"))}))
                ret.Add(icom);
            conn.CloseConnection();
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
                    typeof(IntercomPlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "DeleteIntercom",
                        new NameValuePair[]{
                            new NameValuePair("context",Context.Name),
                            new NameValuePair("intercomExtension",Number)
                        }
                    ),
                    new IEvent[]{
                                new GenericEvent("IntercomDeleted",new NameValuePair[]{
                                    new NameValuePair("Number",Number),
                                    new NameValuePair("DomainName",Domain.Current.Name)
                            })}
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
            bool ret = true;
            try
            {

                base.Save();
                sDomainExtensionPair[] extensions = new sDomainExtensionPair[Extensions.Length];
                for (int x = 0; x < Extensions.Length; x++)
                    extensions[x] = new sDomainExtensionPair(Extensions[x].Number, Extensions[x].Domain.Name);
                ConfigurationController.RegisterChangeCall(
                    typeof(IntercomPlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "AddIntercom",
                        new NameValuePair[]{
                            new NameValuePair("context",Context.Name),
                            new NameValuePair("intercomExtension",Number),
                            new NameValuePair("oneWay",OneWay),
                            new NameValuePair("extensions",extensions)
                        }
                    ),
                    new IEvent[]{
                                new GenericEvent("IntercomCreated",new NameValuePair[]{
                                    new NameValuePair("Number",Number),
                                    new NameValuePair("DomainName",Domain.Current.Name)
                            })}
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
                sDomainExtensionPair[] extensions = new sDomainExtensionPair[Extensions.Length];
                for (int x = 0; x < Extensions.Length; x++)
                    extensions[x] = new sDomainExtensionPair(Extensions[x].Number, Extensions[x].Domain.Name);
                ConfigurationController.RegisterChangeCall(
                    typeof(IntercomPlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "UpdateIntercom",
                        new NameValuePair[]{
                            new NameValuePair("context",Context.Name),
                            new NameValuePair("intercomExtension",OriginalNumber),
                            new NameValuePair("newIntercomExtension",Number),
                            new NameValuePair("oneWay",OneWay),
                            new NameValuePair("extensions",extensions)
                        }
                    ),
                    new IEvent[]{
                                new GenericEvent("IntercomUpdated",new NameValuePair[]{
                                    new NameValuePair("Number",Number),
                                    new NameValuePair("DomainName",Domain.Current.Name)
                            })}
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
    }
}