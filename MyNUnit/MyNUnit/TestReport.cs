using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit;

internal class MyTestInfo
{
    public MyTestInfo(IEnumerable<MyTestResult> testResults)
    {
        this.TestResults.Union(testResults);

        foreach (var testResult in this.TestResults)
        {
            if (!string.IsNullOrEmpty(testResult.Message))
            {
                ++this.Ignored;
            }
            else if (testResult.Status.Equals("Passed"))
            {
                ++this.Passed;
            }
            else
            {
                ++this.Failed;
            }
        }
    }

    public HashSet<MyTestResult> TestResults { get; } = new();

    public int Passed { get; private set; }

    public int Failed { get; private set; }

    public int Ignored { get; private set; }

    public int TotalTests => this.Passed + this.Failed + this.Ignored;
}
