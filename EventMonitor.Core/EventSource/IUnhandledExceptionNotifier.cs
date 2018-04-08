using System;

namespace EventMonitor.Core.EventSource
{
    public interface IUnhandledExceptionNotifier
    {
        void Notify(Exception ex);
    }
}