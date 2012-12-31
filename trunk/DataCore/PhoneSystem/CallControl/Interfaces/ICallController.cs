using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces
{
    public interface ICallController
    {
        void Start(ADialPlan[] plans);
        void Stop();
        void DialplanUpdated(ADialPlan plan);
        void DialplanDeployed(ADialPlan plan, int index);
        void DialplanUnDeployed(ADialPlan plan);
        string Name { get; }
        string Description { get; }
        XmlContextFile GenerateContextFile(sDeployedContext context);
    }
}
