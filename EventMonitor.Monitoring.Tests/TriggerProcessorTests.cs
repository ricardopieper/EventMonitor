using EventMonitor.Core.EventSource;
using EventMonitor.Core.Events;
using EventMonitor.Monitoring.Triggers.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace EventMonitor.Monitoring.Tests
{
    public class TriggerProcessorTests
    {
        [Fact]
        public void HoldsEventsForTheSpecifiedTimeSpan()
        {
            UnifiedEventSource ues = new UnifiedEventSource(new SwallowException());
            IDateTimeProvider dateTimeProvider = new MockDateTimeProvider(new DateTime(2018, 01, 01, 00, 00, 30));
            Resource origin = new Resource { Location = "localhost", Name = nameof(TriggerProcessorTests) };

            TriggerProcessor tp = new TriggerProcessor(
                TimeSpan.FromSeconds(15),
                Expr.Avg("cpu.usage").Gte(15),
                ues, dateTimeProvider                
            );

            Assert.Empty(tp.InspectEventsUnderMonitoring());
            
            //ignore
            tp.Receive(new Event {
                Name = "cpu.usage",
                Value = 2,
                Origin = origin,
                TimestampUtc = new DateTime(2018, 01, 01, 00, 00, 00)
            });

            //ignore
            tp.Receive(new Event
            {
                Name = "cpu.usage",
                Value = 3,
                Origin = origin,
                TimestampUtc = new DateTime(2018, 01, 01, 00, 00, 01)
            });

            //ignore
            tp.Receive(new Event
            {
                Name = "cpu.usage",
                Value = 7,
                Origin = origin,
                TimestampUtc = new DateTime(2018, 01, 01, 00, 00, 7)
            });
            Assert.Empty(tp.InspectEventsUnderMonitoring());

            //accept
            tp.Receive(new Event
            {
                Name = "cpu.usage",
                Value = 7,
                Origin = origin,
                TimestampUtc = new DateTime(2018, 01, 01, 00, 00, 16)
            });

            var currentEvents = tp.InspectEventsUnderMonitoring();
            Assert.True(currentEvents.Count == 1);
        }
    }

    public class MockDateTimeProvider : IDateTimeProvider
    {
        private DateTime whichTime;
        public MockDateTimeProvider(DateTime whichTime)
        {
            SetTime(whichTime);
        }
        public void SetTime(DateTime whichTime)
        {
            this.whichTime = whichTime;
        }
        public DateTime Now => whichTime;
    }

    public class SwallowException : IUnhandledExceptionNotifier
    {
        public void Notify(Exception ex)
        {
            
        }
    }
}
