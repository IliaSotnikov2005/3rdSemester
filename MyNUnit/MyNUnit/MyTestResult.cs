using System.Dynamic;

namespace MyNUnit;

public record MyTestResult(string Name, TestStatus Status, string Message, TimeSpan TimeElapsed = default)
{
    public TestStatus Status {get; internal set;} = Status;

    public string GetFormattedTimeElapsed()
    {
        return $"{(int)TimeElapsed.TotalSeconds}.{TimeElapsed.Milliseconds}";
    }
}