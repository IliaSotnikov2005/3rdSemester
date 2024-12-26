// <copyright file="Client.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace SimpleFTP;

using System.Net.Sockets;

/// <summary>
/// Client class.
/// </summary>
/// <param name="hostName">The name of host.</param>
/// <param name="port">The port.</param>
public class Client(string hostName, int port)
{
    /// <summary>
    /// Gets the host name.
    /// </summary>
    public string HostName { get; } = hostName;

    /// <summary>
    /// Gets the port.
    /// </summary>
    public int Port { get; } = port;

    /// <summary>
    /// Does list request to the server.
    /// </summary>
    /// <param name="path">The path to directory on the server.</param>
    /// <returns>An array of directory elements.</returns>
    public async Task<(string, bool)[]?> List(string path)
    {
        using var client = new TcpClient(this.HostName, this.Port);
        var stream = client.GetStream();

        var writer = new StreamWriter(stream) { AutoFlush = true };
        await writer.WriteLineAsync($"{(int)RequestType.List} {path}");

        return await GetResponseToListRequestAsync(stream);
    }

    /// <summary>
    /// Does get request to the server.
    /// </summary>
    /// <param name="path">The path to file on the server.</param>
    /// <returns>Byte array.</returns>
    public async Task<byte[]> Get(string path)
    {
        using var client = new TcpClient(this.HostName, this.Port);
        var stream = client.GetStream();

        var writer = new StreamWriter(stream) { AutoFlush = true };
        await writer.WriteLineAsync($"{(int)RequestType.Get} {path}");

        return await GetResponseToGetRequestAsync(stream);
    }

#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
    private static async Task<(string, bool)[]?> GetResponseToListRequestAsync(Stream stream)
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
    {
        using var reader = new StreamReader(stream);
        var response = await reader.ReadLineAsync();
        if (response == null)
        {
            return null;
        }

        var responseParts = response.Split();
        if (responseParts[0] == "-1")
        {
            return null;
        }

        var count = int.Parse(responseParts[0]);

        var directoryItems = new (string, bool)[count];

        for (var i = 0; i < count; ++i)
        {
            directoryItems[i] = (responseParts[1 + (i * 2)], bool.Parse(responseParts[2 + (i * 2)]));
        }

        return directoryItems;
    }

    private static async Task<byte[]> GetResponseToGetRequestAsync(Stream stream)
    {
        using var reader = new BinaryReader(stream);

        long size = reader.ReadInt64();
        if (size == -1)
        {
            return [];
        }

        var response = new byte[size];
        long bytesRead = 0;
        const int bufferSize = 4096;
        var buffer = new byte[bufferSize];

        while (bytesRead < size)
        {
            int toRead = (int)Math.Min(bufferSize, size - bytesRead);
            int read = await stream.ReadAsync(buffer, 0, toRead);

            if (read == 0)
            {
                break;
            }

            Array.Copy(buffer, 0, response, bytesRead, read);
            bytesRead += read;
        }

        return response;
    }
}