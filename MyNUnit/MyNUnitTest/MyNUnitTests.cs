namespace MyNUnitTest;

using NUnit.Framework;
using MyNUnit;

public class Tests
{
    [Test]
    public async Task Test_RunsTestsCorrectly()
    {
        List<MyTestResult> res = await Tester.RunTestsAsync("../../../../ProjectForTesting/bin/Debug/net8.0");

        var expected = new List<MyTestResult>
        {
        new ("Test_ShouldBePassed", "Passed", ""),
        new ("Test_ShouldBeIgnored", "Ignored", "ignore"),
        new ("Test_ShouldBeFailed", "Failed", "Exception has been thrown by the target of an invocation."),
        new ("Test_ShouldThrowException", "Passed", "")
        };

        for (int i = 0; i < res.Count; ++i)
        {
            Assert.That(Comparer(res[i], expected[i]), Is.True);
        }
    }

    [Test]
    public static void Test_ThrowsDirectoryNotFoundException()
    {
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await Tester.RunTestsAsync("bambam/net8.0"));
    }

    private bool Comparer(MyTestResult actual, MyTestResult expected)
    {
        return actual.Name == expected.Name && actual.Message == expected.Message && actual.Status == expected.Status;
    }
}
