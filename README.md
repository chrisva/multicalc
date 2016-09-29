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


### The restore function on the top

The restore function at the top is for now manual as a proof of concept, but could easily be automated in the future with proper user authentication and id system.

- For now you can send many requests from a session before it arrives to the browser again. 
- Then you can copy the session id. 
- Open a new session by opening a new browser or refreshing the page in the browser. 
- Paste the previous session id in the input field.
- Click restore, and watch those processed messages belonging to the previous session flow into this sessions results. (those messages was persisted in Azure Table Storage when the other session went down)
