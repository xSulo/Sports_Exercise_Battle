using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SportsBattleApp.Controllers;
using SportsBattleApp.Data;
using SportsBattleApp.Repositories;
using SportsBattleApp.Services;

namespace SportsBattleApp.Http
{
    public class RequestRouter
    {
        private readonly Dictionary<(string method, string path), Func<string, Task<string>>> _routesSimple;
        private readonly Dictionary<(string method, string path), Func<string, string, Task<string>>> _routesWithToken;
        private readonly List<(string method, string pattern, Func<string, string, Task<string>> handler)> _dynamicRoutes;
        private readonly AuthController _authController;
        public RequestRouter(DatabaseConnection db)
        {
            _routesSimple = new Dictionary<(string method, string path), Func<string, Task<string>>>(new RouteComparer());
            _routesWithToken = new Dictionary<(string method, string path), Func<string, string, Task<string>>>(new RouteComparer());
            _dynamicRoutes = new List<(string method, string pattern, Func<string, string, Task<string>> handler)>();

            var userRepository = new UserRepository(db);
            var pushUpRecordRepository = new PushUpRecordRepository(db);


            var userService = new UserService(userRepository);
            var authService = new AuthService(userRepository);
            var pushUpRecordService = new PushUpRecordService(pushUpRecordRepository, userRepository, authService);

            var userController = new UserController(userService, authService);
            var pushUpRecordController = new PushUpRecordController(pushUpRecordService);
            _authController = new AuthController(authService);

            AddRoute("POST", "/users", userController.RegisterAsync);
            AddRoute("POST", "/sessions", _authController.LoginAsync);

            // Profile
            AddDynamicRoute("GET", "/users/{username}", userController.GetUserByUsernameAsync);
            AddDynamicRoute("PUT", "/users/{username}", userController.EditUserProfileAsync);

            // History
            AddRoute("GET", "/history", pushUpRecordController.GetHistoryByTokenHashAsync);
            AddRouteWithToken("POST", "/history", pushUpRecordController.PostHistoryByTokenHashAsync);
        }

        public void AddRoute(string method, string path, Func<string, Task<string>> handler)
        {
            _routesSimple[(method, path)] = handler;
        }

        public void AddRouteWithToken(string method, string path, Func<string, string, Task<string>> handler)
        {
            _routesWithToken[(method, path)] = handler;
        }

        public void AddDynamicRoute(string method, string pattern, Func<string, string, Task<string>> handler)
        {
            _dynamicRoutes.Add((method, pattern, handler));
        }

        public async Task<string> RouteHttpRequestAsync(string method, string path, string body, Dictionary<string, string> header)
        {
            // Add functionality later -> users with tokens cannot use login or register

            if (_routesSimple.TryGetValue((method, path), out var simpleHandler) && (path == "/users" || path == "/sessions"))
            {
                return await simpleHandler(body);
            }

            if (_routesWithToken.TryGetValue((method, path), out var tokenHandler))
            {
                if ((!header.TryGetValue("Authorization", out var token) || !await _authController.IsTokenValidAsync(token)))
                {
                    return JsonConvert.SerializeObject(new { error = "401 Unauthorized" });
                }
                return await tokenHandler(token, body);
            }

            if (_routesSimple.TryGetValue((method, path), out var tokenHandlerGet) && method == "GET")
            {
                if ((!header.TryGetValue("Authorization", out var token) || !await _authController.IsTokenValidAsync(token)))
                {
                    return JsonConvert.SerializeObject(new { error = "401 Unauthorized" });
                }
                return await tokenHandlerGet(token);
            }

            foreach (var route in _dynamicRoutes)
            {
                var match = Regex.Match(path, route.pattern.Replace("{username}", "(\\w+)"));
                if (match.Success && method.Equals(route.method, StringComparison.OrdinalIgnoreCase))
                {
                    var username = match.Groups[1].Value;

                    if(!header.TryGetValue("Authorization", out var token) || !await _authController.IsTokenValidAsync(token))
                    {
                        return JsonConvert.SerializeObject(new { error = "401 Unauthorized" });
                    }

                    return await route.handler(username, body);
                }
            }

            return JsonConvert.SerializeObject(new { error = "404 Not Found" });
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
