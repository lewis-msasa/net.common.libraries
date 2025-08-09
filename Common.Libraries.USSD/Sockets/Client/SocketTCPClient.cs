using Common.Libraries.USSD.Sockets.Framing;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.Sockets.Client
{
 

    public abstract class SocketTcpClient : IDisposable
    {
        protected Socket _clientSocket;
        private readonly string _host;
        private readonly int _port;
        private CancellationTokenSource _cts;
        protected readonly ILogger _logger;
        protected readonly IProtocolFramer _protocolFramer;
        protected SocketTcpClient(string host, int port, ILogger logger, IProtocolFramer protocolFramer)
        {
            _host = host;
            _port = port;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _protocolFramer = protocolFramer ?? throw new ArgumentNullException(nameof(protocolFramer));
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            await Task.Factory.FromAsync(
                _clientSocket.BeginConnect,
                _clientSocket.EndConnect,
                _host, _port, null);

            _logger.LogInformation("Connected to {Host}:{Port}", _host, _port);

            await ProcessAsync(_clientSocket, _cts.Token);
        }

        public async Task SendMessageAsync(byte[] message, CancellationToken cancellationToken = default)
        {
            var lengthBuffer = BitConverter.GetBytes(message.Length);
            await _clientSocket.SendAsync(lengthBuffer, SocketFlags.None, cancellationToken);
            await _clientSocket.SendAsync(message, SocketFlags.None, cancellationToken);
        }

        public async Task<byte[]> ReceiveMessageAsync(CancellationToken cancellationToken = default)
        {
            var lengthBuffer = new byte[4];
            await ReceiveExactAsync(lengthBuffer, 0, 4, cancellationToken);

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            if (messageLength <= 0 || messageLength > 10_000_000)
                throw new InvalidOperationException($"Invalid message length: {messageLength}");

            var messageBuffer = new byte[messageLength];
            await ReceiveExactAsync(messageBuffer, 0, messageLength, cancellationToken);

            return messageBuffer;
        }

        private async Task ReceiveExactAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
        {
            int received = 0;
            while (received < size)
            {
                int bytes = await _clientSocket.ReceiveAsync(buffer.AsMemory(offset + received, size - received), SocketFlags.None, cancellationToken);
                if (bytes == 0)
                    throw new SocketException((int)SocketError.ConnectionReset);
                received += bytes;
            }
        }

        /// <summary>
        /// Override to implement your client logic with framing
        /// </summary>
        protected abstract Task ProcessAsync(Socket socket, CancellationToken cancellationToken);

        public void Dispose()
        {
            try
            {
                if (_clientSocket != null)
                {
                    if (_clientSocket.Connected)
                    {
                        _clientSocket.Shutdown(SocketShutdown.Both);
                    }
                    _clientSocket.Close();
                    _clientSocket.Dispose();
                }
            }
            catch
            {
                // ignore
            }
        }
    }

}
