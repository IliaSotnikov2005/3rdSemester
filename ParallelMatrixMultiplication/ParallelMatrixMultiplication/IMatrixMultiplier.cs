// <copyright file="IMatrixMultiplier.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

/// <summary>
/// Interface for matrix multiplier.
/// </summary>
public interface IMatrixMultiplier
{
    /// <summary>
    /// The method for multiplication.
    /// </summary>
    /// <param name="inputMatrix1">First matrix.</param>
    /// <param name="inputMatrix2">Second matrix.</param>
    /// <returns>Result of multiplication.</returns>
    /// <exception cref="ArgumentException">Throws if the matrices cannot be multiplied.</exception>
    public int[][] Multiply(int[][] inputMatrix1, int[][] inputMatrix2);
}