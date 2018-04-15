using EventMonitor.Core.Events;
using EventMonitor.Core.EventSource;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EventMonitor.Core.Tests
{
    [Collection("Event source")]
    public class UnifiedEventSourceTest : WriteToStdout
    {
        private UESTestFixture fixture;

        public UnifiedEventSourceTest(ITestOutputHelper helper, UESTestFixture fixture) : base(helper)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void AllCallbacksAreBeingCalledOnPush()
        {
            MockUES mock = fixture.Mock();
            UnifiedEventSource es = mock.Instance;
            var calledCallbacks = new[] { false, false, false };

            es.Subscribe(@event => calledCallbacks[0] = true);
            es.Subscribe(@event => calledCallbacks[1] = true);
            es.Subscribe(@event => calledCallbacks[2] = true);

            es.Push(new Event());

            Assert.All(calledCallbacks, Assert.True);
        }

        [Fact]
        public async Task AsyncAllCallbacksAreBeingCalledOnPush()
        {
            MockUES mock = fixture.Mock();
            UnifiedEventSource es = mock.Instance;
            var calledCallbacks = new[] { false, false, false };

            es.Subscribe(@event => calledCallbacks[0] = true);
            es.Subscribe(@event => calledCallbacks[1] = true);
            es.Subscribe(@event => calledCallbacks[2] = true);

            await es.PushAsync(new Event());

            Assert.All(calledCallbacks, Assert.True);
        }

        [Fact]
        public async Task AsyncAllTaskCallbacksAreBeingCalledOnPush()
        {
            var tasks = Enumerable.Range(0, 500).Select(async x =>
            {
                MockUES mock = fixture.Mock();
                UnifiedEventSource es = mock.Instance;
                var calledCallbacks = new[] { false, false, false };

                es.SubscribeTask(@event => Task.Run(() => calledCallbacks[0] = true));
                es.SubscribeTask(@event => Task.Run(() => calledCallbacks[1] = true));
                es.SubscribeTask(@event => Task.Run(() => calledCallbacks[2] = true));

                await es.PushAsync(new Event());

                Assert.All(calledCallbacks, Assert.True);

                mock.Notifier.Verify((n) => n.Notify(It.IsAny<Exception>()), Times.Never(), "");
            });
            await Task.WhenAll(tasks);
        }

        [Fact]
        public void PushDoesNotBreakCallerIfConsumerThrows()
        {
            MockUES mock = fixture.Mock();
            UnifiedEventSource es = mock.Instance;
            es.Subscribe(@event => throw new Exception("Oops"));
            es.Push(new Event());
        }

        [Fact]
        public async Task PushDoesNotBreakCallerIfConsumerThrowsAsync()
        {
            MockUES mock = fixture.Mock();
            UnifiedEventSource es = mock.Instance;
            es.SubscribeTask(@event => throw new Exception("Oops"));
            await es.PushAsync(new Event());
        }

        [Fact]
        public void IfConsumerThrowsContinueCallingConsumers()
        {
            MockUES mock = fixture.Mock();
            UnifiedEventSource es = mock.Instance;

            bool called = false;
            es.Subscribe(@event => throw new Exception("Oops"));
            es.Subscribe(@event => called = true);

            es.Push(new Event());
            Assert.True(called);
        }

        [Fact]
        public async Task IfConsumerThrowsContinueCallingConsumersAsync()
        {
            MockUES mock = fixture.Mock();
            UnifiedEventSource es = mock.Instance;

            bool called = false;
            es.SubscribeTask(@event => throw new Exception("Oops"));
            es.SubscribeTask(@event => Task.Run(() => called = true));

            await es.PushAsync(new Event());
            Assert.True(called);
        }

        [Fact]
        public void IfThrowsThenNotifyIsCalled()
        {
            Exception e = new Exception("Oops");
            MockUES mock = fixture.Mock();
            mock.Instance.Subscribe(@event => throw e);
            mock.Instance.Push(new Event());
            mock.Notifier.Verify(x => x.Notify(e));
        }

        [Fact]
        public async Task IfThrowsThenNotifyIsCalledAsync()
        {
            Exception e = new Exception("Oops");
            MockUES mock = fixture.Mock();
            mock.Instance.SubscribeTask(@event => throw e);
            await mock.Instance.PushAsync(new Event());
            mock.Notifier.Verify(x => x.Notify(e));
        }
    }

    public class WriteToStdout : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly TextWriter _originalOut;
        private readonly TextWriter _textWriter;

        public WriteToStdout(ITestOutputHelper output)
        {
            _output = output;
            _originalOut = Console.Out;
            _textWriter = new StringWriter();
            Console.SetOut(_textWriter);
        }

        public void Dispose()
        {
            _output.WriteLine(_textWriter.ToString());
            Console.SetOut(_originalOut);
        }
    }

    public class UESTestFixture
    {
        public MockUES Mock()
        {
            var notifier = new Mock<IUnhandledExceptionNotifier>();
            return new MockUES
            {
                Instance = new UnifiedEventSource(notifier.Object),
                Notifier = notifier
            };
        }
    }

    public class MockUES
    {
        public UnifiedEventSource Instance { get; set; }
        public Mock<IUnhandledExceptionNotifier> Notifier { get; set; }
    }

    [CollectionDefinition("Event source")]
    public class UESTestCollection : ICollectionFixture<UESTestFixture> { }
}
