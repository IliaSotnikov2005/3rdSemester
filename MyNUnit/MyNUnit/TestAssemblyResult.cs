// <copyright file="TestAssemblyResult.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Class that stores test results in a separate assembly.
/// </summary>
public class TestAssemblyResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestAssemblyResult"/> class.
    /// </summary>
    /// <param name="assemblyName">The name of assembly.</param>
    /// <param name="testClassResults">The list of test class results.</param>
    public TestAssemblyResult(string assemblyName, List<TestClassResult> testClassResults)
    {
        this.AssemblyName = assemblyName;
        this.AddTestClassResults(testClassResults);
    }

    private TestAssemblyResult()
    {
    }

    /// <summary>
    /// Gets the id of test assembly result.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the list of test class results.
    /// </summary>
    public List<TestClassResult> TestClassResults { get; private set; } = [];

    /// <summary>
    /// Gets the assembly name.
    /// </summary>
    public string AssemblyName { get; init; } = string.Empty;

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
    /// Adds test class results.
    /// </summary>
    /// <param name="testClassResults">Test class results.</param>
    public void AddTestClassResults(List<TestClassResult> testClassResults)
    {
        this.TestClassResults.AddRange(testClassResults);
        this.Passed += testClassResults.Sum(x => x.Passed);
        this.Failed += testClassResults.Sum(x => x.Failed);
        this.Ignored += testClassResults.Sum(x => x.Ignored);
    }
}