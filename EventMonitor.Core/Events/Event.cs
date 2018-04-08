using System;
using System.Collections.Generic;

namespace EventMonitor.Core.Events
{
    public class Event
    {
        public Resource Origin { get; set; }
        public String Type { get; set; }
        public IDictionary<String, Object> Value { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
