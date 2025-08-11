// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

try
{
    string xmlMessage = @"<ussd>
                          <sessionid>{session_id}</sessionid>
                          <type>{type}</type>
                          <msg>{message}</msg>
                          <msisdn>{msisdn}</msisdn>
                        </ussd>";
    using (var client = new TcpClient())
    {
        xmlMessage = xmlMessage.Replace("{session_id}", "12345")
            //.Replace("{type}", "1")
            //.Replace("{message}", "*777#")
            .Replace("{msisdn}", "0881286653");
        await client.ConnectAsync("127.0.0.1", 9001);
        using (var stream = client.GetStream())
        {
            while (true)
            {
                Console.Write("message> ");
                string message = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("Empty message, please type something.");
                    continue;
                }
                if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting.");
                    break;
                }
                Console.Write("type> ");
                string type = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(type))
                {
                    Console.WriteLine("Empty message, please type something.");
                    continue;
                }
                xmlMessage = xmlMessage.Replace("{message}", message).Replace("{type}", type);

                byte[] data = Encoding.ASCII.GetBytes(xmlMessage);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine("XML message sent.");
                byte[] messageReceived = new byte[1024];
                var result = await stream.ReadAsync(messageReceived);
                string response = Encoding.ASCII.GetString(messageReceived, 0, result);
                Console.WriteLine($"Server: {response}");
            }

        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
}
