// <copyright file="NUnitTests.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace NUnitTest;

using MyNUnit;
using NUnit;
using NUnit.Framework;

/// <summary>
/// Tests for NUnit.
/// </summary>
public class NUnitTests
{
    /// <summary>
    /// Checks that throws exception if path not exists.
    /// </summary>
    //[Test]
    //public static void Test_ThrowsDirectoryNotFoundException()
    //{
    //    Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await MyTester.RunTestsFromDirectory("bambam/net8.0"));
    //}

    /// <summary>
    /// Checks that tests runs correctly.
    /// </summary>
    /// <returns>Task.</returns>
    //[Test]
    //public async Task Test_RunsTestsCorrectly()
    //{
    //    var result = await MyTester.RunTestsFromDirectory("../../../../ProjectForTesting/bin/Debug/net9.0");

    //    var expected = new List<TestResult>
    //    {
    //    new("Test_ShouldBePassed", TestStatus.Passed, string.Empty),
    //    new("Test_ShouldBeIgnored", TestStatus.Ignored, "ignore"),
    //    new("Test_ShouldBeFailed", TestStatus.Failed, "Exception has been thrown by the target of an invocation."),
    //    new("Test_ShouldThrowException", TestStatus.Passed, "Throwed expected exception."),
    //    };

    //    foreach (var testClass in result)
    //    {
    //        for (int i = 0; i < testClass.TestResults.Count; ++i)
    //        {
    //            Assert.That(this.Comparer(testClass.TestResults[i], expected[i]), Is.True);
    //        }
    //    }
    //}

    private bool Comparer(TestResult actual, TestResult expected)
    {
        return actual.Name == expected.Name && actual.Message == expected.Message && actual.Status == expected.Status;
    }
}