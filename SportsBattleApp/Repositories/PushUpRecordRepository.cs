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

        public async Task<PushUpRecordGetHistoryDTO> GetHistoryByTokenHashAsync(string tokenHash)
        {
            string query = "SELECT SUM(count), SUM(duration) FROM history INNER JOIN users u ON u.token_hash = @tokenHash";
            var parameters = new Dictionary<string, object>
            {
                { "@tokenHash", tokenHash }
            };
            try
            {
                var result = await _db.ExecuteReaderAsync(query, parameters);
                var row = result[0];

                var totalRecord = new PushUpRecordGetHistoryDTO
                {
                    TotalCount = Convert.ToInt32(row["count"]),
                    TotalDurationInSeconds = Convert.ToInt32(row["duration"])
                };

                return totalRecord;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PushUpRecordRepository] Error in GetHistoryByTokenHash: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> PostHistoryByTokenHashAsync(int userId, string tokenHash, PushUpRecordPostHistoryDTO data)
        {
            Console.WriteLine($"[PushUpRecordRepository] PostHistoryByTokenHashAsync: userId={userId}, tokenHash={tokenHash}, name={data.Name}, count={data.Count}, duration={data.DurationInSeconds}");

            string query = "INSERT INTO history (user_id, tournament_number, name, count, duration) VALUES (@userId, @tournamentNumber, @name, @count, @duration)";
            var parameters = new Dictionary<string, object>
            {
                { "@userId", userId },
                { "@tournamentNumber", 0 },
                { "@name", data.Name },
                { "@count", data.Count },
                { "@duration", data.DurationInSeconds },
                { "@tokenHash", tokenHash }
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
