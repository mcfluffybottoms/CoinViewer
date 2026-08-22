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
    static async Task<int> Main(string[] args)
    {
        Console.Title = "TCP Server";

        using TcpListener server = new(address, port);
        server.Start();
        Console.WriteLine($"[SERVER] Listening on port: {port}");

        try
        {
            while (true)
            {
                await HandleClientAsync(server);
            }
        } catch (Exception e) { 
            Console.Error.WriteLine($"[SERVER] Error while accepting message: {e.Message}");
            return 1;
        }
    }

    private static async Task HandleClientAsync(TcpListener server)
    {
        try
        {
            using TcpClient client = await server.AcceptTcpClientAsync();
            string clientEndPoint = client.Client?.RemoteEndPoint?.ToString() ?? "Unknown";
            Console.WriteLine($"[SERVER] Client connected on {clientEndPoint}");

            try
            {
                using NetworkStream stream = client.GetStream();
                byte[] response = Encoding.UTF8.GetBytes(messageToSend);
                await stream.WriteAsync(response);
            } catch (OperationCanceledException e)
            {
                Console.Error.WriteLine($"[SERVER] Operation was cancelled for client {clientEndPoint}: {e.Message}");
            } catch (IOException e) when (e.InnerException is SocketException)
            {
                Console.Error.WriteLine($"[SERVER] Socket error for {clientEndPoint}: {e.Message}");
            } catch (IOException e)
            {
                Console.Error.WriteLine($"[SERVER] Error while writing into stream for {clientEndPoint}: {e.Message}");
            } catch (ObjectDisposedException e)
            {
                Console.Error.WriteLine($"[SERVER] NetworkStream was closed for {clientEndPoint}: {e.Message}");
            } finally
            {
                Console.WriteLine($"[SERVER] Client {clientEndPoint} disconnected");
            }
        } catch (SocketException e)
        {
            Console.Error.WriteLine($"[SERVER] Socket error while accepting client: {e.Message}");
            return;
        }
    }
}