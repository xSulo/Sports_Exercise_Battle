using Npgsql;

namespace SportsBattleApp.Data
{
    public class DatabaseConnection //: IDisposable
    {
        private readonly string _connectionString;
        //private NpgsqlConnection? _connection;

        // Constructor to initialize the connection string
        public DatabaseConnection()
        {
            _connectionString = "Host=localhost;Username=stegmen;Password=passwort1;Database=sportsbattledb";
        }

        // For INSERT, UPDATE and DELETE queries
        public async Task ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                Console.WriteLine("[Database] Database connection established successfully.");


                await using var command = new NpgsqlCommand(query, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("[Database] Query executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Database] Error while executing query: {ex.Message}");
            }
        }

        // For SELECT queries for one item (e. g. password)
        public async Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object> parameters)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                Console.WriteLine("[Database] Database connection established successfully.");

                await using var command = new NpgsqlCommand(query, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }


                var result = await command.ExecuteScalarAsync();
                Console.WriteLine("[Database] Query executed successfully.");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Database] Error while executing query: {ex.Message}");
                return null;
            }
        }

        // SELECT queries for multiple rows
        public async Task<List<Dictionary<string, object>>> ExecuteReaderAsync(string query, Dictionary<string, object> parameters)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                Console.WriteLine("[Database] Database connection established successfully.");

                await using var command = new NpgsqlCommand(query, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }

           
                using var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }

                    result.Add(row);
                }
                Console.WriteLine("[Database] Query executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Database] Error while executing query: {ex.Message}");
            }
            return result;
        }
    }
}
