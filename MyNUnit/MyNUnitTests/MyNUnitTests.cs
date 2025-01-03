// <copyright file="MyNUnitTests.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnitTest;

using MyNUnit;
using NUnit.Framework;

/// <summary>
/// Tests for MyNUnit.
/// </summary>
public class MyNUnitTests
{
    /// <summary>
    /// Checks that throws exception if path not exists.
    /// </summary>
    [Test]
    public static void Test_ThrowsDirectoryNotFoundException()
    {
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await MyTester.RunTestsFromDirectoty("bambam/net8.0"));
    }

    /// <summary>
    /// Checks that tests runs correctly.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task Test_RunsTestsCorrectly()
    {
        var result = await MyTester.RunTestsFromDirectoty("../../../../ProjectForTesting/bin/Debug/net9.0");

        var expected = new List<MyTestResult>
        {
        new("Test_ShouldBePassed", TestStatus.Passed, string.Empty),
        new("Test_ShouldBeIgnored", TestStatus.Ignored, "ignore"),
        new("Test_ShouldBeFailed", TestStatus.Failed, "Exception has been thrown by the target of an invocation."),
        new("Test_ShouldThrowException", TestStatus.Passed, "Throwed expected exception."),
        };

        foreach (var testClass in result)
        {
            for (int i = 0; i < testClass.TestResults.Count; ++i)
            {
                Assert.That(this.Comparer(testClass.TestResults[i], expected[i]), Is.True);
            }
        }
    }

    private bool Comparer(MyTestResult actual, MyTestResult expected)
    {
        return actual.Name == expected.Name && actual.Message == expected.Message && actual.Status == expected.Status;
    }
}