using System.Reflection.Metadata.Ecma335;

namespace MyNUnit;

using System.Reflection;
using System.Runtime.ConstrainedExecution;

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

    private static MyTestClassResults RunTestClass(Type testClass)
    {
        return new ();
    }
}