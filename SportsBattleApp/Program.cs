using DatabaseConnection = SportsBattleApp.Data.DatabaseConnection;

Console.WriteLine("Hello, World!");
DatabaseConnection dbConnection;
dbConnection = new DatabaseConnection();
dbConnection.OpenConnection();
dbConnection.CloseConnection();
dbConnection.Dispose();
