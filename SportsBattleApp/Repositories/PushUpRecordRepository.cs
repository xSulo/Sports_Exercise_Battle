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

        public async Task<PushUpRecordGetHistoryDTO> GetHistoryByUserIdAsync(int userId)
        {
            string query = "SELECT SUM(count) AS total_count, SUM(duration) AS total_duration FROM history WHERE user_id = @userId";
            var parameters = new Dictionary<string, object>
            {
                { "@userId", userId }
            };
            try
            {
                var result = await _db.ExecuteReaderAsync(query, parameters);
                var row = result[0];

                var totalRecord = new PushUpRecordGetHistoryDTO
                {
                    TotalCount = Convert.ToInt32(row["total_count"]),
                    TotalDurationInSeconds = Convert.ToInt32(row["total_duration"])
                };

                return totalRecord;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordRepository] Error in GetHistoryByTokenHash: {ex.Message}");
                return null;
            }
        }

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
    }
}
