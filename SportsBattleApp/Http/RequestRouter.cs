using Newtonsoft.Json;
using SportsBattleApp.Controllers;
using SportsBattleApp.Data;
using SportsBattleApp.Repositories;
using SportsBattleApp.Services;

namespace SportsBattleApp.Http
{
    public class RequestRouter
    {
        private readonly Dictionary<(string method, string path), Func<string, Task<string>>> _routes;

        public RequestRouter(DatabaseConnection db)
        {
            _routes = new Dictionary<(string method, string path), Func<string, Task<string>>>(new RouteComparer());
        
            var userRepository = new UserRepository(db);

            var userService = new UserService(userRepository);
            var authService = new AuthService(userRepository);

            var userController = new UserController(userService, authService);
            var authController = new AuthController(authService);

            AddRoute("POST", "/users", userController.RegisterAsync);
            AddRoute("POST", "/sessions", authController.LoginAsync);
        }

        public void AddRoute(string method, string path, Func<string, Task<string>> handler)
        {
            _routes[(method, path)] = handler;
        }

        public async Task<string> RouteHttpRequestAsync(string method, string path, string body)
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
