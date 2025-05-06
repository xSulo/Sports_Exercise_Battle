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
        private readonly HashingService _hashingService;
        private readonly TokenService _tokenService;
        public AuthService(UserRepository userRepository, HashingService hashingfunction, TokenService tokenService)
        {
            _userRepository = userRepository;
            _hashingService = hashingfunction;
            _tokenService = tokenService;
        }


        // POST for /sessions aka login, in order to login a user, also creates a token
        public async Task<bool> LoginAsync(string username, string plainPassword)
        {
            try
            {
                var storedPasswordHash = await _userRepository.GetPasswordHashByUsernameAsync(username);
                if (await _userRepository.UserExistsAsync(username) != true)
                {
                    throw new InvalidOperationException("User does not exist.");
                }

                bool isPasswordValid = _hashingService.VerifyHash(plainPassword, storedPasswordHash);
                if (!isPasswordValid)
                {
                    throw new InvalidOperationException("Invalid password.");
                }

                string token = _tokenService.CreateToken(username);
                string tokenHash = _hashingService.HashValue(token);
                DateTime expireDateToken = _tokenService.CreateTokenExpiraryDate();

                return await _userRepository.UpdateTokenHashAsync(username, tokenHash, expireDateToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during Login: {ex.Message}");
                return false;
            }
        }

        // POST for /users aka register, in order to register a new user
        public async Task<bool> RegisterAsync(string username, string plainPassword)
        {
            try
            {
                if (await _userRepository.UserExistsAsync(username))
                {
                    throw new InvalidOperationException("Username already taken");
                }

                string passwordHash = _hashingService.HashValue(plainPassword);
                return await _userRepository.RegisterAsync(username, passwordHash);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during Register: {ex.Message}");
                return false;
            }
        }

        // Used everytime there is a request, checks if the token is valid
        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                 var storedTokenList = await _userRepository.GetTokenDataAsync();

                if (storedTokenList == null || storedTokenList.Count == 0)
                {
                    return false;
                }

                var result = _tokenService.ValidateToken(token, storedTokenList, false);

                if (result.IsValid)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during token validation: {ex.Message}");
                return false;
            }
        }

        // Used by various services to get the userId from the token
        public async Task<int> ValidateTokenDataAndGetUserId(string token)
        {
            try
            {
                var storedTokenList = await _userRepository.GetTokenDataAsync();

                if (storedTokenList == null || storedTokenList.Count == 0)
                {
                    return 0;
                }

                var result = _tokenService.ValidateToken(token, storedTokenList, true);

                if (!result.IsValid)
                {
                    return 0;
                }

                return await _userRepository.GetUserIdByTokenHashAsync(result.TokenHash);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during Validating of token data: {ex.Message}");
                return 0;
            }
        }
    }
}
