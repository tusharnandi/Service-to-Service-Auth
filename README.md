# STS: 'Service-to-Service' Calls on behalf of user.

Your app needs to acquire a token for the downstram Web API(called A) for the user to call the web API(called A). That  Web API(A) again needs to acquire a token for the user to calls another downstram Web API(called B).

Microsoft EntraId is playing crucial role for Authentication as well as Authorization. 

We created Application Object for each web Api and web app. and appended on the top of each application. 

**For new user, Microsoft EntraId get user consent from the user first time only.(final testing pending)**


It also knows as WebAPI That calls another Web API on behalf of user.




![High level diagram](./docs/images/service-2-service-v1.png)