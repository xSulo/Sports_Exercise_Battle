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

        public async Task<List<PushUpRecordGetHistoryDTO>> GetHistoryByUserIdAsync(int userId)
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
        public async Task<int?> GetTotalCountByUserIdAsync(int userId)
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
    }
}
