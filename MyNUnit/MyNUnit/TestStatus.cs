// <copyright file="TestStatus.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Test statuses.
/// </summary>
public enum TestStatus
{
    /// <summary>
    /// Passed status.
    /// </summary>
    Passed,

    /// <summary>
    /// Failed status.
    /// </summary>
    Failed,

    /// <summary>
    /// Ignored status.
    /// </summary>
    Ignored,

    /// <summary>
    /// Errored status.
    /// </summary>
    Errored,
}