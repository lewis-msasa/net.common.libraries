using Common.Libraries.USSD.Sockets.Client;
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

namespace Common.Libraries.USSD.Example
{
    public class ExampleSocketServer : SocketTcpServer
    {
        public ExampleSocketServer(IPAddress ip, int port, ILogger logger, IProtocolFramer protocolFramer) : base(ip, port, logger, protocolFramer) { }

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

        public class EchoFramedClient : SocketTcpClient
        {
            public EchoFramedClient(string host, int port, ILogger logger, IProtocolFramer protocolFramer) : base(host, port, logger, protocolFramer)
            {
            }

            protected override async Task ProcessAsync(Socket socket, CancellationToken cancellationToken)
            {
                Console.WriteLine("Type messages to send. Type 'exit' to quit.");

                while (!cancellationToken.IsCancellationRequested)
                {
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input) || input.ToLower() == "exit")
                        break;

                    var messageBytes = Encoding.UTF8.GetBytes(input);
                    await _protocolFramer.WriteMessageAsync(socket, messageBytes, cancellationToken);

                    var responseBytes = await _protocolFramer.ReadMessageAsync(socket, cancellationToken);
                    var response = Encoding.UTF8.GetString(responseBytes);

                    _logger.LogInformation("Server responded: {Response}", response);
                    Console.WriteLine($"Server: {response}");
                }
            }
        }
    }
}
