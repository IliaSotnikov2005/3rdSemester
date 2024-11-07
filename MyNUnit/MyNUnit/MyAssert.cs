namespace MyNUnit;

public static class MyAssert
{
    public static void IsTrue(bool condition)
    {
        if (!condition)
        {
            throw new AssertionException("Assertion failed");
        }
    }

    public static void Throws<T>(Func<T> function)
    {
        try
        {
            function();
        } catch (Exception ex)
        {
            if (typeof(T) != ex.GetType())
            {
                throw new AssertionException("Expected exception of type " + typeof(T).Name + " but got " + ex.GetType().Name);
            }
        }

    }

    public static void AreEqual<T>(T expected, T actual)
    {
        if (expected == null)
        {
            if (actual != null)
            {
                throw new AssertionException("Values are not equal");
            }
        }

        if (!expected!.Equals(actual))
        {
            throw new AssertionException("Values are not equal");
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