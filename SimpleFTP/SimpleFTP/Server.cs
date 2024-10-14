using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace SimpleFTP;

public class Server(string localAddress, int port)
{
    private readonly TcpListener tcpListener = new (IPAddress.Parse(localAddress), port);
    private CancellationTokenSource cancellationToken;

    public void Start()
    {
        cancellationToken = new CancellationTokenSource();
        tcpListener.Start();
        _ = Listen();
    }

    public void Stop()
    {
        tcpListener.Stop();
    }

    private async Task HandleConnection(Socket clientConnection)
    {
        return;
    }

    private async Task Listen()
    {
        List<Task> clientConnectionsTasks = [];

        while (!cancellationToken.IsCancellationRequested)
        {
            Socket clientConnection = await tcpListener.AcceptSocketAsync(cancellationToken.Token);
            clientConnectionsTasks.Add(HandleConnection(clientConnection));
        }

        foreach (var clientConnectionTask in clientConnectionsTasks)
        {
            clientConnectionTask.Wait();
        }
    }
}
