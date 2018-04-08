namespace EventMonitor.Core.Events
{
    public interface IConsumer
    {
        void Consume(Event @event);
    }
}
