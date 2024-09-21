// <copyright file="MultiThreadedLazy.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Represents a lazy-initialized, thread-safe value.
/// </summary>
/// <typeparam name="T">The type of the value to be lazily initialized.</typeparam>
/// <param name="supplier">A function that will be calculated lazily.</param>
public class MultiThreadedLazy<T>(Func<T> supplier) : ILazy<T>
{
    private readonly Func<T> supplier = supplier;
    private readonly object lockObject = new ();
    private bool evaluated = false;
    private T? value;

    /// <inheritdoc/>
    public T Get()
    {
        if (this.evaluated)
        {
            return this.value ?? throw new NullValueException("Value is null.");
        }

        lock (this.lockObject)
        {
            if (!this.evaluated)
            {
                this.value = this.supplier();
                this.evaluated = true;
            }
        }

        return this.value ?? throw new NullValueException("Value is null.");
    }
}