using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Models.PBXConfig
{
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/Pins.js")]
    [ModelRoute("/core/models/pbxConfig/Pin")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.PBX")]
    public class mPin : IModel
    {
        private string[] _idBlock;

        public mPin()
        {
            _idBlock = new string[3];
        }

        public string SetName
        {
            get { return _idBlock[0]; }
            set { _idBlock[0] = value; }
        }

        public string Context
        {
            get { return _idBlock[1]; }
            set { _idBlock[1] = value; }
        }

        private string _extensionNumber;
        public string ExtensionNumber
        {
            get { return _extensionNumber; }
            set { _extensionNumber = value; }
        }

        private string _extensionDomain;
        public string ExtensionDomain
        {
            get { return _extensionDomain; }
            set { _extensionDomain = value; }
        }

        private string _originalPin;
        private string _pinNumber;
        public string PinNumber
        {
            get { return _pinNumber; }
            set { _pinNumber = value; }
        }

        public int ID
        {
            get { return int.Parse(_idBlock[2]); }
            set { _idBlock[2] = value.ToString(); }
        }

        private mPin(ClassQuery cq)
        {
            ID = cq.GetInt32(0);
            SetName = cq.GetString(1);
            Context = cq.GetString(2);
            if (!cq.IsDBNull(3))
            {
                ExtensionNumber = cq.GetString(3);
                ExtensionDomain = cq.GetString(4);
            }
            PinNumber = cq.GetString(5);
            _originalPin = PinNumber;
        }

        [ModelLoadMethod()]
        public static mPin Load(string id)
        {
            mPin ret = null;
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data",
                "SELECT pn.ID,pn.OwningSet.Name,pn.OwningSet.Context.Name,pn.Extension.Number,pn.Extension.Domain.Name,pn.PinNumber FROM Pin pn " +
                "WHERE pn.OwningSet.Name = @setName AND pn.OwningSet.Context.Name = @contextName AND pn.ID = @id");
            cq.Execute(new IDbDataParameter[]{
                cq.CreateParameter("@setName",id.Substring(0,id.IndexOf("@"))),
                cq.CreateParameter("@contextName",id.Substring(id.IndexOf("@")+1,id.IndexOf(":")-id.IndexOf("@"))),
                cq.CreateParameter("@id",id.Substring(id.IndexOf(":")+1))
            });
            if (cq.Read())
                ret = new mPin(cq);
            cq.Close();
            return ret;
        }

        [ModelListMethod("/core/models/search/pins/{0}")]
        public static List<mPin> GetPinsForPinset(string pinsetName)
        {
            List<mPin> ret = new List<mPin>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data",
                "SELECT pn.ID,pn.OwningSet.Name,pn.OwningSet.Context.Name,pn.Extension.Number,pn.Extension.Domain.Name,pn.PinNumber FROM Pin pn " +
                "WHERE pn.OwningSet.Name = @setName AND pn.OwningSet.Context.Name = @contextName");
            cq.Execute(new IDbDataParameter[]{
                cq.CreateParameter("@setName",pinsetName),
                cq.CreateParameter("@contextName",Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core.Context.Current.Name)
            });
            while (cq.Read())
                ret.Add(new mPin(cq));
            cq.Close();
            return ret;
        }

        [ModelUpdateMethod()]
        public bool Update()
        {
            bool ret = true;
            try
            {
                Pin pn = Pin.Load(SetName, Context, ID);
                pn.PinNumber = PinNumber;
                pn.Update();
                ConfigurationController.RegisterChangeCall(
                    typeof(PinnedRoutePlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                            (ExtensionNumber!=null ? "UpdateAdvancedPin" : "UpdateBasicPin"),
                            new NameValuePair[]{
                                new NameValuePair("context",Context),
                                new NameValuePair("name",SetName),
                                new NameValuePair("oldPin",(ExtensionNumber!=null ? (object)new PinnedRoutePlan.sAdvancedPin(ExtensionNumber,ExtensionDomain,_originalPin) : (object)_originalPin)),
                                new NameValuePair("newPin",(ExtensionNumber!=null ? (object)new PinnedRoutePlan.sAdvancedPin(ExtensionNumber,ExtensionDomain,PinNumber) : (object)PinNumber))
                            }
                        ),
                            new IEvent[]{
                                new GenericEvent("PinSetUpdated",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",SetName),
                                        new NameValuePair("Context",Context),
                                        new NameValuePair("IsAdvanced",(ExtensionNumber!=null))
                                })
                            }
                    );
                _originalPin = PinNumber;
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
        public bool Delete()
        {
            bool ret = true;
            try
            {
                Pin pn = Pin.Load(SetName, Context, ID);
                pn.Delete();
                ConfigurationController.RegisterChangeCall(
                    typeof(PinnedRoutePlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                            (ExtensionNumber != null ? "DeleteAdvancedPin" : "DeleteBasicPin"),
                            new NameValuePair[]{
                                new NameValuePair("context",Context),
                                new NameValuePair("name",SetName),
                                new NameValuePair("pin",(ExtensionNumber!=null ? (object)new PinnedRoutePlan.sAdvancedPin(ExtensionNumber,ExtensionDomain,PinNumber) : (object)PinNumber))
                            }
                        ),
                            new IEvent[]{
                                new GenericEvent("PinSetUpdated",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",SetName),
                                        new NameValuePair("Context",Context),
                                        new NameValuePair("IsAdvanced",(ExtensionNumber!=null))
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

        [ModelSaveMethod()]
        public bool Save()
        {
            bool ret = true;
            try
            {
                Pin pn = new Pin();
                pn.PinNumber = PinNumber;
                if (ExtensionNumber != null)
                    pn.Extension = Extension.Load(ExtensionNumber + "@" + ExtensionDomain);
                pn.OwningSet = PinSet.Load(SetName+"@"+Context);
                pn.Save();
                ID = pn.ID;
                ConfigurationController.RegisterChangeCall(
                    typeof(PinnedRoutePlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                            (ExtensionNumber != null ? "AddAdvancedPin" : "AddBasicPin"),
                            new NameValuePair[]{
                                new NameValuePair("context",Context),
                                new NameValuePair("name",SetName),
                                new NameValuePair("pin",(ExtensionNumber!=null ? (object)new PinnedRoutePlan.sAdvancedPin(ExtensionNumber,ExtensionDomain,PinNumber) : (object)PinNumber))
                            }
                        ),
                            new IEvent[]{
                                new GenericEvent("PinSetUpdated",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",SetName),
                                        new NameValuePair("Context",Context),
                                        new NameValuePair("IsAdvanced",(ExtensionNumber!=null))
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

        #region IModel Members

        public string id
        {
            get { return _idBlock[0] + "@" + _idBlock[1] + ":" + _idBlock[2]; }
        }

        #endregion
    }
}
