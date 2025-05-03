using System.Net;
using Newtonsoft.Json;

namespace SportsBattleApp.Http
{
    public class HttpServer
    {
        private readonly HttpListener _httpListener;
        private readonly RequestRouter _router;

        public HttpServer(RequestRouter router)
        {
            _router = router;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://localhost:10001/");
        }

        // Method to start the server
        public async Task StartAsync()
        {
            _httpListener.Start();
            Console.WriteLine("[HTTP Server] HTTP server started. Listening on http://localhost:10001/");

            while (true)
            {
                var context = await _httpListener.GetContextAsync();
                _ = HandleRequestAsync(context);
            }
        }

        // Method to handle incoming requests
        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                Console.WriteLine($"[HTTP Server] Received request: {request.HttpMethod} {request.Url}");

                string path = request.Url!.AbsolutePath;
                string method = request.HttpMethod;
                string body = string.Empty;

                if (request.HasEntityBody)
                {
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    body = await reader.ReadToEndAsync();
                }

                string result = await _router.RouteRequestAsync(method, path, body);

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";

                using var output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HTTP Server] Error handling request: {ex.Message}");
                response.StatusCode = 500;
                string errorJson = JsonConvert.SerializeObject(new { error = "Internal Server Error" });
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(errorJson);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";

                using var output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
            }
            finally
            {
                response.Close(); 
            }
        }
    } 
}
