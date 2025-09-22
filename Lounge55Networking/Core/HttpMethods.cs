using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lounge55Networking.Core
{
    
    public class HttpMethods : Attribute
    {
        
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : Attribute
    {
        public string Url;
        public string Method;
        public GetAttribute(string Url)
        {
            this.Method = "GET";
            this.Url = Url;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : Attribute
    {
        public string Url;
        public string Method;
        public PostAttribute(string Url)
        {
            this.Method = "POST";
            this.Url = Url;
        }
    }
    // ill add more HTTP Methods when i feel like it, gfy
    [AttributeUsage(AttributeTargets.Method)]
    public class PutAttribute : Attribute
    {
        public string Url;
        public string Method;
        public PutAttribute(string Url)
        {
            this.Method = "PUT";
            this.Url = Url;
        }
    }
}
