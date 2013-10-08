using System;
using System.Collections.Generic;

using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Trunks;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.Dbpro;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

/// <summary>
/// Summary description for VacationRoute
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/Vacation.js")]
    [ModelRoute("/core/models/pbxConfig/VacationRoute")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Routes")]
    public class VacationRoute : ExtensionNumber
    {
        
        public VacationRoute()
        {
        }

        private string _name;
        [Field(250,false)]
        [ModelRequiredField()]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private DateTime? _originalStartDate;

        private DateTime _startDate;
        [PrimaryKeyField(false)]
        [ModelRequiredField()]
        public DateTime StartDate
        {
            get { return _startDate; }
            set {
                _originalStartDate = (_originalStartDate.HasValue ? _originalStartDate.Value : value);
                _startDate = value; 
            }
        }

        private DateTime _endDate;
        [Field(false)]
        [ModelRequiredField()]
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        private bool _endWithVoicemail;
        [Field(false)]
        [ModelRequiredField()]
        public bool EndWithVoicemail
        {
            get { return _endWithVoicemail; }
            set { _endWithVoicemail = value; }
        }

        private VacationRouteRedirectTypes _type;
        [Field(false)]
        [ModelRequiredField()]
        public VacationRouteRedirectTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private Extension _bridgeExtension;
        [ForeignField(true,ForeignField.UpdateDeleteAction.CASCADE,ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelPropertyLazyLoadExternalModel()]
        public Extension BridgeExtension
        {
            get { return _bridgeExtension; }
            set { _bridgeExtension = value; }
        }

        private sCallExtensionReference _extensionReference;
        [Field("EXTENSION_REFERENCE",FieldType.STRING,true,1024)]
        public sCallExtensionReference ExtensionReference
        {
            get { return _extensionReference; }
            set { _extensionReference = value; }
        }

        private Gateway _outGateway;
        [ForeignField(true, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ModelPropertyLazyLoadExternalModel()]
        public Gateway OutGateway
        {
            get { return _outGateway; }
            set { _outGateway = value; }
        }

        private string _gatewayNumber;
        [Field(100,true)]
        public string GatewayNumber
        {
            get { return _gatewayNumber; }
            set { _gatewayNumber = value; }
        }

        private File _audioFile;
        [Field("AudioFile", FieldType.STRING, true, 2000)]
        [ModelPropertyLazyLoadExternalModel()]
        public File AudioFile
        {
            get { return _audioFile; }
            set { _audioFile = value; }
        }

        private int? _timeout;
        [Field(true)]
        public int? Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public new string id
        {
            get { return Number + "@" + Utility.DateTimeToUnixTimestamp(StartDate).ToString(); }
        }

        [ModelLoadMethod()]
        public new static VacationRoute Load(string id)
        {
            VacationRoute ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(VacationRoute));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(VacationRoute),
                new SelectParameter[]{new EqualParameter("Number",id.Substring(0,id.IndexOf("@"))),
                    new EqualParameter("StartDate",Utility.UnixTimestampToDateTime(long.Parse(id.Substring(id.IndexOf('@')+1)))),
                    new EqualParameter("Context",Context.Current)});
            if (tmp.Count > 0)
                ret = (VacationRoute)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadAllMethod()]
        public new static List<VacationRoute> LoadAll()
        {
            List<VacationRoute> ret = new List<VacationRoute>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(VacationRoute));
            foreach (VacationRoute vr in conn.Select(typeof(VacationRoute),
                new SelectParameter[] { new EqualParameter("Context", Context.LoadByName("Internal")) }))
                ret.Add(vr);
            conn.CloseConnection();
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
                            new NameValuePair("context",Context.Name),
                            new NameValuePair("extension",Number),
                            new NameValuePair("startDate",StartDate),
                            new NameValuePair("enddate",EndDate),
                            new NameValuePair("endWithVoicemail",EndWithVoicemail)
                        });
                    switch(Type){
                        case VacationRouteRedirectTypes.PhoneExtension:
                            pars.Add(new NameValuePair("bridgeExtension",new sDomainExtensionPair(BridgeExtension.Number,BridgeExtension.Domain.Name)));
                            break;
                        case VacationRouteRedirectTypes.PlayFile:
                            pars.Add(new NameValuePair("audioFile", AudioFile));
                            break;
                        case VacationRouteRedirectTypes.TransferToExtension:
                            pars.Add(new NameValuePair("callExtension", ExtensionReference));
                            break;
                        case VacationRouteRedirectTypes.OutGateway:
                            pars.Add(new NameValuePair("outGateway", new sGatewayNumberPair(GatewayNumber, OutGateway.Name)));
                            break;
                    }
                    ConfigurationController.RegisterChangeCall(
                        typeof(VacationRoutePlan),
                        new ADialPlan.sUpdateConfigurationsCall(
                            "AddVacationRoute",
                            pars.ToArray()),
                         new IEvent[]{
                                new GenericEvent("VacationRouteCreated",
                                    new NameValuePair[]{
                                        new NameValuePair("DomainName",Domain.Current.Name),
                                        new NameValuePair("Name",Name),
                                        new NameValuePair("Number",Number)
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

        [ModelDeleteMethod()]
        public new bool Delete()
        {
            bool ret = true;
            try
            {
                base.Delete();
                ConfigurationController.RegisterChangeCall(
                        typeof(VacationRoutePlan),
                        new ADialPlan.sUpdateConfigurationsCall(
                            "DeleteVacationRoute",
                            new NameValuePair[]{
                                new NameValuePair("context",Context.Name),
                                new NameValuePair("extension",Number),
                                new NameValuePair("startDate",StartDate)
                            }),
                         new IEvent[]{
                                new GenericEvent("VacationRouteDeleted",
                                    new NameValuePair[]{
                                        new NameValuePair("DomainName",Domain.Current.Name),
                                        new NameValuePair("Name",Name),
                                        new NameValuePair("Number",Number)
                                    })
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
            bool ret = _isValid;
            if (ret)
            {
                try
                {
                    base.Update();
                    List<NameValuePair> pars = new List<NameValuePair>(
                        new NameValuePair[]{
                            new NameValuePair("context",Context.Name),
                            new NameValuePair("extension",Number),
                            new NameValuePair("originalStartDate",_originalStartDate),
                            new NameValuePair("startDate",StartDate),
                            new NameValuePair("enddate",EndDate),
                            new NameValuePair("endWithVoicemail",EndWithVoicemail)
                        });
                    switch (Type)
                    {
                        case VacationRouteRedirectTypes.PhoneExtension:
                            pars.Add(new NameValuePair("bridgeExtension", new sDomainExtensionPair(BridgeExtension.Number, BridgeExtension.Domain.Name)));
                            break;
                        case VacationRouteRedirectTypes.PlayFile:
                            pars.Add(new NameValuePair("audioFile", AudioFile));
                            break;
                        case VacationRouteRedirectTypes.TransferToExtension:
                            pars.Add(new NameValuePair("callExtension", ExtensionReference));
                            break;
                        case VacationRouteRedirectTypes.OutGateway:
                            pars.Add(new NameValuePair("outGateway", new sGatewayNumberPair(GatewayNumber, OutGateway.Name)));
                            break;
                    }
                    ConfigurationController.RegisterChangeCall(
                        typeof(VacationRoutePlan),
                        new ADialPlan.sUpdateConfigurationsCall(
                            "UpdateVacationRoute",
                            pars.ToArray()),
                         new IEvent[]{
                                new GenericEvent("VacationRouteUpdated",
                                    new NameValuePair[]{
                                        new NameValuePair("DomainName",Domain.Current.Name),
                                        new NameValuePair("Name",Name),
                                        new NameValuePair("Number",Number)
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
    }
}