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
    static async Task Main(string[] args)
    {
        Console.Title = "TCP Client";

        Console.WriteLine($"Client connecting to server: {address}:{port}");
        using TcpClient client = new(address.ToString(), port);
        Console.WriteLine($"Client connected to server: {address}:{port}");

        // get client stream
        var stream = client.GetStream();

        // read data from stream
        byte[] buffer = new byte[1024];
        int bytesRecieved = await stream.ReadAsync(buffer);

        // convert to string 
        if (bytesRecieved == 0) {
            Console.WriteLine($"Zero bytes received.");
            return;
        }

        var message = Encoding.UTF8.GetString(buffer, 0, bytesRecieved);
        if (message == awaitedMessage) {
            Console.WriteLine("Message received!");
        }
        else {
            Console.WriteLine($"Wrong message received: {message}");
        }
    }
}