namespace Lazy;

public class MultiThreadedLazy<T>(Func<T> supplier) : ILazy<T>
{
    private readonly object lockObject = new();
    private bool evaluated = false;
    private Func<T> supplier = supplier;
    private T? value;

    public T Get()
    {
        if (this.evaluated)
        {
            return this.value ?? throw new InvalidOperationException("Value has not been evaluated yet.");
        }

        lock (lockObject)
        {
            if (!evaluated)
            {
                this.value = supplier();
                this.evaluated = true;
            }
        }

        return this.value ?? throw new InvalidOperationException("Value has not been evaluated yet.");
    }
}