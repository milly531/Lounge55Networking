using System;
using System.Reflection;

namespace Lounge55Networking.Core
{
    internal class MethodBase
    {
        public string Route { get; set; }
        public string Method { get; set; }
        public MethodInfo TargetMethod { get; set; }
        public Type ControllerType { get; set; }

        public MethodBase(string method, string route, MethodInfo targetMethod, Type controllerType)
        {
            // map some shit!!!!
            this.Method = method;
            this.Route = route;
            this.TargetMethod = targetMethod;
            this.ControllerType = controllerType;
        }
    }
}
