// <copyright file="MyThreadPool.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyThreadPoolSpace;

using System.Threading;

/// <summary>
/// Represents a thread pool for executing tasksin multiple threads.
/// </summary>
public class MyThreadPool
{
    private readonly Queue<Action> tasksQueue = new();
    private readonly Thread[] threads;
    private readonly CancellationTokenSource cancellationSource = new();
    private readonly AutoResetEvent gettingStarted = new(false);

    /// <summary>
    /// The number of tasks for which a result has not yet been received.
    /// </summary>
    private volatile int remainingResults = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="numberOfThreads">The number of threads.</param>
    /// <exception cref="ArgumentException">Throws if number of threads is not positive.</exception>
    public MyThreadPool(int numberOfThreads)
    {
        if (numberOfThreads <= 0)
        {
            throw new ArgumentException("Number of threads must be positive", nameof(numberOfThreads));
        }

        this.threads = new Thread[numberOfThreads];
        for (int i = 0; i < numberOfThreads; ++i)
        {
            this.threads[i] = new Thread(() => this.PerformTasks(this.cancellationSource.Token));
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Gets a value indicating whether thread pool shutted down.
    /// </summary>
    public bool ShuttedDown { get; private set; } = false;

    /// <summary>
    /// Adds a new task to the pool.
    /// </summary>
    /// <typeparam name="TResult">The type of function execution result.</typeparam>
    /// <param name="function">Function to be performed in the task.</param>
    /// <returns>Task object.</returns>
    /// <exception cref="OperationCanceledException">Throws if threadpool was shutted down.</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (this.cancellationSource.IsCancellationRequested)
        {
            throw new AggregateException("Thread pool was shutted down.");
        }

        var newTask = new MyTask<TResult>(function, this, this.cancellationSource.Token);

        lock (this.tasksQueue)
        {
            this.tasksQueue.Enqueue(newTask.Start);
        }

        Interlocked.Increment(ref this.remainingResults);

        this.gettingStarted.Set();

        return newTask;
    }

    /// <summary>
    /// Shut down the threadpool.
    /// </summary>
    public void Shutdown()
    {
        if (this.cancellationSource.IsCancellationRequested)
        {
            return;
        }

        this.cancellationSource.Cancel();

        foreach (var thread in this.threads)
        {
            thread.Join();
        }

        this.ShuttedDown = true;
    }

    /// <summary>
    /// Adds continuing task to the pool.
    /// </summary>
    /// <param name="continuingTaskStartAction">Continuing task start action.</param>
    internal void AddContinuingTask(Action continuingTaskStartAction)
    {
        lock (this.tasksQueue)
        {
            this.tasksQueue.Enqueue(continuingTaskStartAction);
        }

        this.gettingStarted.Set();
    }

    /// <summary>
    /// Increments remaining resulst.
    /// </summary>
    internal void IncrementRemainingResults()
    {
        Interlocked.Increment(ref this.remainingResults);
    }

    /// <summary>
    /// Decrements remaining resulst.
    /// </summary>
    internal void DecrementRemainingResults()
    {
        Interlocked.Decrement(ref this.remainingResults);
    }

    private void PerformTasks(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested || this.remainingResults > 0)
        {
            Action? myTask = null;

            lock (this.tasksQueue)
            {
                if (this.tasksQueue.Count == 0)
                {
                    continue;
                }

                myTask = this.tasksQueue.Dequeue();
            }

            myTask();
        }
    }

    /// <summary>
    /// Represents a task that executes function.
    /// </summary>
    /// <typeparam name="TResult">The type of function result.</typeparam>
    /// <param name="function">The function to be executed.</param>
    /// <param name="threadPool">The trhread pool object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public class MyTask<TResult>(Func<TResult> function, MyThreadPool threadPool, CancellationToken cancellationToken) : IMyTask<TResult>
    {
        private readonly ManualResetEvent resultReadiness = new(false);
        private TResult? result;
        private Exception? thrownException = null;
        private List<Action> continuingTasks = [];

        /// <inheritdoc/>
        public bool IsCompleted { get; private set; } = false;

        /// <inheritdoc/>
        public TResult Result
        {
            get
            {
                this.resultReadiness.WaitOne();

                if (this.thrownException is not null)
                {
                    throw new AggregateException("Task failed.", this.thrownException);
                }
                else if (this.result is null)
                {
                    throw new ArgumentException("Function does not return a not null result.");
                }

                this.resultReadiness.Set();

                return this.result;
            }
        }

        /// <inheritdoc/>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> nextTask)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new AggregateException("Thread pool is shutted down.");
            }

            var continuingTask = new MyTask<TNewResult>(() => nextTask(this.Result), threadPool, cancellationToken);
            if (this.IsCompleted)
            {
                threadPool.AddContinuingTask(continuingTask.Start);
            }
            else
            {
                this.continuingTasks.Add(continuingTask.Start);
            }

            Interlocked.Increment(ref threadPool.remainingResults);

            return continuingTask;
        }

        /// <summary>
        /// Starts the task.
        /// </summary>
        internal void Start()
        {
            try
            {
                this.result = function();
                function = null!;
            }
            catch (Exception ex)
            {
                this.thrownException = ex;
            }

            this.IsCompleted = true;
            this.resultReadiness.Set();
            Interlocked.Decrement(ref threadPool.remainingResults);

            this.continuingTasks.ForEach(threadPool.AddContinuingTask);
        }
    }
}