using System;
using System.Collections.Generic;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.Dbpro;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

/// <summary>
/// Summary description for TimedRoute
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/TimedRoute.js")]
    [ModelRoute("/core/models/pbxConfig/TimedRoute")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Routes")]
    public class TimedRoute : Org.Reddragonit.Dbpro.Structure.Table,IModel
    {
        public TimedRoute()
        {
        }

        private string _originalName;

        private string _name;
        [PrimaryKeyField(false,100)]
        [ModelRequiredField()]
        public string Name
        {
            get { return _name; }
            set {
                _originalName = (_originalName == null ? value : _originalName);
                _name = value; 
            }
        }

        private Context _routeContext;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE,ForeignField.UpdateDeleteAction.CASCADE)]
        public Context RouteContext
        {
            get { return _routeContext; }
            set { _routeContext = value; }
        }

        private sCallTime _start;
        [Field("START",FieldType.STRING,false,24)]
        public sCallTime Start
        {
            get { return _start; }
            set { _start = value; }
        }

        private sCallTime? _end;
        [Field("END", FieldType.STRING, true, 24)]
        public sCallTime? End
        {
            get { return _end; }
            set { _end= value; }
        }

        private NPANXXValue _destCondition;
        [Field("DESTINATION_CONDITION",FieldType.STRING,false,8000)]
        public NPANXXValue DestinationCondition
        {
            get { return _destCondition; }
            set { _destCondition = value; }
        }

        private bool _performOnFail = false;
        [Field(false)]
        public bool PerformOnFail
        {
            get { return _performOnFail; }
            set { _performOnFail = value; }
        }

        private VacationRouteRedirectTypes _type;
        [Field(false)]
        public VacationRouteRedirectTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private Extension _bridgeExtension;
        [ForeignField(true, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public Extension BridgeExtension
        {
            get { return _bridgeExtension; }
            set { _bridgeExtension = value; }
        }

        private sCallExtensionReference _extensionReference;
        [Field("EXTENSION_REFERENCE", FieldType.STRING, true, 1024)]
        public sCallExtensionReference ExtensionReference
        {
            get { return _extensionReference; }
            set { _extensionReference = value; }
        }

        private Gateway _outGateway;
        [ForeignField(true, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public Gateway OutGateway
        {
            get { return _outGateway; }
            set { _outGateway = value; }
        }

        private string _gatewayNumber;
        [Field(100, true)]
        public string GatewayNumber
        {
            get { return _gatewayNumber; }
            set { _gatewayNumber = value; }
        }


        private File _audioFile;
        [Field("AudioFile", FieldType.STRING, true, 2000)]
        public File AudioFile
        {
            get { return _audioFile; }
            set { _audioFile = value; }
        }

        [ModelLoadMethod()]
        public static TimedRoute Load(string name)
        {
            TimedRoute ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(TimedRoute)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(TimedRoute),
                new SelectParameter[]{new EqualParameter("Name",name),
                    new EqualParameter("RouteContext",Context.Current)});
            if (tmp.Count > 0)
                ret = (TimedRoute)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<TimedRoute> LoadAll()
        {
            List<TimedRoute> ret = new List<TimedRoute>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(TimedRoute)).getConnection();
            foreach (TimedRoute tr in conn.Select(typeof(TimedRoute),
                new SelectParameter[] { new EqualParameter("RouteContext", Context.Current)}))
                ret.Add(tr);
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
                    typeof(TimedRoutePlan),
                    new ADialPlan.sUpdateConfigurationsCall("DeleteTimedRoute",
                        new NameValuePair[]{
                            new NameValuePair("context",RouteContext.Name),
                            new NameValuePair("name",Name)
                        }),
                        new IEvent[]{
                                new GenericEvent("TimedRouteDeleted",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",Name),
                                        new NameValuePair("DomainName",Domain.Current.Name),
                                        new NameValuePair("ContextName",RouteContext.Name)
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

        private bool _isValid
        {
            get
            {
                bool ret = true;
                switch (Type)
                {
                    case VacationRouteRedirectTypes.TransferToExtension:
                        ret = BridgeExtension != null;
                        break;
                    case VacationRouteRedirectTypes.PhoneExtension:
                        ret = ExtensionReference != null;
                        break;
                    case VacationRouteRedirectTypes.PlayFile:
                        ret = AudioFile != null;
                        break;
                    case VacationRouteRedirectTypes.OutGateway:
                        ret = (OutGateway != null) && (GatewayNumber != null);
                        break;
                }
                return ret;
            }
        }

        [ModelUpdateMethod()]
        public new bool Update()
        {
            bool ret = _isValid;
            if (ret)
            {
                try
                {
                    base.Update();
                    List<NameValuePair> pars = new List<NameValuePair>(
                        new NameValuePair[]{
                            new NameValuePair("context",RouteContext.Name),
                            new NameValuePair("name",Name),
                            new NameValuePair("originalName",_originalName),
                            new NameValuePair("condition",DestinationCondition.Value),
                            new NameValuePair("performOnFail",PerformOnFail),
                            new NameValuePair("start",Start),
                            new NameValuePair("end",End)
                        });
                    switch (Type)
                    {
                        case VacationRouteRedirectTypes.PhoneExtension:
                            pars.Add(new NameValuePair("extensionNumber", new sDomainExtensionPair(BridgeExtension.Number, BridgeExtension.Domain.Name)));
                            break;
                        case VacationRouteRedirectTypes.PlayFile:
                            pars.Add(new NameValuePair("audioFile", AudioFile.ToString()));
                            break;
                        case VacationRouteRedirectTypes.TransferToExtension:
                            pars.Add(new NameValuePair("callExtension", ExtensionReference));
                            break;
                        case VacationRouteRedirectTypes.OutGateway:
                            pars.Add(new NameValuePair("gateway", new sGatewayNumberPair(GatewayNumber, OutGateway.Name)));
                            break;
                    }
                    ConfigurationController.RegisterChangeCall(
                        typeof(TimedRoute),
                        new ADialPlan.sUpdateConfigurationsCall(
                            "UpdateTimedRoute",
                            pars.ToArray()),
                         new IEvent[]{
                                new GenericEvent("TimedRouteUpdated",
                                    new NameValuePair[]{
                                        new NameValuePair("DomainName",Domain.Current.Name),
                                        new NameValuePair("Name",Name)
                                    })
                            });
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                    ret = false;
                }
            }
            return ret;
        }

        [ModelSaveMethod()]
        public new bool Save()
        {
            bool ret = _isValid;
            if (ret)
            {
                try
                {
                    base.Save();
                    List<NameValuePair> pars = new List<NameValuePair>(
                        new NameValuePair[]{
                            new NameValuePair("context",RouteContext.Name),
                            new NameValuePair("name",Name),
                            new NameValuePair("condition",DestinationCondition.Value),
                            new NameValuePair("performOnFail",PerformOnFail),
                            new NameValuePair("start",Start),
                            new NameValuePair("end",End)
                        });
                    switch (Type)
                    {
                        case VacationRouteRedirectTypes.PhoneExtension:
                            pars.Add(new NameValuePair("extensionNumber", new sDomainExtensionPair(BridgeExtension.Number, BridgeExtension.Domain.Name)));
                            break;
                        case VacationRouteRedirectTypes.PlayFile:
                            pars.Add(new NameValuePair("audioFile", AudioFile.ToString()));
                            break;
                        case VacationRouteRedirectTypes.TransferToExtension:
                            pars.Add(new NameValuePair("callExtension", ExtensionReference));
                            break;
                        case VacationRouteRedirectTypes.OutGateway:
                            pars.Add(new NameValuePair("gateway", new sGatewayNumberPair(GatewayNumber, OutGateway.Name)));
                            break;
                    }
                    ConfigurationController.RegisterChangeCall(
                        typeof(TimedRoute),
                        new ADialPlan.sUpdateConfigurationsCall(
                            "AddTimedRoute",
                            pars.ToArray()),
                         new IEvent[]{
                                new GenericEvent("TimedRouteSaved",
                                    new NameValuePair[]{
                                        new NameValuePair("DomainName",Domain.Current.Name),
                                        new NameValuePair("Name",Name)
                                    })
                            });
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                    ret = false;
                }
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