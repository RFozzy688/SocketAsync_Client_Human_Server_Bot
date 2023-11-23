using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

namespace ServerAsync
{
    internal class Program
    {
        static Random _random = new Random();
        static async Task Main(string[] args)
        {
            try
            {
                Console.Write("Input IP address: ");
                IPAddress ipAddress = IPAddress.Parse(Console.ReadLine());

                Console.Write("Input port: ");
                int port = Int32.Parse(Console.ReadLine());

                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

                using Socket server = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(ipEndPoint);
                server.Listen();

                Console.WriteLine("Waiting for connection on port {0}", ipEndPoint);

                Socket handler = await server.AcceptAsync();

                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                    string response = Encoding.Unicode.GetString(buffer, 0, received);
                    Console.WriteLine(response);

                    if (response.IndexOf("<Bye>") > -1)
                    {
                        byte[] bytes = Encoding.Unicode.GetBytes("Disconnected to client " + DateTime.Now.ToString());

                        await handler.SendAsync(bytes, SocketFlags.None);

                        break;
                    }

                    string str = "Server: " + RandomResponse();
                    byte[] msg = Encoding.Unicode.GetBytes(str);

                    await handler.SendAsync(msg, SocketFlags.None);
                    Console.WriteLine(str);
                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static string RandomResponse()
        {
            return "Response " + _random.Next(1, 10) + " " + DateTime.Now.ToString();
        }
    }
}