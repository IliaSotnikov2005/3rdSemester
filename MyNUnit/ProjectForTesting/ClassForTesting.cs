namespace ProjectForTesting;

using MyNUnit;

public class ClassForTesting
{
    [BeforeClass]
    public static void BeforeClassMethod()
    {
        Console.WriteLine("Before class");
    }

    [AfterClass]
    public static void AfterClassMethod()
    {
        Console.WriteLine("After class");
    }

    [Before]
    public static void BeforeMethod()
    {
        Console.WriteLine("Before test");
    }

    [After]
    public static void AfterMethod()
    {
        Console.WriteLine("After test");
    }

    [MyTest]
    public static void Test_ShouldBePassed()
    {
        MyAssert.IsTrue(true);
    }

    [MyTest(typeof(Exception), "ignore")]
    public static void Test_ShouldBeIgnored()
    {
        MyAssert.IsTrue(true);
    }

    [MyTest]
    public static void Test_ShouldBeFailed()
    {
        MyAssert.IsTrue(false);
    }

    [MyTest(typeof(Exception))]
    public static void Test_ShouldThrowException()
    {
        MyAssert.Throws<Exception>(() => throw new Exception());
    }
}
