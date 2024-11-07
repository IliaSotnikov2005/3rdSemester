using System.Reflection;

namespace MyNUnit;

[AttributeUsage(AttributeTargets.Method)]
public class MyTestAttribute : Attribute
{
    public Type? Expected { get; }
    public string? Ignore { get; }

    public MyTestAttribute() { }
    public MyTestAttribute(Type expected) => Expected = expected;
    public MyTestAttribute(Type expected, string ignore)
    {
        Expected = expected;
        Ignore = ignore;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class BeforeAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class BeforeClassAttribute : Attribute
{
    // public BeforeClassAttribute(MethodInfo method)
    // {
    //     if (!method.IsStatic)
    //     {
    //         throw new InvalidOperationException("The method must be static.");
    //     }
    // }
}

[AttributeUsage(AttributeTargets.Method)]
public class AfterClassAttribute : Attribute
{
    // public AfterClassAttribute(MethodInfo method)
    // {
    //     if (!method.IsStatic)
    //     {
    //         throw new InvalidOperationException("The method must be static.");
    //     }
    // }
}