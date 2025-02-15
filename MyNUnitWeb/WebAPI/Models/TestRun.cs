using MyNUnit;

namespace WebAPI.Models;

public class TestRun
{
    public int Id { get; set; }
    public DateTime LaunchTime { get; set; }
    public TestRunResult Result { get; set; }
}
