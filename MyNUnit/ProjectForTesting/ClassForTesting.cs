namespace ProjectForTesting;

using MyNUnit;

public class ClassForTesting
{
    [Test]
    public static void Test_ShouldBePassed()
    {
        MyAssert.IsTrue(false);
    }
}
