// <copyright file="MyAssert.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Class for asserting.
/// </summary>
public static class MyAssert
{
    /// <summary>
    /// Checks whether true or false.
    /// </summary>
    /// <param name="condition">COndition to be checked.</param>
    /// <exception cref="AssertionException">Throws if assertion failed.</exception>
    public static void IsTrue(bool condition)
    {
        if (!condition)
        {
            throw new AssertionException("Assertion failed");
        }
    }

    /// <summary>
    /// Asserts whether throws exception.
    /// </summary>
    /// <typeparam name="T">Type of expected exception.</typeparam>
    /// <param name="function">Function to be checked.</param>
    /// <exception cref="AssertionException">Throws if assertion failed.</exception>
    public static void Throws<T>(Func<T> function)
    {
        try
        {
            function();
        }
        catch (Exception ex)
        {
            if (typeof(T) != ex.GetType())
            {
                throw new AssertionException("Expected exception of type " + typeof(T).Name + " but got " + ex.GetType().Name);
            }
        }
    }
}