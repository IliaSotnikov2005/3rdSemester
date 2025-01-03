// <copyright file="MyTestResult.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Record for test result.
/// </summary>
/// <param name="Name">Name of the test.</param>
/// <param name="Status">Status of the test.</param>
/// <param name="Message">Message of the test.</param>
/// <param name="TimeElapsed">Time elapsed by the test.</param>
public record MyTestResult(string Name, TestStatus Status, string Message, TimeSpan TimeElapsed = default)
{
    /// <summary>
    /// Gets the status of the test run.
    /// </summary>
    public TestStatus Status { get; internal set; } = Status;

    /// <summary>
    /// Gets formatted time elapsed.
    /// </summary>
    /// <returns>Formatted time elapsed.</returns>
    public string GetFormattedTimeElapsed()
    {
        return $"{(int)this.TimeElapsed.TotalSeconds}.{this.TimeElapsed.Milliseconds}";
    }
}