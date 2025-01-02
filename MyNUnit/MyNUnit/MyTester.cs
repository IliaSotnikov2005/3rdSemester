using System.Reflection.Metadata.Ecma335;

namespace MyNUnit;

using System.Reflection;

public static class MyTester
{
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

        var TestClasses = new List<Type>();

        foreach (var assembly in assembliesNames)
        {
            var TestClassesInAssembly = assembly.GetTypes().Where(t => t.GetMethods().Any(m => m.GetCustomAttributes(typeof(MyTestAttribute), false).Length != 0));
            foreach (var testClass in TestClassesInAssembly)
            {
                TestClasses.Add(testClass);
            }
        }

        return TestClasses;
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

                testClassResults.Add(new MyTestResult(method.Name, TestStatus.Passed, string.Empty, DateTime.Now - startTime));
            }
            catch (Exception ex)
            {
                if (testAttribute.Expected is not null && ex.InnerException!.GetType() == testAttribute.Expected.GetType())
                {
                    testClassResults.Add(new MyTestResult(method.Name, TestStatus.Passed, "Throwed expected exception.", DateTime.Now - startTime));
                    return;
                }

                testClassResults.Add(new MyTestResult(method.Name, TestStatus.Failed, ex.Message));
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