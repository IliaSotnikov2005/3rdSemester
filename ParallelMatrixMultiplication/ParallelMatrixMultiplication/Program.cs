using ParallelMatrixMultiplication;


public class Program
{
    static void RunBenchmark()
    {
        for (int size = 100; size < 1001; size += 100)
        {
            MatrixMultiplicationBenchmark.Benchmark(size, 3);
        }

        MatrixMultiplicationBenchmark.MakeGraph();
    }

    static int[][] ExtractMatrix(string filename)
    {
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException(filename);
        }

        var lines = File.ReadAllLines(filename);
        int[][] result = new int[lines.Length][];

        for (int i = 0; i < lines.Length; i++)
        {
            result[i] = Array.ConvertAll(lines[i].Split(' '), int.Parse);
        }

        return result;
    }

    static void EnterMatrix(int[][] matrix)
    {
        StreamWriter sw = new("result.txt");
        for (int i = 0; i < matrix.Length; ++i)
        {
            sw.WriteLine(String.Join(" ", matrix[i]));
        }

        sw.Close();
    }

    static void Main(string[] args)
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

                try
                {
                    int[][] matrix1 = ExtractMatrix(filename1);
                    int[][] matrix2 = ExtractMatrix(filename2);

                    int[][] result = ParallelMultiplication.Multiply(matrix1, matrix2);

                    EnterMatrix(result);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found.");
                    return;
                }

                Console.WriteLine("OK");
                break;

            case "-b":
                RunBenchmark();
                break;

            default:
                Console.WriteLine("Unknown command.\nUse - m < filename > to multiply the matrices\n-b to run the benchmark.");
                break;
        }
    }
}