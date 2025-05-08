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
        private readonly TournamentService _tournamentService;

        public PushUpRecordService(PushUpRecordRepository pushUpRecordRepository, UserRepository userRepository, AuthService authService, TournamentService tournamentService)
        {
            _pushUpRecordRepository = pushUpRecordRepository;
            _userRepository = userRepository;
            _authService = authService;
            _tournamentService = tournamentService;
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


                var userStats = await _authService.ValidateTokenDataAndGetUserStatsTournamentAsync(token);

                if (userStats == null)
                {
                    return false;
                }

                userStats.Count = pushUpRecord.Count;
                userStats.Duration = pushUpRecord.DurationInSeconds;
                await _tournamentService.AddOrUpdateStatsAsync(userStats);

                return await _pushUpRecordRepository.PostHistoryByUserIdAsync(userStats.UserId, pushUpRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordService] Error during PostHistoryByTokenHashAsync: {ex.Message}");
                return false;
            }
        }
    }
}
