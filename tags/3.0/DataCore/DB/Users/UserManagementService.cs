using System;
using System.Collections.Generic;
using System.Collections;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;

/// <summary>
/// Summary description for UserManagementService
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users
{
    [EmbeddedServiceNamespace("FreeswitchConfig.Services")]
    public class UserManagementService : EmbeddedService
    {
        public UserManagementService()
        {
        }

        [WebMethod(true)]
        public List<string> GetAvailableExtensions()
        {
            List<string> ret = new List<string>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones",
                "SELECT ext.Number FROM Extension ext");
            cq.Execute();
            while (cq.Read())
            {
                ret.Add(cq[0].ToString());
            }
            cq.Close();
            return ret;
        }

        [WebMethod(true)]
        public ArrayList GetCurrentUsers()
        {
            ArrayList ret = new ArrayList();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users",
                "SELECT usr.UserName,usr.FirstName,usr.LastName,usr.Email,usr.UserExtension.Number as extNumber,usr.Locked,usr.Disabled,usr.ID FROM User usr");
            cq.Execute();
            while (cq.Read())
            {
                Hashtable ht = new Hashtable();
                ht.Add("UserName", cq[0].ToString());
                ht.Add("FirstName", cq[1].ToString());
                ht.Add("LastName", cq[2].ToString());
                if (!cq.IsDBNull(3))
                    ht.Add("Email", cq[3].ToString());
                if (!cq.IsDBNull(4))
                    ht.Add("Extension", cq[4].ToString());
                ht.Add("Locked", cq.GetBoolean(5));
                ht.Add("Disabled", cq.GetBoolean(6));
                ht.Add("ID", cq.GetInt64(7));
                ret.Add(ht);
            }
            cq.Close();
            return ret;
        }

        [WebMethod(true)]
        public void DisableAccount(string username)
        {
            User.DisableUser(username);
        }

        [WebMethod(true)]
        public void EnableAccount(string username)
        {
            User.EnableUser(username);
        }

        [WebMethod(true)]
        public void UnlockAccount(string username)
        {
            User.UnlockUser(username);
        }

        [WebMethod(true)]
        public bool UpdateUserPassword(string username, string newpass)
        {
            try
            {
                User usr = User.LoadByUsername(username);
                usr.SetPassword(newpass, Constants.HTTP_AUTH_REALM);
                usr.Update();
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
                return false;
            }
            return true;
        }

        [WebMethod(true)]
        public List<NameValuePair> GetUserRights(string username)
        {
            List<NameValuePair> ret = new List<NameValuePair>();
            User usr = User.LoadByUsername(username);
            foreach (UserRight ur in UserRight.All)
            {
                ret.Add(new NameValuePair(ur.Name, usr.HasRight(ur.Name).ToString().ToLower()));
            }
            return ret;
        }

        [WebMethod(true)]
        public bool AssignRightsToUser(string username, ArrayList rights)
        {
            bool ret = true;
            try
            {
                User usr = User.LoadByUsername(username);
                foreach (string right in rights)
                {
                    if (!usr.HasRight(right))
                    {
                        usr.AssignRight(right);
                    }
                }
                List<UserRight> tmp = new List<UserRight>(usr.Rights);
                foreach (UserRight right in tmp)
                {
                    if (!rights.Contains(right.Name))
                        usr.RemoveRight(right.Name);
                }
                usr.Update();
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
                ret = false;
            }
            return ret;
        }

        [WebMethod(true)]
        public bool UpdateUserInformation(long id, string username, string firstname, string lastname, string email, string extension,string domain)
        {
            bool ret = true;
            try
            {
                User usr = User.Load(id.ToString());
                usr.UserName = username;
                usr.FirstName = firstname;
                usr.LastName = lastname;
                usr.Email = email;
                usr.UserExtension = Extension.Load(extension,Domain.Load(domain));
                usr.Update();
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
                ret = false;
            }
            return ret;
        }

        [WebMethod(true)]
        public long? CreateUser(string username, string firstname, string lastname,string password, string email, string extension,string domain)
        {
            long? ret = null;
            try
            {
                User usr = User.Create(username, firstname, lastname, password,email, Extension.Load(extension,Domain.Load(domain)),null);
                if (usr != null)
                    ret = usr.ID;
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
                ret = null;
            }
            return ret;
        }
    }
}