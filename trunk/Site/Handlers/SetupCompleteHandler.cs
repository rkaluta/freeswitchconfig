using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.EmbeddedWebServer.Components;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.Site.Services;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class SetupCompleteHandler : IRequestHandler
    {
        private const string _URL_PATH = "/core/SetupComplete";

        #region IRequestHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public bool CanProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            return request.URL.AbsolutePath == _URL_PATH && !Utility.IsSiteSetup;
        }

        public void ProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            bool isComplete = false;
            try
            {
                if (Domain.AllDomainNames.Count == 0)
                    throw new Exception("No Domains have been created");

                if (Context.AllContextNames.Count < 2)
                    throw new Exception("You need a minimum of 2 contexts");

                if (SipProfile.AllSipProfileNames.Count < 2)
                    throw new Exception("You need a minimum of 2 sip profiles");

                User usr = User.Create(request.Parameters["UserName"],
                    request.Parameters["FirstName"],
                    request.Parameters["LastName"],
                    request.Parameters["Password"],
                    null,
                    null,
                    UserRight.All.ToArray());
                if (usr == null)
                    throw new Exception("Unable to create primary user");
                else
                {
                    usr.AllowedDomains = Domain.LoadAll().ToArray();
                    usr.Update();
                }

                foreach (string c in Context.AllContextNames)
                    CoreGenerator.RegenerateContextFile(c);
                foreach (SipProfile sp in SipProfile.LoadAll())
                    CoreGenerator.RegenerateSIPProfile(sp);
                foreach (Domain d in Domain.LoadAll())
                    CoreGenerator.RegenerateDomainFile(d);

                isComplete = true;
            }
            catch (Exception e)
            {
                request.ResponseWriter.WriteLine(e.Message);
            }
            if (!isComplete)
                request.ResponseStatus = HttpStatusCodes.Forbidden;
            else
            {
                request.ResponseStatus = HttpStatusCodes.OK;
                EventController.TriggerEvent(new SetupCompleteEvent());
            }
            request.SendResponse();
        }

        public void Init()
        {
        }

        public void DeInit()
        {
        }

        public bool RequiresSessionForRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            return true;
        }

        #endregion
    }
}
