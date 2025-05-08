using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using SportsBattleApp.Data;
using SportsBattleApp.DTOs;
using SportsBattleApp.Models;

namespace SportsBattleApp.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseConnection _db;
        public UserRepository(DatabaseConnection db)
        {
            _db = db;
        }

        // Checks if the user already exists in the database
        public async Task<bool> UserExistsAsync(string username)
        {
            string query = "SELECT COUNT(*) FROM users WHERE username = @username";
            var parameters = new Dictionary<string, object>
            {
                { "@username", username }
            };

            try
            {
                var result = await _db.ExecuteScalarAsync(query, parameters);
                return result != null && Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in UserExists: {ex.Message}");
                return false;
            }
        }

        // Creates the user in the database
        public async Task<bool> RegisterAsync(string username, string passwordHash)
        {
            try
            {
                string query = "INSERT INTO users (username, password_hash) VALUES (@username, @passwordHash)";
                var parameters = new Dictionary<string, object>
                {
                    { "@username", username },
                    { "@passwordHash", passwordHash }
                };

                await _db.ExecuteNonQueryAsync(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in RegisterAsync: {ex.Message}");
                return false;
            }
        }

        // Checks if the user already exists in the database
        public async Task<string> GetPasswordHashByUsernameAsync(string username)
        {
            string query = "SELECT password_hash FROM users WHERE username = @username";
            var parameters = new Dictionary<string, object>
            {
                { "@username", username }
            };

            try
            {
                var result = await _db.ExecuteScalarAsync(query, parameters);

                return result?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in GetPasswordHashByUsernameAsync: {ex.Message}");
                return null;
            }
        }

        // GET for /users/{username} aka profile, in order to view the profile
        public async Task<UserProfileDTO> GetUserProfileByUsernameAsync(string username)
        {
            string query = "SELECT * FROM users WHERE username = @username";
            var parameters = new Dictionary<string, object>
            {
                { "@username", username }
            };

            try
            {
                var result = await _db.ExecuteReaderAsync(query, parameters);
                var row = result[0];

                var userDto = new UserProfileDTO
                {
                    Username = row["username"].ToString(),
                    Elo = Convert.ToInt32(row["elo"]),
                    Image = row["image"].ToString(),
                    Bio = row["bio"].ToString(),
                    WinningSpeech = row["winning_speech"].ToString()
                };
                return userDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in GetUserProfileByUsernameAsync: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> UpdateTokenHashAsync(string username, string tokenHash, DateTime tokenExpiresAt)
        {
            string query = "UPDATE users SET token_hash = @tokenHash, token_expires_at = @tokenExpiresAt WHERE username = @username";
            var parameters = new Dictionary<string, object>
            {
                { "@username", username },
                { "@tokenHash", tokenHash },
                { "@tokenExpiresAt", tokenExpiresAt }
            };

            try
            {
                await _db.ExecuteNonQueryAsync(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in UpdateTokenHashAsync: {ex.Message}");
                return false;
            }
        }

        // Is used to check the token in case only the token is provided
        public async Task<List<TokenHashAndExpireDateDTO>> GetTokenDataAsync()
        {
            string query = "SELECT token_expires_at, token_hash FROM users";

            try
            {
                var results = await _db.ExecuteReaderAsync(query, new Dictionary<string, object>());

                if (results == null || results.Count == 0)
                {
                    return null;
                }

                var TokenList = new List<TokenHashAndExpireDateDTO>();

                foreach (var row in results)
                {
                    if (row == null)
                    {
                        continue;
                    }

                    var tokenHash = row["token_hash"]?.ToString();
                    var expireDateValue = row["token_expires_at"];

                    if (string.IsNullOrEmpty(tokenHash) || expireDateValue == DBNull.Value)
                    {
                        continue;
                    }

                    TokenList.Add(new TokenHashAndExpireDateDTO
                    {
                        TokenHash = tokenHash,
                        ExpireDate = Convert.ToDateTime(row["token_expires_at"])
                    });
                }

                return TokenList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in GetTokenDataAsync: {ex.Message}");
                return null;
            }
        }

        // PUT for /users/{username} aka profile, in order to change profile
        public async Task<bool> UpdateUserProfileAsync(string username, User newUserData)
        {
            bool isTokenChangeNeed = false;
            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object> {
                { "@usernameOld", username },
            };

            if (!string.IsNullOrEmpty(newUserData.Username))
            {
                setClauses.Add("username = @usernameNew");
                parameters["@usernameNew"] = newUserData.Username;
                isTokenChangeNeed = true;
            }

            if (!string.IsNullOrEmpty(newUserData.PasswordHash))
            {
                setClauses.Add("password_hash = @passwordHash");
                parameters["@passwordHash"] = newUserData.PasswordHash;
                isTokenChangeNeed = true;
            }

            if (!string.IsNullOrEmpty(newUserData.Bio))
            {
                setClauses.Add("bio = @bio");
                parameters["@bio"] = newUserData.Bio;
            }

            if (!string.IsNullOrEmpty(newUserData.Image))
            {
                setClauses.Add("image = @image");
                parameters["@image"] = newUserData.Image;
            }

            if (!string.IsNullOrEmpty(newUserData.WinningSpeech))
            {
                setClauses.Add("winning_speech = @winningSpeech");
                parameters["@winningSpeech"] = newUserData.WinningSpeech;
            }

            if (isTokenChangeNeed)
            {
                setClauses.Add("token_hash = @tokenHash");
                parameters["@tokenHash"] = DBNull.Value;

                setClauses.Add("token_expires_at = @tokenExpireDate");
                parameters["@tokenExpireDate"] = DBNull.Value;
            }

            if (setClauses.Count == 0)
            {
                Console.WriteLine("[UserRepository] No values provided to update.");
                return false;
            }

            string setClause = string.Join(", ", setClauses);
            string query = $"UPDATE users SET {setClause} WHERE username = @usernameOld";

            try
            {
                await _db.ExecuteNonQueryAsync(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in UpdateUserProfileAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<TokenDataAndUserIdDTO>> GetTokenDataAndUserIdAsync()
        {
            string query = "SELECT id, token_expires_at, token_hash FROM users";

            try
            {
                var results = await _db.ExecuteReaderAsync(query, new Dictionary<string, object>());

                if (results == null || results.Count == 0)
                {
                    return null;
                }

                var TokenListAndUserId = new List<TokenDataAndUserIdDTO>();

                foreach (var row in results)
                {
                    TokenListAndUserId.Add(new TokenDataAndUserIdDTO
                    {
                        UserId = Convert.ToInt32(row["id"]),
                        TokenHash = row["token_hash"].ToString(),
                        ExpireDate = Convert.ToDateTime(row["token_expires_at"])
                    });
                }

                return TokenListAndUserId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in GetTokenDataAndUserIdAsync: {ex.Message}");
                return null;
            }
        }

        // GET for /stats, in order to view user stats, this is used to get the elo of an user
        public async Task<int?> GetEloByUserIdAsync(int? userId)
        {
            string query = "SELECT elo FROM users WHERE id = @userId";

            var parameters = new Dictionary<string, object>
            {
                { "@userID", userId }
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
                Console.WriteLine($"[UserRepository] Error in GetEloByUserIdAsync: {ex.Message}");
                return null;
            }
        }

        // Used by authService to get the userId by tokenHash
        public async Task<int> GetUserIdByTokenHashAsync(string tokenHash)
        {
            string query = "SELECT id FROM users WHERE token_hash = @tokenHash";

            var parameters = new Dictionary<string, object>
            {
                { "@tokenHash", tokenHash }
            };

            try
            {
                var result = await _db.ExecuteScalarAsync(query, parameters);

                if (result == null)
                {
                    return 0;
                }

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in GetUserIdByTokenHsahAsync: {ex.Message}");
                return 0;
            }
        }

        // Used by pushUpRecordsService/authService to get user stats for the tournament
        public async Task<UserStatsTournamentDTO?> GetUserStatsTournamentByTokenHashAsync(string tokenHash)
        {
            string query = "SELECT id, username, elo, winning_speech FROM users WHERE token_hash = @tokenHash";

            var parameters = new Dictionary<string, object>
            {
                { "@tokenHash", tokenHash }
            };

            try
            {
                var result = await _db.ExecuteReaderAsync(query, parameters);
                var row = result[0];

                if (row == null)
                {
                    return null;
                }

                var userStatsDto = new UserStatsTournamentDTO
                {
                    UserId = Convert.ToInt32(row["id"]),
                    Username = row["username"].ToString(),
                    Elo = Convert.ToInt32(row["elo"]),
                    WinningSpeech = row["winning_speech"].ToString()
                };

                return userStatsDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in GetUserIdByTokenHsahAsync: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateEloTournament(int userId, int eloUpdate)
        {
            string query = "UPDATE users SET elo = elo + @eloUpdate WHERE id = @userId";
            var parameters = new Dictionary<string, object>
            {
                { "@userId", userId},
                {"@eloUpdate", eloUpdate }
            };

            try
            {
                await _db.ExecuteNonQueryAsync(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in UpdateEloTournament: {ex.Message}");
            }
        }

        public async Task<Dictionary<int, UserScoreDTO>> GetUserDataScoreAsync()
        {
            string query = "SELECT id, username, elo FROM users";

            try
            {
                var results = await _db.ExecuteReaderAsync(query, new Dictionary<string, object>());

                if (results == null || results.Count == 0)
                {
                    return null;
                }

                var userDataScore = new Dictionary<int, UserScoreDTO>();

                foreach (var row in results)
                {
                    var userId = Convert.ToInt32(row["id"]);
                    var userScoreDTO = new UserScoreDTO
                    { 
                        UserId = userId,
                        Username = row["username"].ToString(),
                        Elo = Convert.ToInt32(row["elo"])
                    };
                    userDataScore[userId] = userScoreDTO;
                }

                return userDataScore;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserRepository] Error in GetUserDataScoreAsync: {ex.Message}");
                return null;
            }
        }
    }
}
