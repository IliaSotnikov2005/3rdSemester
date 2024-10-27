namespace MyNUnit;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine($"Invalid input.");
            return;
        }

        string path = args[0];
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"File '{path}' not found.");
            return;
        }


    }
}