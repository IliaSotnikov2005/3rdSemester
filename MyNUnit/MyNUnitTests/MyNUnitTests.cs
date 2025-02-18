// <copyright file="MyNUnitTests.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnitTests;

using MyNUnit;
using NUnit.Framework;

/// <summary>
/// Tests for NUnit.
/// </summary>
public class MyNUnitTests
{
    /// <summary>
    /// Checks that throws exception if path not exists.
    /// </summary>
    [Test]
    public static void Test_ThrowsDirectoryNotFoundException()
    {
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await MyTester.RunTestsFromDirectory("bambam/net8.0"));
    }

    /// <summary>
    /// Checks that tests runs correctly.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task Test_RunsTestsCorrectly()
    {
        var result = await MyTester.RunTestsFromDirectory("../../../../ProjectForTesting/bin/Debug/net9.0");

        var expected = new List<TestResult>
        {
        new("Test_ShouldBePassed", TestStatus.Passed, string.Empty),
        new("Test_ShouldBeIgnored", TestStatus.Ignored, "ignore"),
        new("Test_ShouldBeFailed", TestStatus.Failed, "Exception has been thrown by the target of an invocation."),
        new("Test_ShouldThrowException", TestStatus.Passed, "Throwed expected exception."),
        };

        foreach (var testAssembly in result.TestAssemblyResults)
        {
            foreach (var testClass in testAssembly.TestClassResults)
            {
                for (int i = 0; i < testClass.TestResults.Count; ++i)
                {
                    Assert.That(Comparer(testClass.TestResults[i], expected[i]), Is.True);
                }
            }
        }
    }

    private static bool Comparer(TestResult actual, TestResult expected)
    {
        return actual.Name == expected.Name && actual.Message == expected.Message && actual.Status == expected.Status;
    }
}