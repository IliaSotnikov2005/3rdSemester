// <copyright file="CheckSumCalculatorsComparison.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace CheckSumCalculator;

/// <summary>
/// Class for comparison between two realisations.
/// </summary>
public static class CheckSumCalculatorsComparison
{
    /// <summary>
    /// Method to compare single and multi thread calculator.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <returns>Task.</returns>
    public static async Task Compare(string path)
    {
        var singleThreadWatch = System.Diagnostics.Stopwatch.StartNew();
        byte[] result1 = CheckSumSingleThreadCalculator.Run(path);
        singleThreadWatch.Stop();
        var singleThreadElapsed = singleThreadWatch.ElapsedMilliseconds;

        var multiThreadWatch = System.Diagnostics.Stopwatch.StartNew();
        byte[] result2 = await CheckSumMultiThreadCalculator.RunAsync(path);
        multiThreadWatch.Stop();
        var multiThreadElapsed = multiThreadWatch.ElapsedMilliseconds;

        Console.WriteLine($"////////////////////");
        Console.WriteLine($"Single thread calculator: {singleThreadElapsed} ms.");
        Console.WriteLine(string.Join(", ", result1));
        Console.WriteLine($"Multi thread calculator: {multiThreadElapsed} ms.");
        Console.WriteLine(string.Join(", ", result2));
        Console.WriteLine($"////////////////////");
    }
}