namespace MyNUnit;

public class MyTestClassResults
{
    public int Passed {get; private set;}
    public int Failed {get; private set;}
    public int Ignored {get; private set;}
    public int Errored {get; private set;}

    public List<MyTestResult> TestResults {get; private set;} = [];

    public MyTestClassResults(List<MyTestResult> results)
    {
        this.AddResults(results);
    }

    public void AddResults(List<MyTestResult> results)
    {
        foreach (var result in results)
        {
            switch (result.Status)
            {
                case TestStatus.Passed:
                ++Passed;
                break;
                case TestStatus.Failed:
                ++Failed;
                break;
                case TestStatus.Ignored:
                ++Ignored;
                break;
                case TestStatus.Errored:
                ++Errored;
                break;
            }

            TestResults.Add(result);
        }
    }
}