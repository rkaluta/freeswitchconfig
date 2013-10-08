using System;
using System.Collections.Generic;

using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;

/// <summary>
/// Summary description for CDR
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/CDRSearch.js")]
    [ModelRoute("/core/models/phones/cdrs")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm|ModelBlockJavascriptGenerations.View|ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.PBX")]
    public class CDR : Org.Reddragonit.Dbpro.Structure.Table,IModel
    {
        public CDR()
        {
        }

        private Domain _owningDomain;
        [ForeignPrimaryKeyField(ForeignPrimaryKeyField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        [ReadOnlyModelProperty()]
        public Domain OwningDomain
        {
            get { return _owningDomain; }
            set { _owningDomain = value; }
        }

        private string _id;
        [PrimaryKeyField(true)]
        [ReadOnlyModelProperty()]
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _callerIDName;
        [Field(255, true)]
        [ReadOnlyModelProperty()]
        public string CallerIDName
        {
            get { return _callerIDName; }
            set { _callerIDName = value; }
        }

        private string _callerIDNumber;
        [Field(255, true)]
        [ReadOnlyModelProperty()]
        public string CallerIDNumber
        {
            get { return _callerIDNumber; }
            set { _callerIDNumber = value; }
        }

        private string _destinationNumber;
        [Field(255, false)]
        [ReadOnlyModelProperty()]
        public string DestinationNumber
        {
            get { return _destinationNumber; }
            set { _destinationNumber = value; }
        }

        private DateTime _callStart;
        [Field(false)]
        [ReadOnlyModelProperty()]
        public DateTime CallStart
        {
            get { return _callStart; }
            set { _callStart = value; }
        }

        private DateTime? _callAnswerTime;
        [Field(true)]
        [ReadOnlyModelProperty()]
        public DateTime? CallAnswerTime
        {
            get { return _callAnswerTime; }
            set { _callAnswerTime = value; }
        }

        private DateTime _callEndTime;
        [Field(false)]
        [ReadOnlyModelProperty()]
        public DateTime CallEndTime
        {
            get { return _callEndTime; }
            set { _callEndTime = value; }
        }

        private long _duration;
        [Field(false)]
        [ReadOnlyModelProperty()]
        public long Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        private long _billableDuration;
        [Field(false)]
        [ReadOnlyModelProperty()]
        public long BillableDuration
        {
            get { return _billableDuration; }
            set { _billableDuration = value; }
        }

        private Context _callContext;
        [ForeignField(true,ForeignField.UpdateDeleteAction.SET_NULL,ForeignField.UpdateDeleteAction.SET_NULL)]
        [ReadOnlyModelProperty()]
        public Context CallContext
        {
            get { return _callContext; }
            set { _callContext = value; }
        }

        private string _hangupCause;
        [Field(150,false)]
        [ReadOnlyModelProperty()]
        public string HangupCause
        {
            get { return _hangupCause; }
            set { _hangupCause = value; }
        }

        private string _uniqueID;
        [Field(255, false)]
        [ReadOnlyModelProperty()]
        public string UniqueID
        {
            get { return _uniqueID; }
            set { _uniqueID = value; }
        }

        private string _coreUUID;
        [Field(255,false)]
        [ReadOnlyModelProperty()]
        public string CoreUUID
        {
            get { return _coreUUID; }
            set { _coreUUID = value; }
        }

        private Extension _internalExtension;
        [ForeignField(true, ForeignField.UpdateDeleteAction.SET_NULL, ForeignField.UpdateDeleteAction.SET_NULL)]
        [ReadOnlyModelProperty()]
        public Extension InternalExtension
        {
            get { return _internalExtension; }
            set { _internalExtension = value; }
        }

        private string _pin;
        [Field(25, true)]
        [ReadOnlyModelProperty()]
        public string Pin
        {
            get { return _pin; }
            set { _pin = value; }
        }

        [ReadOnlyModelProperty()]
        public string FormattedBillableDuration
        {
            get
            {
                long tmp = 0;
                string ret = "";
                long dur = BillableDuration;
                if (dur >= (60 * 60))
                {
                    tmp = (long)Math.Floor((decimal)dur / (decimal)(60 * 60));
                    dur = dur - (tmp * 60 * 60);
                    ret += tmp.ToString("00") + ":";
                }
                else
                    ret += "00:";
                if (dur >= 60)
                {
                    tmp = (long)Math.Floor((decimal)dur / (decimal)60);
                    dur = dur - (tmp * 60);
                    ret += tmp.ToString("00") + ":";
                }
                else
                    ret += "00:";
                ret += dur.ToString("00");
                return ret;
            }
        }

        [ModelLoadMethod()]
        public static CDR Load(string id)
        {
            CDR ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(CDR));
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(CDR),
                new SelectParameter[] { new EqualParameter("ID", id) });
            if (tmp.Count > 0)
                ret = (CDR)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelListMethod("/core/models/search/cdrs/{0}/{1}/{2}/{3}/{4}/{5}",true)]
        public static List<CDR> SearchCDRs(string extension, 
            string callerID, 
            string destination, 
            string callerName, 
            DateTime? startDate, 
            DateTime? endDate, 
            long startIndex,
            int pageSize,
            out int totalPages)
        {
            totalPages = 0;
            if (User.Current == null)
                return null;
            if (!User.Current.HasRight(Constants.CDR_RIGHT))
                return null;
            List<CDR> ret = new List<CDR>();
            List<SelectParameter> pars = new List<SelectParameter>();
            if ((extension != null) && (extension.Length > 0))
                pars.Add(new EqualParameter("InternalExtension", Extension.Load(extension, Domain.Current)));
            if ((callerID != null) && (callerID.Length > 0))
                pars.Add(new EqualParameter("CallerIDNumber", callerID));
            if ((callerName != null) && (callerName.Length > 0))
                pars.Add(new EqualParameter("CallerIDName", callerName));
            if ((destination != null) && (destination.Length > 0))
                pars.Add(new EqualParameter("DestinationNumber", destination));
            if (startDate.HasValue)
                pars.Add(new GreaterThanEqualToParameter("CallStart", startDate.Value));
            if (endDate.HasValue)
                pars.Add(new LessThanEqualToParameter("CallStart", endDate.Value));
            pars.Add(new EqualParameter("OwningDomain", Domain.Current));
            Connection conn = ConnectionPoolManager.GetConnection(typeof(CDR));
            totalPages = (int)Math.Ceiling((decimal)conn.SelectCount(typeof(CDR), pars.ToArray())/(decimal)pageSize);
            foreach (CDR c in conn.SelectPaged(typeof(CDR), pars.ToArray(), (ulong)startIndex, (ulong)pageSize))
            {
                ret.Add(c);
            }
            conn.CloseConnection();
            return ret;
        }

        internal static CDR Create(string domainName,string callerIDName,string callerIDNumber,string DestinationNumber,
            DateTime callStart,DateTime? answerTime,DateTime endTime,long duration,long billableSecs,
            Context context,string uniqueID,string coreUUID,string hangupCause,Extension internalExtension,string pin)
        {
            CDR ret = new CDR();
            ret.OwningDomain = Domain.Load(domainName);
            ret.CallerIDName = callerIDName;
            ret.CallerIDNumber = callerIDNumber;
            ret.DestinationNumber = DestinationNumber;
            ret.CallStart = callStart;
            ret.CallAnswerTime = answerTime;
            ret.CallEndTime = endTime;
            ret.Duration = duration;
            ret.BillableDuration = billableSecs;
            ret.CallContext = context;
            ret.UniqueID = uniqueID;
            ret.CoreUUID = coreUUID;
            ret.InternalExtension = internalExtension;
            ret.HangupCause = hangupCause;
            ret.Pin = pin;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(CDR));
            ret = (CDR)conn.Save(ret);
            conn.Commit();
            conn.CloseConnection();
            return ret;
        }

        #region IModel Members

        public string id
        {
            get { return ID; }
        }

        #endregion
    }
}