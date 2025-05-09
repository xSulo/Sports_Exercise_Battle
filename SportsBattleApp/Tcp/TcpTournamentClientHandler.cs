using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using SportsBattleApp.Controllers;
using SportsBattleApp.DTOs;
//using SportsBattleApp.TournamentCore;

namespace SportsBattleApp.Tcp
{
    public class TcpTournamentClientHandler
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private bool _isConnected = true;
        private bool _isEventHandlerRegistered = false;
        private readonly Action<string> _broadcastHandler;

        public TcpTournamentClientHandler(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            _broadcastHandler = async (msg) => await SendMessageAsync(msg);
            TournamentState.Instance.OnEventBroadcast += _broadcastHandler;
        }

        public async Task HandleClientAsync()
        {
            void BroadcastHandler(string msg) => _ = SendMessageAsync(msg);

            try
            { 
                var log = TournamentState.Instance.GetEventLog();
                foreach (var entry in log)
                {
                    if (!_client.Connected)
                    {
                        break;
                    }
                    await SendMessageAsync(entry);
                }

                if (!_isEventHandlerRegistered)
                { 
                    _isEventHandlerRegistered = true;
                }

                byte[] buffer = new byte[1];
                while (_isConnected && _client.Connected)
                {
                    if (_stream.DataAvailable)
                    {
                        int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                    }
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TcpTournamentClientHandler] Error: {ex.Message}");
            }
            finally
            {
                _isConnected = false;
                TournamentState.Instance.OnEventBroadcast -= BroadcastHandler;
                _client.Close();
                Console.WriteLine("[TcpTournamentClientHandler] Client disconnected");
                _isEventHandlerRegistered = false;
            }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            { 
                if (!_client.Connected)
                {
                    return;
                }

                byte[] buffer = Encoding.UTF8.GetBytes($"{message} \n");
                await _stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TcpTournamentClientHandler] Error sending message: {ex.Message}");
            }
        }
    }
}
