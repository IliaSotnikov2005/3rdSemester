if (args.Length != 1)
{
    Console.WriteLine($"Invalid input. Expected 1 argument: path to the assembly.");
    return;
}

string path = args[0];

if (!Directory.Exists(path))
{
    Console.WriteLine($"File '{path}' not found.");
    return;
}