using Common.Libraries.USSD.Settings;
using Common.Libraries.USSD.Sockets.Framing;
using Common.Libraries.USSD.Sockets.Server;
using Common.Libraries.USSD.XML;
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
        private readonly ILogger _logger;
        private readonly IProtocolFramer _protocolFramer;
        private readonly IProcessRequest<UssdRequest> _processRequest;
        private readonly IProcessResponse<ServerRequest, ServerResponse> _processResponse;
        private readonly IResponseMapper<ServerResponse, UssdResponse> _responseMapper;
        private readonly IRequestMapper<UssdRequest, ServerRequest> _requestMapper;
        private readonly IUSSDSettings _ussdSettings;
        private readonly Dictionary<string, DateTime> _sessions;
        public TNMSocketServer(ILogger logger, IProtocolFramer protocolFramer,
            IProcessRequest<UssdRequest> processRequest,
            IProcessResponse<ServerRequest, ServerResponse> processResponse,
            IResponseMapper<ServerResponse, UssdResponse> responseMapper, IUSSDSettings ussdSettings,
            IRequestMapper<UssdRequest, ServerRequest> requestMapper) : base(IPAddress.Any/*,IPAddress.Parse(ussdSettings.IPAddress)*/, ussdSettings.IncomingPacketsPort, logger, protocolFramer)
        {
            _logger = logger;
            _protocolFramer = protocolFramer;
            _processRequest = processRequest;
            _processResponse = processResponse;
            _responseMapper = responseMapper;
            _ussdSettings = ussdSettings;
            _requestMapper = requestMapper;
            _sessions = new Dictionary<string, DateTime>();
        }

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

                //send a response after processing the message
                var request = _processRequest.Process(message);
                if (request == null)
                {
                    _logger.LogWarning("Failed to process request from {ClientId}: {Message}", clientId, message);
                    continue; // Skip sending a response if processing failed
                }
                //the request is what we send to server
                
                var serverRequest = await _requestMapper.Map(request);
                if (!_sessions.ContainsKey(serverRequest.SessionID))
                {
                    _sessions.Add(serverRequest.SessionID, DateTime.UtcNow);
                }
                else
                {
                    var sessionDate = _sessions[serverRequest.SessionID];
                    if (DateTime.UtcNow.Subtract(sessionDate) >= TimeSpan.FromMinutes(_ussdSettings.SessionMinutesToRefresh))
                    {
                        _sessions[serverRequest.SessionID] = DateTime.UtcNow;
                    }
                    serverRequest.FirstRequest = false;
                }

                //return session id, screen id, message, if response is required - to airtel this is a RESPONSE request_type ( this should be returned in the ServerResponse class
                //turn the ServerResponse to UssdResponse, then turn this to xml and then add headers
                var serverResponse = await _processResponse.Process(serverRequest);
                if (serverResponse == null)
                {
                    _logger.LogWarning("Failed to process server response for {ClientId}: {Message}", clientId, message);
                    continue; // Skip sending a response if processing failed
                }
                var ussdResponse = await _responseMapper.Map(serverResponse);
                if (ussdResponse == null)
                {
                    _logger.LogWarning("Failed to map server response to USSD response for {ClientId}: {Message}", clientId, message);
                    continue; // Skip sending a response if mapping failed
                }
                string xmlResponse = await _responseMapper.Process(ussdResponse);

                //sent to the client
                byte[] responseBytes = Encoding.UTF8.GetBytes(xmlResponse);
                await _protocolFramer.WriteMessageAsync(clientSocket, responseBytes, cancellationToken);

            }
        }
    }
}
