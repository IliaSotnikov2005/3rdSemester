namespace MyNUnitWeb.Models;

public class TestClassResults
{
    public string AssemblyName { get; set; }

    public int Id { get; set; }

    public int Passed { get; set; }

    public int Failed { get; set; }

    public int Errored { get; set; }

    public int Ignored { get; set; }
}
