namespace MyThreadPool;

public class MyThreadPool
{
    private readonly Thread[] threads;
    private readonly Queue<IMyTask<object>> tasksQueue = new ();
    private readonly object queueLock = new object();
    private bool isShutdown = false;

    public MyThreadPool(int numberOfThreads)
    {
        threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; ++i)
        {
            var thread = new Thread(Work);
            threads[i] = thread;
            thread.Start();
        }
    }

    private void Work()
    {
        while (true)
        {
            MyTask<object> taskToStart = null;

            //taskAvailable.WaitOne();

            lock (queueLock)
            {
                if (tasksQueue.Count > 0)
                {
                    taskToStart = (MyTask<object>?)tasksQueue.Dequeue();
                }

                else if (isShutdown)
                {
                    break;
                }
            }

            taskToStart?.Start();
        }
    }
}