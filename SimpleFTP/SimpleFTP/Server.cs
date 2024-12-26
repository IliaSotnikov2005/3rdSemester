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
    private Task? listenTask;

    /// <summary>
    /// The method that starts the server.
    /// </summary>
    public void Start()
    {
        this.cancellationToken = new CancellationTokenSource();
        this.tcpListener.Start();
        this.listenTask = this.Listen();
    }

    /// <summary>
    /// The method that stops the server.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task StopAsync()
    {
        this.cancellationToken.Cancel();
        this.tcpListener.Stop();

        if (this.listenTask != null)
        {
            await this.listenTask;
        }
    }

    private static RequestType? GetRequestType(string input)
    {
        if (Enum.TryParse<RequestType>(input, out var requestType))
        {
            return requestType;
        }
        else
        {
            return null;
        }
    }

    private static async Task SendFileAsync(NetworkStream stream, string path)
    {
        using var writer = new BinaryWriter(stream);

        if (!File.Exists(path))
        {
            writer.Write(-1L);
            return;
        }

        long size = new FileInfo(path).Length;
        writer.Write(size);

        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

        await fileStream.CopyToAsync(stream);
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

    private static async Task SendMessageAsync(NetworkStream stream, string message)
    {
        using var writer = new StreamWriter(stream);

        await writer.WriteAsync(message + "\n");
    }

    private static async Task ProcessRequestAsync(string request, NetworkStream stream)
    {
        var requestArgs = request.Split();
        if (requestArgs.Length != 2)
        {
            await SendMessageAsync(stream, "Error: expected 2 arguments.");
            return;
        }

        var requestType = GetRequestType(requestArgs[0]);
        if (requestType == null)
        {
            await SendMessageAsync(stream, "Error: unknown request type.");
            return;
        }

        var path = requestArgs[1];

        switch (requestType)
        {
            case RequestType.List:
                await SendDirectoryList(stream, path);
                break;
            case RequestType.Get:
                await SendFileAsync(stream, path);
                break;
        }
    }

    private static async Task HandleConnection(Socket clientConnection)
    {
        var stream = new NetworkStream(clientConnection);
        using var streamReader = new StreamReader(stream);
        var request = await streamReader.ReadLineAsync();

        if (request == null)
        {
            return;
        }

        await ProcessRequestAsync(request, stream);
        clientConnection.Close();
    }

    private async Task Listen()
    {
        List<Task> clientConnectionsTasks = [];

        while (!this.cancellationToken.IsCancellationRequested)
        {
            Socket clientConnection = await this.tcpListener.AcceptSocketAsync(this.cancellationToken.Token);
            clientConnectionsTasks.Add(HandleConnection(clientConnection));
        }

        await Task.WhenAll(clientConnectionsTasks);
    }
}
