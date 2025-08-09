using Common.Libraries.USSD.Sockets.Framing;
using Common.Libraries.USSD.Sockets.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.TNM
{
    public class TNMSocketServer : SocketTcpServer
    {
        public TNMSocketServer(IPAddress ip, int port, ILogger logger, IProtocolFramer protocolFramer) : base(ip, port, logger, protocolFramer) { }

        protected override async Task HandleClientAsync(Guid clientId, Socket clientSocket)
        {
            var cancellationToken = CancellationToken.None;

            while (true)
            {
                byte[] messageBytes;
                try
                {
                    messageBytes = await _protocolFramer.ReadMessageAsync(clientSocket, cancellationToken);
                }
                catch
                {
                    // Client disconnected or error reading
                    break;
                }

                string message = Encoding.UTF8.GetString(messageBytes);
                _logger.LogInformation("Received from {ClientId}: {Message}", clientId, message);

                string response = "Echo: " + message;
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await _protocolFramer.WriteMessageAsync(clientSocket, responseBytes, cancellationToken);
            }
        }
    }
}
