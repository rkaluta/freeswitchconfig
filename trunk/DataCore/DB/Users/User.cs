/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 08/11/2009
 * Time: 2:39 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using Org.Reddragonit.Dbpro.Structure.Attributes;

using System.Security.Cryptography;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.Dbpro;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet;
using Org.Reddragonit.EmbeddedWebServer;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users
{
	/// <summary>
	/// Description of User.
	/// </summary>
	[Table()]
    [ModelJSFilePath("/resources/scripts/Core.js")]
    [ModelRoute("/core/models/Core/User")]
    [ModelViewTag(ModelViewTagTypes.tr)]
    [ModelNamespace("FreeswitchConfig.Core")]
    [TableIndex("ind_username",new string[]{"UserName"},true,true)]
	public class User : Org.Reddragonit.Dbpro.Structure.Table,IModel
	{
        internal const string STORAGE_ID = "Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users.User";

		public User()
		{
		}
		
		private long _id;
		[PrimaryKeyField(true)]
        [ModelIgnoreProperty()]
		public long ID{
			get{return _id;}
			set{_id=value;}
		}
		
		private string _firstName;
		[Field(250,false)]
		public string FirstName{
			get{return _firstName;}
			set{_firstName=value;}
		}
		
		private string _lastName;
		[Field(250,false)]
		public string LastName{
			get{return _lastName;}
			set{_lastName=value;}
		}
		
		private string _userName;
		[Field(250,false)]
		public string UserName{
			get{return _userName;}
			set{_userName=value;}
		}
		
		private string _secPass;
		[Field(250,false)]
        [ModelIgnoreProperty()]
		public string SecPass{
			get{return _secPass;}
			set{_secPass=value;}
		}

        private string _email;
		[Field(1024,true)]
		public string Email{
			get{return _email;}
			set{_email=value;}
		}
		
		private Extension _extension;
		[ForeignField(true,ForeignField.UpdateDeleteAction.SET_NULL,ForeignField.UpdateDeleteAction.SET_NULL)]
		public Extension UserExtension{
			get{return _extension;}
			set{_extension=value;}
		}

        private UserRight[] _rights;
        [ForeignField(true, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public UserRight[] Rights
        {
            get { return _rights; }
            set { _rights = value; }
        }

        private bool _locked = false;
        [Field(false)]
        public bool Locked
        {
            get { return _locked; }
            set { _locked = value; }
        }

        private bool _disabled = false;
        [Field(false)]
        public bool Disabled
        {
            get { return _disabled; }
            set { _disabled = value; }
        }
        
        private Domain _domain;
        [ForeignField(true, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public Domain AttachedDomain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        private Domain[] _allowedDomains;
        [ForeignField(true, ForeignField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public Domain[] AllowedDomains
        {
            get { return _allowedDomains; }
            set { _allowedDomains = value; }
        }

        public static User Current
        {
            get
            {
                User ret = null;
                if (HttpRequest.CurrentRequest != null)
                {
                    if (HttpRequest.CurrentRequest.Session != null)
                    {
                        if (HttpRequest.CurrentRequest.Session[STORAGE_ID] != null)
                            ret = (User)HttpRequest.CurrentRequest.Session[STORAGE_ID];
                    }
                }
                return ret;
            }
        }
		
		[MethodInvokeChangesField(new string[]{"SecPass"})]
		public void SetPassword(string password,string realm){
            SecPass = BitConverter.ToString(MD5.Create().ComputeHash(ASCIIEncoding.ASCII.GetBytes(UserName + ":" + realm + ":" + password))).Replace("-", "").ToLower();
		}

        internal static void UnlockUser(string userName)
        {
            User usr = LoadByUsername(userName);
            if (usr != null)
            {
                usr.Locked = false;
                usr.Update();
            }
        }

        internal static void LockUser(string userName)
        {
            User usr = LoadByUsername(userName);
            if (usr != null)
            {
                usr.Locked = true;
                usr.Update();
            }
        }

        internal static void EnableUser(string userName)
        {
            User usr = LoadByUsername(userName);
            if (usr != null)
            {
                usr.Disabled = false;
                usr.Update();
            }
        }

        internal static void DisableUser(string userName)
        {
            User usr = LoadByUsername(userName);
            if (usr != null)
            {
                usr.Disabled = true;
                usr.Update();
            }
        }

        internal static User LoadByUsername(string username)
        {
            User ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(User)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(User),
                                                                          new SelectParameter[] { new EqualParameter("UserName", username) });
            if (tmp.Count > 0)
                ret = (User)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadMethod()]
        public static User Load(string id)
        {
            User ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(User)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(User),
                                                                          new SelectParameter[] { new EqualParameter("ID", long.Parse(id)) });
            if (tmp.Count > 0)
                ret = (User)tmp[0];
            else
            {
                tmp = conn.Select(typeof(User),
                    new SelectParameter[] { new EqualParameter("UserName", id) });
                if (tmp.Count > 0)
                    ret = (User)tmp[0];
            }
            conn.CloseConnection();
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> SelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users",
                "SELECT DISTINCT u.ID,u.UserName from User u "+(Domain.Current==null ? "" : "WHERE u.AttachedDomain.Name = @domainName OR u.AllowedDomains.Name = @domainName"));
            if (Domain.Current == null)
                cq.Execute();
            else
                cq.Execute(new IDbDataParameter[]{cq.CreateParameter("@domainName",Domain.Current.Name)});
            while (cq.Read())
                ret.Add(new sModelSelectOptionValue(cq[0].ToString(), cq[1].ToString()));
            cq.Close();
            return ret;
        }

        public static User Create(string username, string firstname, string lastname, string password,string email, Extension extension,UserRight[] rights)
        {
            Log.Trace("Creating new user with username " + username);
            if (LoadByUsername(username)!=null)
                throw new Exception("Unable to create user, one already exists with that username.");
            User ret = new User();
            ret.UserName = username;
            ret.FirstName = firstname;
            ret.LastName = lastname;
            ret.SetPassword(password, Constants.HTTP_AUTH_REALM);
            ret.UserExtension = extension;
            ret.Rights = rights;
            ret.Email = email;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(User)).getConnection();
            ret = (User)conn.Save(ret);
            conn.CloseConnection();
            return ret;
        }

        public bool HasRight(string right)
        {
            if (right == null || right == "")
                return true;
            Log.Trace("Checking if user "+UserName+" has right " + right);
            if (right.Contains("(") || right.Contains("||") || right.Contains("&&") || right.Contains("!"))
                return _CheckRoles(right);
            else
            {
                if (Rights != null)
                {
                    foreach (UserRight ur in Rights)
                    {
                        if (ur.Name == right)
                        {
                            Log.Trace("User " + UserName + " has right " + right);
                            return true;
                        }
                    }
                }
                Log.Trace("User " + UserName + " does not have right " + right);
                return false;
            }
        }

        private bool _CheckRoles(string role)
        {
            string ret = "";
            string cur = "";
            for (int x = 0; x < role.Length; x++)
            {
                switch (role[x])
                {
                    case '(':
                    case '!':
                        ret += role[x];
                        break;
                    case '&':
                    case '|':
                        if (cur != "")
                        {
                            ret += HasRight(cur);
                            ret += role[x] + role[x];
                            cur = "";
                        }
                        x++;
                        break;
                    case ')':
                        if (cur != "")
                        {
                            ret += HasRight(cur);
                            ret += ")";
                            cur = "";
                        }
                        break;
                    default:
                        cur += role[x];
                        break;
                }
            }
            if (cur != "")
                ret += HasRight(cur);
            ret = ret.Replace(" ", "");
            while (ret != "true" && ret != "false")
            {
                ret = ret.Replace("true&&false", "false").Replace("false&&true", "false").Replace("false&&false", "false").Replace("true&&true", "true");
                ret = ret.Replace("true||false", "true").Replace("false||true", "true").Replace("false||false", "false").Replace("true||true", "true");
                ret = ret.Replace("(true)", "true").Replace("(false)", "false");
                ret = ret.Replace("!true", "false").Replace("!false", "true");
            }
            return bool.Parse(ret);
        }

        [MethodInvokeChangesField("Rights")]
        internal void AssignRight(string right)
        {
            List<UserRight> tmp = new List<UserRight>();
            if (Rights != null)
                tmp.AddRange(Rights);
            tmp.Add(UserRight.Load(right));
            this.Rights = tmp.ToArray();
        }

        [MethodInvokeChangesField("Rights")]
        internal void RemoveRight(string right)
        {
            List<UserRight> tmp = new List<UserRight>();
            if (Rights != null)
                tmp.AddRange(Rights);
            for (int x = 0; x < tmp.Count; x++)
            {
                if (tmp[x].Name == right)
                {
                    tmp.RemoveAt(x);
                    x--;
                }
            }
            this.Rights = tmp.ToArray();
        }

        [CompleteLazyLoadPriorToCall()]
        public bool CanAccessDomain(string domain)
        {
            bool ret = false;
            if (this.AttachedDomain != null)
            {
                ret = this.AttachedDomain.Name == domain;
            }
            if (!ret)
            {
                if (this.AllowedDomains != null)
                {
                    foreach (Domain d in this.AllowedDomains)
                    {
                        if (d.Name == domain)
                        {
                            ret = true;
                            break;
                        }
                    }
                }
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
