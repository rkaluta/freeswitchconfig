using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using System.Net;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.EmbeddedWebServer.Components;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users
{

    public static class UserAuthentication
    {
        private static int _maxAttempts
        {
            get
            {
                if (Settings.Current["MAXIMUM_LOGIN_ATTEMPTS"] == null)
                    Settings.Current["MAXIMUM_LOGIN_ATTEMPTS"] = 3;
                return (int)Settings.Current["MAXIMUM_LOGIN_ATTEMPTS"];
            }
        }

        private static int _loginCount
        {
            get
            {
                if (HttpRequest.CurrentRequest != null)
                {
                    if (HttpRequest.CurrentRequest.Session != null)
                    {
                        if (HttpRequest.CurrentRequest.Session["LOGIN_ATTEMPTS"] != null)
                            return (int)HttpRequest.CurrentRequest.Session["LOGIN_ATTEMPTS"];
                    }
                }
                return 0;
            }
            set
            {
                if (HttpRequest.CurrentRequest != null)
                {
                    if (HttpRequest.CurrentRequest.Session != null)
                    {
                        HttpRequest.CurrentRequest.Session["LOGIN_ATTEMPTS"] = value;
                    }
                }
            }
        }

        public static sHttpAuthUsernamePassword[] GetAuthenticationInformationForUrl(Uri url, string username)
        {
            if (User.Current != null)
                return new sHttpAuthUsernamePassword[] { new sHttpAuthUsernamePassword(User.Current.UserName, null, User.Current.SecPass) };
            else
            {
                if (_loginCount>=_maxAttempts)
                    return new sHttpAuthUsernamePassword[0];
                List<sHttpAuthUsernamePassword> ret = new List<sHttpAuthUsernamePassword>();
                ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users",
                    "SELECT u.UserName,u.SecPass FROM User u WHERE u.Locked = @locked AND u.Disabled = @locked AND u.UserName = @username");
                cq.Execute(new IDbDataParameter[]{cq.CreateParameter("@locked",false),
                cq.CreateParameter("@username",username)});
                while (cq.Read())
                    ret.Add(new sHttpAuthUsernamePassword(cq[0].ToString(), null, cq[1].ToString()));
                cq.Close();
                return ret.ToArray();
            }
        }

        public static void PostAuthenticationFailure(HttpRequest request, string username)
        {
            _loginCount++;
            if (_loginCount >= _maxAttempts)
            {
                EventController.TriggerEvent(new UserLoginEvent(username, ((IPEndPoint)request.Client).Address, UserLoginEvent.LoginEventTypes.ATTEMPTS_EXCEEDED));
                User usr = User.LoadByUsername(username);
                if (usr != null)
                {
                    usr.Locked = true;
                    usr.Update();
                }
                request.ResponseStatus = HttpStatusCodes.Forbidden;
            }
            else
                EventController.TriggerEvent(new UserLoginEvent(username, ((IPEndPoint)request.Client).Address, UserLoginEvent.LoginEventTypes.FAILURE));
        }

        public static void PostAuthentication(HttpRequest request, sHttpAuthUsernamePassword user)
        {
            _loginCount = 0;
            User usr = User.LoadByUsername(user.UserName);
            if (usr.AttachedDomain != null)
            {
                request.Session[Context.SESSION_ID] = usr.AttachedDomain.InternalProfile.Context;
                request.Session[Domain.SESSION_ID] = usr.AttachedDomain;
            }
            else if (usr.AllowedDomains != null)
            {
                if (usr.AllowedDomains.Length > 0)
                {
                    request.Session[Context.SESSION_ID] = usr.AllowedDomains[0].InternalProfile.Context;
                    request.Session[Domain.SESSION_ID] = usr.AllowedDomains[0];
                }
            }
            request.Session[User.STORAGE_ID] = usr;
            EventController.TriggerEvent(new UserLoginEvent(user.UserName, ((IPEndPoint)request.Client).Address, UserLoginEvent.LoginEventTypes.SUCCESS));
        }
    }
}
