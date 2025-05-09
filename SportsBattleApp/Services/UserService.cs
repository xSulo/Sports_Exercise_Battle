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

        // GET for /users/{username} aka profile, in order to view the profile
        public async Task<UserProfileDTO> GetUserProfileByUsernameAsync(string username)
        {
            try
            {
                return await _userRepository.GetUserProfileByUsernameAsync(username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error during GetUserByUsernameAsync: {ex.Message}");
                return null;
            }
        }

        // PUT for /users/{username} aka profile, in order to change profile
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
                Console.WriteLine($"[UserService] Error during EditUserProfileAsync: {ex.Message}");
                return false;
            }
        }
    }
}
