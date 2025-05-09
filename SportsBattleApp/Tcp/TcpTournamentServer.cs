using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace SportsBattleApp.Tcp
{
    public class TcpTournamentServer
    {
        private readonly int _port;
        private TcpListener? _listener;
        private readonly ConcurrentBag<TcpTournamentClientHandler> _clients = new();

        public TcpTournamentServer(int port)
        {
            _port = port;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"[TcpTournamentServer] Server started on port {_port}...");

            _ = AcceptClientAsync();
        }

        public async Task UpdateEloAfterTournament()
        {

        }

        private async Task AcceptClientAsync()
        {
            while (true)
            {
                try
                { 
                    TcpClient client = await _listener!.AcceptTcpClientAsync();
                    var handler = new TcpTournamentClientHandler(client);
                    _clients.Add(handler);

                    Console.WriteLine("[TcpTournamentServer] Client connected");
                    _ = handler.HandleClientAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TcpTournamentServer] Error accepting client: {ex.Message}");
                }
            }
        }

        public void BroadcastMessage(string message)
        {
            foreach (var client in _clients)
            {
                client.SendMessageAsync(message);
            }
        }
    }
}
