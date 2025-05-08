using Newtonsoft.Json;
using SportsBattleApp.DTOs;
using SportsBattleApp.Services;

namespace SportsBattleApp.Controllers
{
    public class TournamentController
    {
        private readonly TournamentService _tournamentService;
    
        public TournamentController(TournamentService tournamentService) 
        {
            _tournamentService = tournamentService;
        }

        public async Task<string> GetTournamentStatusAsync(string header)
        {
            try
            {
                var tournamentData = await _tournamentService.GetTournamentStatusAsync();
                if (tournamentData == null)
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "No current tournaments." });
                }

                Console.WriteLine($"[TournamentController] Getting tournament status was successful!");
                return JsonConvert.SerializeObject(new { success = true, tournamentData });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TournamentController] Error during StartTournamentAsync: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }
    }
}
