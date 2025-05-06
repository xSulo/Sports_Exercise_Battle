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

        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                 var storedTokenList = await _userRepository.GetTokenDataAsync();

                if (storedTokenList == null || storedTokenList.Count == 0)
                {
                    return false;
                }

                foreach(var storedToken in storedTokenList)
                {
                    string tokenWithoutPrefix = _tokenService.RemoveTokenPrefix(token);
                    bool isTokenValid = _hashingService.VerifyHash(tokenWithoutPrefix, storedToken.TokenHash);

                    if (isTokenValid)
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

        public int ValidateTokenDataAndGetUserId(string token, List<TokenDataAndUserIdDTO> TokenAndUserData)
        {
            try
            {
                foreach (var data in TokenAndUserData)
                {
                    string tokenWithoutPrefix = _tokenService.RemoveTokenPrefix(token);
                    bool isTokenValid = _hashingService.VerifyHash(tokenWithoutPrefix, data.TokenHash);

                    if (isTokenValid)
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
    }
}
