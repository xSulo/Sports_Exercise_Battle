using SportsBattleApp.DTOs;
using SportsBattleApp.Repositories;
using SportsBattleApp.Tcp;

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
                if (TournamentState.Instance.IsTournamentRunning)
                {
                    Console.WriteLine($"[PushUpRecordService] Tournament is active, cannot get stats.");
                    return null;
                }
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

        public async Task<Dictionary<string, UserScoreDTO>> GetScoreByUserIdAsync(string token)
        {
            try
            {
                var userData = await _userRepository.GetUserDataScoreAsync();
                var userIdAndCount = await _pushUpRecordRepository.GetAllPushUpsWithUserIdAsync();

                foreach (var pushUpRecord in userIdAndCount)
                {
                    int userId = pushUpRecord.UserId;
                    if (userData.ContainsKey(userId))
                    {
                        userData[userId].TotalPushUps += pushUpRecord.TotalPushUps;
                    }
                    else
                    {
                        userData[userId] = new UserScoreDTO
                        {
                            UserId = userId,
                            TotalPushUps = pushUpRecord.TotalPushUps,
                            Username = userData[userId].Username,
                            Elo = userData[userId].Elo
                        };
                    }
                }

                var sorted = userData.Values
                    .OrderByDescending(u => u.Elo)
                    .ThenByDescending(u => u.TotalPushUps)
                    .ToList();

                var rankedUsers = new Dictionary<string, UserScoreDTO>();
                for (int i = 0; i < sorted.Count; i++)
                {
                    rankedUsers.Add($"Rank {i + 1}", sorted[i]);
                }

                return rankedUsers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordService] Error during GetHistoryByTokenHashAsync: {ex.Message}");
                return null;
            }
        }
    }
}
