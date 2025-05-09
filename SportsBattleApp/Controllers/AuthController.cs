using Newtonsoft.Json;
using SportsBattleApp.DTOs;
using SportsBattleApp.Models;
using SportsBattleApp.Services;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<HttpResponseDTO> RegisterAsync(string body)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<User>(body);
                if (data == null || string.IsNullOrWhiteSpace(data.Username) || string.IsNullOrWhiteSpace(data.PasswordHash))
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 400,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Invalid input." })
                    };
                }

                bool success = await _authService.RegisterAsync(data.Username, data.PasswordHash);

                if (success)
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 201,
                        JsonContent = JsonConvert.SerializeObject(new { success = true, message = "User successfully created!" })
                    };
                }
                else
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 409,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Username already taken." })
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Error during Register: {ex.Message}");
                return new HttpResponseDTO
                {
                    StatusCode = 500,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" })
                };
            }
        }

        // POST for /sessions aka login, in order to login a user
        public async Task<HttpResponseDTO> LoginAsync(string body)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<User>(body);
                if (data == null || string.IsNullOrWhiteSpace(data.Username) || string.IsNullOrWhiteSpace(data.PasswordHash))
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 400,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Invalid input." })
                    };
                }

                bool success = await _authService.LoginAsync(data.Username, data.PasswordHash);

                if (!success)
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 401,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Password or Username is incorrect." })
                    };
                }

                Console.WriteLine($"[AuthController] Login with user was successful!");
                return new HttpResponseDTO
                {
                    StatusCode = 200,
                    JsonContent = JsonConvert.SerializeObject(new { success = true, message = $"You successfully logged in as user {data.Username}." })
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Error during Login: {ex.Message}");
                return new HttpResponseDTO
                {
                    StatusCode = 400,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" })
                };
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
                    /*return new HttpResponseDTO
                    {
                        StatusCode = 404,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = "No token provided." })
                    };*/
                    return false;
                }

                bool isTokenValid = await _authService.IsTokenValidAsync(token);

                if (!isTokenValid)
                {
                    Console.WriteLine("[AuthController] Token is not valid.");
                    /* return new HttpResponseDTO
                     {
                         StatusCode = 401,
                         JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Token is not valid." })
                     };*/
                    return false;
                }

                Console.WriteLine("[AuthController] Token is valid.");

                /*return new HttpResponseDTO
                {
                    StatusCode = 200,
                    JsonContent = JsonConvert.SerializeObject(new { success = true, error = "Token is valid." })
                };*/
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Error during token validation: {ex.Message}");
                /*return new HttpResponseDTO
                {
                    StatusCode = 500,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Error during token validation." })
                };*/
                return false;
            }
        }
    }
}
