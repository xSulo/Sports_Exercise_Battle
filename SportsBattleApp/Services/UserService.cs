using Newtonsoft.Json;
using SportsBattleApp.DTOs;
using SportsBattleApp.Models;
using SportsBattleApp.Repositories;

namespace SportsBattleApp.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
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
