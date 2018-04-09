using EventMonitor.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventMonitor.Persistence
{
    public class PersistedEvent
    {
        public String Id { get; set; }
        public Resource Origin { get; set; }
        public String Name { get; set; }
        public Object Value { get; set; }
        public DateTime TimestampUtc { get; set; }

        public Event ToEvent() => new Event
        {
            Origin = new Resource
            {
                Name = Origin.Name,
                Location = Origin.Location
            },
            Name = Name,
            Value = Value,
            TimestampUtc = TimestampUtc
        };
    }
}
