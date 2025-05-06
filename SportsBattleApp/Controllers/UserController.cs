using Newtonsoft.Json;
using SportsBattleApp.Models;
using SportsBattleApp.Services;

namespace SportsBattleApp.Controllers
{
    public class UserController
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<string> GetUserByUsernameAsync(string username, string body)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "User not found." });
                }
                return JsonConvert.SerializeObject(new { success = true, user});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserController] Error during Register: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }

        public async Task<string> EditUserProfileAsync(string username, string body)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<User>(body);
                if (data == null || string.IsNullOrWhiteSpace(username))
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "Invalid input." });
                }

                bool success = await _userService.EditUserProfileAsync(username, data);
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
