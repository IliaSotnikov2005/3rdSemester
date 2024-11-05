namespace MyNUnit;

public static class MyAssert
{
    public static void IsTrue(bool condition, string message = "Assertion failed")
    {
        if (!condition)
        {
            throw new Exception(message);
        }
    }

    public static void AreEqual<T>(T expected, T actual, string message = "Values are not equal")
    {
        if (!expected.Equals(actual))
        {
            throw new AssertionException(message);
        }
    }
}

public class AssertionException : Exception
{
    public AssertionException()
    {
    }
    public AssertionException(string message) : base(message)
    {
    }

    public AssertionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}