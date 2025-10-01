// <copyright file="ParallelMatrixMultiplicationTests.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace ParallelMatrixMultiplicationTest;

using ParallelMatrixMultiplication;

/// <summary>
/// Tests for matrix multiplication.
/// </summary>
public class ParallelMatrixMultiplicationTests
{
    /// <summary>
    /// Checks that the multiplication is performed correctly.
    /// </summary>
    /// <param name="matrixMultiplier">Type of myltiplier.</param>
    [TestCaseSource(nameof(ParallelMultiplicationTestData))]
    public void MatrixMultiplication_ReturnsCorrect(IMatrixMultiplier matrixMultiplier)
    {
        int[][] matrix1 = [[1, 2], [3, 4]];
        int[][] matrix2 = [[5, 6], [7, 8]];

        int[][] expected = [[19, 22], [43, 50]];

        var result = matrixMultiplier.Multiply(matrix1, matrix2);

        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Checks that throws exception if the number of rows and columns is different.
    /// </summary>
    /// <param name="matrixMultiplier">Type of myltiplier.</param>
    [TestCaseSource(nameof(ParallelMultiplicationTestData))]
    public void Multiplication_MismatchRowsColumns_ThrowsException(IMatrixMultiplier matrixMultiplier)
    {
        int[][] matrix1 = [[1, 2], [3, 4], [1, 1]];

        int[][] matrix2 = [[5, 6], [7, 8]];

        Assert.Throws<ArgumentException>(() => matrixMultiplier.Multiply(matrix1, matrix2));
    }

    /// <summary>
    /// Checks if calculator throws an exception with null matrix1.
    /// </summary>
    /// <param name="matrixMultiplier">Matrix multiplier.</param>
    [TestCaseSource(nameof(ParallelMultiplicationTestData))]
    public void Multiplication_NullMatrix1_ThrowsException(IMatrixMultiplier matrixMultiplier)
    {
        int[][] matrix2 = [[5, 6], [7, 8]];

        Assert.Throws<ArgumentNullException>(() => matrixMultiplier.Multiply(null, matrix2));
    }

     /// <summary>
    /// Checks if calculator throws an exception with null matrix2.
    /// </summary>
    /// <param name="matrixMultiplier">Matrix multiplier.</param>
    [TestCaseSource(nameof(ParallelMultiplicationTestData))]
    public void Multiplication_NullMatrix2_ThrowsException(IMatrixMultiplier matrixMultiplier)
    {
        int[][] matrix1 = [[1, 2], [3, 4]];

        Assert.Throws<ArgumentNullException>(() => matrixMultiplier.Multiply(matrix1, null));
    }

    /// <summary>
    /// Checks if calculator throws an exception with empty matrix1.
    /// </summary>
    /// <param name="matrixMultiplier">Matrix multiplier.</param>
    [TestCaseSource(nameof(ParallelMultiplicationTestData))]
    public void Multiplication_EmptyMatrix1_ThrowsException(IMatrixMultiplier matrixMultiplier)
    {
        int[][] matrix1 = { };
        int[][] matrix2 = [[5, 6], [7, 8]];

        Assert.Throws<ArgumentException>(() => matrixMultiplier.Multiply(matrix1, matrix2));
    }

    /// <summary>
    /// Checks if calculator throws an exception with empty matrix1.
    /// </summary>
    /// <param name="matrixMultiplier">Matrix multiplier.</param>
    [TestCaseSource(nameof(ParallelMultiplicationTestData))]
    public void Multiplication_EmptyMatrix2_ThrowsException(IMatrixMultiplier matrixMultiplier)
    {
        int[][] matrix1 = [[1, 2], [3, 4]];
        int[][] matrix2 = { };

        Assert.Throws<ArgumentException>(() => matrixMultiplier.Multiply(matrix1, matrix2));
    }

    /// <summary>
    /// Checks if parallel matrix multiplication result mathes sequential one.
    /// </summary>
    [Test]
    public void ParallelMultiplication_BigMatrices_MatchesSequentialResult()
    {
        int size = 100;
        int[][] matrix1 = GenerateBigMatrix(size, size);
        int[][] matrix2 = GenerateBigMatrix(size, size);

        var sequentialMultiplier = new SequentialMultiplier();
        var parallelMultiplier = new ParallelMultiplier();

        int[][] sequentialResult = sequentialMultiplier.Multiply(matrix1, matrix2);
        int[][] parallelResult = parallelMultiplier.Multiply(matrix1, matrix2);

        Assert.That(parallelResult, Has.Length.EqualTo(sequentialResult.Length));
        for (int i = 0; i < sequentialResult.Length; i++)
        {
            Assert.That(parallelResult[i], Is.EqualTo(sequentialResult[i]));
        }
    }

    private static int[][] GenerateBigMatrix(int rows, int cols)
    {
        var random = new Random();
        var matrix = new int[rows][];
        for (int i = 0; i < rows; ++i)
        {
            matrix[i] = new int[cols];
            for (int j = 0; j < cols; ++j)
            {
                matrix[i][j] = random.Next(1, 100);
            }
        }

        return matrix;
    }

    private static IEnumerable<TestCaseData> ParallelMultiplicationTestData()
    {
        yield return new TestCaseData(new SequentialMultiplier());
        yield return new TestCaseData(new ParallelMultiplier());
    }
}
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
