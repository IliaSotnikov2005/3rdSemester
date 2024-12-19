// <copyright file="Program.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly
using ParallelMatrixMultiplication;
#pragma warning restore SA1200 // Using directives should be placed correctly

if (args.Length == 0)
{
    Console.WriteLine("There are no arguments.\nUse -help for help\n-m <filename> <filename> to multiply the matrices\n-b to run the benchmark.");

    return;
}

switch (args[0])
{
    case "-help":
        Console.WriteLine("Use -m <filename> <filename> to multiply the matrices\n-b to run the benchmark.");
        break;

    case "-m":
        if (args.Length < 3)
        {
            Console.WriteLine("Enter filenames for multiplication.");
            return;
        }

        string filename1 = args[1];
        string filename2 = args[2];

        try
        {
            int[][] matrix1 = ExtractMatrix(filename1);
            int[][] matrix2 = ExtractMatrix(filename2);

            var parallelMultiplier = new ParallelMultiplier();
            int[][] result = parallelMultiplier.Multiply(matrix1, matrix2);

            WriteMatrix(result);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found.");
            return;
        }

        Console.WriteLine("OK");
        break;

    case "-b":
        RunBenchmark();
        break;

    default:
        Console.WriteLine("Unknown command.\nUse -m <filename> <filename> to multiply the matrices\n-b to run the benchmark.");
        break;
}

void RunBenchmark()
{
    var benchmark = new MatrixMultiplicationBenchmark();

    for (int size = 100; size < 1001; size += 100)
    {
        benchmark.Benchmark(size, 3);
    }

    benchmark.MakeGraph();
}

int[][] ExtractMatrix(string filename)
{
    if (!File.Exists(filename))
    {
        throw new FileNotFoundException(filename);
    }

    var lines = File.ReadAllLines(filename);
    int[][] result = new int[lines.Length][];

    for (int i = 0; i < lines.Length; i++)
    {
        result[i] = Array.ConvertAll(lines[i].Split(' '), int.Parse);
    }

    return result;
}

void WriteMatrix(int[][] matrix)
{
    using StreamWriter sw = new("result.txt");
    for (int i = 0; i < matrix.Length; ++i)
    {
        sw.WriteLine(string.Join(" ", matrix[i]));
    }

    sw.Close();
}
