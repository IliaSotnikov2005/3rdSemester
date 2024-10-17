using System.Data.SqlTypes;
using System.Net.Sockets;

namespace SimpleFTP;

public class Client(string hostName, int port)
{
    public string HostName { get; } = hostName;
    public int Port { get; } = port;

    public async Task<(string, bool)[]> List(string path)
    {
        using var client = new TcpClient(HostName, Port);
        var stream = client.GetStream();

        var writer = new StreamWriter(stream) { AutoFlush = true };
        await writer.WriteLineAsync($"{RequestType.List} {path}");

        return await GetResponseToListRequest(stream);
    }

    private static async Task<(string, bool)[]> GetResponseToListRequest(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var response = await reader.ReadLineAsync();

        if (response == null)
        {
            return [];
        }

        var responseParts = response.Split();

        var count = int.Parse(responseParts[0]);
        var directoryItems = new (string, bool)[count];

        for (var i = 1; i < count; i += 2)
        {
            directoryItems[i] = (responseParts[i], bool.Parse(responseParts[i + 1]));
        }

        return directoryItems;
    }

    public async Task<byte[]> Get(string path)
    {
        using var client = new TcpClient(HostName, Port);
        var stream = client.GetStream();

        var writer = new StreamWriter(stream) { AutoFlush = true };
        await writer.WriteLineAsync($"{RequestType.Get} {path}");

        return GetResponseToGetRequest(stream);
    }

    private static byte[] GetResponseToGetRequest(Stream stream)
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
            int read = stream.Read(buffer, 0, toRead);

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