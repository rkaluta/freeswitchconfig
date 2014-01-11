using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Procurios.Public;
using Org.Reddragonit.EmbeddedWebServer.Components;
using System.Collections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class MappedRequest : IHttpRequest
    {
        private HttpRequest _request;

        public MappedRequest(HttpRequest request)
        {
            _request = request;
        }

        #region IHttpRequest Members

        public Uri URL
        {
            get { return _request.URL; }
        }

        public string Method
        {
            get { return _request.Method; }
        }

        public string ParameterContent
        {
            get { return JSON.JsonEncode(_request.JSONParameter); }
        }

        public void SetResponseContentType(string type)
        {
            _request.ResponseHeaders.ContentType = type;
        }

        public void WriteContent(string content)
        {
            _request.ResponseWriter.Write(content);
        }

        public void SendResponse()
        {
            _request.SendResponse();
        }

        public void SetResponseStatus(int statusNumber)
        {
            _request.ResponseStatus = (HttpStatusCodes)statusNumber;
        }

        public string AcceptLanguageHeaderValue {
            get { return _request.Headers["Accept-Language"]; }
        }

        public Hashtable AdditionalBackboneVariables {
            get
            {
                Hashtable ret = new Hashtable();
                ret.Add("HasConfigurationChangesToMake", ConfigurationController.HasChangesToMake);
                return ret;
            }
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
