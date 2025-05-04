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

                //string tokenHash = CreateTokenHash(username);
                string token = CreateToken(username);
                //DateTime expireDateToken = CreateTokenExpiraryDate();

                //await _userRepository.UpdateTokenHashAsync(username, token, expireDateToken);
                await _userRepository.UpdateTokenHashAsync(username, token);

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

        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                if (token.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    token = token.Substring("Basic ".Length + 1);
                }
                
                // var storedTokenExpireDate = await _userRepository.GetTokenHashByTokenHashAsync(token);

                // I know that this doesnt make any sense, but Im probably failing this class cos I spent the entire day trying to fix this, worked perfeclty fine before -.-
                var storedToken = await _userRepository.GetTokenHashByTokenHashAsync(token);
                if (storedToken == null)
                {
                    throw new InvalidOperationException("Token not found.");
                }

                //if (storedTokenExpireDate < DateTime.UtcNow)
                //{
                //    throw new InvalidOperationException("Invalid token.");
                //}
                //return VerifyHash(token, storedToken);

                return token == storedToken;
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

        public static string CreateToken(string username)
        {
            return $"{username}-sebToken";
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
