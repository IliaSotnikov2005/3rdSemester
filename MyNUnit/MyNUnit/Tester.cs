// <copyright file="Tester.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

using System.Reflection;

/// <summary>
/// Tester class for MyNUnit.
/// </summary>
public static class Tester
{
    /// <summary>
    /// Method for runninig tests.
    /// </summary>
    /// <param name="path">Path to project directory.</param>
    /// <returns>List of results.</returns>
    /// <exception cref="DirectoryNotFoundException">Throws if directory not found.</exception>
    public static async Task<List<MyTestResult>> RunTestsAsync(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory '{path}' not found.");
        }

        var assemblies = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom);

        var testResults = new List<MyTestResult>();

        foreach (var assembly in assemblies)
        {
            var testClasses = assembly.GetTypes().Where(t => t.GetMethods().Any(m => m.GetCustomAttributes(typeof(MyTestAttribute), false).Length != 0));

            foreach (var testClass in testClasses)
            {
                testResults.AddRange(await RunTestClass(testClass));
            }
        }

        return testResults;
    }

    /// <summary>
    /// Method for running tests and printing results.
    /// </summary>
    /// <param name="path">Path to project directory.</param>
    /// <returns>Task.</returns>
    public static async Task RunTestAndPrintResultsAsync(string path)
    {
        var testResults = await RunTestsAsync(path);

        PrintReport(testResults);
    }

    private static async Task<List<MyTestResult>> RunTestClass(Type testClass)
    {
        var results = new List<MyTestResult>();

        object? instance = Activator.CreateInstance(testClass);
        if (instance == null)
        {
            return [];
        }

        RunBeforeClass(testClass);

        var testMethods = testClass.GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(MyTestAttribute), false).Length != 0);

        var tasks = testMethods.Select(async method =>
        {
            var testAttr = (MyTestAttribute)method.GetCustomAttributes(typeof(MyTestAttribute), false).First();

            if (!string.IsNullOrEmpty(testAttr.Ignore))
            {
                results.Add(new MyTestResult(method.Name, "Ignored", testAttr.Ignore));
                return;
            }

            try
            {
                RunBefore(instance);

                try
                {
                    Console.WriteLine($"Testing");
                    var result = method.Invoke(instance, null);

                    if (result is Task taskResult)
                    {
                        await taskResult;
                    }
                    else if (result != null)
                    {
                        await Task.CompletedTask;
                    }
                }
                catch (Exception ex)
                {
                    if (testAttr.Expected != null && ex.InnerException!.GetType() == testAttr.Expected.GetType())
                    {
                        results.Add(new MyTestResult(method.Name, "Passed (Expected Exception)", string.Empty));
                        return;
                    }

                    throw;
                }

                results.Add(new MyTestResult(method.Name, "Passed", string.Empty));
            }
            catch (Exception ex)
            {
                results.Add(new MyTestResult(method.Name, "Failed", ex.Message));
            }
            finally
            {
                RunAfter(instance);
            }
        });

        await Task.WhenAll(tasks);

        RunAfterClass(testClass);

        return results;
    }

    private static void RunBefore(object instance)
    {
        var beforeMethods = instance.GetType().GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(BeforeAttribute), false).Length != 0);

        foreach (var method in beforeMethods)
        {
            method.Invoke(instance, null);
        }
    }

    private static void RunAfter(object instance)
    {
        var afterMethods = instance.GetType().GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(AfterAttribute), false).Length != 0);

        foreach (var method in afterMethods)
        {
            method.Invoke(instance, null);
        }
    }

    private static void RunBeforeClass(Type testClass)
    {
        var beforeClassMethods = testClass.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttributes(typeof(BeforeClassAttribute), false).Length != 0);

        foreach (var method in beforeClassMethods)
        {
            method.Invoke(null, null);
        }
    }

    private static void RunAfterClass(Type testClass)
    {
        var afterClassMethods = testClass.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttributes(typeof(AfterClassAttribute), false).Length != 0);

        foreach (var method in afterClassMethods)
        {
            method.Invoke(null, null);
        }
    }

    private static void PrintReport(List<MyTestResult> results)
    {
        foreach (var result in results)
        {
            Console.WriteLine($"{result.Name}: {result.Status} - {result.Message ?? string.Empty}");
        }
    }
}

/// <summary>
/// Class for test result.
/// </summary>
/// <param name="name">Name of the test.</param>
/// <param name="status">Status of the test.</param>
/// <param name="message">Message of the test.</param>
public class MyTestResult(string name, string status, string message)
{
    /// <summary>
    /// Gets name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets status.
    /// </summary>
    public string Status { get; } = status;

    /// <summary>
    /// Gets message.
    /// </summary>
    public string Message { get; } = message;
}
