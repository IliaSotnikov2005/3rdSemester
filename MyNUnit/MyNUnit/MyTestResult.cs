// <copyright file="MyTestResult.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Class for test result.
/// </summary>
/// <param name="name">Name of the test.</param>
/// <param name="status">Status of the test.</param>
/// <param name="message">Message of the test.</param>
public record MyTestResult(string name, string status, string message)
{
    /// <summary>
    /// Gets name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets status.
    /// </summary>
    public string Status { get; } = status;

    /// <summary>
    /// Gets message.
    /// </summary>
    public string Message { get; } = message;
}