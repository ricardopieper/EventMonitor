using System;

namespace EventMonitor.Core.Triggers
{
    public class TriggerDefinition
    {
        public String EventName { get; set; }
        public TimeSpan EventLifetime { get; set; }
        public Expression TriggerCondition { get; set; }
    }
}
