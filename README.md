# EventMonitor

I'm reconstructing an old project I did while working at Migrate Company. It was an infrastructure monitor, like Cloudwatch. 

I want to approach the problem using the latest things I've learned by reading TDD by Kent Back and Clean Code and Clean Architecture by Uncle Bob.

The architecture should be simple. The Unified Event Source will be used to get events from potentially multiple sources. 
The event will be fanned out to multiple listeners that subscribe to the unified event source. 
Each listener will:
 - Persist the event for reporting
 - Process triggers
 - Redirect to a UI application (using SignalR or another websocket abstraction)


Other features it should have:

 - Trigger definitions
 - Cool realtime charts   
