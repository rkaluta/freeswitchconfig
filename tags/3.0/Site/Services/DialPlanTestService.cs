using System;
using System.Collections.Generic;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using System.Data;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;

/// <summary>
/// Summary description for DialPlanTestService
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.Services
{
    [EmbeddedServiceNamespace("FreeswitchConfig.Services")]
    public class DialPlanTestService : EmbeddedService
    {
        public DialPlanTestService()
        { }

        [WebMethod(true)]
        public List<string> GetAvailableExtensions()
        {
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones",
                "SELECT ext.Number FROM Extension ext WHERE ext.Domain = @domain");
            cq.Execute(new IDbDataParameter[]{cq.CreateParameter("@domain",Domain.Current)});
            List<string> ret = new List<string>();
            while (cq.Read())
            {
                ret.Add(cq[0].ToString());
            }
            return ret;
        }

        [WebMethod(true)]
        public string TestDialedNumber(string extensionNumber, string dialedNumber, string pin,DateTime? currentTime)
        {
            return CallControlManager.TestDialNumber(Extension.Load(extensionNumber, Domain.Current), dialedNumber, (pin == "" ? null : pin), currentTime).ToString();
        }
    }
}