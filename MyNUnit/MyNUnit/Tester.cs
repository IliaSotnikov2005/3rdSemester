using System.Reflection;

namespace MyNUnit;


public static class Tester
{
    public static async Task RunTestsAsync(string path)
    {
        var assemblies = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom);

        var testResults = new List<TestResult>();

        foreach (var assembly in assemblies)
        {
            var testClasses = assembly.GetTypes().Where(t => t.GetMethods().Any(m => m.GetCustomAttributes(typeof(MyTestAttribute), false).Length != 0));

            foreach (var testClass in testClasses)
            {
                testResults.AddRange(await RunTestClass(testClass));
            }
        }

        PrintReport(testResults);
    }

    private static async Task<List<TestResult>> RunTestClass(Type testClass)
    {
        var results = new List<TestResult>();

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
                results.Add(new TestResult(method.Name, "Ignored", testAttr.Ignore));
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
                        results.Add(new TestResult(method.Name, "Passed (Expected Exception)", string.Empty));
                        return;
                    }

                    throw;
                }

                results.Add(new TestResult(method.Name, "Passed", string.Empty));
            }
            catch (Exception ex)
            {
                results.Add(new TestResult(method.Name, "Failed", ex.Message));
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

    private static void PrintReport(List<TestResult> results)
    {
        foreach (var result in results)
        {
            Console.WriteLine($"{result.Name}: {result.Status} - {result.Message ?? ""}");
        }
    }

    public class TestResult(string name, string status, string message)
    {
        public string Name { get; } = name;
        public string Status { get; } = status;
        public string Message { get; } = message;
    }
}
