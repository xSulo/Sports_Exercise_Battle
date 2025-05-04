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

                if (!VerifyPassword(passwordHash, storedPasswordHash))
                {
                    throw new InvalidOperationException("Invalid password.");
                }

                string tokenHash = CreateTokenHash(username);
                DateTime expireDateToken = CreateTokenExpiraryDate();

                await _userRepository.AddTokenHashAsync(username, tokenHash, expireDateToken);

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
                Console.WriteLine($"[UserService] Error during Register: {ex.Message}");
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
            return DateTime.UtcNow.AddMinutes(3);
        }

        private bool VerifyPassword(string plainTextPassword, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(plainTextPassword, passwordHash);
        }
    }
}
