using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MyThreadPool;

public class MyThreadPool
{
    private readonly Thread[] threads;
    private readonly Queue<Action> tasksQueue = new();
    private readonly CancellationTokenSource cancellationSource = new();
    private readonly AutoResetEvent gettingStarted = new(false);
    private readonly AutoResetEvent queueAccess = new(true);

    public MyThreadPool(int numberOfThreads)
    {
        if (numberOfThreads <= 0)
        {
            throw new ArgumentException("Number of threads must be positive", nameof(numberOfThreads));
        }

        threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; ++i)
        {
            threads[i] = new Thread(PerformTasks);
            threads[i].Start();
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        var newTask = new MyTask<TResult>(function, this, cancellationSource.Token);
        queueAccess.WaitOne();

        tasksQueue.Enqueue(newTask.Start);
        gettingStarted.Set();

        queueAccess.Set();

        return newTask;
    }

    private void ContinuousSubmit(Action task)
    {
        queueAccess.WaitOne();

        tasksQueue.Enqueue(task);
        gettingStarted.Set();
        
        queueAccess.Set();
    }

    public void Shutdown()
    {
        cancellationSource.Cancel();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        cancellationSource.Dispose();
    }

    private void PerformTasks()
    {
        while (!this.cancellationSource.IsCancellationRequested)
        {
            gettingStarted.WaitOne();
            queueAccess.WaitOne();

            Action task = tasksQueue.Dequeue();
            queueAccess.Set();

            task();

            gettingStarted.Set();
        }
    }

    public class MyTask<TResult>(Func<TResult> function, MyThreadPool threadPool, CancellationToken token) : IMyTask<TResult>
    {
        private readonly MyThreadPool threadPool = threadPool;
        private readonly Func<TResult> function = function;
        private TResult? result;
        private readonly AutoResetEvent resultAccess = new (false);
        private readonly CancellationToken token = token;
        private Exception? thrownException = null;

        public bool IsCompleted { get; private set; } = false;

        public void Start()
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (!IsCompleted)
            {
                try
                {
                    result = function();
                    IsCompleted = true;
                    resultAccess.Set();
                }
                catch (Exception ex)
                {
                    thrownException = ex;
                }
            }
        }

        public TResult Result
        {
            get
            {
                resultAccess.WaitOne();
                
                if (token.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                if (this.thrownException != null)
                {
                    throw new AggregateException("Task failed.", this.thrownException);
                }

                ArgumentNullException.ThrowIfNull(this.result);
                return result;
            }
        }

        IMyTask<TNewResult> IMyTask<TResult>.ContinueWith<TNewResult>(Func<TResult, TNewResult> nextTask)
        {
            var newTask = new MyTask<TNewResult>(() => nextTask(Result), threadPool, token);
            threadPool.ContinuousSubmit(newTask.Start);
            return newTask;
        }
    }
}