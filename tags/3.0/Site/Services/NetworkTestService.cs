using System;
using System.Collections.Generic;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;

/// <summary>
/// Summary description for NetworkTestService
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.Services
{
    [EmbeddedServiceNamespace("FreeswitchConfig.Services")]
    public class NetworkTestService : EmbeddedService
    {
        public NetworkTestService()
        {
        }

        [WebMethod(true)]
        public List<string> AvailableInterfaces()
        {
            List<string> ret = new List<string>();
            foreach (string str in NetworkSettings.Current.InterfaceNames)
            {
                if (!NetworkSettings.Current[str].IsBondSlave)
                    ret.Add(str);
            }
            return ret;
        }

        [WebMethod(true)]
        public string GetPacketLoss(string interfaceName, string destination, int count)
        {
            if (!NetworkSettings.Current[interfaceName].Live)
                return "Unable to test ping since interface is not connected.";
            return NetworkSettings.Current.GetPacketLoss(interfaceName, destination, count)+"% packet loss to destination.";
        }
    }
}