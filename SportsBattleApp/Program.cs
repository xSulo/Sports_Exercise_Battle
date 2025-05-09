using SportsBattleApp.Http;
using SportsBattleApp.Tcp;

using DatabaseConnection = SportsBattleApp.Data.DatabaseConnection;
DatabaseConnection dbConnection;
dbConnection = new DatabaseConnection();
/*dbConnection.OpenConnection();
dbConnection.CloseConnection();
dbConnection.Dispose();*/
TcpTournamentServerSingleton.Instance.Start();
var router = new RequestRouter(dbConnection);
var httpServer1 = new HttpServer(router);
await httpServer1.StartAsync();

//TcpTournamentServerSingleton.Instance.Start();



