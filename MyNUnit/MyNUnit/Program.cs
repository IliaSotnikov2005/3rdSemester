// <copyright file="Program.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly
using MyNUnit;
#pragma warning restore SA1200 // Using directives should be placed correctly

if (args.Length != 1)
{
    Console.WriteLine($"Invalid input. Expected 1 argument: path to the directory.");
    return;
}

string path = args[0];

if (!Directory.Exists(path))
{
    Console.WriteLine($"File '{path}' not found.");
    return;
}

await Tester.RunTestAndPrintResultsAsync(path);