using System;
using System.Collections.Generic;

namespace EventMonitor.Core.Events
{
    public class Event
    {
        public Resource Origin { get; set; }
        public String Name { get; set; }
        public Object Value { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
