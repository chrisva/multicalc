# MultiCalc
Application that can be given a number and can find the sum of particular multiples. Optimised for Cloud Service and Azure.

### Running web solution:

- [MultiCalc web](http://www.google.com)
- [MultiCalc Swagger Rest API docs](http://www.google.com)

## How to use the application:

The application has a very simple use.
- Enter a comma seperated list of factors you want to use
- Enter the particular max number in the range of numbers as the upper boarder
- Send to MultiCalc engine

You can send as many calculation requests as you like hitting the send button, and change values as you like between the requests. This will send all the requests to a queue in Azure where it waits for a worker to pick it up eventually.

You can just sit tight and wait for the responses to flow into the user interface. I am using a technology called WebSockets using SignalR to sync everything that works in the background with the user interface. 

![](https://github.com/chrisva/multicalc/blob/master/GuideImages/MultiCalcScreen1.png)

### The 3 columns of data:

- In the first column you will see your request in the current session. NB if you refresh the browser, you will create a new session.

- In the second column you will see your own calculation results only. 

- In the third column you will see every user's calculation results, including yours.

Once you have some data you can see your parameters and the result for every entity. *(You can expand the raw data to see more about the process and time and more.)*

### The restore function on the top

The restore function at the top is for now manual as a proof of concept, but could easily be automated in the future with proper user authentication and id system.

- For now you can send many requests from a session before it arrives to the browser again. 
- Then you can copy the session id. 
- Open a new session by opening a new browser or refreshing the page in the browser. 
- Paste the previous session id in the input field.
- Click restore, and watch those processed messages belonging to the previous session flow into this sessions results. (those messages was persisted in Azure Table Storage when the other session went down)

## About the infrastructure
The solution is running in Azure in its own resource group called "multicalc".

![](https://github.com/chrisva/multicalc/blob/master/GuideImages/AzureResources.png)

As you can see from the image above, there are a few resources here.

- **multicalcweb** is running the visual web that you can visit with your browser and also the SignalR (WebSocket) server and the REST API. So everything that has to do with lightweight web request. The web is using Bootstrap and JQuery as bundled with a standard ASP.NET MVC solution. In a production solution either React or Angular 2 would be preferable.
- **multicalcworker** is running a [WebJob](http://www.hanselman.com/blog/IntroducingWindowsAzureWebJobs.aspx) continuously  with a calculated delay, so that every 10 second a new calculation process is started. It could be longer if the previous one is not done yet. This could easily be extended to handle many processes in parallell.
- **multicalc (storage account)** is handling table storage of persisted documents
- **multicalc (service bus)** handles queues of messages and storage of messages to be processed

In addition there are Application Insight services to monitor the solution.

### So why this infrastructure?

It is redundant, depending on the service plans, it scales and it is efficient. The worker and the web can scale independent of each other, both vertical and horizontal. WebJobs is handled by Azure and will self restore if something fails. WebJobs is the choice over Azure Scheduler and Azure Functions because of the fetching every 10 seconds. Normally we would have the Function or WebJob triggering on new queue item and scale out to handle in parallell. However, that was not the case given here. It was to be fixed to 10 seconds which is also a too small time span for the scheduler to handle. (only minutes and up) The scheduler is preferred for most other cases as it has Ring 1 priority in Azure.

So now the worker checks the queue for the latest message. Processes it and sends it to the web using SignalR. Then the receiving of that message is handled. If the original session is active then the result is returned immediately. Otherwise it is stored to Table Storage and can be retrieved by using the restore function on the web.

Table Storage is super fast and simple for this kind of storage. A partition is made using the client id as key, and that is sufficient to look up the client id very fast.

The service bus handles messages to be processed. And they stay there for days (can be configured). The reappear on errors and is redundant with high priority SLA in Azure. 


## What is there and what is not

This is a prototype / proof of concept and is not perfect.  So the solution does is decribed well above.

### What it does:
- The web solution can be given a number and can find the sum of particular multiples.
- It can autoscale if configured to do so in the service plans.
- All components can scale independently.
- The messages flows as it could do in production between components.
- Dependency Injection in parts of the solution using Autofac.
- Basic unit tests on functions.


### What is not there and could be different, but is not because of time:
- No React or Angular but should be in next version
- No general logging strategy - should use Serilog with sinks to a log hub such as ElasticSearch / Kibana, Seq ++
- Dependency injection should be implemented everywhere
- Proper integration tests
- More code comments throughout the solution
- Better docs
- A good authentication solution to sync sessions and handle access rights
- Continues deployment and build
- ++


## Files in the repository
In the repository there is the **MultiCalc.sln** that is the starting point for working with the solution in Visual Studio 2015 and above. 
In addition there is a folder called **HostingScripts** that holds scripts to recreate the resource group or use as a template for it from CLI or other code.

## Some screenshots from Azure portal:

