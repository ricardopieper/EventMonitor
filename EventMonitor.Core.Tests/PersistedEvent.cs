using EventMonitor.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventMonitor.Core.Tests
{
    public class PersistedEvent
    {
        public String Id { get; set; }
        public Resource Origin { get; set; }
        public String Type { get; set; }
        public Dictionary<String, Object> Value { get; set; }
        public DateTime TimestampUTC { get; set; }

        public Event ToEvent() => new Event
        {
            Origin = new Resource
            {
                Name = Origin.Name,
                Location = Origin.Location
            },
            Type = Type,
            Value = new Dictionary<String, Object>(Value),
            TimestampUTC = TimestampUTC
        };
    }
}
