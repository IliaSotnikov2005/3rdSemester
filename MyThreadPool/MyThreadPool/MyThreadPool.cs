namespace MyThreadPool;

public class MyThreadPool
{
    private readonly Thread[] threads;
    private readonly Queue<Action> tasksQueue = new();
    private readonly CancellationTokenSource cancellationSource = new();
    private readonly AutoResetEvent queueAccess = new(true);
    private AutoResetEvent gettingStarted = new(false);

    public MyThreadPool(int numberOfThreads)
    {
        if (numberOfThreads <= 0)
        {
            throw new ArgumentException("Number of threads must be positive", nameof(numberOfThreads));
        }

        threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; ++i)
        {
            threads[i] = new Thread(() => PerformTasks(cancellationSource.Token));
            threads[i].Start();
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (cancellationSource.IsCancellationRequested)
        {
            throw new OperationCanceledException("The threadpool was shutted down.", cancellationSource.Token);
        }

        var newTask = new MyTask<TResult>(function, this);

        queueAccess.WaitOne();
        tasksQueue.Enqueue(newTask.Start);
        queueAccess.Set();

        gettingStarted.Set();

        return newTask;
    }

    public void Shutdown()
    {
        if (cancellationSource.IsCancellationRequested)
        {
            return;
        }

        this.cancellationSource.Cancel();
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    private void PerformTasks(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            gettingStarted.WaitOne();
            queueAccess.WaitOne();
            if (tasksQueue.Count == 0)
            {
                queueAccess.Set();
                continue;
            }

            Action task = tasksQueue.Dequeue();
            queueAccess.Set();

            task();

            gettingStarted.Set();
        }

        queueAccess.Set();
    }

    private void AddContinuingTask(Action taskStartAction)
    {
        queueAccess.WaitOne();
        tasksQueue.Enqueue(taskStartAction);
        queueAccess.Set();

        gettingStarted.Set();
    }

    public class MyTask<TResult>(Func<TResult> function, MyThreadPool threadPool) : IMyTask<TResult>
    {
        private readonly MyThreadPool threadPool = threadPool;
        private readonly Func<TResult> function = function;
        private TResult? result;
        private Exception? thrownException = null;
        private Action? continuingTaskStartAction = null;
        private ManualResetEvent ResultReadiness = new (false);

        public bool IsCompleted { get; private set; } = false;

        public void Start()
        {
            if (IsCompleted)
            {
                return;
            }

            try
            {

                result = function();
                IsCompleted = true;
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }

            ResultReadiness.Set();

            if (continuingTaskStartAction != null)
            {
                threadPool.AddContinuingTask(continuingTaskStartAction!);
            }
        }

        public TResult Result
        {
            get
            {
                ResultReadiness.WaitOne();
                
                if (thrownException != null)
                {
                    throw new AggregateException("Task failed.", thrownException);
                }
                
                if (result == null)
                {
                    throw new ArgumentException("Function does not return a result.");
                }

                return result;
            }
        }

        IMyTask<TNewResult> IMyTask<TResult>.ContinueWith<TNewResult>(Func<TResult, TNewResult> nextTask)
        {
            if (IsCompleted)
            {
                return threadPool.Submit(() => nextTask(Result));
            }

            var continuingTask = new MyTask<TNewResult>(() => nextTask(Result), threadPool);
            continuingTaskStartAction = continuingTask.Start;

            return continuingTask;
        }
    }
}