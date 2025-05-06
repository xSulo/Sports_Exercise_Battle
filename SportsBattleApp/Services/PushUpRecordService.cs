using Newtonsoft.Json.Linq;
using SportsBattleApp.DTOs;
using SportsBattleApp.Models;
using SportsBattleApp.Repositories;

namespace SportsBattleApp.Services
{
    public class PushUpRecordService
    {
        private readonly PushUpRecordRepository _pushUpRecordRepository;
        private readonly UserRepository _userRepository;
        private readonly AuthService _authService;
        public PushUpRecordService(PushUpRecordRepository pushUpRecordRepository, UserRepository userRepository, AuthService authService)
        {
            _pushUpRecordRepository = pushUpRecordRepository;
            _userRepository = userRepository;
            _authService = authService;
        }

        // GET for /history, in order to view users entire history
        public async Task<List<PushUpRecordGetHistoryDTO>> GetHistoryByUserIdAsync(string token)
        {
            try
            {
                var TokenDataAndUserId = await _userRepository.GetTokenDataAndUserIdAsync();

                int? userId = await _authService.ValidateTokenDataAndGetUserId(token);

                return await _pushUpRecordRepository.GetHistoryByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordService] Error during GetHistoryByTokenHashAsync: {ex.Message}");
                return null;
            }
        }

        // POST for /history, in order to add a new record to the history
        public async Task<bool> PostHistoryByUserIdAsync(string token, PushUpRecordPostHistoryDTO pushUpRecord)
        {
            try
            {
                var TokenDataAndUserId = await _userRepository.GetTokenDataAsync();


                int userId = await _authService.ValidateTokenDataAndGetUserId(token);

                if (userId == 0)
                {
                    return false;
                }

                return await _pushUpRecordRepository.PostHistoryByUserIdAsync(userId, pushUpRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordService] Error during PostHistoryByTokenHashAsync: {ex.Message}");
                return false;
            }
        }
    }
}
