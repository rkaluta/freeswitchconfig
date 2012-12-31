/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 25/01/2009
 * Time: 12:06 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Org.Reddragonit.FreeSwitchConfig.Site
{
	public class FunctionNotFoundException : Exception{
		public FunctionNotFoundException(Type clazz,string functionName) :
			base("Unable to locate function : "+functionName+" in class : "+clazz.FullName)
		{}
	}
	
	public class EmbeddedCssLocationFailure : Exception{
		public EmbeddedCssLocationFailure(string resourceName) : 
			base("Unable to locate embedded css file "+resourceName){
		}
	}

    public class InvalidFreeSwitchModuleLoadDirectoryException : Exception
    {
        public InvalidFreeSwitchModuleLoadDirectoryException() :
            base("An invalid directory was located in the BaseModules file that contains base free switch module information to load.")
        {}

    }
}
