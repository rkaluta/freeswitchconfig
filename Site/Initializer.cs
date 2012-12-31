/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 06/12/2008
 * Time: 12:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.Drawing;
using System.Xml;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

namespace Org.Reddragonit.FreeSwitchConfig.Site
{
	/// <summary>
	/// Description of Initializer.
	/// </summary>
    /// 

    //
    // TODO: IMplement a hot desking concept using the sqlite database and a set of extension, password and current extensions list
    // it will cause a forward according to the number dialed and the last known location assuming no logout has occured.
    //
    //
	public class Initializer
	{
		private static bool Initialized=false;
		private static Mutex mut = new Mutex(false);
		
		
		public static void Initialize()
		{
			if (!Initialized)
			{
				mut.WaitOne();
				if (!Initialized)
				{
					Initialized = true;
                    InitBaseData();
                    //Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users.User.Initialize();
                    ModuleController.Current.LoadModulesFromDirectory(Utility.LocateDirectory("LoadedModules"));
                    foreach (Type t in typeof(Initializer).Assembly.GetTypes())
                    {
                        if (t.FullName.StartsWith("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans") && t.IsSubclassOf(typeof(ADialPlan)))
                        {
                            ADialPlan adp = (ADialPlan)t.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                            CallControlManager.DeployDialPlan(adp);
                        }
                    }
				}
                Log.Trace("Releasing initializer mutex...");
				mut.ReleaseMutex();
			}
		}

        public static void DeInit(){
            foreach (Type t in Utility.LocateTypeInstances(typeof(IInitializer)))
            {
                IInitializer init = (IInitializer)t.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                init.DeInit();
            }
        }

        private static void InitBaseData()
        {
            Log.Trace("Begin setup of base user rights");
            UserRight.CreateRight(Constants.EXTENSION_CONFIG_RIGHT);
            UserRight.CreateRight(Constants.SYSTEM_CONTROL_RIGHT);
            UserRight.CreateRight(Constants.CDR_RIGHT);
            UserRight.CreateRight(Constants.FILE_ACCESS_RIGHT);
            UserRight.CreateRight(Constants.HOLD_MUSIC_ACCESS_RIGHT);
            UserRight.CreateRight(Constants.PIN_SECURITY_ACESS_RIGHT);
            UserRight.CreateRight(Constants.TRUNK_SETTINGS_ACCESS_RIGHT);
            UserRight.CreateRight(Constants.RELOAD_CONFIGURATIONS_RIGHT);
            UserRight.CreateRight(Constants.CHANGE_FREESWITCH_MODULE_SETTINGS_RIGHT);
            UserRight.CreateRight(Constants.DOMAIN_PROFILE_SETUP_RIGHT);
            Log.Trace("Finished creating base rights");
            Assembly ass = Assembly.GetExecutingAssembly();
            foreach (Type t in Utility.LocateTypeInstances(typeof(IInitializer)))
            {
                try
                {
                    Log.Trace("Initializing type of " + t.FullName);
                    IInitializer init = (IInitializer)t.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                    init.Init();
                }
                catch (Exception e)
                {
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                    Log.Error(e);
                }
            }
        }
	}
}
