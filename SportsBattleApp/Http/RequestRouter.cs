using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SportsBattleApp.Controllers;
using SportsBattleApp.Data;
using SportsBattleApp.DTOs;
using SportsBattleApp.Repositories;
using SportsBattleApp.Services;

namespace SportsBattleApp.Http
{
    public class RequestRouter
    {
        private readonly Dictionary<(string method, string path), Func<string, Task<HttpResponseDTO>>> _routesSimple;
        private readonly Dictionary<(string method, string path), Func<string, string, Task<HttpResponseDTO>>> _routesWithToken;
        private readonly List<(string method, string pattern, Func<string, string, Task<HttpResponseDTO>> handler)> _dynamicRoutes;
        private readonly AuthController _authController;
        public RequestRouter(DatabaseConnection db)
        {
            _routesSimple = new Dictionary<(string method, string path), Func<string, Task<HttpResponseDTO>>>(new RouteComparer());
            _routesWithToken = new Dictionary<(string method, string path), Func<string, string, Task<HttpResponseDTO>>>(new RouteComparer());
            _dynamicRoutes = new List<(string method, string pattern, Func<string, string, Task<HttpResponseDTO>> handler)>();

            var userRepository = new UserRepository(db);
            var pushUpRecordRepository = new PushUpRecordRepository(db);

            var hashingService = new HashingService();
            var tokenService = new TokenService(hashingService);
            var authService = new AuthService(userRepository, hashingService, tokenService);
            var tournamentService = new TournamentService(userRepository);
            var userService = new UserService(userRepository, hashingService);
            var pushUpRecordService = new PushUpRecordService(pushUpRecordRepository, userRepository, authService, tournamentService);
            var statsService = new StatsService(userRepository, pushUpRecordRepository, authService);

            var userController = new UserController(userService);
            var pushUpRecordController = new PushUpRecordController(pushUpRecordService);
            var statsController = new StatsController(statsService);
            var tournamentController = new TournamentController(tournamentService);
            _authController = new AuthController(authService);

            //Login and Register
            AddRoute("POST", "/users", _authController.RegisterAsync);
            AddRoute("POST", "/sessions", _authController.LoginAsync);

            // Profile
            AddDynamicRoute("GET", "/users/{username}", userController.GetUserProfileByUsernameAsync);
            AddDynamicRoute("PUT", "/users/{username}", userController.EditUserProfileAsync);

            // History
            AddRoute("GET", "/history", pushUpRecordController.GetHistoryByTokenAsync);
            AddRouteWithToken("POST", "/history", pushUpRecordController.PostHistoryByTokenAsync);

            // Stats
            AddRoute("GET", "/stats", statsController.GetStatsByTokenAsync);
            AddRoute("GET", "/score", statsController.GetScoreByTokenAsync);

            // Tournament
            AddRoute("GET", "/tournament", tournamentController.GetTournamentStatusAsync);
        }

        public void AddRoute(string method, string path, Func<string, Task<HttpResponseDTO>> handler)
        {
            _routesSimple[(method, path)] = handler;
        }

        public void AddRouteWithToken(string method, string path, Func<string, string, Task<HttpResponseDTO>> handler)
        {
            _routesWithToken[(method, path)] = handler;
        }

        public void AddDynamicRoute(string method, string pattern, Func<string, string, Task<HttpResponseDTO>> handler)
        {
            _dynamicRoutes.Add((method, pattern, handler));
        }

        public async Task<HttpResponseDTO> RouteHttpRequestAsync(string method, string path, string body, Dictionary<string, string> header)
        {
            if (_routesSimple.TryGetValue((method, path), out var simpleHandler) && (path == "/users" || path == "/sessions"))
            {
                return await simpleHandler(body);
            }

            if (_routesWithToken.TryGetValue((method, path), out var tokenHandler))
            {
                if (!header.TryGetValue("Authorization", out var token) || !await _authController.IsTokenValidAsync(token))
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 401,
                        JsonContent = JsonConvert.SerializeObject(new { error = "Unauthorized" })
                    };
                }

                return await tokenHandler(token, body);
            }

            if (_routesSimple.TryGetValue((method, path), out var getHandler) && method == "GET")
            {
                if (!header.TryGetValue("Authorization", out var token) || !await _authController.IsTokenValidAsync(token))
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 401,
                        JsonContent = JsonConvert.SerializeObject(new { error = "Unauthorized" })
                    };
                }

                return await getHandler(token);
            }

            foreach (var route in _dynamicRoutes)
            {
                var match = Regex.Match(path, route.pattern.Replace("{username}", "(\\w+)"));
                if (match.Success && method.Equals(route.method, StringComparison.OrdinalIgnoreCase))
                {
                    var username = match.Groups[1].Value;

                    if (!header.TryGetValue("Authorization", out var token) || !await _authController.IsTokenValidAsync(token))
                    {
                        return new HttpResponseDTO
                        {
                            StatusCode = 401,
                            JsonContent = JsonConvert.SerializeObject(new { error = "Unauthorized" })
                        };
                    }

                    return await route.handler(username, body);
                }
            }

            return new HttpResponseDTO
            {
                StatusCode = 404,
                JsonContent = JsonConvert.SerializeObject(new { error = "Not Found" })
            };
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
