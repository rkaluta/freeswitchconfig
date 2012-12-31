using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.HomePage
{
    public class DialPlanTester : IHomePageComponent
    {
        #region IHomePageComponent Members

        public bool IsValidForUser(User user)
        {
            return true;
        }

        public string Title
        {
            get { return "Test Dialed Number"; }
        }

        public string ComponentRenderCode
        {
            get {
                return Utility.ReadEmbeddedResource("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.HomePage.resources.DialPlanTester.js");
            }
        }

        public string[] CSSUrls
        {
            get { return null; }
        }

        public string[] JSUrls
        {
            get { return new string[] { "/EmbeddedJSGenerator.js?TYPE=FreeswitchConfig.Services.DialPlanTestService" }; }
        }

        #endregion
    }
}
