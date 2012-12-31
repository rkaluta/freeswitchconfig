using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Org.Reddragonit.EmbeddedWebServer;


namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events
{
    public static class EventController
    {
        private delegate void delHandleEvent(IEventHandler handle,IEvent evnt,ref Thread delThread);

        private const int _EVENT_HANDLING_TIMEOUT = 60000;

        private static List<IEventHandler> _handlers;
        private static object _lock=new object();
        private static MT19937 _rand;
        private static readonly delHandleEvent _eventHandle = new delHandleEvent(_HandleEvent);

        static EventController()
        {
            _rand = new MT19937(DateTime.Now.Ticks);
            _handlers = new List<IEventHandler>();
        }

        private static void _ProcessEvent(object obj)
        {
            IEvent evnt = (IEvent)obj;
            lock(_rand){
                Thread.CurrentThread.Name = "EventProcessor{" + evnt.Name + "}["+_rand.Next().ToString()+"]";
            }
            Thread.CurrentThread.IsBackground = true;
            IEventHandler[] handlers = new IEventHandler[0];
            Monitor.Enter(_lock);
            handlers = new IEventHandler[_handlers.Count];
            _handlers.CopyTo(handlers);
            Monitor.Exit(_lock);
            foreach (IEventHandler handler in handlers)
            {
                Log.Trace("Checking if " + handler.GetType().FullName + " handles event " + evnt.Name);
                if (handler.HandlesEvent(evnt))
                {
                    Thread delThread = null;
                    Log.Trace("Handling event " + evnt.Name + " with handler " + handler.GetType().FullName);
                    IAsyncResult res = _eventHandle.BeginInvoke(handler, evnt,ref delThread, null, null);
                    if (res.AsyncWaitHandle.WaitOne(_EVENT_HANDLING_TIMEOUT))
                        _eventHandle.EndInvoke(ref delThread,res);
                    else
                    {
                        Log.Error("Handle " + handler.GetType().FullName + " timed out handling event " + evnt.Name);
                        try
                        {
                            delThread.Abort();
                        }
                        catch (Exception e) { }
                        _eventHandle.EndInvoke(ref delThread,res);
                    }
                }
            }
        }

        private static void _HandleEvent(IEventHandler handler,IEvent evnt,ref Thread delThread)
        {
            delThread = Thread.CurrentThread;
            if (delThread.Name == null)
            {
                lock (_rand)
                {
                    Thread.CurrentThread.Name = "EventHandlerCall{" + handler.GetType().FullName + "}[" + _rand.Next().ToString() + "]";
                }
            }
            Thread.CurrentThread.IsBackground = true;
            handler.ProcessEvent(evnt);
        }

        public static void TriggerEvent(IEvent evnt)
        {
            new Thread(new ParameterizedThreadStart(_ProcessEvent)).Start(evnt);
        }

        public static void RegisterEventHandler(IEventHandler handler)
        {
            Monitor.Enter(_lock);
            if (!_handlers.Contains(handler))
                _handlers.Add(handler);
            Monitor.Exit(_lock);
        }

        public static void UnRegisterEventHandler(IEventHandler handler)
        {
            Monitor.Enter(_lock);
            _handlers.Remove(handler);
            Monitor.Exit(_lock);
        }
    }
}
