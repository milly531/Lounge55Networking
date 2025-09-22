### Lounge55Networking
I got bored and started developing a networking tool, It's still a work in progress
 so let me know if some stuff doesnt work. 

 ## How to use
 To use this you have to map endpoints, to do this you make a new class that inherits
 ```
ApiControllerBase
```
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
to actually make this map the endpoints, you do
```
NetworkManager.MapEndpoints(typeof(ApiController).Assembly);
```
Then to start the listening you do
```
using System;
using System.Threading.Tasks;
using Lounge55Networking.Server;

namespace MyProject
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            NetworkManager.MapEndpoints(typeof(ApiController).Assembly);
            await NetworkManager.StartListenAsync(new string[] { "http://localhost:5000/" });
            Console.ReadKey();
        }
    }
}
```
Then to stop you do
```
NetworkManager.StopListen():
```
### This is still a WIP
So let me know about any issues in the library
