using Newtonsoft.Json;
using Npgsql;
using SportsBattleApp.Data;
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

        //public async Task<User> GetUserAsync(string username, string passwordHash)
        //{
        //    string query = "SELECT * FROM users WHERE username = @username AND password_hash = @passwordHash";
        //    var parameters = new Dictionary<string, object>
        //    {
        //        { "@username", username },
        //        { "@passwordHash", passwordHash }
        //    };

        //    try
        //    {
        //        var result = await _db.ExecuteReaderAsync(query, parameters);

        //        var row = result[0];


        //        User newUser = new User();


        //        //Id = Convert.ToInt32(row["id"]),
        //        newUser.Username = row["username"].ToString();
        //        newUser.SetPasswordHash(row["password_hash"].ToString());
        //        newUser.Elo = Convert.ToInt32(row["elo"]);
        //        newUser.SetTokenHash(row["token_hash"].ToString());
        //        newUser.Image = row["image"].ToString();
        //        newUser.Bio = row["bio"].ToString();

        //        return newUser;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[UserRepository] Error in GetUser: {ex.Message}");
        //        return null;
        //    }
        //}

        public async Task<bool> AddTokenHashAsync(string username, string tokenHash, DateTime tokenExpiresAt)
        {
         
            string query = "UPDATE users SET token_hash = @tokenHash, token_expires_at = @tokenExpiresAt WHERE username = @username";
            var parameters = new Dictionary<string, object>
            {
                { "@username", username },
                { "@tokenHash", tokenHash },
                { "@tokenExpiresAt", tokenExpiresAt }
            };
            Console.WriteLine($"Token expires at: {tokenExpiresAt}");

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
    }
}
