using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer;

class TCPServer
{
    const int port = 8080;
    static readonly IPAddress address = IPAddress.Parse("127.0.0.1");
    const string messageToSend = "OK\n";
    static async Task Main(string[] args)
    {
        Console.Title = "TCP Server";

        using TcpListener server = new(address, port);
        server.Start();
        Console.WriteLine($"[SERVER] Listening on port: {port}");

        try
        {
            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine($"[SERVER] Client connected on {client.Client.RemoteEndPoint}");
                HandleClientAsync(client);
            }
        } catch (Exception e) { 
            Console.WriteLine($"Error while accepting message:{e.Message}");
        } finally { 
            Console.WriteLine(""); 
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using NetworkStream stream = client.GetStream();
            byte[] response = Encoding.UTF8.GetBytes(messageToSend);
            await stream.WriteAsync(response);
        } catch (OperationCanceledException e)
        {
            Console.Error.WriteLine($"Operation was cancelled for client {client.Client.RemoteEndPoint}: {e.Message}");
        }

        Console.WriteLine($"[SERVER] Client disconnected {client.Client.RemoteEndPoint}");
    }
}