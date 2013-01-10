using System;
using System.Collections.Generic;

using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;

/// <summary>
/// Summary description for CdrListener
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones
{
    public class CdrListener : IEventHandler
    {

        public const string CALL_HANGUP_EVENT = "CHANNEL_HANGUP_COMPLETE";

        private const string SOCKET_EVENT_NAME = "Socket Event";
        private const string HTTP_EVENT_NAME = "HTTP CDR Event";
        private const string SUB_EVENT_NAME = "Event-Name";
        private const string CALLER_ID_NAME = "Caller-Caller-ID-Name";
        private const string CALLER_ID_NUMBER = "Caller-Caller-ID-Number";
        private const string CALLER_DESTINATION = "Caller-Destination-Number";
        private const string CALL_START = "variable_start_stamp";
        private const string CALL_PICKUP = "variable_answer_stamp";
        private const string CALL_END = "variable_end_stamp";
        private const string CALL_DURATION = "variable_duration";
        private const string CALL_BILL_DURATION = "variable_billsec";
        private const string CALL_HANGUP_CAUSE = "variable_hangup_cause";
        private const string EXTENSION_NUM = "variable_accountcode";
        private const string CALL_CONTEXT = "Caller-Context";
        private const string DOMAIN_NAME = "variable_domain_name";
        private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private const string CHANNEL_NAME = "Channel-Name";
        private const string UNIQUE_ID = "Unique-ID";
        private const string CORE_UUID = "Core-UUID";
        private const string VALIDATING_PIN = "variable_inputted_validation_pin";

        public CdrListener()
        {
        }
        #region IEventHandler Members

        public bool HandlesEvent(IEvent Event)
        {
            if (Event.Name == SOCKET_EVENT_NAME)
                return (string)Event[SUB_EVENT_NAME] == CALL_HANGUP_EVENT;
            return Event.Name == HTTP_EVENT_NAME;
        }

        public void ProcessEvent(IEvent Event)
        {
            Log.Trace("Creating cdr record for call " + (string)Event["Core-UUID"]);
            try
            {
                DateTime? ansTime = null;
                string domain = (string)Event[DOMAIN_NAME];
                if (domain == null)
                {
                    domain = (string)Event[CHANNEL_NAME];
                    domain = domain.Substring(domain.IndexOf("@") + 1);
                    if (domain.Contains(":"))
                        domain = domain.Substring(0, domain.IndexOf(":"));
                }
                if (Event[CALL_PICKUP] != null)
                    ansTime = DateTime.ParseExact((string)Event[CALL_PICKUP], DATE_TIME_FORMAT, null);
                Extension ext = Extension.Load((string)Event[EXTENSION_NUM],Domain.Load(domain));
                CDR tmp = CDR.Create(
                    domain,
                    (string)Event[CALLER_ID_NAME],
                    (string)Event[CALLER_ID_NUMBER],
                    (string)Event[CALLER_DESTINATION],
                    DateTime.ParseExact((string)Event[CALL_START], DATE_TIME_FORMAT, null),
                    ansTime,
                    DateTime.ParseExact((string)Event[CALL_END], DATE_TIME_FORMAT, null),
                    long.Parse((string)Event[CALL_DURATION]),
                    long.Parse((string)Event[CALL_BILL_DURATION]),
                    Context.LoadByName((string)Event[CALL_CONTEXT]),
                    (string)Event[UNIQUE_ID],
                    (string)Event[CORE_UUID],
                    (string)Event[CALL_HANGUP_CAUSE],
                    ext,
                    (string)Event[VALIDATING_PIN]);
                if (tmp == null)
                {
                    EventController.TriggerEvent(new ErrorOccuredEvent("An error occured in creating the cdr"));
                    Log.Error("An error occured in creating the cdr");
                }
                else
                    Log.Trace("Successfully create cdr record for " + tmp.UniqueID + " at " + tmp.CallStart.ToString(DATE_TIME_FORMAT) + " to " + tmp.CallEndTime.ToString(DATE_TIME_FORMAT));
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
            }
        }

        #endregion
    }
}