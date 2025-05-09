using System.Diagnostics;
using Newtonsoft.Json;
using SportsBattleApp.DTOs;
using SportsBattleApp.Services;
using SportsBattleApp.Tcp;
using static System.Formats.Asn1.AsnWriter;

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
        public async Task<HttpResponseDTO> GetStatsByTokenAsync(string header)
        {
            try
            {
                var stats = await _statsService.GetStatsByUserIdAsync(header);
                if (stats == null)
                {
                    string errorMsg = "No stats found.";
                    if (TournamentState.Instance.IsTournamentRunning)
                    {
                        errorMsg = "Cannot retrieve stats since Tournament is active.";
                    }

                    return new HttpResponseDTO
                    {
                        StatusCode = 404,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = errorMsg })
                    };
                }

                Console.WriteLine($"[UserController] Getting user stats was successful!");
                return new HttpResponseDTO
                {
                    StatusCode = 200,
                    JsonContent = JsonConvert.SerializeObject(new { success = true, stats })
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StatsController] Error during GetStatsByTokenAsync: {ex.Message}");
                return new HttpResponseDTO
                {
                    StatusCode = 500,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" })
                };
            }
        }

        public async Task<HttpResponseDTO> GetScoreByTokenAsync(string header)
        {
            try
            {
                var score = await _statsService.GetScoreByUserIdAsync(header);
                if (score == null)
                {
                    return new HttpResponseDTO
                    {
                        StatusCode = 404,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = "No scores found." })
                    };
                }

                Console.WriteLine($"[UserController] Getting user stats was successful!");
                return new HttpResponseDTO
                {
                    StatusCode = 200,
                    JsonContent = JsonConvert.SerializeObject(new { success = true, score })
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StatsController] Error during GetScoreByTokenAsync: {ex.Message}");
                return new HttpResponseDTO
                {
                    StatusCode = 500,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" })
                };
            }
        }
    }
}
