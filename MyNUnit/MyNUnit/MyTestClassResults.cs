namespace MyNUnit;

public class MyTestClassResults
{
    public int Passed {get; private set;}
    public int Failed {get; private set;}
    public int Ignored {get; private set;}

    public List<MyTestResult> TestResults {get; private set;} = [];

    public void AddResults(List<MyTestResult> results)
    {
        foreach (var result in results)
        {
            switch (result.Status)
            {
                case Status.Passed:
                ++Passed;
                break;
                case Status.Failed:
                ++Failed;
                break;
                case Status.Ignored:
                ++Ignored;
                break;
            }

            TestResults.Add(result);
        }
    }
}