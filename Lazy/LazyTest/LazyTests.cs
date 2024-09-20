namespace Lazy.LazyTests;

using NUnit.Framework;

public class LazyTests
{
    private static int evaluations = 0;

    private static readonly Delegate[] suppliers = [Supplier1, Supplier2];
    private static readonly object[] expectedValues = [6, new int[] {2, 4, 6, 8, 10}];
    private static readonly int[] amountOfGetRequests = [1, 10, 100];

    private static IEnumerable<TestCaseData> OneThread_Cases()
    {
        for (int i = 0; i < suppliers.Length; ++i)
        {
            var supplier = suppliers[i];
            var expected = expectedValues[i];
            foreach (var countOfRequests in amountOfGetRequests)
            {
                yield return new TestCaseData(supplier, expected, countOfRequests);
            }
        }
    }

    [TestCaseSource(nameof(OneThread_Cases))]
    public static void OneThread_ReturnsCorrectResult_EvaluateOneTime<T>(Func<T> testMethod, T expectedValue, int numberOfGets)
    {
        ILazy<T>[] lazies = [new SingleThreadedLazy<T>(testMethod), new MultiThreadedLazy<T>(testMethod)];

        foreach (var lazy in lazies)
        {
            evaluations = 0;
            for (int i = 0; i < numberOfGets; ++i)
            {
                Assert.That(lazy.Get(), Is.EqualTo(expectedValue));
            }

            Assert.That(evaluations, Is.EqualTo(1));
        }
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