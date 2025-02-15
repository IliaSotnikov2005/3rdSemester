// <copyright file="Program.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

using MyNUnit;
using System.Net.WebSockets;

if (args.Length != 1 && false)
{
    Console.WriteLine($"Invalid input. Expected 1 argument: path to the assembly.");
    return;
}

string path = args[0];

if (!Directory.Exists(path))
{
    Console.WriteLine($"File '{path}' not found.");
    return;
}

var tester = new MyTester();
var testsResults = await tester.RunTestsFromDirectory(path);
PrintTestsResults(testsResults);

void PrintTestsResults(TestRunResult results)
{
    foreach (var testAssemblyResult in results.TestAssemblyResults)
    {
        Console.WriteLine($"Test assembly: {testAssemblyResult.AssemblyName}");
        Console.WriteLine($"Passed: {testAssemblyResult.Passed}");
        Console.WriteLine($"Failed: {testAssemblyResult.Failed}");
        Console.WriteLine($"Ignored: {testAssemblyResult.Ignored}");

        foreach (var testClassResult in testAssemblyResult.TestClassResults)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Test Class Results:");
            Console.ResetColor();

            Console.WriteLine($"Passed: {testClassResult.Passed}");
            Console.WriteLine($"Failed: {testClassResult.Failed}");
            Console.WriteLine($"Ignored: {testClassResult.Ignored}");
            Console.WriteLine($"Errored: {testClassResult.Errored}");

            foreach (var testResult in testClassResult.TestResults)
            {
                PrintTestResult(testResult);
            }

            Console.WriteLine();
        }
    }
}

void PrintTestResult(TestResult testResult)
{
    if (testResult.Status == TestStatus.Passed)
    {
        Console.ForegroundColor = ConsoleColor.Green;
    }
    else if (testResult.Status == TestStatus.Failed)
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }
    else if (testResult.Status == TestStatus.Ignored)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }

    Console.WriteLine($"- {testResult.Name}: {testResult.Status};");
    Console.WriteLine($"Time elapsed: {testResult.GetFormattedTimeElapsed()} s");
    if (!string.IsNullOrEmpty(testResult.Message))
    {
        Console.WriteLine($"Message: {testResult.Message}");
    }

    Console.ResetColor();
}