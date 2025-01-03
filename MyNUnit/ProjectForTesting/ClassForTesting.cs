// <copyright file="ClassForTesting.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>
namespace ProjectForTesting;

using MyNUnit;

/// <summary>
/// Class for testing.
/// </summary>
public class ClassForTesting
{
    /// <summary>
    /// Before class method.
    /// </summary>
    [BeforeClass]
    public static void BeforeClassMethod()
    {
        Console.WriteLine($"Before class");
    }

    /// <summary>
    /// After class method.
    /// </summary>
    [AfterClass]
    public static void AfterClassMethod()
    {
        Console.WriteLine("After class");
    }

    /// <summary>
    /// Before method.
    /// </summary>
    [Before]
    public static void BeforeMethod()
    {
        Console.WriteLine("Before test");
    }

    /// <summary>
    /// After method.
    /// </summary>
    [After]
    public static void AfterMethod()
    {
        Console.WriteLine("After test");
    }

    /// <summary>
    /// Test that should be passed.
    /// </summary>
    [MyTest]
    public static void Test_ShouldBePassed()
    {
        MyAssert.IsTrue(true);
    }

    /// <summary>
    /// Test that should be ignored.
    /// </summary>
    [MyTest(typeof(Exception), "ignore")]
    public static void Test_ShouldBeIgnored()
    {
        MyAssert.IsTrue(true);
    }

    /// <summary>
    /// Test that should be failed.
    /// </summary>
    [MyTest]
    public static void Test_ShouldBeFailed()
    {
        MyAssert.IsTrue(false);
    }

    /// <summary>
    /// Test that should throw exception and be passed.
    /// </summary>
    /// <exception cref="Exception">Exception.</exception>
    [MyTest(typeof(Exception))]
    public static void Test_ShouldThrowException()
    {
        throw new Exception();
    }
}