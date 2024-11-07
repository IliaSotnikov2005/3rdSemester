// <copyright file="Program.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Main entry point for program.
/// </summary>
public class Program
{
    /// <summary>
    /// Main method.
    /// </summary>
    /// <param name="args">Arguments for application.</param>
    /// <returns>Task.</placeholder></returns>
    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine($"Invalid input.");
            return;
        }

        string path = args[0];

        // string path = Path.GetFullPath("../ProjectForTesting/bin/Debug/net8.0");
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"File '{path}' not found.");
            return;
        }

        await Tester.RunTestAndPrintResultsAsync(path);
    }
}