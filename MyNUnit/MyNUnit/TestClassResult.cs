// <copyright file="MyTestClassResults.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Class for test class results.
/// </summary>
public class TestClassResult
{
    private TestClassResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestClassResult"/> class.
    /// </summary>
    /// <param name="results">List of test results.</param>
    public TestClassResult(string testClassName, List<TestResult> results)
    {
        this.TestClassName = testClassName;
        this.AddResults(results);
    }

    public int Id { get; set; }

    public string TestClassName { get; init; }

    /// <summary>
    /// Gets the number of passed tests.
    /// </summary>
    public int Passed { get; private set; }

    /// <summary>
    /// Gets the number of failed tests.
    /// </summary>
    public int Failed { get; private set; }

    /// <summary>
    /// Gets the number of ignored tests.
    /// </summary>
    public int Ignored { get; private set; }

    /// <summary>
    /// Gets the number of errore tests.
    /// </summary>
    public int Errored { get; private set; }

    /// <summary>
    /// Gets the test results.
    /// </summary>
    public List<TestResult> TestResults { get; private set; } = [];

    /// <summary>
    /// Adds test results to the class.
    /// </summary>
    /// <param name="results">List of test results.</param>
    public void AddResults(List<TestResult> results)
    {
        foreach (var result in results)
        {
            switch (result.Status)
            {
                case TestStatus.Passed:
                ++this.Passed;
                break;
                case TestStatus.Failed:
                ++this.Failed;
                break;
                case TestStatus.Ignored:
                ++this.Ignored;
                break;
                case TestStatus.Errored:
                ++this.Errored;
                break;
            }

            this.TestResults.Add(result);
        }
    }
}