using Newtonsoft.Json;
using SportsBattleApp.DTOs;
using SportsBattleApp.Models;
using SportsBattleApp.Services;

namespace SportsBattleApp.Controllers
{
    public class PushUpRecordController
    {
        private readonly PushUpRecordService _pushUpRecordService;

        public PushUpRecordController(PushUpRecordService pushUpRecordService)
        {
            _pushUpRecordService = pushUpRecordService;
        }

        public async Task<string> GetHistoryByTokenHashAsync(string header)
        {
            try
            {
                var history = await _pushUpRecordService.GetHistoryByTokenHashAsync(header);
                if (history == null)
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "No history found." });
                }
                return JsonConvert.SerializeObject(new { success = true, history });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordController] Error during GetHistoryByUsernameAsync: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }

        public async Task<string> PostHistoryByTokenHashAsync(string token, string body)
        {
            try
            {
                if (token.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    token = token.Substring("Basic ".Length + 1);
                }

                var data = JsonConvert.DeserializeObject<PushUpRecordPostHistoryDTO>(body);
                if (data == null || string.IsNullOrWhiteSpace(data.Name) || data.Count <= 0 || data.DurationInSeconds <= 0)
                {
                    return JsonConvert.SerializeObject(new { success = false, error = "Invalid input." });
                }

                bool success = await _pushUpRecordService.PostHistoryByTokenHashAsync(token, data);
                return JsonConvert.SerializeObject(new { success });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordController] Error during PostHistoryByTokenHashAsync: {ex.Message}");
                return JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" });
            }
        }
    }
}
