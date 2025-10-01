// <copyright file="TestRunResult.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Class represents a test run result.
/// </summary>
public class TestRunResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestRunResult"/> class.
    /// </summary>
    /// <param name="testAssemblyResults">The test assembly results.</param>
    public TestRunResult(List<TestAssemblyResult> testAssemblyResults)
    {
        this.AddTestAssemblyResults(testAssemblyResults.ToList());
    }

    private TestRunResult()
    {
    }

    /// <summary>
    /// Gets the id of test run result.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the list of test assembly results.
    /// </summary>
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

    /// <summary>
    /// Gets a value indicating whether all tests are passed.
    /// </summary>
    public bool AllTestsPassed => this.Failed == 0;

    /// <summary>
    /// Adds test assembly results.
    /// </summary>
    /// <param name="testAssemblyResults">Test assembly results.</param>
    public void AddTestAssemblyResults(List<TestAssemblyResult> testAssemblyResults)
    {
        this.TestAssemblyResults.AddRange(testAssemblyResults);
        this.Passed += testAssemblyResults.Sum(x => x.Passed);
        this.Failed += testAssemblyResults.Sum(x => x.Failed);
        this.Ignored += testAssemblyResults.Sum(x => x.Ignored);
    }
}
