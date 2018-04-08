using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventMonitor.Core.Events;
using EventMonitor.Core.EventSource;
using Microsoft.AspNetCore.Mvc;

namespace EventMonitor.Http.Controllers
{
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private UnifiedEventSource unifiedEventSource;

        public EventController(UnifiedEventSource unifiedEventSource)
        {
            this.unifiedEventSource = unifiedEventSource;
        }
        
        [HttpPut("emit")]
        public void Put([FromBody]Event value)
        {
            unifiedEventSource.Push(value);
        }

    }
}
