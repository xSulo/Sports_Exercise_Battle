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
                Console.WriteLine($"[UserRepository] Error in CreateUser: {ex.Message}");
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
                Console.WriteLine($"[UserRepository] Error in GetPasswordHashByUsername: {ex.Message}");
                return null;
            }
        }

        public async Task<UserProfileDTO> GetUserByUsernameAsync(string username)
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
                Console.WriteLine($"[UserRepository] Error in GetUser: {ex.Message}");
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
                Console.WriteLine($"[UserRepository] Error in AddTokenHashsync: {ex.Message}");
                return false;
            }
        }

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
                    TokenList.Add(new TokenHashAndExpireDateDTO
                    {
                        TokenHash = row["token_hash"].ToString(),
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

        public async Task<bool> UpdateUserProfileAsync(string username, User newUserData)
        {

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object> { 
                { "@usernameOld", username },
            };

            if (!string.IsNullOrEmpty(newUserData.Username))
            {
                setClauses.Add("username = @usernameNew");
                parameters["@usernameNew"] = newUserData.Username;
            }

            if (!string.IsNullOrEmpty(newUserData.PasswordHash))
            {
                setClauses.Add("password_hash = @passwordHash");
                parameters["@passwordHash"] = newUserData.PasswordHash;
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
            setClauses.Add("token_hash = @tokenHash");
            parameters["@tokenHash"] = DBNull.Value;

            setClauses.Add("token_expires_at = @tokenExpireDate");
            parameters["@tokenExpireDate"] = DBNull.Value;

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

        public async Task<int?> GetEloByUserIdAsync(int userId)
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
    }
}
