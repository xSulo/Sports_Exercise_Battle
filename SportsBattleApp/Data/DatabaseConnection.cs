using Npgsql;

namespace SportsBattleApp.Data
{
    public class DatabaseConnection: IDisposable
    {
        private readonly string _connectionString;
        private NpgsqlConnection? _connection;

        // Constructor to initialize the connection string
        public DatabaseConnection()
        {
            _connectionString = "Host=localhost;Username=stegmen;Password=passwort1;Database=sportsbattledb";
        }

        // Method to establish connection
        public void OpenConnection()
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
            }

            try
            {
                _connection.Open();
                Console.WriteLine("Connection established succesfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while trying to establish connection: {ex.Message}");
            }
        }

        // Method to close the connection
        public void CloseConnection()
        {
            try
            {
                _connection?.Close();
                Console.WriteLine("Connection closed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while trying to close connection: {ex.Message}");
            }
        }

        // For INSERT, UPDATE and DELETE queries
        public void ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            if (_connection == null)
            {
                Console.WriteLine("Connection is not established.");
                return;
            }

            using var command = new NpgsqlCommand(query, _connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Query executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while executing query: {ex.Message}");
            }
        }

        // For SELECT queries for one item (e. g. password)
        public object? ExecuteScalar(string query, Dictionary<string, object> parameters)
        {
            if (_connection == null)
            {
                Console.WriteLine("Connection is not established.");
                return null;
            }

            using var command = new NpgsqlCommand(query, _connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            try
            {
                var result = command.ExecuteScalar();
                Console.WriteLine("Query executed successfully.");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while executing query: {ex.Message}");
                return null;
            }
        }

        // For SELECT queries for one row
        public List<Dictionary<string, object>> ExecuteReader(string query, Dictionary<string, object> parameters)
        {
            var result = new List<Dictionary<string, object>>();
            if (_connection == null)
            {
                Console.WriteLine("Connection is not established.");
                return result;
            }

            using var command = new NpgsqlCommand(query, _connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            try
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }

                    result.Add(row);
                }
                Console.WriteLine("Query executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while executing query: {ex.Message}");
            }

            return result;
        }



        // Makes sure to correctly dispose of the connection
        public void Dispose()
        {
            _connection?.Dispose();
            _connection = null;
            Console.WriteLine("Database connection disposed.");
        }
    }
}
