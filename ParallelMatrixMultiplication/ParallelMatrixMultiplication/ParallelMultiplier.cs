// <copyright file="ParallelMultiplier.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

/// <summary>
/// The class of parallel matrix multiplication.
/// </summary>
public class ParallelMultiplier : IMatrixMultiplier
{
    /// <inheritdoc/>
    public int[][] Multiply(int[][] matrix1, int[][] matrix2)
    {
        if (matrix1 is null || matrix2 is null)
        {
            throw new ArgumentNullException("Matrix1 cannot be null.");
        }

        if (matrix1.Length == 0 || matrix2.Length == 0)
        {
            throw new ArgumentException("The matrices must not be empty.");
        }

        if (matrix1.Length != matrix2[0].Length)
        {
            throw new ArgumentException(
                "Number of rows in matrix1 must be equal to number of columns in matrix2.");
        }

        int rows = matrix1.Length;
        int columns = matrix2[0].Length;

        var result = new int[rows][];
        for (int i = 0; i < rows; ++i)
        {
            result[i] = new int[columns];
        }

        int threadsCount = Math.Min(Environment.ProcessorCount, rows * columns);
        var threads = new Thread[threadsCount];

        int chunkSize = (int)Math.Ceiling((float)(rows * columns) / threadsCount);

        for (int i = 0; i < threads.Length; ++i)
        {
            int chunkIndex = i;

            threads[i] = new Thread(() =>
            {
                CalculateChunk(matrix1, matrix2, result, chunkIndex, chunkSize);
            });
            
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return result;
    }

    private static void CalculateChunk(int[][] matrix1, int[][] matrix2, int[][] result, int chunkIndex, int chunkSize)
    {   
        for (int i = chunkIndex * chunkSize; i < Math.Min((chunkIndex + 1) * chunkSize, result.Length * result[0].Length); ++i)
        {
            int row = i / result[0].Length;
            int column = i % result[0].Length;

            result[row][column] = MultiplyRowByColumn(matrix1, matrix2, row, column);
        }
    }

    private static int MultiplyRowByColumn(int[][] matrix1, int[][] matrix2, int row, int column)
    {
        int result = 0;
        for (int i = 0; i < matrix1[0].Length; ++i)
        {
            result += matrix1[row][i] * matrix2[i][column];
        }

        return result;
    }
}
