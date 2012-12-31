using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.DialPlans;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/DirectLine.js")]
    [ModelRoute("/core/models/pbxConfig/DirectLine")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.View|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.PBX")]
    public class DirectLine : Org.Reddragonit.Dbpro.Structure.Table,IModel
    {
        private string _originalDialedNumber = null;

        private string _dialedNumber;
        [PrimaryKeyField(false,25)]
        [ModelFieldValidationRegex("^(\\d{7,25})$")]
        public string DialedNumber
        {
            get { return _dialedNumber; }
            set {
                _originalDialedNumber = (_originalDialedNumber == null ? value : _originalDialedNumber);
                _dialedNumber = value; 
            }
        }

        private Context _dialedContext;
        [ForeignPrimaryKeyField(ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public Context DialedContext
        {
            get { return _dialedContext; }
            set { _dialedContext = value; }
        }

        private ExtensionNumber _transferTo;
        [ForeignField(false, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public ExtensionNumber TransferTo
        {
            get { return _transferTo; }
            set { _transferTo = value; }
        }

        [ModelLoadMethod()]
        public static DirectLine Load(string dialedNumber)
        {
            List<SelectParameter> pars = new List<SelectParameter>();
            if (dialedNumber.Contains("@"))
            {
                pars.Add(new EqualParameter("DialedContext.Name", dialedNumber.Substring(dialedNumber.LastIndexOf('@')+1)));
                pars.Add(new EqualParameter("DialedNumber", dialedNumber.Substring(0, dialedNumber.LastIndexOf('@'))));
            }
            else
            {
                pars.Add(new EqualParameter("DialedContext",Context.Current));
                pars.Add(new EqualParameter("DialedNumber", dialedNumber));
            }

            DirectLine ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(DirectLine)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(DirectLine),pars.ToArray());
            conn.CloseConnection();
            if (tmp.Count > 0)
                ret = (DirectLine)tmp[0];
            return ret;
        }

        [ModelLoadAllMethod()]
        public static List<DirectLine> LoadAll()
        {
            List<DirectLine> ret = new List<DirectLine>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(DirectLine)).getConnection();
            foreach (DirectLine dl in  conn.Select(typeof(DirectLine),new SelectParameter[]{new EqualParameter("DialedContext",Context.Current)}))
                ret.Add(dl);
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
                ConfigurationController.RegisterChangeCall(
                    typeof(GatewayRoutePlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "AddDirectLine",
                        new NameValuePair[]{
                            new NameValuePair("externalContext",DialedContext.Name),
                            new NameValuePair("dialedNumber",DialedNumber),
                            new NameValuePair("internalContext",TransferTo.Context.Name),
                            new NameValuePair("extension",TransferTo.Number)
                        }
                    ),
                    new IEvent[]{
                        new GenericEvent("DirectLineCreated",
                            new NameValuePair[]{
                                new NameValuePair("externalContext",DialedContext.Name),
                                new NameValuePair("dialedNumber",DialedNumber),
                                new NameValuePair("internalContext",TransferTo.Context.Name),
                                new NameValuePair("extension",TransferTo.Number)
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
                base.Save();
                ConfigurationController.RegisterChangeCall(
                    typeof(GatewayRoutePlan),
                    new ADialPlan.sUpdateConfigurationsCall(
                        "UpdateDirectLine",
                        new NameValuePair[]{
                            new NameValuePair("externalContext",DialedContext.Name),
                            new NameValuePair("originalDialedNumber",_originalDialedNumber),
                            new NameValuePair("dialedNumber",DialedNumber),
                            new NameValuePair("internalContext",TransferTo.Context.Name),
                            new NameValuePair("extension",TransferTo.Number)
                        }
                    ),
                    new IEvent[]{
                        new GenericEvent("DirectLineUpdated",
                            new NameValuePair[]{
                                new NameValuePair("externalContext",DialedContext.Name),
                                new NameValuePair("dialedNumber",DialedNumber),
                                new NameValuePair("internalContext",TransferTo.Context.Name),
                                new NameValuePair("extension",TransferTo.Number)
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
                        "DeleteDirectLine",
                        new NameValuePair[]{
                            new NameValuePair("context",DialedContext.Name),
                            new NameValuePair("dialedNumber",DialedNumber)
                        }
                    ),
                    new IEvent[]{
                        new GenericEvent("DirectLineDeleted",
                            new NameValuePair[]{
                                new NameValuePair("externalContext",DialedContext.Name),
                                new NameValuePair("dialedNumber",DialedNumber),
                                new NameValuePair("internalContext",TransferTo.Context.Name),
                                new NameValuePair("extension",TransferTo.Number)
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
            get { return DialedNumber+"@"+DialedContext.Name; }
        }

        #endregion
    }
}
