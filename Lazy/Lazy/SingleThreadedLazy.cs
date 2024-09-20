namespace Lazy;

public class SingleThreadedLazy<T>(Func<T> supplier) : ILazy<T>
{
    private bool evaluated = false;
    private Func<T> supplier = supplier;
    private T? value;

    public T Get()
    {
        if (!evaluated)
        {
            this.value = supplier();
            this.evaluated = true;
        }

        return this.value ?? throw new InvalidOperationException("Value has not been evaluated yet.");
    }
}