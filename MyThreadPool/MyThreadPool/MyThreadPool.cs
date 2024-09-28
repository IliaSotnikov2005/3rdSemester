namespace MyThreadPool;

public class MyThreadPool
{
    private readonly Thread[] threads;
    private readonly Queue<Action> tasksQueue = new();
    private readonly CancellationTokenSource cancellation = new();

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
        MyTask<TResult> task = new(function, cancellation.Token);
        this.Enqueue(() => task.Start());
        // Сделать чтобы таски передавались в очередь
        return task;
    }

    public void Shutdown()
    {
        cancellation.Cancel();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        cancellation.Dispose();
    }

    public void Enqueue(Action task)
    {
        lock (tasksQueue)
        {
            tasksQueue.Enqueue(task);
        }

        Monitor.Pulse(tasksQueue);
    }

    private void PerformTasks()
    {
        while (!cancellation.IsCancellationRequested || tasksQueue.Count > 0)
        {
            Action task;
            lock (tasksQueue)
            {
                while (tasksQueue.Count == 0)
                {
                    if (this.cancellation.IsCancellationRequested)
                    {
                        return;
                    }

                    Monitor.Wait(tasksQueue);
                }

                task = tasksQueue.Dequeue();
            }

            task();
        }
    }
}