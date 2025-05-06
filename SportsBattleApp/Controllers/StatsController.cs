using Newtonsoft.Json;
using SportsBattleApp.Services;

namespace SportsBattleApp.Controllers
{
    public class StatsController
    {
        private readonly StatsService _statsService;

        public StatsController(StatsService statsService)
        {
            _statsService = statsService;
        }

        public async Task<string> GetStatsByTokenAsync(string header)
        {
            try
            {
                var stats = await _statsService.GetStatsByUserIdAsync(header);
                if (stats == null)
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "No stats found." });
                }
                return JsonConvert.SerializeObject(new { success = true, stats });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordController] Error during GetStatsyByTokenAsync: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }
    }
}
