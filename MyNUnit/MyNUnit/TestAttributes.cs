using System.Reflection;

namespace MyNUnit;

[AttributeUsage(AttributeTargets.Method)]
class TestAttribute : Attribute
{
    public Exception? Expected { get; }
    public string? Ignore { get; }

    public TestAttribute() { }
    public TestAttribute(Exception expected) => Expected = expected;
    public TestAttribute(Exception expected, string ignore)
    {
        Expected = expected;
        Ignore = ignore;
    }
}

[AttributeUsage(AttributeTargets.Method)]
class BeforeAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
class AfterAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
class BeforeClassAttribute : Attribute
{
    public BeforeClassAttribute(MethodInfo method)
    {
        if (!method.IsStatic)
        {
            throw new InvalidOperationException("The method must be static.");
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
class AfterClassAttribute : Attribute
{
    public AfterClassAttribute(MethodInfo method)
    {
        if (!method.IsStatic)
        {
            throw new InvalidOperationException("The method must be static.");
        }
    }
}