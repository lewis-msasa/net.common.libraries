using Common.Libraries.USSD.Sockets.Framing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.Sockets.Server
{
    public abstract class SocketTcpServer : IDisposable
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private Socket _listenerSocket;
        private CancellationTokenSource _cts;
        protected readonly ILogger _logger;
        protected readonly IProtocolFramer _protocolFramer;

        // Track connected clients
        protected ConcurrentDictionary<Guid, Socket> ConnectedClients { get; } = new();

        protected SocketTcpServer(IPAddress ipAddress, int port, ILogger logger, IProtocolFramer protocolFramer)
        {
            _ipAddress = ipAddress;
            _port = port;
            _protocolFramer = protocolFramer;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenerSocket.Bind(new IPEndPoint(_ipAddress, _port));
            _listenerSocket.Listen(100);

            _logger.LogInformation("Server listening on {IpAddress}:{Port}", _ipAddress, _port);

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var clientSocket = await Task.Factory.FromAsync(
                        _listenerSocket.BeginAccept,
                        _listenerSocket.EndAccept,
                        null);

                    var clientId = Guid.NewGuid();
                    ConnectedClients[clientId] = clientSocket;

                    _logger.LogInformation("Client connected: {ClientId}, Total clients: {Count}", clientId, ConnectedClients.Count);

                    _ = Task.Run(() => HandleClientWrapperAsync(clientId, clientSocket), _cts.Token);
                }
            }
            catch (ObjectDisposedException)
            {
                // Socket closed normally on shutdown
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
            try
            {
                foreach (var kvp in ConnectedClients)
                {
                    try
                    {
                        kvp.Value.Shutdown(SocketShutdown.Both);
                        kvp.Value.Close();
                    }
                    catch { }
                }
                ConnectedClients.Clear();

                _listenerSocket?.Close();
                _logger.LogInformation("Server stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during server stop.");
            }
        }

        private async Task HandleClientWrapperAsync(Guid clientId, Socket clientSocket)
        {
            try
            {
                await HandleClientAsync(clientId, clientSocket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling client {ClientId}", clientId);
            }
            finally
            {
                if (clientSocket.Connected)
                {
                    try
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                    }
                    catch { }

                    clientSocket.Close();
                }
                ConnectedClients.TryRemove(clientId, out _);
                _logger.LogInformation("Client disconnected: {ClientId}, Total clients: {Count}", clientId, ConnectedClients.Count);
            }
        }
        public void Dispose()
        {
            try
            {
                if (_listenerSocket != null)
                {
                    if (_listenerSocket.Connected)
                    {
                        _listenerSocket.Shutdown(SocketShutdown.Both);
                    }
                    _listenerSocket.Close();
                    _listenerSocket.Dispose();
                }
            }
            catch
            {
                // ignore exceptions on dispose
            }
        }

        /// <summary>
        /// Override to implement client handling.
        /// Use provided framing helpers below for receiving/sending length-prefixed messages.
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="clientSocket">Connected socket</param>
        protected abstract Task HandleClientAsync(Guid clientId, Socket clientSocket);


    }

}
