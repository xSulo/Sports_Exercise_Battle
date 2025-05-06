using Newtonsoft.Json;
using SportsBattleApp.DTOs;
using SportsBattleApp.Models;
using SportsBattleApp.Repositories;

namespace SportsBattleApp.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly HashingService _hashingService;

        public UserService(UserRepository userRepository, HashingService hashingService)
        {
            _userRepository = userRepository;
            _hashingService = hashingService;
        }

        public async Task<bool> EditUserProfileAsync(string username, User newUserData)
        {
            try
            {
                if (!string.IsNullOrEmpty(newUserData.PasswordHash))
                {
                    string passwordHash = _hashingService.HashValue(newUserData.PasswordHash);
                    newUserData.SetPasswordHash(passwordHash);
                }

                return await _userRepository.UpdateUserProfileAsync(username, newUserData);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Error during Updating User Profile: {ex.Message}");
                return false;
            }
        }

        public async Task<UserProfileDTO> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _userRepository.GetUserByUsernameAsync(username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error during Register: {ex.Message}");
                return null;
            }
        }
    }
}
