namespace Lazy.LazyTests;

using NUnit.Framework;

public class LazyTests
{
    private static int evaluations = 0;
    private static readonly Delegate[] suppliers = [Supplier1, Supplier2];
    private static readonly object[] expectedValues = [6, new int[] { 2, 4, 6, 8, 10 }];
    private static readonly int[] numbersOfGetRequests = [1, 10, 100];
    private static readonly int[] numbersOfThreads = [10, 100, 1000];

    private static IEnumerable<TestCaseData> MultiThreadCases()
    {
        for (int i = 0; i < suppliers.Length; ++i)
        {
            var supplier = suppliers[i];
            var expected = expectedValues[i];
            foreach (var threadCount in numbersOfThreads)
            {
                foreach (var requestsCount in numbersOfGetRequests)
                {
                    yield return new TestCaseData(supplier, expected, threadCount, requestsCount);
                }
            }
        }
    }

    private static IEnumerable<TestCaseData> OneThreadCases()
    {
        for (int i = 0; i < suppliers.Length; ++i)
        {
            var supplier = suppliers[i];
            var expected = expectedValues[i];
            foreach (var countOfRequests in numbersOfGetRequests)
            {
                yield return new TestCaseData(supplier, expected, countOfRequests);
            }
        }
    }

    [TestCaseSource(nameof(OneThreadCases))]
    public static void OneThread_ReturnsCorrectResult_EvaluateOneTime<T>(Func<T> testMethod, T expectedValue, int numberOfRequests)
    {
        ILazy<T>[] lazies = [new SingleThreadedLazy<T>(testMethod), new MultiThreadedLazy<T>(testMethod)];

        foreach (var lazy in lazies)
        {
            evaluations = 0;
            for (int i = 0; i < numberOfRequests; ++i)
            {
                Assert.That(lazy.Get(), Is.EqualTo(expectedValue));
            }

            Assert.That(evaluations, Is.EqualTo(1));
        }
    }

    [TestCaseSource(nameof(MultiThreadCases))]
    public static void MultiThread_ReturnsCorrectResult_EvaluateOneTime<T>(Func<T> supplier, T expected, int numberOfThreads, int numberOfRequests)
    {
        evaluations = 0;
        var lazy = new MultiThreadedLazy<T>(supplier);
        var threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; ++i)
        {
            threads[i] = new Thread(() =>
            {
                for (int i = 0; i < numberOfRequests; ++i)
                {
                    Assert.That(lazy.Get(), Is.EqualTo(expected));
                }
            });
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.That(evaluations, Is.EqualTo(1));
    }

    private static int Supplier1()
    {
        Interlocked.Increment(ref evaluations);
        return 2 + 2 * 2;
    }

    private static int[] Supplier2()
    {
        Interlocked.Increment(ref evaluations);
        int[] array = [1, 2, 3, 4, 5];
        for (int i = 0; i < 5; ++i)
        {
            array[i] *= 2;
        }

        return array;
    }
}