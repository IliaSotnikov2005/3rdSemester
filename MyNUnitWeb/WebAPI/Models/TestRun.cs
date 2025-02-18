// <copyright file="TestRun.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace WebAPI.Models;

using MyNUnit;

/// <summary>
/// Test run model.
/// </summary>
public class TestRun
{
    /// <summary>
    /// Gets the id.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the launch time.
    /// </summary>
    public DateTime LaunchTime { get; init; }

    /// <summary>
    /// Gets the test run result.
    /// </summary>
    public required TestRunResult Result { get; init; }
}
