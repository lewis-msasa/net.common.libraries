// See https://aka.ms/new-console-template for more information
using Common.Libraries.USSD;
using Common.Libraries.USSD.Airtel;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

try
{

  
    using (var client = new TcpClient())
    {
        
        await client.ConnectAsync("127.0.0.1", 9002);
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
                //Console.Write("type> ");
                //string type = Console.ReadLine();
                //if (string.IsNullOrWhiteSpace(type))
                //{
                //    Console.WriteLine("Empty message, please type something.");
                //    continue;
                //}
                var req = new UssdRequest
                {
                    Params = new List<Param>
                    {
                        new Param
                        {
                            Value = new Value
                            {
                                Struct = new Struct
                                {
                                    Members = new List<Member>
                                    {
                                        new Member { Name = "SESSION_ID", Value = new MemberValue { String = "12345" } },
                                        new Member { Name = "SEQUENCE", Value = new MemberValue { String = "1" } },
                                        new Member { Name = "USSD_BODY", Value = new MemberValue { String = message } },
                                        new Member { Name = "MOBILE_NUMBER", Value = new MemberValue { String = "0991286653" } },
                                        new Member { Name = "SEQUENCE_KEY", Value = new MemberValue { String = "1234" } },  //True or False
                                    }
                                }
                            }
                        }
                    }
                };
                var serializer = new XmlSerializer(typeof(UssdRequest));
                using var writer = new StringWriter();
                serializer.Serialize(writer, req);
                var xmlMessage = writer.ToString();

                byte[] data = Encoding.ASCII.GetBytes(xmlMessage);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine("XML message sent.");
                byte[] messageReceived = new byte[2048];
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
