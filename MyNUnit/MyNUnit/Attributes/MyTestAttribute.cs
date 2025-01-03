// <copyright file="MyTestAttribute.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Attribute for my tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class MyTestAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyTestAttribute"/> class.
    /// </summary>
    public MyTestAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTestAttribute"/> class.
    /// </summary>
    /// <param name="expected">Expected exception.</param>
    public MyTestAttribute(Type expected)
    {
        this.Expected = expected;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTestAttribute"/> class.
    /// </summary>
    /// <param name="expected">Expected exception.</param>
    /// <param name="ignore">Comment why ignore the test.</param>
    public MyTestAttribute(Type expected, string ignore)
    {
        this.Expected = expected;
        this.Ignore = ignore;
    }

    /// <summary>
    /// Gets expected exception.
    /// </summary>
    public Type? Expected { get; private set; } = null;

    /// <summary>
    /// Gets comment why ignore the test.
    /// </summary>
    public string? Ignore { get; private set; } = string.Empty;
}
