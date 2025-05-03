using Npgsql;

namespace SportsBattleApp.Data
{
    public class DatabaseConnection
    {
        private string _connectionString;
        private NpgsqlConnection? _connection;

        // Constructor to initialize the connection string
        public DatabaseConnection()
        {
            _connectionString = "Host=localhost:5432;Username=stegmen;Password=passwort1;Database=sportsbattledb";
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

        // Makes sure to correctly dispose of the connection
        public void Dispose()
        {
            _connection?.Dispose();
            _connection = null;
            Console.WriteLine("Database connection disposed.");
        }
    }
}
