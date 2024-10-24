// <copyright file="Server.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Server class.
/// </summary>
/// <param name="localAddress">The address of the server.</param>
/// <param name="port">The port of the server.</param>
public class Server(string localAddress, int port)
{
    private readonly TcpListener tcpListener = new (IPAddress.Parse(localAddress), port);
    private CancellationTokenSource cancellationToken = new ();

    /// <summary>
    /// The method that starts the server.
    /// </summary>
    public void Start()
    {
        this.cancellationToken = new CancellationTokenSource();
        this.tcpListener.Start();
        _ = this.Listen();
    }

    /// <summary>
    /// The method that stops the server.
    /// </summary>
    public void Stop()
    {
        this.tcpListener.Stop();
    }

    private static RequestType GetRequestType(string input)
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

    private static void SendFile(NetworkStream stream, string path)
    {
        using var writer = new BinaryWriter(stream);

        if (!File.Exists(path))
        {
            writer.Write(-1L);
            return;
        }

        long size = new FileInfo(path).Length;
        writer.Write(size);

        const int bufferSize = 4096;
        byte[] buffer = new byte[bufferSize];

        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        int bytesRead;

        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            writer.Write(buffer, 0, bytesRead);
        }
    }

    private static async Task SendDirectoryList(NetworkStream stream, string path)
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

    private async Task HandleConnection(Socket clientConnection)
    {
        var stream = new NetworkStream(clientConnection);
        using var streamReader = new StreamReader(stream);
        var request = await streamReader.ReadLineAsync();

        if (request == null)
        {
            return;
        }

        await this.ProcessRequestAsync(request, stream);
        clientConnection.Close();
    }

    private async Task Listen()
    {
        List<Task> clientConnectionsTasks = [];

        while (!this.cancellationToken.IsCancellationRequested)
        {
            Socket clientConnection = await this.tcpListener.AcceptSocketAsync(this.cancellationToken.Token);
            clientConnectionsTasks.Add(this.HandleConnection(clientConnection));
        }

        foreach (var clientConnectionTask in clientConnectionsTasks)
        {
            clientConnectionTask.Wait();
        }
    }
}
