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
        public async Task<PushUpRecordGetHistoryDTO> GetHistoryByTokenHashAsync(string tokenHash)
        {
            try
            {
                return await _pushUpRecordRepository.GetHistoryByTokenHashAsync(tokenHash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordService] Error during GetHistoryByTokenHashAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> PostHistoryByTokenHashAsync(string tokenHash, PushUpRecordPostHistoryDTO pushUpRecord)
        {
            try
            {
                var userId = await _userRepository.GetTokenDataAndUserIdAsync();

                int userId2 = Convert.ToInt32(userId);

                //if (userId == 0) return false;

                return await _pushUpRecordRepository.PostHistoryByTokenHashAsync(userId2, tokenHash, pushUpRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordService] Error during PostHistoryByTokenHashAsync: {ex.Message}");
                return false;
            }
        }
    }
}
