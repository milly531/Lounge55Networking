using Lounge55Networking.Core;
using Lounge55Networking.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lounge55Networking.Server
{
    public interface IActionResult // still WIP
    {
        Task ExecuteResultAsync(HttpListenerContext context);
    }

    public sealed class NetworkManager
    {
        public HttpListenerContext Context;

        private byte[] _cachedBody;

        public string GetPostString()
        {
            if (_cachedBody == null) ReadAndCacheBody();
            return Context.Request.ContentEncoding.GetString(_cachedBody);
        }

        public byte[] GetPostBytes()
        {
            if (_cachedBody == null) ReadAndCacheBody();
            return _cachedBody;
        }

        public HttpListenerContext HttpContext() => Context;

        private void ReadAndCacheBody()
        {
            if (Context == null) return;

            using (var memory = new MemoryStream())
            {
                Context.Request.InputStream.CopyTo(memory);
                _cachedBody = memory.ToArray();
            }

            string contentType = Context.Request.ContentType?.ToLowerInvariant() ?? "";

            if (contentType.Contains("application/json") ||
                contentType.Contains("text/") ||
                contentType.Contains("application/x-www-form-urlencoded") ||
                contentType.Contains("xml"))
            {
                string bodyText = Context.Request.ContentEncoding.GetString(_cachedBody);
                Logger.LogInfo($"API Post Body: {bodyText}");
            }
            else
            {
                Logger.LogInfo("API Post Body Cannot Be Displayed.");
            }
        }

        public static bool IsListening { get; private set; } = false;
        private static HttpListener listener = new HttpListener();
        static readonly List<Core.MethodBase> ValidEndpoints = new List<Core.MethodBase>();

        public async Task StartListenAsync(string[] ListenerPrefixes)
        {
            try
            {
                foreach (string prefix in ListenerPrefixes)
                {
                    listener.Prefixes.Add(prefix);
                    Logger.LogInfo("Added Listener Prefix " + prefix);
                }

                listener.Start();
                IsListening = true;
                foreach (string pre in ListenerPrefixes)
                    Logger.LogInfo("Listening On " + pre);

                while (IsListening)
                {
                    var context = await listener.GetContextAsync();
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await HandleRequestAsync(context);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Error handling request: {ex.Message}");
                            try
                            {
                                Logger.LogInfo("API Response: 500 Internal Server Error");
                                context.Response.StatusCode = 500;
                                context.Response.Close();
                            }
                            catch { }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private static async Task HandleRequestAsync(HttpListenerContext context)
        {
            string url = context.Request.Url.AbsolutePath;
            bool endpointFound = false;
            Logger.LogInfo("Client Requested: " + context.Request.RawUrl);

            foreach (var endpoint in ValidEndpoints)
            {
                if (endpoint.Route == url)
                {
                    endpointFound = true;
                    if (endpoint.Method == context.Request.HttpMethod)
                    {
                        var controller = Activator.CreateInstance(endpoint.ControllerType);
                        object[] parameters = endpoint.TargetMethod.GetParameters().Length > 0
                            ? new object[] { await ReadRequestBodyAsync(context) }
                            : Array.Empty<object>();

                        object response = endpoint.TargetMethod.Invoke(controller, parameters);

                        if (response is IActionResult actionResult)
                        {
                            await actionResult.ExecuteResultAsync(context);
                        }
                        else
                        {
                            string responseString = response?.ToString() ?? "";
                            string contentType = ContentType.DetectContentType(responseString);
                            byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);
                            context.Response.StatusCode = 200;
                            context.Response.ContentType = contentType;
                            Logger.LogInfo("API Response: 200 OK");
                            context.Response.ContentLength64 = responseBytes.Length;
                            await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                            context.Response.OutputStream.Close();
                        }
                    }
                    else
                    {
                        Logger.LogInfo("API Response: 405 Method Not Allowed");
                        context.Response.StatusCode = 405;
                        context.Response.Close();
                    }
                    break;
                }
            }

            if (!endpointFound)
            {
                Logger.LogInfo("API Response: 404 Not Found");
                context.Response.StatusCode = 404;
                context.Response.Close();
            }
        }

        private static async Task<object> ReadRequestBodyAsync(HttpListenerContext context)
        {
            using (var memory = new MemoryStream())
            {
                await context.Request.InputStream.CopyToAsync(memory);
                byte[] bodyBytes = memory.ToArray();

                string contentType = context.Request.ContentType?.ToLowerInvariant() ?? "";

                if (contentType.Contains("application/json") ||
                    contentType.Contains("text/") ||
                    contentType.Contains("application/x-www-form-urlencoded") ||
                    contentType.Contains("xml"))
                {
                    string bodyText = context.Request.ContentEncoding.GetString(bodyBytes);
                    Logger.LogInfo($"API Post Body: {bodyText}");
                    return bodyText;
                }
                else
                {
                    Logger.LogInfo("API Post Body Cannot Be Displayed.");
                    return bodyBytes;
                }
            }
        }

        public void StopListen()
        {
            if (listener.IsListening)
            {
                Logger.LogInfo("Stopped Listening.");
                listener.Stop();
            }
            else
            {
                Logger.LogWarning("Listener Is Not Listening!");
            }
        }

        public void MapEndpoints(Assembly assembly)
        {
            var controllers = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ApiControllerBase)) && !t.IsAbstract);

            foreach (var controller in controllers)
            {
                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    var getAttr = method.GetCustomAttribute<GetAttribute>();
                    if (getAttr != null)
                    {
                        Logger.LogInfo($"Mapped GET {getAttr.Url} -> {controller.Name}.{method.Name}");
                        ValidEndpoints.Add(new Core.MethodBase(getAttr.Method, getAttr.Url, method, controller));
                    }

                    var postAttr = method.GetCustomAttribute<PostAttribute>();
                    if (postAttr != null)
                    {
                        Logger.LogInfo($"Mapped POST {postAttr.Url} -> {controller.Name}.{method.Name}");
                        ValidEndpoints.Add(new Core.MethodBase(postAttr.Method, postAttr.Url, method, controller));
                    }

                    var putAttr = method.GetCustomAttribute<PutAttribute>();
                    if (putAttr != null)
                    {
                        Logger.LogInfo($"Mapped PUT {putAttr.Url} -> {controller.Name}.{method.Name}");
                    }
                }
            }
        }
    }

    internal static class ContentType
    {
        public static string DetectContentType(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "text/plain";

            string trimmed = content.Trim();

            if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
                (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
                return "application/json";

            if (trimmed.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("<html", StringComparison.OrdinalIgnoreCase))
                return "text/html";

            return "text/plain";
        }
    }

    public abstract class ApiControllerBase
    {
        // ill add some actual apicontrollerbase stuff in here when i feel like it
    }
}
