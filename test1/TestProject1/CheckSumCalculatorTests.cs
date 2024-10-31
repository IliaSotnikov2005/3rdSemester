namespace TestProject1;

using NUnit.Framework;
using CheckSumCalculator;

public class Tests
{
    [Test]
    public async Task ResultsAreEqual()
    {
        byte[] expected = CheckSumSingleThreadCalculator.Run("subfolder");

        byte[] actual = await CheckSumMultiThreadCalculator.RunAsync("subfolder");

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void InvalidPath()
    {
        Assert.ThrowsAsync<ArgumentException>(async () => await CheckSumMultiThreadCalculator.RunAsync("invalidPath"));
        Assert.Throws<ArgumentException>(() => CheckSumSingleThreadCalculator.Run("invalidPath"));
    }

    [Test]
    public async Task FileCheckSum()
    {
        byte[] expected = CheckSumSingleThreadCalculator.Run("file1.txt");

        byte[] actual = await CheckSumMultiThreadCalculator.RunAsync("file1.txt");

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task EmptyDirectoryCheckSum()
    {
        Directory.CreateDirectory("emptyFolder");
        byte[] expected = CheckSumSingleThreadCalculator.Run("emptyFolder");

        byte[] actual = await CheckSumMultiThreadCalculator.RunAsync("emptyFolder");

        Assert.That(actual, Is.EqualTo(expected));
    }
}