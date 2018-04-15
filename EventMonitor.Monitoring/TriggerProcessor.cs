using System;
using System.Collections.Generic;
using System.Text;
using EventMonitor.Core.Events;
using EventMonitor.Core.EventSource;
using EventMonitor.Monitoring.Triggers.Expressions;
using System.Linq;
using System.Collections.Immutable;

namespace EventMonitor.Monitoring
{
    public class TriggerProcessor
    {
        private TimeSpan eventLifetime;
        private Expression triggerCondition;
        private UnifiedEventSource eventSource;
        private IDateTimeProvider dateTimeProvider;

        private List<Event> CurrentEvents = new List<Event>();

        public TriggerProcessor(
            TimeSpan eventLifetime,
            Expression triggerCondition,
            UnifiedEventSource eventSource,
            IDateTimeProvider dateTimeProvider)
        {
            this.eventLifetime = eventLifetime;
            this.triggerCondition = triggerCondition;
            this.eventSource = eventSource;
            this.dateTimeProvider = dateTimeProvider;
        }

        public void Tick()
        {
            Cleanup();
        }

        public void Receive(Event @event)
        {
            CurrentEvents.Add(@event);
            Tick();
        }

        void Cleanup()
        {
            var now = dateTimeProvider.Now;
            CurrentEvents = CurrentEvents.Where(NotExpired).ToList();
        }

        private Boolean NotExpired(Event e)
        {
            DateTime expirationDate = dateTimeProvider.Now.Subtract(eventLifetime);
            return e.TimestampUtc >= expirationDate;
        }

        private bool EvaluateBooleanExpression()
        {
            return false;
        }
        /// <summary>
        /// Use this method only for tests. The interface exposed by this method only means that
        /// the collection returned is not lazy. Actual implementation is System.Collections.Immutable.ImmutableList,
        /// the caller shouldn't need to know about this dependency.
        /// </summary>
        /// <returns>An immutable collection of events currently being held in memory</returns>
        public IReadOnlyCollection<Event> InspectEventsUnderMonitoring() => CurrentEvents.ToImmutableList();
    }
}
