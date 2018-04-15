using EventMonitor.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventMonitor.Core.EventSource
{
    public class UnifiedEventSource
    {
        private IUnhandledExceptionNotifier unhandledExceptionNotifier;
        private List<Func<Event, Task>> subscribers = new List<Func<Event, Task>>();

        public UnifiedEventSource(IUnhandledExceptionNotifier unhandledExceptionNotifier)
        {
            this.unhandledExceptionNotifier = unhandledExceptionNotifier;
        }

        public Task PushAsync(Event @event)
        {
            var tasks = subscribers.Select(subscriber => DispatchEvent(@event, subscriber));
            return Task.WhenAll(tasks);
        }

        public void Push(Event @event)
        {
            PushAsync(@event).Wait();
        }

        public void Subscribe(Action<Event> callback)
        {
            subscribers.Add(@event =>
                Task.Run(() => callback(@event)));
        }

        public void SubscribeTask(Func<Event, Task> callback)
        {
            subscribers.Add(@event => callback(@event));
        }

        private async Task DispatchEvent(Event @event, Func<Event, Task> callback)
        {
            try {
                var task = callback(@event);
                await task;
            }
            catch (Exception ex)
            {
                unhandledExceptionNotifier.Notify(ex);
            }
        }

    }

}
