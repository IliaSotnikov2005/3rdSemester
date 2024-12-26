// <copyright file="IMyTask.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>
namespace MyThreadPool;

/// <summary>
/// The interface representing the task.
/// </summary>
/// <typeparam name="TResult">The returned result.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether task is completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of the task.
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Sets an ongoing task.
    /// </summary>
    /// <typeparam name="TNewResult">Result of the task.</typeparam>
    /// <param name="nextTask">Next task.</param>
    /// <returns>New task.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> nextTask);
}