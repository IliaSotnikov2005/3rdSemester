// <copyright file="LazyTests.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace Lazy.LazyTests;

using NUnit.Framework;

/// <summary>
/// Class for testing the ILazy classes.
/// </summary>
public class LazyTests
{
    private static readonly Delegate[] Suppliers = [Supplier1, Supplier2];
    private static readonly object[] ExpectedValues = [6, new int[] { 2, 4, 6, 8, 10 }];
    private static readonly int[] NumbersOfGetRequests = [1, 10, 100];
    private static readonly int[] NumbersOfThreads = [10, 100, 1000];
    private static int evaluations = 0;

    /// <summary>
    /// Tests with one thread.
    /// </summary>
    /// <typeparam name="T">Type of returned value.</typeparam>
    /// <param name="supplier">Function to be evaluated.</param>
    /// <param name="expected">Expected value.</param>
    /// <param name="numberOfRequests">Number of Get requessts.</param>
    [TestCaseSource(nameof(OneThreadCases))]
    public static void OneThread_ReturnsCorrectResult_EvaluateOneTime<T>(Func<T> supplier, T expected, int numberOfRequests)
    {
        ILazy<T>[] lazies = [new SingleThreadedLazy<T>(supplier), new MultiThreadedLazy<T>(supplier)];

        foreach (var lazy in lazies)
        {
            evaluations = 0;
            for (int i = 0; i < numberOfRequests; ++i)
            {
                Assert.That(lazy.Get(), Is.EqualTo(expected));
            }

            Assert.That(evaluations, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// Tests with one thread.
    /// </summary>
    /// <typeparam name="T">Type of returned value.</typeparam>
    /// <param name="supplier">Function to be evaluated.</param>
    /// <param name="expected">Expected value.</param>
    /// <param name="numberOfThreads">Number of threads.</param>
    /// <param name="numberOfRequests">Number of Get requessts.</param>
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

    /// <summary>
    /// Test that Lazies throw exceptions.
    /// </summary>
    #pragma warning disable CS8603 // Dereference of a possibly null reference.
    [Test]
    public static void NullValueException_IsThrownWhenValueIsNull()
    {
        ILazy<object>[] lazies = [new SingleThreadedLazy<object>(() => default), new MultiThreadedLazy<object>(() => default)];

        foreach (var lazy in lazies)
        {
            Assert.That(() => lazy.Get(), Throws.InstanceOf<NullValueException>());
        }
    }
    #pragma warning restore CS8603 // Dereference of a possibly null reference.

    private static IEnumerable<TestCaseData> MultiThreadCases()
    {
        for (int i = 0; i < Suppliers.Length; ++i)
        {
            var supplier = Suppliers[i];
            var expected = ExpectedValues[i];
            foreach (var threadCount in NumbersOfThreads)
            {
                foreach (var requestsCount in NumbersOfGetRequests)
                {
                    yield return new TestCaseData(supplier, expected, threadCount, requestsCount);
                }
            }
        }
    }

    private static IEnumerable<TestCaseData> OneThreadCases()
    {
        for (int i = 0; i < Suppliers.Length; ++i)
        {
            var supplier = Suppliers[i];
            var expected = ExpectedValues[i];
            foreach (var countOfRequests in NumbersOfGetRequests)
            {
                yield return new TestCaseData(supplier, expected, countOfRequests);
            }
        }
    }

    private static int Supplier1()
    {
        Interlocked.Increment(ref evaluations);
        return 2 + (2 * 2);
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