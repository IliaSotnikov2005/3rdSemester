using System.Reflection;

namespace MyNUnit;

public class TestAssemblyResult
{

    private TestAssemblyResult()
    {
    }
    public TestAssemblyResult(string assemblyName, List<TestClassResult> testClassResults)
    {
        this.AssemblyName = assemblyName;
        this.AddTestClassResults(testClassResults);
    }

    public int Id { get; set; }

    public List<TestClassResult> TestClassResults { get; private set; } = [];
    public string AssemblyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the number of passed tests.
    /// </summary>
    public int Passed { get; private set; } = 0;

    /// <summary>
    /// Gets the number of failed tests.
    /// </summary>
    public int Failed { get; private set; } = 0;

    /// <summary>
    /// Gets the number of ignored tests.
    /// </summary>
    public int Ignored { get; private set; } = 0;

    public void AddTestClassResults(List<TestClassResult> testClassResults)
    {
        this.TestClassResults.AddRange(testClassResults);
        this.Passed += testClassResults.Sum(x => x.Passed);
        this.Failed += testClassResults.Sum(x => x.Failed);
        this.Ignored += testClassResults.Sum(x => x.Ignored);
    }
}