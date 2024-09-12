// <copyright file="ParallelMultiplier.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

/// <summary>
/// The class of parallel matrix multiplication.
/// </summary>
public class ParallelMultiplier : IMatrixMultiplier
{
    private int[][] result = [];
    private int[][] matrix1 = [];
    private int[][] matrix2 = [];
    private int columns = 0;
    private int rows = 0;

    /// <inheritdoc/>
    public int[][] Multiply(int[][] inputMatrix1, int[][] inputMatrix2)
    {
        if (inputMatrix1.Length != inputMatrix2[0].Length)
        {
            throw new ArgumentException("Number of rows in matrix1 must be equal to number of columns in matrix2.");
        }

        this.matrix1 = inputMatrix1;
        this.matrix2 = inputMatrix2;

        this.rows = this.matrix1.Length;
        this.columns = this.matrix2[0].Length;

        this.result = new int[this.rows][];
        for (int i = 0; i < this.rows; ++i)
        {
            this.result[i] = new int[this.columns];
        }

        Thread[] threads = new Thread[this.rows];

        for (int i = 0; i < this.rows; ++i)
        {
            int rowIndex = i;

            threads[i] = new Thread(() => this.MultiplyRowByColumn(rowIndex));
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return this.result;
    }

    private void MultiplyRowByColumn(int rowIndex)
    {
        for (int column = 0; column < this.columns; ++column)
        {
            this.result[rowIndex][column] = 0;
            for (int i = 0; i < this.matrix1[0].Length; ++i)
            {
                this.result[rowIndex][column] += this.matrix1[rowIndex][i] * this.matrix2[i][column];
            }
        }
    }
}
