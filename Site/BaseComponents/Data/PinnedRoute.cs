using System;
using System.Collections.Generic;

using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

/// <summary>
/// Summary description for PinnedRoute
/// </summary>

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/PinnedRoute.js")]
    [ModelRoute("/core/models/pbxConfig/PinnedRoute")]
    [ModelNamespace("FreeswitchConfig.Routes")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView)]
    public class PinnedRoute : Org.Reddragonit.Dbpro.Structure.Table, IModel
    {
        public PinnedRoute()
        {
        }

        private string _originalName;

        private string _name;
        [PrimaryKeyField(false, 250)]
        [ModelRequiredField()]
        [ReadOnlyModelProperty()]
        public string Name
        {
            get { return _name; }
            set {
                _originalName = (_originalName == null ? value : _originalName);
                _name = value; 
            }
        }

        private int _index;
        [Field(false)]
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private NPANXXValue _destCondition;
        [Field("DESTINATION_CONDITION",FieldType.STRING,false,8000)]
        public NPANXXValue DestinationCondition
        {
            get { return _destCondition; }
            set { _destCondition = value; }
        }

        private PinSet _pinFile;
        [ForeignField(false, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        [ModelPropertyLazyLoadExternalModel()]
        public PinSet PinFile
        {
            get { return _pinFile; }
            set { _pinFile = value; }
        }

        private Context _routeContext;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        [ModelPropertyLazyLoadExternalModel()]
        public Context RouteContext
        {
            get { return _routeContext; }
            set { _routeContext = value; }
        }

        [ModelLoadAllMethod()]
        public static List<PinnedRoute> LoadAll()
        {
            List<PinnedRoute> ret = new List<PinnedRoute>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(PinnedRoute));
            foreach (Org.Reddragonit.Dbpro.Structure.Table tbl in conn.Select(typeof(PinnedRoute),
                new SelectParameter[]{new EqualParameter("RouteContext",Context.Current)},
                new string[]{"Index"}))
            {
                ret.Add((PinnedRoute)tbl);
            }
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadMethod()]
        public static PinnedRoute Load(string name)
        {
            PinnedRoute ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(PinnedRoute));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(PinnedRoute),
                new SelectParameter[] { new EqualParameter("Name", name) ,
                new EqualParameter("RouteContext",Context.Current)});
            if (tmp.Count > 0)
                ret = (PinnedRoute)tmp[0];
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
                            typeof(PinnedRoutePlan),
                            new ADialPlan.sUpdateConfigurationsCall(
                                "DeletePinnedRoute",
                                new NameValuePair[]{
                                    new NameValuePair("name",Name),
                                    new NameValuePair("context",RouteContext.Name)
                                }
                            ),
                            new IEvent[]{
                                new GenericEvent("PinnedRouteDeleted",new NameValuePair[]{
                                    new NameValuePair("Name",Name),
                                    new NameValuePair("Context",RouteContext.Name)
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
                ConfigurationController.RegisterChangeCall(
                            typeof(PinnedRoutePlan),
                            new ADialPlan.sUpdateConfigurationsCall(
                                "UpdatePinnedRoute",
                                new NameValuePair[]{
                                    new NameValuePair("name",Name),
                                    new NameValuePair("context",RouteContext.Name),
                                    new NameValuePair("condition",DestinationCondition),
                                    new NameValuePair("pinsetName",PinFile.Name)
                                }
                            ),
                            new IEvent[]{
                                new GenericEvent("PinnedRouteUpdated",new NameValuePair[]{
                                    new NameValuePair("Name",Name),
                                    new NameValuePair("Context",RouteContext.Name)
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
            bool ret = false;
            try
            {
                base.Save();
                ConfigurationController.RegisterChangeCall(
                            typeof(PinnedRoutePlan),
                            new ADialPlan.sUpdateConfigurationsCall(
                                "AddPinnedRoute",
                                new NameValuePair[]{
                                    new NameValuePair("name",Name),
                                    new NameValuePair("context",RouteContext.Name),
                                    new NameValuePair("condition",DestinationCondition),
                                    new NameValuePair("pinsetName",PinFile.Name)
                                }
                            ),
                            new IEvent[]{
                                new GenericEvent("PinnedRouteCreated",new NameValuePair[]{
                                    new NameValuePair("Name",Name),
                                    new NameValuePair("Context",RouteContext.Name)
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

        #region IModel Members

        public string id
        {
            get { return Name; }
        }

        #endregion
    }
}