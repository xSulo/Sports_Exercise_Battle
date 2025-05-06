using Newtonsoft.Json;
using SportsBattleApp.Models;
using SportsBattleApp.Services;

namespace SportsBattleApp.Controllers
{
    public class AuthController
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST for /users aka register, in order to register a new user
        public async Task<string> RegisterAsync(string body)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<User>(body);
                if (data == null || string.IsNullOrWhiteSpace(data.Username) || string.IsNullOrWhiteSpace(data.PasswordHash))
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "Invalid input." });
                }

                bool success = await _authService.RegisterAsync(data.Username, data.PasswordHash);

                if (success)
                {
                    Console.WriteLine($"[AuthController] Creating user was successful!");
                }

                return JsonConvert.SerializeObject(new { success = true, message = $"User {data.Username} was created successfully."});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Error during Register: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }

        // POST for /sessions aka login, in order to login a user
        public async Task<string> LoginAsync(string body)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<User>(body);
                if (data == null || string.IsNullOrWhiteSpace(data.Username) || string.IsNullOrWhiteSpace(data.PasswordHash))
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "Invalid input." });
                }

                bool success = await _authService.LoginAsync(data.Username, data.PasswordHash);

                if (!success)
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "Password or Username is incorrect." });
                }

                Console.WriteLine($"[AuthController] Login with user was successful!");

                return JsonConvert.SerializeObject(new { success = true, message = $"You successfully logged in as user {data.Username}." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Error during Login: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }

        // Used everytime there is a request, checks if the token is valid
        public async Task<bool> IsTokenValidAsync(string token)
        {
            try 
            { 
                if(string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("[AuthController] No token provided.");
                    return false;
                }

                bool isTokenValid = await _authService.IsTokenValidAsync(token);

                if (!isTokenValid)
                {
                    Console.WriteLine("[AuthController] Token is not valid.");
                    return false;
                }

                Console.WriteLine("[AuthController] Token is valid.");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Error during token validation: {ex.Message}");
                return false;
            }
        }
    }
}
