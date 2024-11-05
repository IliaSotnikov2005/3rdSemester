namespace MyNUnit;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine($"Invalid input.");
            //return;
        }

        //string path = args[0];
        string path = "../ProjectForTesting/bin/Debug/net8.0";
        string path1 = Path.GetFullPath(path);
        if (!Directory.Exists(path1))
        {
            Console.WriteLine($"File '{path}' not found.");
            return;
        }

        await Tester.RunTestsAsync(path1);
    }
}