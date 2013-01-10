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
    }
}
