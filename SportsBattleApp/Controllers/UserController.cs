using Newtonsoft.Json;
using SportsBattleApp.Models;
using SportsBattleApp.Services;

namespace SportsBattleApp.Controllers
{
    public class UserController
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public UserController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        public async Task<string> RegisterAsync(string body)
        {
            try
            {
                var  data = JsonConvert.DeserializeObject<User>(body);
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
    }
}
