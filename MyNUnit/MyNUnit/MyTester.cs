// <copyright file="MyTester.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

using System.Reflection;

/// <summary>
/// Class for running test classes.
/// </summary>
public static class MyTester
{
    /// <summary>
    /// Runs test classes from assemblies from directory.
    /// </summary>
    /// <param name="path">Path to the assemblies.</param>
    /// <returns>List of test classes results.</returns>
    /// <exception cref="DirectoryNotFoundException">Throws if directory not found.</exception>
    public static async Task<List<MyTestClassResults>> RunTestsFromDirectoty(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory {path} not found.");
        }

        var testClasses = GetTestClassesFromAssemblies(path);

        var tasks = testClasses.Select(testClass => Task.Run(() => RunTestClass(testClass))).ToArray();

        var results = await Task.WhenAll(tasks);

        return [.. results];
    }

    private static List<Type> GetTestClassesFromAssemblies(string path)
    {
        var assembliesNames = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom);

        var testClasses = new List<Type>();

        foreach (var assembly in assembliesNames)
        {
            var testClassesInAssembly = assembly.GetTypes().Where(t => t.GetMethods().Any(m => m.GetCustomAttributes(typeof(MyTestAttribute), false).Length != 0));
            foreach (var testClass in testClassesInAssembly)
            {
                testClasses.Add(testClass);
            }
        }

        return testClasses;
    }

    private static async Task<MyTestClassResults> RunTestClass(Type testClass)
    {
        var testClassResults = new List<MyTestResult>();
        bool classErrored = false;

        try
        {
            RunBeforeClass(testClass);
        }
        catch (Exception ex)
        {
            classErrored = true;
            testClassResults.Add(new MyTestResult("BeforeClass", TestStatus.Errored, ex.Message));
        }

        if (classErrored)
        {
            return new MyTestClassResults(testClassResults);
        }

        var testMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(MyTestAttribute), false).Length != 0);

        var methodsTasks = testMethods.Select(async method =>
        {
            object? instance = Activator.CreateInstance(testClass);
            if (instance is null)
            {
                testClassResults.Add(new MyTestResult(method.Name, TestStatus.Failed, "Could not create instance"));
                return;
            }

            var testAttribute = (MyTestAttribute)method.GetCustomAttributes(typeof(MyTestAttribute), false).First();

            if (!string.IsNullOrEmpty(testAttribute.Ignore))
            {
                testClassResults.Add(new MyTestResult(method.Name, TestStatus.Ignored, testAttribute.Ignore));
                return;
            }

            DateTime startTime = DateTime.Now;

            try
            {
                RunBefore(instance);
            }
            catch (Exception ex)
            {
                testClassResults.Add(new MyTestResult(method.Name, TestStatus.Errored, $"Before: {ex.Message}"));
                return;
            }

            try
            {
                var result = method.Invoke(instance, null);
                if (result is Task taskResult)
                {
                    await taskResult;
                }
                else if (result is not null)
                {
                    await Task.CompletedTask;
                }

                if (testAttribute.Expected is not null)
                {
                    testClassResults.Add(new MyTestResult(method.Name, TestStatus.Failed, "Expected exception was not thrown.", DateTime.Now - startTime));
                    return;
                }

                testClassResults.Add(new MyTestResult(method.Name, TestStatus.Passed, string.Empty, DateTime.Now - startTime));
            }
            catch (Exception ex)
            {
                if (testAttribute.Expected is not null && ex.InnerException!.GetType() == testAttribute.Expected)
                {
                    testClassResults.Add(new MyTestResult(method.Name, TestStatus.Passed, "Throwed expected exception.", DateTime.Now - startTime));
                    return;
                }

                testClassResults.Add(new MyTestResult(method.Name, TestStatus.Failed, ex.Message, DateTime.Now - startTime));
            }
            finally
            {
                try
                {
                    RunAfter(instance);
                }
                catch (Exception ex)
                {
                    testClassResults.Add(new MyTestResult(method.Name, TestStatus.Errored, $"After: {ex.Message}"));
                }
            }
        });

        await Task.WhenAll(methodsTasks);

        try
        {
            RunAfterClass(testClass);
        }
        catch (Exception ex)
        {
            classErrored = true;
            testClassResults.Add(new MyTestResult("AfterClass", TestStatus.Errored, ex.Message));
        }

        if (classErrored)
        {
            foreach (var testResult in testClassResults.Where(r => r.Status == TestStatus.Passed || r.Status == TestStatus.Ignored))
            {
                testResult.Status = TestStatus.Errored;
            }
        }

        return new MyTestClassResults(testClassResults);
    }

    private static void RunBefore(object instance)
    {
        var beforeMethods = instance.GetType().GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(BeforeAttribute), false).Length != 0);

        foreach (var method in beforeMethods)
        {
            ValidateMethod(method, mustBeStatic: false);
            method.Invoke(instance, null);
        }
    }

    private static void RunAfter(object instance)
    {
        var afterMethods = instance.GetType().GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(AfterAttribute), false).Length != 0);

        foreach (var method in afterMethods)
        {
            ValidateMethod(method, mustBeStatic: false);
            method.Invoke(instance, null);
        }
    }

    private static void RunBeforeClass(Type testClass)
    {
        var beforeClassMethods = testClass.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttributes(typeof(BeforeClassAttribute), false).Length != 0);

        foreach (var method in beforeClassMethods)
        {
            ValidateMethod(method, mustBeStatic: true);
            method.Invoke(null, null);
        }
    }

    private static void RunAfterClass(Type testClass)
    {
        var afterClassMethods = testClass.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttributes(typeof(AfterClassAttribute), false).Length != 0);

        foreach (var method in afterClassMethods)
        {
            ValidateMethod(method, mustBeStatic: true);
            method.Invoke(null, null);
        }
    }

    private static void ValidateMethod(MethodInfo method, bool mustBeStatic)
    {
        if (method.IsStatic != mustBeStatic && mustBeStatic)
        {
            throw new InvalidOperationException($"Method {method.Name} must be {(mustBeStatic ? "static" : "instance")}");
        }

        if (method.GetParameters().Length != 0)
        {
            throw new InvalidOperationException($"Method {method.Name} should not have parameters");
        }

        if (method.ReturnType != typeof(void))
        {
            throw new InvalidOperationException($"Method {method.Name} should not have a return type");
        }
    }
}