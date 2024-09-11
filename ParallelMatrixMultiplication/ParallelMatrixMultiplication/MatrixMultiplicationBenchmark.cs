using OxyPlot.Series;
using OxyPlot;
using System.Diagnostics;
using OxyPlot.ImageSharp;
using System.Text;
using OxyPlot.Axes;
using OxyPlot.Legends;

namespace ParallelMatrixMultiplication;

public static class MatrixMultiplicationBenchmark
{
    private static List<double> parallelTimesMeans = [];
    private static List<double> sequentialTimesMeans = [];
    private static List<int> sizes = [];

    private static int[][] GenerateMatrix(int size)
    {
        var random = new Random();
        var matrix = new int[size][];

        for (int i = 0; i < size; i++)
        {
            matrix[i] = new int[size];
            for (int j = 0; j < size; j++)
            {
                matrix[i][j] = random.Next(1, 10);
            }
        }

        return matrix;
    }

    public static void Benchmark(int size, int runs)
    {
        int[][] matrix1 = GenerateMatrix(size);
        int[][] matrix2 = GenerateMatrix(size);

        double[] sequentialTimes = new double[runs];
        double[] parallelTimes = new double[runs];

        for (int i = 0; i < runs; ++i)
        {
            var stopwatch = Stopwatch.StartNew();
            SequentialMultiplication.Multiply(matrix1, matrix2);
            stopwatch.Stop();
            sequentialTimes[i] = stopwatch.Elapsed.TotalMilliseconds;

            stopwatch.Restart();
            ParallelMultiplication.Multiply(matrix1, matrix2);
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

    public static void MakeGraph()
    {
        var plotModel = new PlotModel
        {
            Title = "График производительности",
            Background = OxyColor.FromRgb(255, 255, 255)
        };
        plotModel.Legends.Add(new Legend { LegendTitle = "Легенда", LegendPosition = LegendPosition.BottomRight });
        plotModel.IsLegendVisible = true;

        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Размеры матриц" });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Время (сек)" });

        var sequentialSeries = new LineSeries { Title = "Последовательное умножение", MarkerType = MarkerType.Square };
        for (int i = 0; i < sizes.Count; ++i)
        {
            sequentialSeries.Points.Add(new DataPoint(sizes[i], sequentialTimesMeans[i]));
        }
        plotModel.Series.Add(sequentialSeries);

        var parallelSeries = new LineSeries { Title = "Параллельное умножение", MarkerType = MarkerType.Circle };
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

        Console.WriteLine("График сохранен как graph.png");
    }
}
