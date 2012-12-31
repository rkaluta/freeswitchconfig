using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces
{
    public interface IHomePageComponent
    {
        bool IsValidForUser(User user);
        string Title { get; }
        string ComponentRenderCode { get; }
        string[] CSSUrls { get; }
        string[] JSUrls { get; }
    }
}
