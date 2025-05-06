using SportsBattleApp.DTOs;
using SportsBattleApp.Repositories;

namespace SportsBattleApp.Services
{
    public class StatsService
    {
        private readonly UserRepository _userRepository;
        private readonly PushUpRecordRepository _pushUpRecordRepository;
        private readonly AuthService _authService;

        public StatsService(UserRepository userRepository, PushUpRecordRepository pushUpRecordRepository, AuthService authService)
        {
            _userRepository = userRepository;
            _pushUpRecordRepository = pushUpRecordRepository;
            _authService = authService;
        }

        // GET for /stats, in order to view user stats
        public async Task<GetStatsDTO> GetStatsByUserIdAsync(string token)
        {
            try
            {
                var TokenDataAndUserId = await _userRepository.GetTokenDataAndUserIdAsync();

                int? userId = await _authService.ValidateTokenDataAndGetUserId(token);

                var stats = new GetStatsDTO()
                {
                    Elo = await _userRepository.GetEloByUserIdAsync(userId),
                    TotalPushUps = await _pushUpRecordRepository.GetTotalCountByUserIdAsync(userId)
                };

                return stats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordService] Error during GetHistoryByTokenHashAsync: {ex.Message}");
                return null;
            }
        }
    }
}
