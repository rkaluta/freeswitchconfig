using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes.OperatingSystems;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces.OperatingSystems;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems
{
    public class OSClassProxy : RealProxy,IDisposable
    {
        #region structures
        private struct sPropertyOverride
        {
            private int _matchLevel;
            public int MatchLevel
            {
                get { return _matchLevel; }
            }

            private PropertyInfo _property;
            public PropertyInfo Property
            {
                get { return _property; }
            }

            private Type _overrideClass;
            public Type OverrideClass
            {
                get { return _overrideClass; }
            }

            public sPropertyOverride(PropertyInfo pi, Type overrideClass,int matchLevel)
            {
                _property = pi;
                _overrideClass = overrideClass;
                _matchLevel = matchLevel;
            }
        }

        private struct sMethodOverride
        {
            private int _matchLevel;
            public int MatchLevel
            {
                get { return _matchLevel; }
            }

            private MethodInfo _method;
            public MethodInfo Method
            {
                get { return _method; }
            }

            private Type _overrideClass;
            public Type OverrideClass
            {
                get { return _overrideClass; }
            }

            public bool IsMatchForMethod(MethodInfo mi)
            {
                ParameterInfo[] tpars = _method.GetParameters();
                ParameterInfo[] mpars = mi.GetParameters();
                if (tpars.Length != mpars.Length)
                    return false;
                else
                {
                    for (var x = 0; x < tpars.Length; x++)
                    {
                        if ((tpars[x].Name != mpars[x].Name) || (tpars[x].ParameterType.FullName != mpars[x].ParameterType.FullName))
                            return false;
                    }
                    return true;
                }
            }

            public sMethodOverride(MethodInfo mi, Type overrideClass,int matchLevel)
            {
                _method = mi;
                _overrideClass = overrideClass;
                _matchLevel = matchLevel;
            }
        }
        #endregion

        private object _originalObject;
        private Dictionary<Type, object> _producedClasses;
        private Dictionary<string, sPropertyOverride> _propertyOverrides;
        private Dictionary<string, List<sMethodOverride>> _methodOverrides;

        private OSClassProxy(object obj)
            : base(obj.GetType())
        {
            IOSDefinition os = Utility.OperatingSystem;
            if (os == null)
                Log.Trace("Running using undetected operating system...");
            else
                Log.Trace("Operating system detected for OS Class Proxy is " + os.OsName+" "+os.Version.ToString());
            _originalObject = obj;
            _producedClasses = new Dictionary<Type, object>();
            _propertyOverrides = new Dictionary<string, sPropertyOverride>();
            _methodOverrides = new Dictionary<string, List<sMethodOverride>>();
            AttachServer((MarshalByRefObject)obj);

            //load all overridable methods and properties
            List<string> overridableProperties = new List<string>();
            foreach (PropertyInfo pi in obj.GetType().GetProperties(BindingFlags.Public |      //Get public members
                                                                        BindingFlags.NonPublic |   //Get private/protected/internal members
                                                                        BindingFlags.Static |      //Get static members
                                                                        BindingFlags.Instance |    //Get instance members
                                                                        BindingFlags.DeclaredOnly))
            {
                object[] atts = pi.GetCustomAttributes(typeof(OperatingSystemOverridablePropertyAttribute), false);
                if (atts.Length > 0)
                    overridableProperties.Add(pi.Name);
            }
            List<string> overridableFunctions = new List<string>();
            foreach (MethodInfo mi in obj.GetType().GetMethods(BindingFlags.Public |      //Get public members
                                                                        BindingFlags.NonPublic |   //Get private/protected/internal members
                                                                        BindingFlags.Static |      //Get static members
                                                                        BindingFlags.Instance |    //Get instance members
                                                                        BindingFlags.DeclaredOnly))
            {
                object[] atts = mi.GetCustomAttributes(typeof(OperatingSystemOverridableFunctionAttribute), false);
                if (atts.Length == 1)
                    overridableFunctions.Add(mi.Name);
            }

            //Load all overrides
            foreach (Type t in Utility.LocateTypeInstances(typeof(IOperatingSystemOverrideContainer)))
            {
                Log.Trace("Attempting to load available overrides for " + _originalObject.GetType().FullName + " using " + t.FullName);
                foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Public |      //Get public members
                                                                        BindingFlags.NonPublic |   //Get private/protected/internal members
                                                                        BindingFlags.Static |      //Get static members
                                                                        BindingFlags.Instance |    //Get instance members
                                                                        BindingFlags.DeclaredOnly))
                {
                    Log.Trace("Checking property " + pi.Name + " to see if it is an override property.");
                    object[] atts = pi.GetCustomAttributes(typeof(OperatingSystemPropertyOverrideAttribute), false);
                    if (atts.Length == 1)
                    {
                        OperatingSystemPropertyOverrideAttribute ospoa = (OperatingSystemPropertyOverrideAttribute)atts[0];
                        Log.Trace("Checking property " + pi.Name + " to see if it overrides the type " + obj.GetType().FullName);
                        if (ospoa.ObjectType.FullName == obj.GetType().FullName)
                        {
                            int level = ((OperatingSystemPropertyOverrideAttribute)atts[0]).GetMatchLevelForOperatingSystem(os);
                            if (level != -1)
                            {
                                if (_propertyOverrides.ContainsKey(ospoa.PropertyName))
                                {
                                    if (_propertyOverrides[ospoa.PropertyName].MatchLevel < level)
                                    {
                                        _propertyOverrides.Remove(ospoa.PropertyName);
                                        Log.Trace("Overriding property " + ospoa.PropertyName + " for class " + ospoa.ObjectType.FullName + " with property " + pi.Name + " from class " + t.FullName);
                                        _propertyOverrides.Add(ospoa.PropertyName, new sPropertyOverride(pi, t, level));
                                    }
                                    else if (_propertyOverrides[ospoa.PropertyName].MatchLevel == level)
                                        throw new Exception("An attempt was made to override an operating system property with the same level by the class: " + t.FullName + " using property: " + pi.Name +
                                            " to override: " + ospoa.PropertyName + " of type: " + obj.GetType().FullName + ".  This property was already overriden by the class: " +
                                            _propertyOverrides[ospoa.PropertyName].OverrideClass.FullName + " using property: " + _propertyOverrides[ospoa.PropertyName].Property.Name + ".  This cannot happen, please review both classes and adjust their operating system match parameters accordingly.");
                                }
                                else if (!overridableProperties.Contains(ospoa.PropertyName))
                                    throw new Exception("An attempt was made to override a property of an operating system based class that is not overridable or does not exist.  The property in question is " + ospoa.PropertyName + " and the violating class is " + t.FullName + " using the property " + pi.Name);
                                else
                                {
                                    _propertyOverrides.Add(ospoa.PropertyName, new sPropertyOverride(pi, t, level));
                                    Log.Trace("Overriding property " + ospoa.PropertyName + " for class " + ospoa.ObjectType.FullName + " with property " + pi.Name + " from class " + t.FullName);
                                }
                            }
                        }
                        
                    }
                }
                foreach (MethodInfo mi in t.GetMethods(BindingFlags.Public |      //Get public members
                                                                        BindingFlags.NonPublic |   //Get private/protected/internal members
                                                                        BindingFlags.Static |      //Get static members
                                                                        BindingFlags.Instance |    //Get instance members
                                                                        BindingFlags.DeclaredOnly))
                {
                    Log.Trace("Checking method " + mi.Name + " to see if it is an override method.");
                    object[] atts = mi.GetCustomAttributes(typeof(OperatingSystemFunctionOverrideAttribute), false);
                    if (atts.Length == 1)
                    {
                        OperatingSystemFunctionOverrideAttribute osfoa = (OperatingSystemFunctionOverrideAttribute)atts[0];
                        Log.Trace("Checking method " + mi.Name + " to see if it overrides the type " + obj.GetType().FullName);
                        try
                        {
                            if (osfoa.ObjectType.FullName == obj.GetType().FullName)
                            {
                                Log.Trace("Validating override for method " + osfoa.MethodName + " from " + t.FullName + " in the type " + obj.GetType().FullName);
                                int level = ((OperatingSystemFunctionOverrideAttribute)atts[0]).GetMatchLevelForOperatingSystem(os);
                                if (level != -1)
                                {
                                    Log.Trace("Running validation for method " + osfoa.MethodName + " using the level " + level.ToString());
                                    if (_methodOverrides.ContainsKey(osfoa.MethodName))
                                    {
                                        List<sMethodOverride> tmp = _methodOverrides[osfoa.MethodName];
                                        for (int x = 0; x < tmp.Count; x++)
                                        {
                                            sMethodOverride smo = tmp[x];
                                            if (smo.IsMatchForMethod(mi))
                                            {
                                                if (smo.MatchLevel < level)
                                                {
                                                    tmp.RemoveAt(x);
                                                    Log.Trace("Overriding method " + osfoa.MethodName + " for class " + osfoa.ObjectType.FullName + " with property " + mi.Name + " from class " + t.FullName);
                                                    tmp.Add(new sMethodOverride(mi, t, level));
                                                }
                                                else if (smo.MatchLevel == level)
                                                {
                                                    throw new Exception("An attempt was made to override an operating system function with the same level by the class: " + t.FullName + " using method: " + mi.Name +
                                                        " to override: " + osfoa.MethodName + " of type: " + obj.GetType().FullName + ".  This function was already overriden by the class: " +
                                                        smo.OverrideClass.FullName + " using property: " + smo.Method.Name + ".  This cannot happen, please review both classes and adjust their operating system match parameters accordingly.");
                                                }
                                                break;
                                            }
                                        }
                                        _methodOverrides.Remove(osfoa.MethodName);
                                        _methodOverrides.Add(osfoa.MethodName, tmp);
                                    }
                                    else if (!overridableFunctions.Contains(osfoa.MethodName))
                                        throw new Exception("An attempt was made to override a method of an operating system based class that is not overridable or does not exist.  The method in question is " + osfoa.MethodName + " and the violating class is " + t.FullName + " using the property " + mi.Name);
                                    else
                                    {
                                        _methodOverrides.Add(osfoa.MethodName, new List<sMethodOverride>(new sMethodOverride[]{
                                    new sMethodOverride(mi,t,level)
                                }));
                                        Log.Trace("Overriding method " + osfoa.MethodName + " for class " + osfoa.ObjectType.FullName + " with property " + mi.Name + " from class " + t.FullName);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
            foreach (string str in _propertyOverrides.Keys)
            {
                Log.Trace("Property " + str + " in class " + obj.GetType().ToString() + " has been overridden");
            }
            foreach (string str in _methodOverrides.Keys)
            {
                Log.Trace("Method " + str + " in class " + obj.GetType().ToString() + " has been overridden");
            }
        }

        public static object Instance(Object obj)
        {
            return new OSClassProxy(obj).GetTransparentProxy();
        }

        protected static PropertyInfo GetMethodProperty(MethodInfo methodInfo, object owner, out bool IsGet)
        {
            foreach (PropertyInfo aProp in owner.GetType().GetProperties(BindingFlags.Public |      //Get public members
                                                                        BindingFlags.NonPublic |   //Get private/protected/internal members
                                                                        BindingFlags.Static |      //Get static members
                                                                        BindingFlags.Instance |    //Get instance members
                                                                        BindingFlags.DeclaredOnly))
            {
                MethodInfo mi = null;
                mi = aProp.GetGetMethod(true);
                if (mi != null && (mi.Name == methodInfo.Name))
                {
                    IsGet = true;
                    return aProp;
                }
                mi = aProp.GetSetMethod(true);
                if (mi != null && (mi.Name == methodInfo.Name))
                {
                    IsGet = false;
                    return aProp;
                }
            }
            IsGet = false;
            return null;
        }

        //called to override the hashcode and get it from the underlying object
        public override int GetHashCode()
        {
            return GetUnwrappedServer().GetHashCode();
        }

        //called to override the tostring and get it from the underlying object.
        public override string ToString()
        {
            return GetUnwrappedServer().ToString();
        }

        #region IDisposable Members

        public void Dispose()
        {
            DetachServer();
        }

        #endregion

        public override IMessage Invoke(IMessage msg)
        {
            MethodCallMessageWrapper mc = new MethodCallMessageWrapper((IMethodCallMessage)msg);
            MarshalByRefObject owner = GetUnwrappedServer();
            MethodInfo mi = (MethodInfo)mc.MethodBase;

            Log.Trace("OS Proxy class invoking method " + mi.Name + " in class " + _originalObject.GetType().FullName);

            object outVal = null;
            bool isGet;

            if (mi.Name == "GetType")
                outVal = _originalObject.GetType();
            else{
                PropertyInfo pi = GetMethodProperty(mi, _originalObject, out isGet);
                if (pi != null)
                {
                    if (pi.GetCustomAttributes(typeof(OperatingSystemOverridablePropertyAttribute), false).Length > 0)
                    {
                        Log.Trace("Detected overridable property of " + pi.Name + " in type " + _originalObject.GetType().ToString());
                        if (_propertyOverrides.ContainsKey(pi.Name))
                        {
                            Log.Trace("Overriding property call " + pi.Name + " for class " + owner.GetType().FullName + " using class " + _propertyOverrides[pi.Name].OverrideClass.FullName);
                            Monitor.Enter(_producedClasses);
                            if (!_producedClasses.ContainsKey(_propertyOverrides[pi.Name].OverrideClass))
                                _producedClasses.Add(_propertyOverrides[pi.Name].OverrideClass,
                                    _propertyOverrides[pi.Name].OverrideClass.GetConstructor(Type.EmptyTypes).Invoke(new object[0]));
                            owner = (MarshalByRefObject)_producedClasses[_propertyOverrides[pi.Name].OverrideClass];
                            Monitor.Exit(_producedClasses);
                            mi = (isGet ? _propertyOverrides[pi.Name].Property.GetGetMethod() : _propertyOverrides[pi.Name].Property.GetSetMethod());
                        }
                    }
                }
                else
                {
                    if (mi.GetCustomAttributes(typeof(OperatingSystemOverridableFunctionAttribute), false).Length > 0)
                    {
                        Log.Trace("Detected overridable method of " + mi.Name + " in type " + _originalObject.GetType().ToString());
                        if (_methodOverrides.ContainsKey(mi.Name))
                        {
                            foreach (sMethodOverride smo in _methodOverrides[mi.Name])
                            {
                                if (smo.IsMatchForMethod(mi))
                                {
                                    Log.Trace("Overriding method call " + mi.Name + " for class " + owner.GetType().FullName + " using class " + smo.OverrideClass.FullName);
                                    Monitor.Enter(_producedClasses);
                                    if (!_producedClasses.ContainsKey(smo.OverrideClass))
                                        _producedClasses.Add(smo.OverrideClass,
                                            smo.OverrideClass.GetConstructor(Type.EmptyTypes).Invoke(new object[0]));
                                    owner = (MarshalByRefObject)_producedClasses[smo.OverrideClass];
                                    Monitor.Exit(_producedClasses);
                                    mi = smo.Method;
                                }
                            }
                        }
                    }
                }
                outVal = mi.Invoke(owner, mc.Args);
            }
            return new ReturnMessage(outVal, mc.Args, mc.Args.Length, mc.LogicalCallContext, mc);
        }
    }
}
