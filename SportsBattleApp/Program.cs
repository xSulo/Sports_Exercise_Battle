using SportsBattleApp.Http;
using SportsBattleApp.Tcp;

using DatabaseConnection = SportsBattleApp.Data.DatabaseConnection;
DatabaseConnection dbConnection;
dbConnection = new DatabaseConnection();
/*dbConnection.OpenConnection();
dbConnection.CloseConnection();
dbConnection.Dispose();*/

var tcpServer = new TcpTournament();
await tcpServer.StartAsync();

var router = new RequestRouter();
var httpServer1 = new HttpServer(router);
await httpServer1.StartAsync();


