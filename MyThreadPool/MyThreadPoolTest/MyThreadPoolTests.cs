namespace MyThreadPool2Tests;

using NUnit.Framework;
using MyThreadPool;

public class Tests
{
    [Test]
    public static void Test_NumberOfActiveThreads_NumberGivenToConstructor()
    {
        var numberOfThreads = 6;
        var threadPool = new MyThreadPool2(numberOfThreads);
        var tasks = new IMyTask<int>[numberOfThreads];
        for (int i = 0; i < numberOfThreads; ++i)
        {
            tasks[i] = threadPool.Submit(() => { Thread.Sleep(100); return 1; });
        }

        Thread.Sleep(110);

        foreach (var task in tasks)
        {
            AssertBlock(task, 1);
        }
    }

    [Test]
    public static void Test_ThreadsMoreThenTasks_WorksCorrect()
    {
        var numberOfThreads = 20;
        var numberOfTasks = 10;
        var threadPool = new MyThreadPool2(numberOfThreads);
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
    }

    [Test]
    public static void Test_TasksMoreThenThreads_WorksCorrect()
    {
        var numberOfThreads = 10;
        var numberOfTasks = 20;
        var threadPool = new MyThreadPool2(numberOfThreads);
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
    }

    [Test]
    public static void Test_DifferentTypeTasks_WorksCorrect()
    {
        var numberOfThreads = 4;
        var threadPool = new MyThreadPool2(numberOfThreads);
        var task1 = threadPool.Submit(() => 341 + 12 * 11);
        var task2 = threadPool.Submit(() => "Cucumber" + " banana");
        var task3 = threadPool.Submit(() => 7 % 2 == 0);
        var task4 = threadPool.Submit(() => "12345".ToArray());

        AssertBlock(task1, 341 + 12 * 11);
        AssertBlock(task2, "Cucumber banana");
        AssertBlock(task3, false);
        AssertBlock(task4, ['1', '2', '3', '4', '5']);
    }

    [Test]
    public static void Test_IncorrectNumberOfThreads_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new MyThreadPool2(-10));
        Assert.Throws<ArgumentException>(() => new MyThreadPool2(0));
    }

    [Test]
    public static void Test_TaskThrowsException_PassedUp()
    {
        var numberOfThreads = 6;
        var threadPool = new MyThreadPool2(numberOfThreads);
        var task = threadPool.Submit<int>(() => throw new ArgumentException());
        var exception = Assert.Throws<AggregateException>(() => { int a = task.Result; });
        Assert.Multiple(() =>
        {
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException!.GetType(), Is.EqualTo(typeof(ArgumentException)));
            Assert.That(task.IsCompleted, Is.False);
        });
    }

    [Test]
    public static void Test_ContinueWith_ReturnsCorrect()
    {
        var numberOfThreads = 2;
        var threadPool = new MyThreadPool2(numberOfThreads);
        var task = threadPool.Submit(() => "12" + "34").ContinueWith((number) => int.Parse(number.ToArray()));
        AssertBlock(task, 1234);
    }

    private static void AssertBlock<T>(IMyTask<T> task, T expected)
    {
        Assert.That(task.Result, Is.EqualTo(expected));
        Assert.That(task.IsCompleted, Is.True);
    }
}