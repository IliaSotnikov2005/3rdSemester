// <copyright file="MyThreadPool.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Represents a thread pool for executing tasksin multiple threads.
/// </summary>
public class MyThreadPool
{
    private readonly Thread[] threads;
    private readonly Queue<Action> tasksQueue = new ();
    private readonly CancellationTokenSource cancellationSource = new ();
    private readonly AutoResetEvent gettingStarted = new (false);

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
            throw new OperationCanceledException("The threadpool was shutted down.", this.cancellationSource.Token);
        }

        var newTask = new MyTask<TResult>(function, this);

        lock (this.tasksQueue)
        {
            this.tasksQueue.Enqueue(newTask.Start);
        }

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
    }

    private void PerformTasks(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            this.gettingStarted.WaitOne();
            Action? task = null;

            lock (this.tasksQueue)
            {
                if (this.tasksQueue.Count == 0)
                {
                    Monitor.PulseAll(this.tasksQueue);
                    continue;
                }

                task = this.tasksQueue.Dequeue();
            }

            task();

            this.gettingStarted.Set();
        }
    }

    private void AddContinuingTask(Action taskStartAction)
    {
        lock (this.tasksQueue)
        {
            this.tasksQueue.Enqueue(taskStartAction);
        }

        this.gettingStarted.Set();
    }

    /// <summary>
    /// Represents a task that executes function.
    /// </summary>
    /// <typeparam name="TResult">The type of function result.</typeparam>
    /// <param name="function">The function to be executed.</param>
    /// <param name="threadPool">The trhread pool object.</param>
    public class MyTask<TResult>(Func<TResult> function, MyThreadPool threadPool) : IMyTask<TResult>
    {
        private readonly MyThreadPool threadPool = threadPool;
        private readonly Func<TResult> function = function;
        private readonly ManualResetEvent resultReadiness = new (false);
        private TResult? result;
        private Exception? thrownException = null;
        private Action? continuingTaskStartAction = null;

        /// <inheritdoc/>
        public bool IsCompleted { get; private set; } = false;

        /// <inheritdoc/>
        public TResult Result
        {
            get
            {
                this.resultReadiness.WaitOne();

                if (this.thrownException != null)
                {
                    throw new AggregateException("Task failed.", this.thrownException);
                }

                if (this.result == null)
                {
                    throw new ArgumentException("Function does not return a result.");
                }

                return this.result;
            }
        }

        /// <summary>
        /// Starts the task.
        /// </summary>
        public void Start()
        {
            if (this.IsCompleted)
            {
                return;
            }

            try
            {
                this.result = this.function();
                this.IsCompleted = true;
            }
            catch (Exception ex)
            {
                this.thrownException = ex;
            }

            this.resultReadiness.Set();

            if (this.continuingTaskStartAction != null)
            {
                this.threadPool.AddContinuingTask(this.continuingTaskStartAction!);
            }
        }

        /// <inheritdoc/>
        IMyTask<TNewResult> IMyTask<TResult>.ContinueWith<TNewResult>(Func<TResult, TNewResult> nextTask)
        {
            if (this.IsCompleted)
            {
                return this.threadPool.Submit(() => nextTask(this.Result));
            }

            var continuingTask = new MyTask<TNewResult>(() => nextTask(this.Result), this.threadPool);
            this.continuingTaskStartAction = continuingTask.Start;

            return continuingTask;
        }
    }
}