# EventMonitor

I'm reconstructing an old project I did while working at Migrate Company. It was an infrastructure monitor, like Cloudwatch. 

# Build

You will need powershell on your system. Install the latest version of Powershell on your system. Please refer to https://github.com/PowerShell/PowerShell.

This is necessary for EventMonitor.Grpc. If you don't want a GRPC server/client and instead use the default HTTP 1.1 server, you can ignore this and unload the project.

=======
# TODO
    [ ] UI
        [ ] Trigger CRUD
        [ ] Chart editor
    [ ] Expression
        [X] Expression DSL
        [X] Expression parser
        [ ] Expression runner
    [X] Timed Collection
    [ ] Event actor
    [ ] Windows agent
        [X] CPU
        [ ] RAM
        [ ] Disk
        [ ] Network in/out
        [ ] Per process:
            [ ] CPU 
            [ ] RAM 
            [ ] Network (hard to do on Windows)
    [ ] Unix agent (python)
        [ ] CPU
        [ ] RAM  
        [ ] Disk
        [ ] Network in/out
        [ ] Per process:
            [ ] CPU
            [ ] RAM
            [ ] Network
    [ ] Vendor-specific
        [ ] Redis
        [ ] Postgresql
        [ ] .NET processes
            [ ] GC events
            [ ] Gen1 size
            [ ] Gen2 size
            [ ] LOB size
            [ ] Exception
        [ ] ASP.NET 
            [ ] Requests per sec, per app
        [ ] Log
            [ ] Slf4j format, tail file
	    
# History and naming

We liked to call it "Sisteminha" (little system) but management looked down on us, as if we weren't taking the situation seriously. 

Eventually management named it "Simon", which I thought at the time it was lame. However, now I think it's a "smart" play on words that involves "Sistema", "Monitoramento" and the last name of a sales guy that management liked a lot. I still call it Sisteminha, though.

At the time we just decided to make a system from the ground up and didn't look into existing solutions (like Opserver, Riemann, Graphite, Grafana, etc...) which was a big mistake. But I enjoyed desining and developing the backend for it, and learned a lot about Akka and RX.

However, it is clear that many design decisions were bad. 

## How the design phase went

First we put on paper what we wanted to monitor. We defined a JSON contract that would include ALL of these things. Sisteminha would make a HTTP request to the monitored system and get the info needed. Meanwhile it would also monitor the HTTP request itself, triggering an error if needed.

I think that's okay. It was the simplest thing we could afford, and it mostly worked. What's not okay is to define the whole system architecture based on that contract. That resulted in a rigid schema consisting of:

 - Event sources: Products and Servers. If we think about it, there's no difference between them conceptually, they're just sources with different identifiers. But Sisteminha needs to differentiate between those 2 sources because of that mistake. This resulted in big chunks of code consisting of stream black magic and an unecessary routing system, had we done things right.
 
 - Types of information: CPU usage, memory usage, etc etc... all of these were different classes and extra metadata stored on the DB. If we wanted to monitor a new kind of metric (for instance, number of Redis keys), we would have to:
  
	  - Change the DB schema
	  - Change the JSON contract for the product
	  - Create an actor that would be able to deal with that particular kind of data and nothing else
	  - Change the trigger schema to include a new kind of data
	  - Define a watcher on top of this slice of data, which notifies the UI when it changes
	  - Do all sorts of things in the UI to support:
		   - Creating triggers for it
		   - Displaying it

  Sisteminha did differentiate between discrete and continuous values. That's a thing that I'm still thinking on how to do in the new architecture. Cloudwatch solves this problem in a nice way, and I think I'll do the same. There's no difference between them, they're just datapoints.
   
# Automated Tests

Which automated tests? Thankfully, the rigid schema, the very strongly-typed nature of the backend, and the reliance on Akka and RX made things work when the code compiled. Mostly.

# New Design

I want to approach the problem using the latest things I've learned by reading TDD by Kent Back and Clean Code and Clean Architecture by Uncle Bob.

The architecture should be simple. The Unified Event Source will be used to get events from potentially multiple sources. The event will be fanned out to multiple listeners that subscribe to the unified event source. 
Each listener will:
 - Persist the event for reporting
 - Process triggers
 - Redirect to a UI application (using SignalR or another websocket abstraction)

Other features it should have:

 - Trigger definitions
 - Cool realtime charts (the UI was the coolest thing about the system, honestly)

# Triggers

To reduce the complexity of state and its control, an actor based system will be used to keep the last events within a timespan. Events may be produced in a multithreaded way. Akka will provide the mechanism to control the delivery of such events safely.

Actors are cheap. Therefore I'll not worry about how many will be created, but instead on minimize the state of each actor.

The old approach was to subscribe to a stream, executing filters on top of it and changing the actor's state based on a series of rules. This has caused some problems, many of them related to corner cases when the actor needed information about the past to decide the next state. Essentially, the state machine was complex. 

The new approach will just take all events of the last N minutes/seconds/whatever and compute what the state should be for the given actor. This comes from this presentation:https://www.youtube.com/watch?v=avwDj3KRuLc

## Expirable item in collection?

The actor will hold the last N events of a given timespan. How would that work? Can we design a collection whose items remove themselves from the collection after some time? Or a collection that, when iterated, returns only the events within the correct timestamp?

This has been discussed before, for instance: https://stackoverflow.com/questions/6493064/list-with-timeout-expire-ability

I think I have to think more on this problem. I don't want to introduce elements on the system that are hard to predict and hard to test. This is still a work in progress.

Maybe that is complex enough to be separated into a service. It's clear that these events should be stored somewhere where access to them is fast. Redis is not an option, since list items are not expirable.

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
	
This is so that events can be addressed by specificity, or "namespaced". This could allow for a fast routing algorithm that sends the event to the appropriate actor using a tries algorithm. However, I think that a dictionary lookup would be faster and way simpler. But the idea could be expanded. For instance: I wish to monitor for any process that exceeds 25% CPU usage. A trigger could be configured to monitor on `cpu.process.*.percentage` for any process that exceeds 25%, and then display the event that caused the trigger. So the notation should be respected. A tries algorithm could be implemented to parse that notation and do the lookup quickly.

This is also different than the old approach. That approach was based on a rigid schema, where all the things that I'd want to be monitored had to be defined upfront in classes. This lead to a lot of duplication of code that had to be replicated everywhere. Eventually I created an easy way to define those using some black magic, but that alone created a lot more complexity in the code. The whole black magic now feels unnecessary. That's because the old approach would consider 2 different sources of events: Product (each system that we had developed) or Server (every server would have an agent that sends events via a TCP socket) to the monitor. As stated before, they're just sources. As much as I don't like Cloudwatch (mostly the way they handle lambda logs, sometimes I cant find the events I'm looking for), their model is way nicer.
 
## Event value

The event will always carry a single scalar value that's either a string or a number. And that's it. Multiple values should be multiple events. Or find a smart way to represent it with a name. If an event wants to notify a change of usage percentage in all of the machine's disks, they should be multiple events.

The old approach was horrible. The JSON contract I reffered to later... that was the event value. The whole JSON. Sisteminha would eventually break that up into N events (where N is the number of things Sisteminha was able to monitor) and then setting up listenters to each type (CLR types) filtering for non-null values. I wouldn't be surprised if a profiler revealed that Sisteminha spends most of the time doing null-checking. Thankfully I set up some code that would do most of the null checking early on the pipeline (when the events are broken into multiple events for each type), so I didn't need to check it everywhere.

The new approach should minimze null checking by giving meaning to the event right upon arrival. An event is a well defined value (where the definition is the name, rather than the CLR type) that is not null.
