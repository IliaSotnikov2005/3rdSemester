// <copyright file="SingleThreadedLazy.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// The version with a guarantee of correct operation in single-threaded mode (without synchronization).
/// </summary>
/// <typeparam name="T">The type of the value to be lazily initialized.</typeparam>
/// <param name="supplier">A function that will be calculated lazily.</param>
public class SingleThreadedLazy<T>(Func<T> supplier) : ILazy<T>
{
    private bool evaluated = false;
    private T? value;

    /// <inheritdoc/>
    public T? Get()
    {
        if (!this.evaluated)
        {
            this.value = supplier();
            this.evaluated = true;
#pragma warning disable CS8625
            supplier = null;
#pragma warning restore CS8625
        }

        return this.value ?? throw new NullValueException("Value is null.");
    }
}