using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Lounge55Networking.Server;

namespace Lounge55Networking.Core
{
    public class ContentResult : IActionResult
    {
        public string Content { get; }
        public string ContentType { get; }
        public int StatusCode { get; }

        public ContentResult(string content, string contentType = "text/plain", int statusCode = 200)
        {
            Content = content;
            ContentType = contentType;
            StatusCode = statusCode;
        }

        public async Task ExecuteResultAsync(HttpListenerContext context)
        {
            byte[] data = Encoding.UTF8.GetBytes(Content ?? "");
            context.Response.StatusCode = StatusCode;
            context.Response.ContentType = ContentType;
            context.Response.ContentLength64 = data.Length;
            await context.Response.OutputStream.WriteAsync(data, 0, data.Length);
            context.Response.OutputStream.Close();
        }
    }

    public class JsonResult : IActionResult
    {
        public object Data { get; }
        public int StatusCode { get; }

        public JsonResult(object data, int statusCode = 200)
        {
            Data = data;
            StatusCode = statusCode;
        }

        public async Task ExecuteResultAsync(HttpListenerContext context)
        {
            string json = JsonConvert.SerializeObject(Data);
            byte[] data = Encoding.UTF8.GetBytes(json);
            context.Response.StatusCode = StatusCode;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = data.Length;
            await context.Response.OutputStream.WriteAsync(data, 0, data.Length);
            context.Response.OutputStream.Close();
        }
    }

    public class StatusCodeResult : IActionResult
    {
        public int StatusCode { get; }

        public StatusCodeResult(int statusCode) => StatusCode = statusCode;

        public Task ExecuteResultAsync(HttpListenerContext context)
        {
            context.Response.StatusCode = StatusCode;
            context.Response.Close();
            return Task.CompletedTask;
        }
    }
}
