using EventMonitor.Core.Events;
using EventMonitor.Core.EventSource;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace EventMonitor.Core.Tests
{
    [Collection("Event source")]
    public class UnifiedEventSourceTest
    {
        private UESTestFixture fixture;

        public UnifiedEventSourceTest(UESTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void AllCallbacksAreBeingCalledOnPush()
        {
            UnifiedEventSource es = fixture.Instance;
            var calledCallbacks = new List<bool>();
            calledCallbacks.AddRange(new[] { false, false, false });

            es.Subscribe(@event => calledCallbacks[0] = true);
            es.Subscribe(@event => calledCallbacks[1] = true);
            es.Subscribe(@event => calledCallbacks[2] = true);

            es.Push(new Event());

            Assert.All(calledCallbacks, Assert.True);
        }

        [Fact]
        public void PushDoesNotBreakCallerIfConsumerThrows()
        {
            UnifiedEventSource es = fixture.Instance;
            es.Subscribe(@event => throw new Exception("Oops"));
            es.Push(new Event());
        }

        [Fact]
        public void IfConsumerThrowsContinueCallingConsumers()
        {
            UnifiedEventSource es = fixture.Instance;
            
            bool called = false;
            es.Subscribe(@event => throw new Exception("Oops"));
            es.Subscribe(@event => called = true);

            es.Push(new Event());
            Assert.True(called);
        }

        [Fact]
        public void IfThrowsThenNotifyIsCalled()
        {
            Exception e = new Exception("Oops");
            UnifiedEventSource es = fixture.Instance;
            es.Subscribe(@event => throw e);
            es.Push(new Event());
            fixture.Notifier.Verify(x => x.Notify(e));
        }
    }

    public class UESTestFixture
    {
        public Mock<IUnhandledExceptionNotifier> Notifier { get; } = new Mock<IUnhandledExceptionNotifier>();
        public UnifiedEventSource Instance => new UnifiedEventSource(Notifier.Object);
    }

    [CollectionDefinition("Event source")]
    public class UESTestCollection : ICollectionFixture<UESTestFixture>  { }
}
