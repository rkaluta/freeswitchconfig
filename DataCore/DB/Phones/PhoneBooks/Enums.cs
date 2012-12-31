using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones.PhoneBooks
{
    public enum PhoneBookEntryType
    {
        GENERAL=5,
        FRIEND=4,
        COLLEAGUE=1,
        FAMILY=2,
        PERSONAL=3,
        VIP=0,
        BLACKLIST=7,
        OTHER=6
    }

    public enum PhoneBookSortTypes
    {
        FirstName = 0,
        LastName = 1,
        EntryType = 2
    }
}
