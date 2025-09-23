# Lounge55Networking
I got bored and started developing a networking tool, It's still a work in progress
 so let me know if some stuff doesnt work. 

 ## How to use
 To use this you have to map endpoints, to do this you make a new class that inherits `ApiControllerBase`

A basic api controller should look something like
```
using Lounge55Networking.Server;
using Lounge55Networking.Core;
using System;

namespace MyProject
{
  public class ApiController : ApiControllerBase
  {
    [Get("/")]
    public string RootUrl()
    {
      return "This is server response";
    }
  }
}
```
To map endpoints, you first create a new instance of
`NetworkManager` by doing
```
NetworkManager Api = new NetworkManager();
```
then you can do
```
Api.MapEndpoints(typeof(ApiController).Assembly);
```
Then to start listening,
you call `StartListenAsync` in
your NetworkManager instance.
E.g
```
await Api.StartListenAsync(new string[] { "http://localhost:5000" });
```
And to stop listening you can do
```
Api.StopListen();
```
# This is still a WIP
So let me know about any issues in the library
