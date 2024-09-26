namespace MyThreadPool;

public interface IMyTask<TResult>
{
    public bool IsCompleted { get; }

    public TResult Result { get; }

    public TNewResult ContinueWith<TNewResult>(Func<TResult, TNewResult> continuationFunction);

}