// <copyright file="MatrixMultiplicationBenchmark.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

using System.Diagnostics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Legends;
using OxyPlot.Series;

/// <summary>
/// Benchmark for parallel and sequence matrix multiplication.
/// </summary>
public static class MatrixMultiplicationBenchmark
{
    private static List<double> parallelTimesMeans = [];
    private static List<double> sequentialTimesMeans = [];
    private static List<int> sizes = [];

    /// <summary>
    /// The method to run the benchmark.
    /// </summary>
    /// <param name="size">Size of matrices.</param>
    /// <param name="runs">A number of launches.</param>
    public static void Benchmark(int size, int runs)
    {
        int[][] matrix1 = GenerateMatrix(size);
        int[][] matrix2 = GenerateMatrix(size);

        double[] sequentialTimes = new double[runs];
        double[] parallelTimes = new double[runs];

        var sequentialMultiplier = new SequentialMultiplier();
        var parallerMultiplier = new ParallelMultiplier();

        for (int i = 0; i < runs; ++i)
        {
            var stopwatch = Stopwatch.StartNew();
            sequentialMultiplier.Multiply(matrix1, matrix2);
            stopwatch.Stop();
            sequentialTimes[i] = stopwatch.Elapsed.TotalMilliseconds;

            stopwatch.Restart();
            parallerMultiplier.Multiply(matrix1, matrix2);
            stopwatch.Stop();
            parallelTimes[i] = stopwatch.Elapsed.TotalMilliseconds;
        }

        double sequentialMean = CalculateMean(sequentialTimes);
        double parallelMean = CalculateMean(parallelTimes);
        double sequentialStdDev = CalculateStandardDeviation(sequentialTimes, sequentialMean);
        double parallelStdDev = CalculateStandardDeviation(parallelTimes, parallelMean);

        sequentialTimesMeans.Add(sequentialMean);
        parallelTimesMeans.Add(parallelMean);
        sizes.Add(size);

        Console.WriteLine($"Size: {size}, Sequential Mean: {sequentialMean} ms, Sequential StdDev: {sequentialStdDev} ms");
        Console.WriteLine($"Size: {size}, Parallel Mean: {parallelMean} ms, Parallel StdDev: {parallelStdDev} ms\n");
    }

    /// <summary>
    /// The method of plotting based on the results.
    /// </summary>
    public static void MakeGraph()
    {
        var plotModel = new PlotModel
        {
            Title = "Performance graph",
            Background = OxyColor.FromRgb(255, 255, 255),
        };
        plotModel.Legends.Add(new Legend { LegendTitle = "Legend", LegendPosition = LegendPosition.BottomRight });
        plotModel.IsLegendVisible = true;

        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Matrix sizes" });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Time (sec)" });

        var sequentialSeries = new LineSeries { Title = "Sequential multiplication", MarkerType = MarkerType.Square };
        for (int i = 0; i < sizes.Count; ++i)
        {
            sequentialSeries.Points.Add(new DataPoint(sizes[i], sequentialTimesMeans[i]));
        }

        plotModel.Series.Add(sequentialSeries);

        var parallelSeries = new LineSeries { Title = "Parallel multiplication", MarkerType = MarkerType.Circle };
        for (int i = 0; i < sizes.Count; ++i)
        {
            parallelSeries.Points.Add(new DataPoint(sizes[i], parallelTimesMeans[i]));
        }

        plotModel.Series.Add(parallelSeries);

        var pngExporter = new PngExporter(800, 600, 96);
        using (var stream = File.Create("graph.png"))
        {
            pngExporter.Export(plotModel, stream);
        }

        Console.WriteLine("The graph is saved as graph.png");
    }

    private static int[][] GenerateMatrix(int size)
    {
        var random = new Random();
        var matrix = new int[size][];

        for (int i = 0; i < size; ++i)
        {
            matrix[i] = new int[size];
            for (int j = 0; j < size; ++j)
            {
                matrix[i][j] = random.Next(1, 10);
            }
        }

        return matrix;
    }

    private static double CalculateMean(double[] times)
    {
        double sum = 0;
        foreach (var time in times)
        {
            sum += time;
        }

        return sum / times.Length;
    }

    private static double CalculateStandardDeviation(double[] times, double mean)
    {
        double sumOfSquares = 0;
        foreach (var time in times)
        {
            sumOfSquares += Math.Pow(time - mean, 2);
        }

        return Math.Sqrt(sumOfSquares / times.Length);
    }
}
