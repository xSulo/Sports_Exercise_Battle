using Newtonsoft.Json;
using SportsBattleApp.DTOs;
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

        // GET for /users/{username} aka profile, in order to view the profile
        public async Task<HttpResponseDTO> GetUserProfileByUsernameAsync(string username, string body)
        {
            try
            {
                var user = await _userService.GetUserProfileByUsernameAsync(username);

                if (user == null)
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 404,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = "User not found." })
                    };
                }

                Console.WriteLine($"[UserController] Getting user profile was successful!");
                return new HttpResponseDTO
                {
                    StatusCode = 200,
                    JsonContent = JsonConvert.SerializeObject(new { success = true, user })
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserController] Error during GetUserByUsernameAsync: {ex.Message}");
                return new HttpResponseDTO
                {
                    StatusCode = 500,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" })
                };
            }
        }

        // PUT for /users/{username} aka profile, in order to change profile
        public async Task<HttpResponseDTO> EditUserProfileAsync(string username, string body)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<User>(body);
                if (data == null || string.IsNullOrWhiteSpace(username))
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 400,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Invalid input." })
                    };
                }

                bool success = await _userService.EditUserProfileAsync(username, data);

                if (success)
                {
                    Console.WriteLine($"[UserController] Updating user profile was successful!");
                }

                return new HttpResponseDTO
                {
                    StatusCode = 200,
                    JsonContent = JsonConvert.SerializeObject(new { success, message = $"User {username} was successfully updated." })
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserController] Error during EditUserProfileAsync: {ex.Message}");
                return new HttpResponseDTO
                {
                    StatusCode = 500,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" })
                };
            }
        }
    }
}
