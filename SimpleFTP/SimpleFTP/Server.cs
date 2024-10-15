using System.Net;
using System.Net.Sockets;

namespace SimpleFTP;

public class Server(string localAddress, int port)
{
    private readonly TcpListener tcpListener = new(IPAddress.Parse(localAddress), port);
    private CancellationTokenSource cancellationToken = new CancellationTokenSource();

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

    private RequestType GetRequestType(string input)
    {
        if (Enum.TryParse<RequestType>(input, out var requestType))
        {
            return requestType;
        }
        else
        {
            throw new ArgumentException("Invalid request type");
        }
    }

    private async Task ProcessRequestAsync(string request, NetworkStream stream)
    {
        var requestArgs = request.Split();
        if (requestArgs.Length != 2)
        {
            return;
        }

        var requestType = GetRequestType(requestArgs[0]);
        var path = requestArgs[1];

        switch (requestType)
        {
            case RequestType.List:
                await SendDirectoryList(stream, path);
                break;
            case RequestType.Get:
                SendFile(stream, path);
                break;
        }
    }

    private void SendFile(NetworkStream stream, string path)
    {
        using var writer = new BinaryWriter(stream);

        if (!File.Exists(path))
        {
            writer.Write(-1);
            return;
        }

        byte[] fileBytes = File.ReadAllBytes(path);
        writer.Write(fileBytes.Length);

        writer.Write(fileBytes);
    }

    private async Task SendDirectoryList(NetworkStream stream, string path)
    {
        using var writer = new StreamWriter(stream);

        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }

        var directoryContents = Directory.GetFileSystemEntries(path);
        await writer.WriteAsync(directoryContents.Length.ToString());
        foreach (var item in directoryContents)
        {
            await writer.WriteAsync($" {item} {Directory.Exists(item)}");
        }

        await writer.WriteAsync("\n");
    }

    private async Task HandleConnection(Socket clientConnection)
    {
        var stream = new NetworkStream(clientConnection);
        using var streamReader = new StreamReader(stream);
        var request = await streamReader.ReadLineAsync();

        if (request == null)
        {
            return;
        }

        await ProcessRequestAsync(request, stream);
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
