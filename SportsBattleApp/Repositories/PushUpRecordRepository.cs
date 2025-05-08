using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsBattleApp.Data;
using SportsBattleApp.DTOs;
using SportsBattleApp.Models;

namespace SportsBattleApp.Repositories
{
    public class PushUpRecordRepository
    {
        private readonly DatabaseConnection _db;
        public PushUpRecordRepository(DatabaseConnection db)
        {
            _db = db;
        }

        // GET for /history, in order to view users entire history
        public async Task<List<PushUpRecordGetHistoryDTO>> GetHistoryByUserIdAsync(int? userId)
        {
            string query = "SELECT count, duration FROM history WHERE user_id = @userId";
            var parameters = new Dictionary<string, object>
            {
                { "@userId", userId }
            };
            try
            {
                var result = await _db.ExecuteReaderAsync(query, parameters);

                if (result == null || result.Count == 0)
                {
                    return null;
                }

                var totalRecord = new List<PushUpRecordGetHistoryDTO>();

                foreach (var row in result)
                {
                    totalRecord.Add(new PushUpRecordGetHistoryDTO
                    {
                        PushUpCount = Convert.ToInt32(row["count"]),
                        DurationInSeconds = Convert.ToInt32(row["duration"])
                    });
                }

                return totalRecord;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordRepository] Error in GetHistoryByTokenHash: {ex.Message}");
                return null;
            }
        }

        // POST for /history, in order to add a new record to the history
        public async Task<bool> PostHistoryByUserIdAsync(int userId, PushUpRecordPostHistoryDTO data)
        {
            string query = "INSERT INTO history (user_id, tournament_number, name, count, duration) VALUES (@userId, @tournamentNumber, @name, @count, @duration)";
            var parameters = new Dictionary<string, object>
            {
                { "@userId", userId },
                { "@tournamentNumber", 0 }, // Implement later with tcp
                { "@name", data.Name },
                { "@count", data.Count },
                { "@duration", data.DurationInSeconds }
            };

            try
            {
                await _db.ExecuteNonQueryAsync(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordRepository] Error in PostHistoryByTokenHash: {ex.Message}");
                return false;
            }
        }

        // GET for /stats, in order to view user stats, this is used to get the total amount of push-ups
        public async Task<int?> GetTotalCountByUserIdAsync(int? userId)
        {
            string query = "SELECT SUM(count) AS total_count FROM history WHERE user_id = @userId";
            var parameters = new Dictionary<string, object>
            {
                { "@userId", userId }
            };
            try
            {
                var result = await _db.ExecuteScalarAsync(query, parameters);

                if (result == null)
                {
                    return null;
                }

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordRepository] Error in GetTotalCountByUserIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CountAndUserIdDTO>> GetAllPushUpsWithUserIdAsync()
        {
            string query = "SELECT user_id, SUM(count) AS totalpushups FROM history GROUP BY user_id";

            try
            {
                var results = await _db.ExecuteReaderAsync(query, new Dictionary<string, object>());

                if (results == null || results.Count == 0)
                {
                    return null;
                }

                var userDataScore = new List<CountAndUserIdDTO>();

                foreach (var row in results)
                {
                    userDataScore.Add(new CountAndUserIdDTO
                    {
                        UserId = Convert.ToInt32(row["user_id"]),
                        TotalPushUps = Convert.ToInt32(row["totalpushups"])
                    });
                }
                return userDataScore;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in GetAllPushUpsWithUserIdAsync: {ex.Message}");
                return null;
            }
        }
    }
}
