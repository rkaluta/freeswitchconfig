/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 08/11/2009
 * Time: 2:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using System.Collections.Generic;
using Org.Reddragonit.Dbpro.Connections.Parameters;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users
{
	/// <summary>
	/// Description of UserRight.
	/// </summary>
	[Table()]
	public class UserRight : Org.Reddragonit.Dbpro.Structure.Table
	{
		public UserRight()
		{
		}
		
		private long _id;
		[PrimaryKeyField(true)]
		public long ID{
			get{return _id;}
			set{_id = value;}
		}
		
		private string _name;
		[Field(250,false)]
		public string Name{
			get{return _name;}
			set{_name=value;}
		}
		
		public static UserRight CreateRight(string name){
            Log.Trace("Creating UserRight " + name);
			Connection conn = ConnectionPoolManager.GetConnection(typeof(UserRight)).getConnection();
            Log.Trace("Checking is UserRight " + name + " already exists");
			List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(UserRight),
			                                                              new SelectParameter[]{new EqualParameter("Name",name)});
			UserRight ret = null;
            if (tmp.Count > 0)
            {
                Log.Trace("UserRight " + name + " already exists in the database");
                ret = (UserRight)tmp[0];
            }
            else
            {
                Log.Trace("UserRight " + name + " does not exists in the database, adding it now");
                ret = new UserRight();
                ret.Name = name;
                ret = (UserRight)conn.Save(ret);
                conn.Commit();
                User u = User.LoadByUsername("admin");
                if (u != null)
                {
                    bool found = false;
                    foreach (UserRight ur in u.Rights)
                    {
                        if (ur.Name == ret.Name)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        List<UserRight> rights = new List<UserRight>(u.Rights);
                        rights.Add(ret);
                        u.Rights = rights.ToArray();
                        u.Update();
                    }
                }
            }
			conn.CloseConnection();
			return ret;
		}

        public static List<UserRight> All
        {
            get
            {
                Log.Trace("Loading all UserRights");
                List<UserRight> ret = new List<UserRight>();
                Connection conn = ConnectionPoolManager.GetConnection(typeof(UserRight)).getConnection();
                foreach (UserRight ur in conn.SelectAll(typeof(UserRight)))
                    ret.Add(ur);
                conn.CloseConnection();
                return ret;
            }
        }

        internal static UserRight Load(string name)
        {
            UserRight ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(UserRight)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(UserRight),
                new SelectParameter[]{new EqualParameter("Name",name)});
            if (tmp.Count>0)
                ret=(UserRight)tmp[0];
            conn.CloseConnection();
            return ret;
        }
	}
}
