// <copyright file="Tests.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace SimpleFTPTest;

using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using SimpleFTP;

/// <summary>
/// Tests for SimpleFTP.
/// </summary>
public class Tests
{
    private const string HostName = "localhost";
    private const int Port = 8080;
    private static readonly char DirectorySeparator = Path.DirectorySeparatorChar;
    private static readonly string Files = "files";
    private static readonly string Subfolder = $"files{DirectorySeparator}subfolder";
    private static readonly string TextFile = $"files{DirectorySeparator}text.txt";
    private static readonly string JpgFile = $"files{DirectorySeparator}subfolder{DirectorySeparator}frede.jpg";
    private static Server? server = null;

    /// <summary>
    /// Runs the server.
    /// </summary>
    [SetUp]
    public static void SetupServer()
    {
        server = new Server(IPAddress.Any.ToString(), Port);
        server.Start();
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    [TearDown]
    public static void StopServer()
    {
        server?.Dispose();
    }

    /// <summary>
    /// Test whether the server gives the correct list response.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public static async Task Test_ListRequest()
    {
        var client = new Client(HostName, Port);
        var content = await client.List(Files);

        var expected = new (string, bool)[] { (Subfolder, true), (TextFile, false) };
        Assert.That(content, Has.Length.EqualTo(2));
        foreach (var expectedItem in expected)
        {
            Assert.That(content, Contains.Item(expectedItem));
        }
    }

    /// <summary>
    /// Test whether the server works correctly with list request to not existing directory.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public static async Task Test_ListRequestToNotExistingDirectory_ReturnEmpty()
    {
        var client = new Client(HostName, Port);
        var content = await client.List("not_exist");
        Assert.That(content, Is.Empty);
    }

    /// <summary>
    /// Test whether the server works correctly with list requests from one client.
    /// </summary>
    [Test]
    public static void Test_MultipleListRequestFromOneClient()
    {
        int numberOfRequests = 5;
        var tasks = new Task<(string, bool)[]>[numberOfRequests];
        var client = new Client(HostName, Port);
        for (int i = 0; i < numberOfRequests; ++i)
        {
            tasks[i] = client.List(Files)!;
        }

        Thread.Sleep(500);

        var expected = new (string, bool)[] { (Subfolder, true), (TextFile, false) };

        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EquivalentTo(expected));
        }
    }

    /// <summary>
    /// Test whether the server works correctly with list requests from different clients.
    /// </summary>
    [Test]
    public static void Test_ListRequestFromDifferentClients()
    {
        int numberOfClients = 10;
        var tasks = new Task<(string, bool)[]>[numberOfClients];

        for (int i = 0; i < numberOfClients; ++i)
        {
            var client = new Client(HostName, Port);
            tasks[i] = client.List(Files)!;
        }

        Thread.Sleep(500);

        var expected = new (string, bool)[] { (Subfolder, true), (TextFile, false) };

        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EquivalentTo(expected));
        }
    }

    /// <summary>
    /// Test whether the server gives the correct response.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public static async Task Test_GetRequestForTextFile()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get(TextFile);

        var expected = File.ReadAllBytes(TextFile);

        Assert.That(content, Is.EqualTo(expected));
    }

    /// <summary>
    /// Test whether the server gives the correct response.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public static async Task Test_GetRequestForImageFile()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get(JpgFile);

        var expected = File.ReadAllBytes(JpgFile);

        Assert.That(content, Is.EqualTo(expected));
    }

    /// <summary>
    /// Test whether the server works correctly with not existing file.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public static async Task Test_GetRequestForNotExistingFile_ReturnsEmpty()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get("notexist.txt");
        Assert.That(content, Is.Empty);
    }

    /// <summary>
    /// Test whether the server works correctly with multiple get request from one client.
    /// </summary>
    [Test]
    public static void Test_MultipleGetRequestGetsFromOneClient()
    {
        int numberOfRequests = 4;
        var tasks = new Task<byte[]>[numberOfRequests];
        var client = new Client(HostName, Port);
        for (int i = 0; i < numberOfRequests; ++i)
        {
            tasks[i] = client.Get(JpgFile);
        }

        Thread.Sleep(500);

        var expectedResult = File.ReadAllBytes(JpgFile);

        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EqualTo(expectedResult));
        }
    }

    /// <summary>
    /// Test whether the server works correctly with requests from different clients.
    /// </summary>
    [Test]
    public static void Test_GetRequestsFromDifferentClients()
    {
        int numberOfClients = 5;
        var tasks = new Task<byte[]>[numberOfClients];
        for (int i = 0; i < numberOfClients; ++i)
        {
            var client = new Client(HostName, Port);
            tasks[i] = client.Get(JpgFile);
        }

        Thread.Sleep(500);

        var expectedResult = File.ReadAllBytes(JpgFile);

        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EqualTo(expectedResult));
        }
    }

    /// <summary>
    /// Test whether the server throws exception.
    /// </summary>
    [Test]
    public static void Test_ConnectToWrongPort_ThrowsException()
    {
        var client = new Client(HostName, 123);
        Assert.ThrowsAsync<SocketException>(async () => await client.List(Files));
        Assert.ThrowsAsync<SocketException>(async () => await client.Get(TextFile));
    }
}