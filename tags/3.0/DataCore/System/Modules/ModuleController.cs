using Org.Reddragonit.Dbpro.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;



namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Modules
{
    public class ModuleController
    {
        public delegate void ModuleCallReturnFunction(object result);
        private delegate void InvokeModuleCallFunction(MethodInfo method, IModule module, object[] parameters, ModuleCallReturnFunction ReturnFunction);

        private static ModuleController _current = null;
        private static Mutex _mut = new Mutex(false);
        private static List<IModule> _modules;
        private static List<string> _runningModules;

        public static List<IModule> CurrentModules
        {
            get
            {
                return _modules;
            }
        }

        internal static List<IModule> CurrentActiveModules
        {
            get
            {
                List<IModule> ret = new List<IModule>();
                if (_modules != null)
                {
                    if (_runningModules != null)
                    {
                        foreach (IModule mod in _modules)
                        {
                            if (_runningModules.Contains(mod.ModuleName))
                                ret.Add(mod);
                        }
                    }
                }
                return ret;
            }
        }

        public void LoadModulesFromDirectory(DirectoryInfo di)
        {
            if (di == null)
            {
                Log.Trace("Attempting to load modules from directory that is null, therefore exiting.");
                return;
            }
            Log.Trace("Attempting to load modules from directory " + di.FullName);
            _mut.WaitOne();
            if (_modules == null)
                _modules = new List<IModule>();
            bool changed = false;
            foreach (FileInfo fi in di.GetFiles("*.dll"))
            {
                Log.Trace("Located dll " + fi.FullName + " to be loaded for a site module.");
                Assembly ass = Assembly.LoadFile(fi.FullName);
                if (_modules != null)
                {
                    foreach (Type t in Utility.LocateTypeInstances(typeof(IModule), ass))
                    {
                        Log.Trace("Adding module " + t.FullName + " into current module list from assembly " + ass.GetName());
                        IModule mod = (IModule)t.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                        if (!_modules.Contains(mod))
                        {
                            changed = true;
                            _modules.Add(mod);
                            if (!Settings.Current.IsModuleEnabledSet(_modules[_modules.Count - 1].ModuleName) && _modules[_modules.Count - 1].DefaultEnabled)
                                Settings.Current.EnableModule(_modules[_modules.Count - 1].ModuleName, false);
                        }
                    }
                }
            }
            if (changed)
            {
                Log.Trace("Located new modules in directory " + di.FullName + ", reloading the database structure if necessary.");
                //ConnectionPoolManager.ReloadClasses();
            }
            _mut.ReleaseMutex();
        }

        private static void InitializeModule(IModule module)
        {
            try
            {
                Log.Trace("Initializing module " + module.ModuleName);
                module.Init();
                Log.Trace("Module " + module.ModuleName + " is now running.");
                _runningModules.Add(module.ModuleName);
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(new ModuleInitializationFailure(e, module.GetType())));
                throw new ModuleInitializationFailure(e, module.GetType());
            }
        }

        public bool IsModuleEnabled(string name)
        {
            if (_runningModules == null)
                return false;
            return _runningModules.Contains(name);
        }

        public static ModuleController Current
        {
            get
            {
                _mut.WaitOne();
                try
                {
                    if (_current == null)
                        _current = new ModuleController();
                }
                catch (Exception e)
                {
                    _mut.ReleaseMutex();
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                    throw e;
                }
                _mut.ReleaseMutex();
                return _current;
            }
        }

        public void Shutdown()
        {
            _mut.WaitOne();
            foreach (IModule mod in _modules)
            {
                if (_runningModules.Contains(mod.ModuleName))
                {
                    mod.DeInit();
                    _runningModules.Remove(mod.ModuleName);
                }
            }
            _mut.ReleaseMutex();
        }

        public void EnableModule(string name)
        {
            _mut.WaitOne();
            Log.Trace("Called enable module " + name);
            if (!_runningModules.Contains(name))
            {
                Log.Trace("Module " + name + " is not running, attempting to start");
                foreach (IModule module in _modules)
                {
                    if (module.ModuleName == name)
                    {
                        try
                        {
                            Log.Trace("Calling init for module " + name);
                            module.Init();
                            Log.Trace("Adding module "+name+" into running modules list");
                            _runningModules.Add(module.ModuleName);
                            EventController.TriggerEvent(new ModuleEnabledEvent(module));
                            Settings.Current.EnableModule(name, false);
                        }
                        catch (Exception e)
                        {
                            _mut.ReleaseMutex();
                            EventController.TriggerEvent(new ErrorOccuredEvent(e));
                            Log.Error(e);
                            throw new ModuleInitializationFailure(e, module.GetType());
                        }
                    }
                }
            }
            _mut.ReleaseMutex();
        }

        public void DisableModule(string name)
        {
            _mut.WaitOne();
            Log.Trace("Called disable module " + name);
            if (_runningModules.Contains(name))
            {
                Log.Trace("Module " + name + " is currently running, attempting to disable");
                foreach (IModule module in _modules)
                {
                    if (module.ModuleName == name)
                    {
                        try
                        {
                            Log.Trace("Calling DeInit for module " + name);
                            module.DeInit();
                            Log.Trace("Removing module " + name + " from running modules list");
                            _runningModules.Remove(module.ModuleName);
                            EventController.TriggerEvent(new ModuleDisabledEvent(module));
                            Settings.Current.DisableModule(name);
                        }
                        catch (Exception e)
                        {
                            _mut.ReleaseMutex();
                            EventController.TriggerEvent(new ErrorOccuredEvent(new ModuleDeInitializationFailure(e,module.GetType())));
                            Log.Error(e);
                            throw new ModuleDeInitializationFailure(e, module.GetType());
                        }
                    }
                }
            }
            _mut.ReleaseMutex();
        }

        private ModuleController()
        {
            if (_modules == null)
            {
                _modules = new List<IModule>();
                _runningModules = new List<string>();
                Log.Trace("Locating all modules currently loaded into the app domain");
                foreach (Type t in Utility.LocateTypeInstances(typeof(IModule)))
                {
                    IModule tmod = (IModule)t.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                    if (!_modules.Contains(tmod))
                    {
                        _modules.Add(tmod);
                        if (!Settings.Current.IsModuleEnabledSet(_modules[_modules.Count - 1].ModuleName) && _modules[_modules.Count - 1].DefaultEnabled)
                            Settings.Current.EnableModule(_modules[_modules.Count - 1].ModuleName, false);
                    }
                }
                foreach (IModule mod in _modules)
                {
                    if (Settings.Current.IsModuleEnabled(mod.ModuleName))
                    {
                        try
                        {
                            Log.Trace("Calling init for module " + mod.ModuleName);
                            mod.Init();
                            Log.Trace("Adding module " + mod.ModuleName + " into running modules list");
                            _runningModules.Add(mod.ModuleName);
                            EventController.TriggerEvent(new ModuleEnabledEvent(mod));
                        }
                        catch (Exception e)
                        {
                            _mut.ReleaseMutex();
                            EventController.TriggerEvent(new ErrorOccuredEvent(e));
                            Log.Error(e);
                            throw new ModuleInitializationFailure(e, mod.GetType());
                        }
                    }
                }
            }
        }

        #region AsyncInvokes
        public void InvokeModuleMethodAsync(string ModuleName, string FunctionName)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName+" with no parameters synchronously");
            InvokeModuleMethodAsync(ModuleName, FunctionName, null,null,true);
        }

        public void InvokeModuleMethodAsync(string ModuleName, string FunctionName, NameValuePair[] variables)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName + " parameter count: "+variables.Length.ToString()+" synchronously");
            InvokeModuleMethodAsync(ModuleName, FunctionName, variables, null,true);
        }

        public void InvokeModuleMethodAsync(string ModuleName, string FunctionName, ModuleCallReturnFunction ReturnFunction)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName + " with no parameters asynchronously");
            InvokeModuleMethodAsync(ModuleName, FunctionName,null, ReturnFunction,true);
        }

        public void InvokeModuleMethodAsync(string ModuleName, string FunctionName, NameValuePair[] variables, ModuleCallReturnFunction ReturnFunction)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName + " parameter count: " + variables.Length.ToString() + " asynchronously width return function "+ReturnFunction.ToString());
            InvokeModuleMethodAsync(ModuleName, FunctionName, variables, ReturnFunction,true);
        }

        public void InvokeModuleMethodAsync(string ModuleName, string FunctionName,bool throwOnError)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName + " with no parameters asynchronously");
            InvokeModuleMethodAsync(ModuleName, FunctionName,null, null,throwOnError);
        }

        public void InvokeModuleMethodAsync(string ModuleName, string FunctionName, ModuleCallReturnFunction ReturnFunction,bool throwOnError)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName + " with no parameters asynchronously");
            InvokeModuleMethodAsync(ModuleName, FunctionName, null, ReturnFunction, throwOnError);
        }

        public void InvokeModuleMethodAsync(string ModuleName, string FunctionName, NameValuePair[] variables, bool throwOnError)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName + " parameter count: " + variables.Length.ToString() + " asynchronously");
            InvokeModuleMethodAsync(ModuleName, FunctionName, variables, null,throwOnError);
        }

        public void InvokeModuleMethodAsync(string ModuleName, string FunctionName, NameValuePair[] variables, ModuleCallReturnFunction ReturnFunction, bool throwOnError)
        {
            object tmp;
            _InvokeModuleMethod(ModuleName, FunctionName, variables, ReturnFunction, true, throwOnError, out tmp);
        }
        #endregion

        #region WithReturn
        public object InvokeModuleMethod(string ModuleName, string FunctionName)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName + " with no parameters synchronously");
            return InvokeModuleMethod(ModuleName, FunctionName, null, true);
        }

        public object InvokeModuleMethod(string ModuleName, string FunctionName, NameValuePair[] variables)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName + " parameter count: " + variables.Length.ToString() + " synchronously");
            return InvokeModuleMethod(ModuleName, FunctionName, variables, true);
        }

        public object InvokeModuleMethod(string ModuleName, string FunctionName, bool throwOnError)
        {
            Log.Trace("Calling invoke module method.  Module: " + ModuleName + ", function: " + FunctionName + " with no parameters hronously");
            return InvokeModuleMethod(ModuleName, FunctionName, null, throwOnError);
        }

        public object InvokeModuleMethod(string ModuleName, string FunctionName, NameValuePair[] variables, bool throwOnError)
        {
            object ret;
            _InvokeModuleMethod(ModuleName, FunctionName, variables, null, false, throwOnError, out ret);
            return ret;
        }
        #endregion
        private void _InvokeModuleMethod(string ModuleName,string FunctionName,NameValuePair[] variables,ModuleCallReturnFunction ReturnFunction,bool async,bool throwOnError,out object returnValue)
        {
            returnValue = null;
            Log.Trace("Attempting to invoke method " + FunctionName + " in Module " + ModuleName);
            bool found = false;
            foreach (IModule module in _modules)
            {
                if (module.ModuleName == ModuleName)
                {
                    found = true;
                    Log.Trace("Located module " + ModuleName+" in attempt to invoke method");
                    MethodInfo method = null;
                    object[] parameters = new object[0];
                    if (variables == null)
                    {
                        Log.Trace("Attempting to locate method " + FunctionName + " in module " + ModuleName + " without any parameters");
                        method = module.GetType().GetMethod(FunctionName, Type.EmptyTypes);
                    }
                    else
                    {
                        foreach (MethodInfo mi in module.GetType().GetMethods()){
                            if (mi.Name == FunctionName)
                            {
                                bool contains = true;
                                Log.Trace("Located method " + mi.ToString() + " in module " + ModuleName + ", checking to see if variables were passed.");
                                foreach (ParameterInfo pi in mi.GetParameters())
                                {
                                    bool found_var = false;
                                    foreach (NameValuePair nvp in variables)
                                    {
                                        if (nvp.Name == pi.Name)
                                        {
                                            found_var = true;
                                            break;
                                        }
                                    }
                                    if (!found_var)
                                    {
                                        contains = false;
                                        break;
                                    }
                                }
                                if ((contains)&&(mi.GetParameters().Length==variables.Length))
                                {
                                    Log.Trace("Located proper method in module invoke using parameters supplied");
                                    method = mi;
                                    Log.Trace("Constructing parameter list for module " + ModuleName + " method " + FunctionName);
                                    parameters = new object[variables.Length];
                                    int x = 0;
                                    foreach (ParameterInfo p in mi.GetParameters())
                                    {
                                        foreach (NameValuePair nvp in variables)
                                        {
                                            if (nvp.Name == p.Name)
                                            {
                                                parameters[x] = nvp.Value;
                                                break;
                                            }
                                        }
                                        x++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (method == null)
                    {
                        Log.Error(new ModuleFunctionNotFoundException(ModuleName, FunctionName));
                        if (throwOnError)
                            throw new ModuleFunctionNotFoundException(ModuleName, FunctionName);
                        else
                            return;
                    }
                    else
                    {
                        try
                        {
                            if (!async && ReturnFunction == null)
                            {
                                if (method.ReturnType.ToString().ToLower() == "void")
                                {
                                    Log.Trace("Invoking module method with no return");
                                    method.Invoke(module, parameters);
                                }
                                else
                                {
                                    Log.Trace("Invoking module method and collecting return value");
                                    returnValue = method.Invoke(module, parameters);
                                }
                            }
                            else
                            {
                                InvokeModuleCallFunction asyncCall = new InvokeModuleCallFunction(AsyncInvokeMethod);
                                if (async)
                                {
                                    Log.Trace("Attempting to invoke " + FunctionName + " in module " + ModuleName + " asynchronously.");
                                    asyncCall.BeginInvoke(method, module, parameters, ReturnFunction, new AsyncCallback(AsyncInvokeMethodComplete), null);
                                }
                                else
                                {
                                    Log.Trace("Attempting to invoke " + FunctionName + " in module " + ModuleName + " synchronously.");
                                    asyncCall.Invoke(method, module, parameters, ReturnFunction);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                            if (throwOnError)
                                throw new ModuleFunctionCallException(e, ModuleName, FunctionName);
                            else
                                return;
                        }
                    }
                }
            }
            if (!found)
            {
                Log.Error(new ModuleNotFoundException(ModuleName));
                if (throwOnError)
                    throw new ModuleNotFoundException(ModuleName);
            }
        }

        private void AsyncInvokeMethod(MethodInfo method,IModule module,object[] parameters, ModuleCallReturnFunction ReturnFunction)
        {
            if (ReturnFunction == null)
            {
                Log.Trace("Calling async invoke method in module with no return call");
                ReturnFunction = new ModuleCallReturnFunction(NoReturnCall);
            }
            object ret = null;
            if (method.ReturnType.ToString().ToLower() == "void")
            {
                Log.Trace("Invoking module method with no return");
                method.Invoke(module, parameters);
            }
            else
            {
                Log.Trace("Invoking module method and collecting return value");
                ret = method.Invoke(module, parameters);
            }
            Log.Trace("Invoking the return function after the async call");
            ReturnFunction.Invoke(ret);
        }

        private void AsyncInvokeMethodComplete(IAsyncResult ar)
        {
            ((InvokeModuleCallFunction)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
        }

        private void NoReturnCall(object result) { 
        }

    }
}
