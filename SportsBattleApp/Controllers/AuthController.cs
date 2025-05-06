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
                return JsonConvert.SerializeObject(new { success });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserController] Error during Register: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }

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
                return JsonConvert.SerializeObject(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserController] Error during Login: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            try 
            { 
                if(string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("[AuthController] No token provided.");
                    return false;
                }

                return await _authService.IsTokenValidAsync(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Error during token validation: {ex.Message}");
                return false;
            }
        }
    }
}
