using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using System.Collections;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class VirtualMappedRequest : IHttpRequest
    {
        private Uri _url;
        private string _langHeader;
        private StringBuilder _sb;

        public override string ToString()
        {
            return _sb.ToString();
        }

        public VirtualMappedRequest(Uri url, string langHeader)
        {
            _url = url;
            _langHeader = langHeader;
            _sb = new StringBuilder();
        }

        #region IHttpRequest Members

        public Uri URL
        {
            get { return _url; }
        }

        public string Method
        {
            get { return "GET"; }
        }

        public string ParameterContent
        {
            get { return null; }
        }

        public void SetResponseContentType(string type)
        {
        }

        public void WriteContent(string content)
        {
            _sb.Append(content);
        }

        public void SendResponse()
        {
        }

        public void SetResponseStatus(int statusNumber)
        {
        }

        public string AcceptLanguageHeaderValue
        {
            get { return _langHeader; }
        }

        public System.Collections.Hashtable AdditionalBackboneVariables
        {
            get { return null; }
        }

        #endregion

        #region IHttpRequest Members

        private bool _ReturnOk(out int HttpStatusCode, out string HttpStatusMessage)
        {
            HttpStatusCode = 0;
            HttpStatusMessage = null;
            return true;
        }


        public bool IsLoadAllowed(Type model, string id, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        public bool IsLoadAllAllowed(Type model, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        public bool IsListAllowed(Type model, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        public bool IsSelectAllowed(Type model, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        public bool IsUpdateAllowed(IModel model, System.Collections.Hashtable parameters, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        public bool IsSaveAllowed(Type model, System.Collections.Hashtable parameters, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        public bool IsDeleteAllowed(Type model, string id, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        public bool IsJsURLAllowed(string url, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        public bool IsStaticExposedMethodAllowed(Type type, string methodName, Hashtable parameters, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        public bool IsExposedMethodAllowed(IModel model, string methodName, Hashtable parameters, out int HttpStatusCode, out string HttpStatusMessage)
        {
            return _ReturnOk(out HttpStatusCode, out HttpStatusMessage);
        }

        #endregion
    }
}
