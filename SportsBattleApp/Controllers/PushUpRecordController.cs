using Newtonsoft.Json;
using SportsBattleApp.DTOs;
using SportsBattleApp.Models;
using SportsBattleApp.Services;
using SportsBattleApp.Tcp;

namespace SportsBattleApp.Controllers
{
    public class PushUpRecordController
    {
        private readonly PushUpRecordService _pushUpRecordService;

        public PushUpRecordController(PushUpRecordService pushUpRecordService)
        {
            _pushUpRecordService = pushUpRecordService;
        }

        // GET for /history, in order to view users entire history
        public async Task<HttpResponseDTO> GetHistoryByTokenAsync(string header)
        {
            try
            {
                var history = await _pushUpRecordService.GetHistoryByUserIdAsync(header);
                if (history == null)
                {
                    string errorMsg = "No history found.";
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

                Console.WriteLine($"[UserController] Getting user history was successful!");
                return new HttpResponseDTO
                {
                    StatusCode = 200,
                    JsonContent = JsonConvert.SerializeObject(new { success = true, history })
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordController] Error during GetHistoryByUsernameAsync: {ex.Message}");
                return new HttpResponseDTO
                {
                    StatusCode = 500,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" })
                };
            }
        }

        // POST for /history, in order to add a new record to the history
        public async Task<HttpResponseDTO> PostHistoryByTokenAsync(string token, string body)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<PushUpRecordPostHistoryDTO>(body);
                if (data == null || string.IsNullOrWhiteSpace(data.Name) || data.Count <= 0 || data.DurationInSeconds <= 0)
                { 
                    return new HttpResponseDTO
                    {
                        StatusCode = 401,
                        JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Invalid input." })
                    };
                }

                bool success = await _pushUpRecordService.PostHistoryByUserIdAsync(token, data);

                if (success)
                {
                    Console.WriteLine($"[UserController] Adding user record to history was successful!");
                    return new HttpResponseDTO
                    {
                        StatusCode = 201,
                        JsonContent = JsonConvert.SerializeObject(new { success, message = $"New record has been created successfully." })
                    };
                }

                Console.WriteLine("[PushUpRecordController] Error during PostHistoryByTokenAsync");
                return new HttpResponseDTO
                {
                    StatusCode = 500,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" })
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordController] Error during PostHistoryByTokenAsync: {ex.Message}");
                return new HttpResponseDTO
                {
                    StatusCode = 500,
                    JsonContent = JsonConvert.SerializeObject(new { success = false, error = "Internal Server Error" })
                };
            }
        }
    }
}
