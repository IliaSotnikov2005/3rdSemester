using ParallelMatrixMultiplication;

static void RunBenchmark()
{
    for (int size = 100; size < 1001; size += 100)
    {
        MatrixMultiplicationBenchmark.Benchmark(size, 3);
    }

    MatrixMultiplicationBenchmark.MakeGraph();
}



void Main(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("There are no arguments.\nUse -help for help\n-m <filename> to multiply the matrices\n-b to run the benchmark.");
        return;
    }

    switch (args[0])
    {
        case "-help":
            Console.WriteLine("Use -m <filename> to multiply the matrices\n-b to run the benchmark.");
            break;
        case "-m":
            if (args.Length < 3)
            {
                Console.WriteLine("Enter filename for multiplication.");
                return;
            }
            string filename1 = args[1];
            string filename2 = args[2];
            break;

        case "-b":
            RunBenchmark();
            break;

        default:
            Console.WriteLine("Unknown command.\nUse - m < filename > to multiply the matrices\n-b to run the benchmark.");
            break;
    }

}
