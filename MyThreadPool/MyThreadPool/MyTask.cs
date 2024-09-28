namespace MyThreadPool;

public class MyTask<TResult>(Func<TResult> function, CancellationToken token) : IMyTask<TResult>
{
    private readonly Func<TResult> function = function;
    private TResult? result;
    private readonly object lockObject = new();

    private readonly CancellationToken token = token;
    private Exception? thrownException = null;

    public bool IsCompleted { get; private set; } = false;

    public void Start()
    {
        lock (lockObject)
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
                }
                catch (Exception ex)
                {
                    thrownException = ex;
                }
            }
        }

        Monitor.PulseAll(lockObject);
    }

    public TResult Result
    {
        get
        {
            lock (lockObject)
            {
                while (!IsCompleted && thrownException == null)
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    Monitor.Wait(lockObject);
                }

                if (this.thrownException != null)
                {
                    throw new AggregateException("Task failed.", this.thrownException);
                }

                ArgumentNullException.ThrowIfNull(this.result);
                return result;
            }
        }
    }

    IMyTask<TNewResult> IMyTask<TResult>.ContinueWith<TNewResult>(Func<TResult, TNewResult> nextTask)
    {
        Task task = new Task(() => nextTask(Result), token);

        return new MyTask<TNewResult>(() => nextTask(Result), token);
    }
}