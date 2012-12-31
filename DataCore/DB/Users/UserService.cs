using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;

using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using System.Net;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.EmbeddedWebServer;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users
{
    [EmbeddedServiceNamespace("FreeswitchConfig.Services")]
    public class UserService : EmbeddedService
    {
        private static readonly char[] _USERNAME_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        private MT19937 _rand = new MT19937(DateTime.Now.Ticks);

        protected override bool IsValidAccess(string functionName)
        {
            if (functionName == "UserHasRight" || functionName=="ChangeDomain" || functionName=="GetAvailbleUserDomains" || functionName=="GetCurrentDomain" || functionName == "ChangePassword")
                return User.Current != null;
            return true;
        }

        [WebMethod(true)]
        public string Logout()
        {
            if (Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users.User.Current != null)
                Log.Trace("Logging out user " + Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users.User.Current.FirstName);
            else
            {
                EventController.TriggerEvent(new ErrorOccuredEvent("There is no user logged in to be logged out"));
                Log.Error("There is no user logged in to be logged out");
            }
            Request.Session.Abandon();
            string uname = "ABCDEFGH";
            while (User.LoadByUsername(uname) != null)
            {
                for (int x = 0; x < uname.Length; x++)
                    uname= (x==0 ? "" : uname.Substring(0,x))+_USERNAME_CHARS[_rand.RandomRange(0, _USERNAME_CHARS.Length - 1)].ToString()+(x+1==uname.Length ? "" : uname.Substring(x+1));
            }
            return (Request.IsSSL ? "https://" : "http://")+uname+"@"+Request.URL.Host+":"+Request.URL.Port.ToString()+"/";
        }

        [WebMethod(true)]
        public void ChangePassword(string newPass)
        {
            User.Current.SetPassword(newPass, Constants.HTTP_AUTH_REALM);
        }

        [WebMethod(true)]
        public bool IsUserLoggedIn()
        {
            return Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users.User.Current != null;
        }

        [WebMethod(true)]
        public bool UserHasRight(string right)
        {
            return Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users.User.Current.HasRight(right);
        }

        [WebMethod(true)]
        public bool ChangeDomain(string name)
        {
            string domain = name.Substring(0,name.LastIndexOf("["));
            if (User.Current.CanAccessDomain(domain))
            {
                Domain.SetCurrentDomain(Domain.Load(domain));
                if (name.EndsWith("[internal]"))
                    Context.SetCurrentContext(Domain.Current.InternalProfile.Context);
                else
                    Context.SetCurrentContext(Domain.Current.ExternalProfile.Context);
                return true;
            }
            return false;
        }

        [WebMethod(true)]
        public List<string> GetAvailbleUserDomains()
        {
            List<string> ret = new List<string>();
            if (User.Current.AttachedDomain != null)
                ret.Add(User.Current.AttachedDomain.Name);
            if (User.Current.AllowedDomains != null)
            {
                foreach (Domain d in User.Current.AllowedDomains)
                {
                    ret.Add(d.Name + "[internal]");
                    ret.Add(d.Name + "[external]");
                }
            }
            return ret;
        }

        [WebMethod(true)]
        public string GetCurrentDomain()
        {
            if (Domain.Current == null)
                return null;
            return Domain.Current.Name+(Domain.Current.InternalProfile.Context.Name == Context.Current.Name ? "[internal]" : "[external]");
        }
    }
}
