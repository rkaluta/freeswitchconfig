using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using System.Threading;

using Org.Reddragonit.EmbeddedWebServer.Components.Message;
using Org.Reddragonit.EmbeddedWebServer.Components;


namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System
{
    public class EmbeddedHandlerFactory : IRequestHandler
    {

        public const string BASE_PATH = "/custom_handlers/";

        public static void RegisterHandler(string path, IEmbeddedHandler handler)
        {
            Log.Trace("Registering Embedded Handler at path " + path);
            path = path.TrimStart('/');
            Monitor.Enter(_lock);
            if (_handlers == null)
                _handlers = new Dictionary<string, IEmbeddedHandler>();
            if (_handlers.ContainsKey(path))
                _handlers.Remove(path);
            _handlers.Add(path, handler);
            Monitor.Exit(_lock);
        }

        public static void DeregisterHandler(string path)
        {
            if (path != null)
            {
                Log.Trace("Deregistering Embedded Handler at path " + path);
                Monitor.Enter(_lock);
                if (_handlers != null)
                {
                    if (_handlers.ContainsKey(path))
                        _handlers.Remove(path);
                }
                Monitor.Exit(_lock);
            }
        }

        #region IRequestHandler Members

        private const string CACHE_ID = "LocatedEmbeddedHandler";

        private static object _lock = new object();
        private static Dictionary<string, IEmbeddedHandler> _handlers;

        bool IRequestHandler.CanProcessRequest(HttpRequest request, Site site)
        {
            if (request.URL.AbsolutePath.StartsWith(BASE_PATH))
            {
                IEmbeddedHandler hand = null;
                Dictionary<string, IEmbeddedHandler> handlers = null;
                Monitor.Enter(_lock);
                handlers = _handlers;
                Monitor.Exit(_lock);
                if (handlers != null)
                {
                    if (handlers.ContainsKey(request.URL.AbsolutePath.Substring(BASE_PATH.Length)))
                        hand = handlers[request.URL.AbsolutePath.Substring(BASE_PATH.Length)];
                }
                if (hand == null)
                {
                    if (handlers != null)
                    {
                        foreach (string str in handlers.Keys)
                        {
                            if (str.EndsWith("*"))
                            {
                                if (request.URL.AbsolutePath.Substring(BASE_PATH.Length).StartsWith(str.Substring(0, str.Length - 1)))
                                {
                                    hand = handlers[str];
                                    break;
                                }
                            }
                        }
                    }
                }
                request[CACHE_ID] = hand;
                return hand != null;
            }
            return false;
        }

        void IRequestHandler.DeInit()
        {
        }

        void IRequestHandler.Init()
        {
        }

        bool IRequestHandler.IsReusable
        {
            get { return true; }
        }

        void IRequestHandler.ProcessRequest(HttpRequest request, Site site)
        {
            Log.Trace("Attempting to run embedded Handler for " + request.URL.AbsolutePath + " by searching for a handler of " + request.URL.AbsolutePath.Substring(BASE_PATH.Length));
            IEmbeddedHandler hand = (IEmbeddedHandler)request[CACHE_ID];
            if (hand != null)
            {
                Log.Trace("Found embedded handler of type " + hand.GetType().FullName + " for path " + request.URL.AbsolutePath.Substring(BASE_PATH.Length));
                hand.HandleRequest(request,site);
            }
            else
            {
                request.ResponseStatus = HttpStatusCodes.Not_Found;
                request.ResponseWriter.Write("Embedded Handler not found.");
            }
        }

        bool IRequestHandler.RequiresSessionForRequest(HttpRequest request, Site site)
        {
            return ((IEmbeddedHandler)request[CACHE_ID]).RequiresSessionForRequest(request, site);
        }

        #endregion

        public static bool HandlerExistsAtPath(string p)
        {
            IEmbeddedHandler hand = null;
            Dictionary<string, IEmbeddedHandler> handlers = null;
            Monitor.Enter(_lock);
            handlers = _handlers;
            Monitor.Exit(_lock);
            if (handlers != null)
            {
                if (handlers.ContainsKey(p))
                    hand = handlers[p];
            }
            if (hand == null)
            {
                if (handlers != null)
                {
                    foreach (string str in handlers.Keys)
                    {
                        if (str.EndsWith("*"))
                        {
                            if (p.StartsWith(str.Substring(0, str.Length - 1)))
                            {
                                hand = handlers[str];
                                break;
                            }
                        }
                    }
                }
            }
            return hand != null;
        }
    }
}
