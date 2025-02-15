namespace MyNUnit;

public class TestRunResult
{
    private TestRunResult()
    {
    }

    public TestRunResult(List<TestAssemblyResult> testAssemblyResults)
    {
        this.AddTestAssemblyResults(testAssemblyResults.ToList());
    }

    public int Id { get; set; }

    public List<TestAssemblyResult> TestAssemblyResults { get; private set; } = [];

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

    public void AddTestAssemblyResults(List<TestAssemblyResult> testAssemblyResults)
    {
        this.TestAssemblyResults.AddRange(testAssemblyResults);
        this.Passed += testAssemblyResults.Sum(x => x.Passed);
        this.Failed += testAssemblyResults.Sum(x => x.Failed);
        this.Ignored += testAssemblyResults.Sum(x => x.Ignored);
    }

    public bool AllTestsPassed => this.Failed == 0;
}
