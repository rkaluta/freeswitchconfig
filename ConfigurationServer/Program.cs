using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer;
using System.Reflection;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Security;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using System.Diagnostics;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.Site.Services;
using Org.Reddragonit.EmbeddedWebServer.Diagnostics;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;

namespace Org.Reddragonit.FreeSwitchConfig.ConfigurationServer
{
    public class Program
    {
        internal static ServerService _service;

        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "MainThread";
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            _service = new ServerService(args);
            _service.WaitForExit();
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Interupt called, shutting down Free Switch Config...");
            _service.Stop();
            e.Cancel = true;
        }
    }
}
