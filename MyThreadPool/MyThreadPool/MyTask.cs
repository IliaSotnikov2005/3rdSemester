namespace MyThreadPool;

public class MyTask<TResult>(Func<TResult> function) : IMyTask<TResult>
{
    private readonly Func<TResult> function = function;
    private TResult result;
    private bool isCompleted = false;
    private readonly object lockObject = new object();

    public bool IsCompleted => isCompleted;
    
    public void Start()
    {
        lock (lockObject)
        {
            if (!isCompleted)
            {
                result = function();
                this.isCompleted = true;
            }
        }
    }

    public TResult Result
    {
        get
        {
            lock (lockObject)
            {
                if (isCompleted)
                {
                    return result;
                }

                throw new InvalidOperationException("Task is not completed yet");
            }
        }
    }

    public TNewResult ContinueWith<TNewResult>(Func<TResult, TNewResult> continuationFunction)
    {
        throw new NotImplementedException();
    }
}