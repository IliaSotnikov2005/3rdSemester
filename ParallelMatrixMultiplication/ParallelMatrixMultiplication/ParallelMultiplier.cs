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

        var threads = new Thread[Environment.ProcessorCount];

        for (int i = 0; i < rows; ++i)
        {
            int rowIndex = i;

            threads[i] = new Thread(() =>
            {
                for (int column = 0; column < columns; ++column)
                {
                    result[rowIndex][column] = 0;
                    for (int j = 0; j < matrix1[0].Length; ++j)
                    {
                        result[rowIndex][column] += matrix1[rowIndex][j] * matrix2[j][column];
                    }
                }
            });

            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return result;
    }
}
