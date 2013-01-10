/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 09/11/2009
 * Time: 8:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using System.Collections.Generic;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security.Firewall;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces
{
	/// <summary>
	/// Description of IModule.
	/// </summary>
	public interface IModule
	{
		string ModuleName{get;}
        string Description{get;}
        List<sEmbeddedFile> EmbeddedFiles { get; }
        List<Type> EmbeddedServices{get;}
		void Init();
		void DeInit();
        ExposedModuleFunction[] Functions { get; }
        SubMenuItem[] MenuItemsForParent(SubMenuItemTypes type);
        bool DefaultEnabled { get; }
        List<FirewallRule> FirewallRules { get; }
	}
}
