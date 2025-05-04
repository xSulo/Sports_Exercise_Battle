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
                DateTime expireDateToken = CreateTokenExpiraryDate();

                await _userRepository.UpdateTokenHashAsync(username, tokenHash, expireDateToken);

                return true;
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

        public async Task<bool> IsTokenValidAsync(string token, string username)
        {
            try
            {
                Console.WriteLine(token);
                if (token.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    token = token.Substring("Basic ".Length);
                }
                Console.WriteLine(token);
                var storedTokenHash = await _userRepository.GetTokenHashByUsernameAsync(username);
                
                if (string.IsNullOrWhiteSpace(storedTokenHash.TokenHash))
                {
                    throw new InvalidOperationException("Token not found.");
                }

                if (storedTokenHash.TokenExpireDate < DateTime.UtcNow)
                {
                    throw new InvalidOperationException("Invalid token.");
                }

                return VerifyHash(token, storedTokenHash.TokenHash);
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
                //newUserData.SetTokenHash(HashValue(newUserData.Username));
                //newUserData.TokenExpiresAt = CreateTokenExpiraryDate();

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

        private static string HashValue(string plainTextValue)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainTextValue);
        }

        public static string CreateTokenHash(string username)
        {
            return HashValue($"{username}-sebToken");
        }

        private static DateTime CreateTokenExpiraryDate()
        {
            return DateTime.UtcNow.AddHours(3);
        }

        private static bool VerifyHash(string plainText, string Hash)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, Hash);
        }
    }
}
