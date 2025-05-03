using Newtonsoft.Json;

namespace SportsBattleApp.Http
{
    public class RequestRouter
    {
        private readonly Dictionary<(string method, string path), Func<string, Task<string>>> _routes;

        public RequestRouter()
        {
            _routes = new Dictionary<(string method, string path), Func<string, Task<string>>>(new RouteComparer());
        }

        public void AddRoute(string method, string path, Func<string, Task<string>> handler)
        {
            _routes[(method, path)] = handler;
        }

        public async Task<string> RouteRequestAsync(string method, string path, string body)
        {
            if (_routes.TryGetValue((method, path), out var handler))
            {
                return await handler(body);
            }
            else
            {
                return JsonConvert.SerializeObject(new { error = "404 Not Found" });
            }
        }

        // Create a new class RouteComparer that implements IEqualityComparer
        private class RouteComparer : IEqualityComparer<(string method, string path)>
        {
            public bool Equals((string method, string path) x, (string method, string path) y)
            {
                return string.Equals(x.method, y.method, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(x.path, y.path, StringComparison.OrdinalIgnoreCase);
            }
            public int GetHashCode((string method, string path) obj)
            {
                return HashCode.Combine(obj.method.ToLowerInvariant(), obj.path.ToLowerInvariant());
            }
        }
    }
}
