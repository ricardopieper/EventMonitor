using EventMonitor.Monitoring.Triggers.Expressions;
using System;
namespace EventMonitor.Monitoring.Triggers
{
    public class TriggerDefinition
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public TimeSpan EventLifetime { get; set; }
        public Expression TriggerCondition { get; set; }
    }
}
