using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using System.Threading;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/GatewayRoute.js")]
    [ModelRoute("/core/models/pbxConfig/GatewayRoute")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelNamespace("FreeswitchConfig.Routes")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView | ModelBlockJavascriptGenerations.EditForm)]
    public class GatewayRoute: Org.Reddragonit.Dbpro.Structure.Table,IModel
    {
        private static object _lock = new object();

        private Context _routeContext;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE,ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        public Context RouteContext
        {
            get { return _routeContext; }
            set { _routeContext = value; }
        }

        private int _id;
        [PrimaryKeyField(true)]
        [ModelIgnoreProperty()]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private int _originalIndex=int.MinValue;
        private int _index;
        [Field(false)]
        [ModelRequiredField()]
        public int Index
        {
            get { return _index; }
            set {
                if (_originalIndex == int.MinValue)
                    _originalIndex = value;
                _index = value; 
            }
        }

        private NPANXXValue _destCondition;
        [Field("DESTINATION_CONDITION",FieldType.STRING,false,8000)]
        [ModelRequiredField()]
        public NPANXXValue DestinationCondition
        {
            get { return _destCondition; }
            set { _destCondition = value; }
        }

        private Gateway _outGateway;
        [ForeignField(false, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelRequiredField()]
        public Gateway OutGateway
        {
            get { return _outGateway; }
            set { _outGateway = value; }
        }

        public GatewayRoute() { }

        [ModelLoadMethod()]
        public static GatewayRoute Load(string id)
        {
            GatewayRoute ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(GatewayRoute)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(GatewayRoute),
                new SelectParameter[] {new EqualParameter("RouteContext.Name",Context.Current.Name),
                new EqualParameter("ID",long.Parse(id))});
            if (tmp.Count > 0)
                ret = (GatewayRoute)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<GatewayRoute> LoadAll()
        {
            List<GatewayRoute> ret = new List<GatewayRoute>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(GatewayRoute)).getConnection();
            foreach (GatewayRoute gr in conn.Select(typeof(GatewayRoute),
                new SelectParameter[] { new EqualParameter("RouteContext", Context.Current) },
                new string[] { "Index" }))
                ret.Add(gr);
            conn.CloseConnection();
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
                    typeof(GatewayRoutePlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "UpdateGatewayRoute",
                        new NameValuePair[]{
                            new NameValuePair("context",RouteContext.Name),
                            new NameValuePair("id",ID),
                            new NameValuePair("gateway",OutGateway.Name),
                            new NameValuePair("condition",DestinationCondition)
                        }
                    ),
                    new IEvent[]{
                        new GenericEvent("GatewayRouteUpdated",
                            new NameValuePair[]{
                                new NameValuePair("ContextName",RouteContext.Name),
                                new NameValuePair("DomainName",Domain.Current.Name),
                                new NameValuePair("Condition",DestinationCondition),
                                new NameValuePair("GatewayName",OutGateway.Name)
                            }
                            )
                    }
                );
                if (_originalIndex != _index)
                {
                    foreach (GatewayRoute gr in LoadAll())
                    {
                        if (gr.Index >= Math.Min(_originalIndex,_index))
                        {
                            if (_index>_originalIndex){
                                if (gr.Index>=_index)
                                    gr.Index++;
                                else
                                    gr.Index--;
                            }else{
                                if (gr.Index>=_index)
                                    gr.Index--;
                                else
                                    gr.Index++;
                            }
                            gr.Update();
                            ConfigurationController.RegisterChangeCall(
                            typeof(GatewayRoutePlan),
                            new ADialPlan.sUpdateConfigurationsCall("UpdateRouteIndex",
                            new NameValuePair[]{
                                new NameValuePair("context",gr.RouteContext.Name),
                                new NameValuePair("id",gr.ID),
                                new NameValuePair("index",gr.Index)
                            }),
                            new IEvent[]{
                                new GenericEvent("GatewayRoutesReOrdered",
                                    new NameValuePair[]{
                                        new NameValuePair("Context",gr.RouteContext.Name)
                                    })
                                }
                            );
                        }
                    }
                    ConfigurationController.RegisterChangeCall(
                        typeof(GatewayRoutePlan),
                        new ADialPlan.sUpdateConfigurationsCall("UpdateRouteIndex",
                        new NameValuePair[]{
                            new NameValuePair("context",RouteContext.Name),
                            new NameValuePair("id",ID),
                            new NameValuePair("index",Index)
                        }),
                        new IEvent[]{
                            new GenericEvent("GatewayRoutesReOrdered",
                                new NameValuePair[]{
                                    new NameValuePair("Context",RouteContext.Name)
                                })
                            }
                        );
                }
                _originalIndex = _index;
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
                    typeof(GatewayRoutePlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "DeleteGatewayRoute",
                        new NameValuePair[]{
                            new NameValuePair("context",RouteContext.Name),
                            new NameValuePair("id",ID)
                        }
                    ),
                    new IEvent[]{
                        new GenericEvent("GatewayRouteDeleted",
                            new NameValuePair[]{
                                new NameValuePair("ContextName",RouteContext.Name),
                                new NameValuePair("Condition",DestinationCondition),
                                new NameValuePair("GatewayName",OutGateway.Name)
                            }
                            )
                    }
                );
                foreach (GatewayRoute gr in LoadAll())
                {
                    if (gr.Index > _index)
                    {
                        gr.Index--;
                        gr.Update();
                    }
                }
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
                ConfigurationController.RegisterChangeCall(
                    typeof(GatewayRoutePlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "AddGatewayRoute",
                        new NameValuePair[]{
                            new NameValuePair("context",RouteContext.Name),
                            new NameValuePair("gateway",OutGateway.Name),
                            new NameValuePair("condition",DestinationCondition),
                            new NameValuePair("index",Index),
                            new NameValuePair("id",ID)
                        }
                    ),
                    new IEvent[]{
                        new GenericEvent("GatewayRouteCreated",
                            new NameValuePair[]{
                                new NameValuePair("ContextName",RouteContext.Name),
                                new NameValuePair("DomainName",Domain.Current.Name),
                                new NameValuePair("Condition",DestinationCondition),
                                new NameValuePair("GatewayName",OutGateway.Name)
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

        #region IModel Members

        public string id
        {
            get { return ID.ToString(); }
        }

        #endregion
    }
}
