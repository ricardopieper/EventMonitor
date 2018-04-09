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
 
 
# Triggers

To reduce the complexity of state and its control, an actor based system will be used to keep the last events within a timespan. Events may be produced in a multithreaded way. Akka will provide the mechanism to control the delivery of such events safely.

Actors are cheap. Therefore I'll not worry about how many will be created, but instead on minimize the state of each actor.

The old approach was to subscribe to a stream, executing filters on top of it and changing the actor's state based on a series of rules. This has caused some problems, many of them related to corner cases when the actor needed information about the past to decide the next state. Essentially, the state machine was complex. 

The new approach will just take all events of the last N minutes/seconds/whatever and compute what the state should be for the given actor. If the 

## Tick rate

Another problem of the old approach was to rely only on the reception of events to update its state. However, sometimes new events wouldn't be produced for quite some time. In abscence of events, the only choice was to maintain the current state.

A constant tick rate will be kept to update the actor's state every second. For now that's how it going to be, maybe another way will be discovered. 

## Name matching

Each event has a name, or type. They should follow patterns, like:

	cpu.total.percentage
	cpu.process.chrome.percentage
	cpu.process.w3wp.percentage
	mem.used.total.percentage
	mem.used.process.chrome.percentage
	mem.free.total.percentage
	mem.free.process.chrome.percentage
	mem.used.total.mb
	mem.used.process.chrome.mb
	mem.free.total.mb
	mem.free.process.chrome.mb
	....
	
This is so that events can be addressed by specificity, or "namespaced". This could allow for a fast routing algorithm that sends the event to the appropriate actor. For now, a dictionary lookup should suffice. But the idea could be expanded. For instance: I wish to monitor for any process that exceeds 25% CPU usage. A trigger could be configured to monitor on `cpu.process.*.percentage` for any process that exceeds 25%, and then display the event that caused the trigger.

This is also different than the old approach. That approach was based on a rigid schema, where all the things that I'd want to be monitored had to be defined upfront in classes. This lead to a lot of duplication of code that had to be replicated. Eventually I created an easy way to define those using some black magic, but that alone created a lot more complexity in the code.
 
## Event value

The event will always carry a single scalar value that's either a string or a number. And that's it. Multiple values should be multiple events. Or find a smart way to represent it with a name. If an event wants to notify a change of usage percentage in all of the machine's disks, they should be multiple events.

