// <copyright file="MyThreadPoolTests.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyThreadPoolTests;

using NUnit.Framework;
using MyThreadPool;

/// <summary>
/// Tests for <see cref="MyThreadPool"/>.
/// </summary>
public class Tests
{
    /// <summary>
    /// Tests that number of active threads is equal to number of threads given to the constructor.
    /// </summary>
    [Test]
    public static void Test_NumberOfActiveThreads_NumberGivenToConstructor()
    {
        var numberOfThreads = 6;
        var threadPool = new MyThreadPool(numberOfThreads);
        var tasks = new IMyTask<int>[numberOfThreads];
        for (int i = 0; i < numberOfThreads; ++i)
        {
            tasks[i] = threadPool.Submit(() =>
            {
                Thread.Sleep(100);
                return 1;
            });
        }

        Thread.Sleep(110);

        foreach (var task in tasks)
        {
            AssertBlock(task, 1);
        }

        threadPool.Shutdown();
    }

    /// <summary>
    /// Tests that works correctly when threads is more than tasks.
    /// </summary>
    [Test]
    public static void Test_ThreadsMoreThanTasks_WorksCorrect()
    {
        var numberOfThreads = 20;
        var numberOfTasks = 10;
        var threadPool = new MyThreadPool(numberOfThreads);
        var tasks = new IMyTask<int>[numberOfTasks];

        for (int i = 0; i < numberOfTasks; ++i)
        {
            var localI = i;
            tasks[i] = threadPool.Submit(() => localI * 2);
        }

        for (int i = 0; i < numberOfTasks; ++i)
        {
            AssertBlock(tasks[i], i * 2);
        }

        threadPool.Shutdown();
    }

    /// <summary>
    /// Tests that works correctly when tasks is more than threads.
    /// </summary>
    [Test]
    public static void Test_TasksMoreThanThreads_WorksCorrect()
    {
        var numberOfThreads = 10;
        var numberOfTasks = 20;
        var threadPool = new MyThreadPool(numberOfThreads);
        var tasks = new IMyTask<int>[numberOfTasks];
        for (int i = 0; i < numberOfTasks; ++i)
        {
            var localI = i;
            tasks[i] = threadPool.Submit(() => localI * localI);
        }

        Thread.Sleep(200);

        for (int i = 0; i < numberOfTasks; ++i)
        {
            AssertBlock(tasks[i], i * i);
        }

        threadPool.Shutdown();
    }

    /// <summary>
    /// Tests that works correctly when tasks have different types.
    /// </summary>
    [Test]
    public static void Test_DifferentTypeTasks_WorksCorrect()
    {
        var numberOfThreads = 4;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task1 = threadPool.Submit(() => 341 + (12 * 11));
        var task2 = threadPool.Submit(() => "Cucumber" + " banana");
        var task3 = threadPool.Submit(() => 7 % 2 == 0);
        var task4 = threadPool.Submit(() => "12345".ToArray());

        AssertBlock(task1, 341 + (12 * 11));
        AssertBlock(task2, "Cucumber banana");
        AssertBlock(task3, false);
        AssertBlock(task4, ['1', '2', '3', '4', '5']);

        threadPool.Shutdown();
    }

    /// <summary>
    /// Tests that throws exception when incorrect number of threads.
    /// </summary>
    [Test]
    public static void Test_IncorrectNumberOfThreads_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new MyThreadPool(-10));
        Assert.Throws<ArgumentException>(() => new MyThreadPool(0));
    }

    /// <summary>
    /// Tests that task exceptions are passed up.
    /// </summary>
    [Test]
    public static void Test_TaskThrowsException_PassedUp()
    {
        var numberOfThreads = 6;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit<int>(() => throw new ArgumentException());
        var exception = Assert.Throws<AggregateException>(() => { int a = task.Result; });
        Assert.That(exception, Is.Not.Null);
        if (exception is not null)
        {
            Assert.Multiple(() =>
            {
                Assert.That(exception.InnerException, Is.Not.Null);
                Assert.That(exception.InnerException!.GetType(), Is.EqualTo(typeof(ArgumentException)));
            });
        }

        threadPool.Shutdown();
    }

    /// <summary>
    /// Tests ContinueWith method.
    /// </summary>
    [Test]
    public static void Test_ContinueWith_ReturnsCorrect()
    {
        var numberOfThreads = 2;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit(() => "12" + "34").ContinueWith((number) => int.Parse(number.ToArray()));
        AssertBlock(task, 1234);
        threadPool.Shutdown();
    }

    /// <summary>
    /// Tests that many ContinueWith works correctly.
    /// </summary>
    [Test]
    public static void Test_ManyContinueWith_ReturnsCorrect()
    {
        var threadPool = new MyThreadPool(2);
        var task = threadPool.Submit(() => "12" + "34").ContinueWith((number) => int.Parse(number.ToArray())).ContinueWith((number) => number * 2);
        AssertBlock(task, 2468);

        threadPool.Shutdown();
    }

    /// <summary>
    /// Tests that after shutting down the tasks are executed.
    /// </summary>
    [Test]
    public static void Test_Shutdown_AllTasksComplete()
    {
        var threadPool = new MyThreadPool(2);
        var flag = false;

        threadPool.Submit(() =>
        {
            Thread.Sleep(500);
            Volatile.Write(ref flag, true);
            return 0;
        });

        threadPool.Shutdown();

        Assert.That(flag, Is.True);
    }

    private static void AssertBlock<T>(IMyTask<T> task, T expected)
    {
        Assert.That(task.Result, Is.EqualTo(expected));
        Assert.That(task.IsCompleted, Is.True);
    }
}