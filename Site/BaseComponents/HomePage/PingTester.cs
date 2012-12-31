using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.HomePage
{
    public class PingTester : IHomePageComponent
    {
        #region IHomePageComponent Members

        public bool IsValidForUser(User user)
        {
            return true;
        }

        public string Title
        {
            get { return "Test Packet Loss"; }
        }

        public string ComponentRenderCode
        {
            get { return Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.HomePage.resources.PingTester.js"); }
        }

        public string[] CSSUrls
        {
            get { return null; }
        }

        public string[] JSUrls
        {
            get { return new string[] { "/EmbeddedJSGenerator.js?TYPE=FreeswitchConfig.Services.NetworkTestService" }; }
        }

        #endregion
    }
}