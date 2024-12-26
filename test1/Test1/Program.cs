// <copyright file="Program.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace CheckSumCalculator;

/// <summary>
/// Main entry point for program.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main function.
    /// </summary>
    /// <returns>Task.</returns>
    public static async Task Main()
    {
        await CheckSumCalculatorsComparison.Compare("testFolder");
    }
}
