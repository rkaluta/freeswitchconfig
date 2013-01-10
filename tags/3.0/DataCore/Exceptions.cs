/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 08/11/2009
 * Time: 3:00 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore
{
    public class SuperUserRequiredException : Exception
    {
        public SuperUserRequiredException() :
            base("You must activate super user mode in order to perform the requested action, or restart the control software to perform the requested action as root.") { }
    }

    public class ModuleMissingRequiredModuleException : Exception
    {
        public ModuleMissingRequiredModuleException(string moduleName, string missingName)
            : base("Unable to activate the module " + moduleName + " because the module " + missingName + " is inactive or missing")
        { }
    }

    public class ModuleNotFoundException : Exception
    {
        public ModuleNotFoundException(string moduleName)
            : base("Unable to locate module " + moduleName)
        { }
    }

    public class ModuleFunctionNotFoundException : Exception{
        public ModuleFunctionNotFoundException(string moduleName, string functionName) 
            : base("Unable to locate function "+functionName+" in module "+moduleName)
        {}
    }

    public class ModuleFunctionCallException : Exception
    {
        public ModuleFunctionCallException(Exception e, string moduleName, string functionName) :
            base("Invoking of the function " + functionName + " in Module " + moduleName + " has caused an exception.", e)
        { }
    }

    public class ModuleInitializationFailure : Exception
    {
        public ModuleInitializationFailure(Exception e, Type clazz)
            : base("The module " + clazz.FullName + " failed to initialize due to an error.", e)
        {}
    }

    public class ModuleDeInitializationFailure : Exception
    {
        public ModuleDeInitializationFailure(Exception e, Type clazz)
            : base("The module " + clazz.FullName + " failed to shutdown properly due to an error.", e)
        { }
    }

	public class InvalidExtension : Exception{
		public InvalidExtension() : base("The extension number entered was invalid.")
		{}
	}

    public class FunctionNotFoundForParametersException : Exception
    {

        public static void CreateException(Type clazz, string functionName, List<string> parameters)
        {
            string pars = "";
            if (parameters != null)
            {
                foreach (string par in parameters)
                    pars += par + ",";
                pars = pars.Substring(0, pars.Length - 1);
            }
            if (pars.Length == 0)
                pars = "none";
            throw new FunctionNotFoundForParametersException(clazz, functionName, pars);
        }

        private FunctionNotFoundForParametersException(Type clazz, string functionName, string pars) :
            base("Unable to locate function : " + functionName + " in class : " + clazz.FullName + " with parameters :(" + pars + ")")
        {
        }
    }

    public class NotLoggedInException : Exception
    {
        public NotLoggedInException() :
            base("A User needs to be logged in to access the area.")
        { }
    }

    public class InvalidServiceAccess : Exception
    {
        public InvalidServiceAccess(string url, string functionName, string requiredRight) :
            base("Unable to access web service without required right.  Service: " + url + " Function: " + functionName + " Required Right: " + requiredRight)
        { }
    }

    public class FunctionNotFoundException : Exception
    {
        public FunctionNotFoundException(Type clazz, string functionName) :
            base("Unable to locate function : " + functionName + " in class : " + clazz.FullName)
        { }
    }

    public class InvalidSkinFileException : Exception
    {
        public InvalidSkinFileException(string path) :
            base("An invalid skin file was located at " + path + ".  Correct before attempting to load.")
        { }
    }

    public class InvalidRealmException : Exception
    {
        public InvalidRealmException(string realm):
            base("The url/ip for the outgoing SIP trunk server is invalid ("+realm+")")
        { }
    }

    public class InvalidProxyException : Exception
    {
        public InvalidProxyException(string proxy) :
            base("The url/ip for the proxy server is invalid (" + proxy + ")")
        { }
    }
}
