using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces
{
    public class ExposedModuleFunction 
    {
        private string _description;
        public string Description
        {
            get { return _description; }
        }

        private string[] _allowedNullParameters;
        public string[] AllowedNullParameters
        {
            get { return _allowedNullParameters; }
        }

        private NameValuePair[] _parameterDescriptions;
        public NameValuePair[] ParameterDescriptions
        {
            get { return _parameterDescriptions; }
        }

        private string _function;
        public string Function
        {
            get { return _function; }
        }

        public ExposedModuleFunction(string function,string description, string[] allowedNullParameters, NameValuePair[] parameterDescriptions)
        {
            _function = function;
            _description=description;
            _allowedNullParameters = allowedNullParameters;
            _parameterDescriptions = parameterDescriptions;
        }
    }
}
