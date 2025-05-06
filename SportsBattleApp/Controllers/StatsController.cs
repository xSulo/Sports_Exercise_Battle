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

        // GET for /stats, in order to view user stats
        public async Task<string> GetStatsByTokenAsync(string header)
        {
            try
            {
                var stats = await _statsService.GetStatsByUserIdAsync(header);
                if (stats == null)
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "No stats found." });
                }

                Console.WriteLine($"[UserController] Getting user stats was successful!");
                return JsonConvert.SerializeObject(new { success = true, stats });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StatsController] Error during GetStatsByTokenAsync: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }
    }
}
