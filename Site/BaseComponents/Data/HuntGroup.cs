using System;
using System.Collections.Generic;

using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Collections;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;

/// <summary>
/// Summary description for HuntGroup
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/HuntGroup.js")]
    [ModelRoute("/core/models/pbxConfig/HuntGroup")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Routes")]
    public class HuntGroup : ExtensionNumber
    {
        public HuntGroup()
        {
        }

        private bool _ringSequential = false;
        [Field(false)]
        [ModelRequiredField()]
        public bool RingSequential
        {
            get { return _ringSequential; }
            set { _ringSequential = value; }
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
        public static new HuntGroup Load(string number)
        {
            HuntGroup ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(HuntGroup)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(HuntGroup),
                new SelectParameter[]{new EqualParameter("Number",number),
                new EqualParameter("Context",Context.Current)});
            if (tmp.Count > 0)
                ret = (HuntGroup)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static new List<HuntGroup> LoadAll()
        {
            List<HuntGroup> ret = new List<HuntGroup>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(HuntGroup)).getConnection();
            foreach (HuntGroup ht in conn.Select(typeof(HuntGroup),
                new SelectParameter[] { new EqualParameter("Context", Context.Current) }))
                ret.Add(ht);
            conn.CloseConnection();
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
                            typeof(HuntGroupPlan),
                            new ADialPlan.sUpdateConfigurationsCall(
                                "AddHuntGroup",
                                new NameValuePair[]{
                                    new NameValuePair("context",Context.Current.Name),
                                    new NameValuePair("extension",Number),
                                    new NameValuePair("sequential",RingSequential),
                                    new NameValuePair("extensions",extensions)
                                }
                            ),
                            new IEvent[]{
                                new GenericEvent("HuntGroupCreated",
                                    new NameValuePair[]{
                                        new NameValuePair("Context",Context.Name),
                                        new NameValuePair("Number",Number)
                                     })
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

        [ModelDeleteMethod()]
        public new bool Delete()
        {
            bool ret = true;
            try
            {
                base.Delete();
                ConfigurationController.RegisterChangeCall(
                    typeof(HuntGroupPlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "DeleteHuntGroup",
                        new NameValuePair[]{
                            new NameValuePair("context",Context.Name),
                            new NameValuePair("extension",Number)
                        }),
                    new IEvent[]{
                        new GenericEvent("HuntGroupDeleted",
                            new NameValuePair[]{
                                new NameValuePair("Context",Context.Name),
                                new NameValuePair("Number",Number)
                            })
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
                sDomainExtensionPair[] extensions = new sDomainExtensionPair[Extensions.Length];
                for (int x = 0; x < Extensions.Length; x++)
                    extensions[x] = new sDomainExtensionPair(Extensions[x].Number, Extensions[x].Domain.Name);
                ConfigurationController.RegisterChangeCall(
                            typeof(HuntGroupPlan),
                            new ADialPlan.sUpdateConfigurationsCall(
                                "UpdateHuntGroup",
                                new NameValuePair[]{
                                    new NameValuePair("context",Context.Name),
                                    new NameValuePair("extension",OriginalNumber),
                                    new NameValuePair("newExtension",Number),
                                    new NameValuePair("sequential",RingSequential),
                                    new NameValuePair("extensions",extensions)
                                }
                            ),
                                new IEvent[]{
                                    new GenericEvent("HuntGroupUpdated",
                                        new NameValuePair[]{
                                            new NameValuePair("Context",Context.Name),
                                            new NameValuePair("Number",OriginalNumber),
                                            new NameValuePair("NewNumber",Number)
                                        })
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
    }
}