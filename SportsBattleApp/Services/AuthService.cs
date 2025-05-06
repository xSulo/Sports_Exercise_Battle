using Newtonsoft.Json.Linq;
using SportsBattleApp.DTOs;
using SportsBattleApp.Models;
using SportsBattleApp.Repositories;
using static SportsBattleApp.Models.User;
using static System.Net.Mime.MediaTypeNames;

namespace SportsBattleApp.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        public AuthService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> LoginAsync(string username, string passwordHash)
        {
            try
            {
                var storedPasswordHash = await _userRepository.GetPasswordHashByUsernameAsync(username);
                if (await _userRepository.UserExistsAsync(username) != true)
                {
                    throw new InvalidOperationException("User does not exist.");
                }

                if (!VerifyHash(passwordHash, storedPasswordHash))
                {
                    throw new InvalidOperationException("Invalid password.");
                }

                string tokenHash = CreateTokenHash(username);
                //string token = CreateToken(username);
                DateTime expireDateToken = CreateTokenExpiraryDate();

                return await _userRepository.UpdateTokenHashAsync(username, tokenHash, expireDateToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during Login: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string username, string passwordHash)
        {
            try
            {
                if (await _userRepository.UserExistsAsync(username))
                {
                    throw new InvalidOperationException("Username already taken");
                }
                
                return await _userRepository.RegisterAsync(username, HashValue(passwordHash));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during Register: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                 var storedTokenList = await _userRepository.GetTokenDataAsync();

                foreach(var storedToken in storedTokenList)
                {
                    if (VerifyHash(RemoveTokenPrefix(token), storedToken.TokenHash))
                    {
                        if (storedToken.ExpireDate < DateTime.UtcNow)
                        {
                            throw new InvalidOperationException("Token expired. Login in order to get a new token.");
                        }

                        return true;
                    }
                }
                
                throw new InvalidOperationException("Token not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during token validation: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EditUserProfileAsync(string username, User newUserData)
        {
            try
            {
                if (!string.IsNullOrEmpty(newUserData.PasswordHash))
                {
                    newUserData.SetPasswordHash(HashValue(newUserData.PasswordHash));
                }

                return await _userRepository.UpdateUserProfileAsync(username, newUserData);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during Updating User Profile: {ex.Message}");
                return false;
            }
        }

        public int ValidateTokenDataAndGetUserId(string token, List<TokenDataAndUserIdDTO> TokenAndUserData)
        {
            try
            {
                foreach (var data in TokenAndUserData)
                {
                    if (VerifyHash(RemoveTokenPrefix(token), data.TokenHash))
                    {
                        if (data.ExpireDate < DateTime.UtcNow)
                        {
                            throw new InvalidOperationException("Token expired. Login in order to get a new token.");
                        }

                        return data.UserId;
                    }
                }
                throw new InvalidOperationException("Token invalid.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during Validating of token data: {ex.Message}");
                return 0;
            }
        }

        private static string HashValue(string plainTextValue)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainTextValue);
        }

        private static string RemoveTokenPrefix(string token)
        {
            if (token.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return token.Substring("Basic ".Length + 1);
            }
            return token;
        }

        private static string CreateTokenHash(string username)
        {
            return HashValue($"{username}-sebToken");
        }

        private static DateTime CreateTokenExpiraryDate()
        {
            return DateTime.UtcNow.AddHours(3);
        }

        private bool VerifyHash(string plainText, string Hash)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, Hash);
        }
    }
}
