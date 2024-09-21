// <copyright file="ILazy.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Interface for lazy computation.
/// </summary>
/// <typeparam name="T">The type of the returned value.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Gets the lazily initialized value.
    /// </summary>
    /// <returns>The lazily initialized value.</returns>
    /// <exception cref="NullValueException">Thrown when the value is null.</exception>
    public T Get();
}