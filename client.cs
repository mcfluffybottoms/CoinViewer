using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient;

class TCPClient
{
    const int port = 8080;
    static readonly IPAddress address = IPAddress.Parse("127.0.0.1");
    const string awaitedMessage = "OK\n";
    const int BUFFER_SIZE = 1024;
    static async Task<int> Main(string[] args)
    {
        Console.Title = "TCP Client";

        using TcpClient? client = ConnectToServer();
        if (client == null)
        {
            return 1;
        }

        string message = await ReadData(client);

        if (message.Length == 0)
        {
            Console.WriteLine($"Empty message received.");
            return 1;
        }

        if (message == awaitedMessage)
        {
            Console.WriteLine("Message received!");
            return 0;
        }
        else
        {
            Console.WriteLine($"Wrong message received: {message}");
            return 1;
        }
    }

    private static TcpClient? ConnectToServer()
    {
        Console.WriteLine($"Client connecting to server: {address}:{port}");
        TcpClient client = new();
        try
        {
            client.Connect(address.ToString(), port);
        }
        catch (SocketException e)
        {
            Console.Error.WriteLine($"SocketException while connecting to server: {e.Message}.");
            return null;
        }
        Console.WriteLine($"Client connected to server: {address}:{port}");
        return client;
    }

    private static async Task<string> ReadData(TcpClient client)
    {
        var stream = client.GetStream();

        using MemoryStream memoryStream = new();
        byte[] buffer = new byte[BUFFER_SIZE];
        int bytesReceived;

        while (true)
        {
            try
            {
                bytesReceived = await stream.ReadAsync(buffer);
            } catch (OperationCanceledException e)
            {
                Console.Error.WriteLine($"Operation was cancelled: {e.Message}");
                break;
            } catch (IOException e) when (e.InnerException is SocketException)
            {
                Console.Error.WriteLine($"Socket error: {e.Message}");
                break;
            }
            catch (IOException e)
            {
                Console.Error.WriteLine($"Error while reading from stream: {e.Message}");
                break;
            } catch (ObjectDisposedException e)
            {
                Console.Error.WriteLine($"NetworkStream was closed: {e.Message}");
                break;
            }

            if(bytesReceived == 0)
            {
                break;
            }
            memoryStream.Write(buffer, 0, bytesReceived);
        }

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}