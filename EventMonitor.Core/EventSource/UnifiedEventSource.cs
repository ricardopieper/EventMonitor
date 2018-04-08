using EventMonitor.Core.Events;
using System;
using System.Collections.Generic;

namespace EventMonitor.Core.EventSource
{
    public class UnifiedEventSource
    {
        private IUnhandledExceptionNotifier unhandledExceptionNotifier;
        private List<Action<Event>> subscribers = new List<Action<Event>>();

        public UnifiedEventSource(IUnhandledExceptionNotifier unhandledExceptionNotifier)
        {
            this.unhandledExceptionNotifier = unhandledExceptionNotifier;
        }

        public void Push(Event @event)
        {
            subscribers.ForEach(x =>
            {
                try { x(@event); }
                catch (Exception ex)
                {
                    unhandledExceptionNotifier.Notify(ex);
                }
            }
          );
        }

        public void Subscribe(Action<Event> callback)
        {
            subscribers.Add(callback);
        }
    }
}
