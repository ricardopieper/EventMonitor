using System;

namespace EventMonitor.Monitoring
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}