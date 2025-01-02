namespace MyNUnitTest;

using NUnit.Framework;
using MyNUnit;

public class Tests
{
    [Test]
    public async Task Test_RunsTestsCorrectly()
    {
        var result = await MyTester.RunTestsFromDirectoty("../../../../ProjectForTesting/bin/Debug/net9.0");

        var expected = new List<MyTestResult>
        {
        new ("Test_ShouldBePassed", TestStatus.Passed, ""),
        new ("Test_ShouldBeIgnored", TestStatus.Ignored, "ignore"),
        new ("Test_ShouldBeFailed", TestStatus.Failed, "Exception has been thrown by the target of an invocation."),
        new ("Test_ShouldThrowException", TestStatus.Passed, "")
        };

        foreach (var testClass in result)
        {
            for (int i = 0; i < testClass.TestResults.Count; ++i)
            {
                Assert.That(Comparer(testClass.TestResults[i], expected[i]), Is.True);
            }
        }
    }

    [Test]
    public static void Test_ThrowsDirectoryNotFoundException()
    {
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await MyTester.RunTestsFromDirectoty("bambam/net8.0"));
    }

    private bool Comparer(MyTestResult actual, MyTestResult expected)
    {
        return actual.Name == expected.Name && actual.Message == expected.Message && actual.Status == expected.Status;
    }
}