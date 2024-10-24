\\\\ <copyright file="Tests.cs" company="IlyaSotnikov">
\\\\ Copyright (c) IlyaSotnikov. All rights reserved.
\\\\ <\\copyright>

namespace SimpleFTPTest;

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using SimpleFTP;

public class Tests
{
    private const string HostName = "localhost";
    private const int Port = 8080;
    private static Server server;

    [SetUp]
    public static void SetupServer()
    {
        server = new Server("127.0.0.1", Port);
        server.Start();
    }

    [TearDown]
    public static void StopServer()
    {
        server.Stop();
    }

    [Test]
    public static async Task Test_ListRequest()
    {
        var client = new Client(HostName, Port);
        var content = await client.List("files");
        Assert.That(content, Is.EqualTo(new (string, bool)[]{("files\\subfolder", true), ("files\\text.txt", false)}));
    }

    [Test]
    public static async Task Test_ListRequestToNotExistingDirectory_ReturnEmpty()
    {
        var client = new Client(HostName, Port);
        var content = await client.List("not_exist");
        Assert.That(content, Is.Empty);
    }

    [Test]
    public static void Test_MultipleListRequestFromOneClient()
    {
        int numberOfRequests = 5;
        var tasks = new Task<(string, bool)[]>[numberOfRequests];
        var client = new Client(HostName, Port);
        for (int i = 0; i < numberOfRequests; ++i)
        {
            tasks[i] = client.List("files");
        }

        Thread.Sleep(500);

        var expected = new (string, bool)[] {("files\\subfolder", true), ("files\\text.txt", false)};

        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EquivalentTo(expected));
        }
    }

    [Test]
    public static void Test_ListRequestFromDifferentClients()
    {
        int numberOfClients = 10;
        var tasks = new Task<(string, bool)[]>[numberOfClients];
        
        for (int i = 0; i < numberOfClients; ++i)
        {
            var client = new Client(HostName, Port);
            tasks[i] = client.List("files");
        }

        Thread.Sleep(500);

        var expected = new (string, bool)[] {("files\\subfolder", true), ("files\\text.txt", false)};

        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EquivalentTo(expected));
        }
    }

    [Test]
    public static async Task Test_GetRequestForTextFile()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get("files\\text.txt");

        var expected = File.ReadAllBytes("files\\text.txt");

        Assert.That(content, Is.EqualTo(expected));
    }

    [Test]
    public static async Task Test_GetRequestForImageFile()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get("files\\subfolder\\frede.jpg");

        var expected = File.ReadAllBytes("files\\subfolder\\frede.jpg");

        Assert.That(content, Is.EqualTo(expected));
    }

    [Test]
    public static async Task Test_GetRequestForNotExistingFile_ReturnsEmpty()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get("notexist.txt");
        Assert.That(content, Is.Empty);
    }

    [Test]
    public static void Test_MultipleGetRequestGetsFromOneClient()
    {
        int numberOfRequests = 4;
        var tasks = new Task<byte[]>[numberOfRequests];
        var client = new Client(HostName, Port);
        for (int i = 0; i < numberOfRequests; ++i)
        {
            tasks[i] = client.Get("files\\subfolder\\frede.jpg");
        }

        Thread.Sleep(500);
        
        var expectedResult = File.ReadAllBytes("files\\subfolder\\frede.jpg");

        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EqualTo(expectedResult));
        }
    }

    [Test]
    public static void Test_GetRequestsFromDifferentClients()
    {
        int numberOfClients = 5;
        var tasks = new Task<byte[]>[numberOfClients];
        for (int i = 0; i < numberOfClients; ++i)
        {
            var client = new Client(HostName, Port);
            tasks[i] = client.Get("files\\subfolder\\frede.jpg");
        }

        Thread.Sleep(500);

        var expectedResult = File.ReadAllBytes("files\\subfolder\\frede.jpg");

        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EqualTo(expectedResult));
        }
    }

    [Test]
    public static void Test_ConnectToWrongPort_ThrowsException()
    {
        var client = new Client(HostName, 123);
        Assert.ThrowsAsync<SocketException>(async () => await client.List("files"));
        Assert.ThrowsAsync<SocketException>(async () => await client.Get("files\\text.txt"));
    }
}