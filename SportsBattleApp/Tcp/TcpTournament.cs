using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SportsBattleApp.Tcp
{
    public class TcpTournament
    {
        private static bool _isRunning = false;
        private readonly int _port;

        public TcpTournament(int port = 10002)
        {
            _port = port;
        }

        public async Task StartAsync()
        {
            if (_isRunning) return;
            _isRunning = true;

            var listener = new TcpListener(IPAddress.Any, _port);
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            var token = cts.Token;

            try
            {
                listener.Start();
                Console.WriteLine("[TCP Server] TCP server started. Listening on port 10002.");

                while (!token.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        Console.WriteLine("[TCP Server] Client connected.");

                        _ = Task.Run(async () => await HandleClientAsync(client));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TCP Server] Error starting server: {ex.Message}");
            }
            finally
            {
                listener.Stop();
                Console.WriteLine("[TCP Server] TCP Listener stopped.");
                _isRunning = false;
            }
        }

        // Function to receive Push Up data for the current tournament
        // Add later
        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using var stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                var received = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine($"[TCP Server] Received data: {received}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TCP Server] Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
