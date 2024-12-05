namespace Tests;

using NUnit.Framework;
using ReflectorSpace;

[TestFixture]
public class ReflectorTests
{
    private Reflector reflector = new ();

    [Test]
    public void PrintStructure_ValidClass_CreatesFileWithCorrectContent()
    {
        Type testClassType = typeof(TestClass);
        string expectedFileName = $"{testClassType.Name}.cs";

        reflector.PrintStructure(testClassType);

        Assert.That(File.Exists(expectedFileName), Is.True);

        string[] expectedLines =
        [
            "class TestClass : ITestInterface",
            "{",
            "    public Int32 Field1;",
            "    private String Field2;",
            "    public Void Method1() { }",
            "    private Void Method2(Int32 param) { }",
            "}"
        ];

        string[] actualLines = File.ReadAllLines(expectedFileName);
        Assert.That(expectedLines, Has.Length.EqualTo(actualLines.Length));

        for (int i = 0; i < expectedLines.Length; i++)
        {
            Assert.That(expectedLines[i], Is.EqualTo(actualLines[i]));
        }

        File.Delete(expectedFileName);
    }

    [Test]
    public void PrintStructure_NonClassType_ThrowsArgumentException()
    {
        Type nonClassType = typeof(IDisposable);

        var ex = Assert.Throws<ArgumentException>(() => reflector.PrintStructure(nonClassType));
        Assert.That(ex.Message, Is.EqualTo("Is not a class."));
    }

    [Test]
    public void PrintStructure_EmptyClass_CreatesFileWithEmptyBody()
    {
        Type emptyClassType = typeof(EmptyClass);
        string expectedFileName = $"{emptyClassType.Name}.cs";

        reflector.PrintStructure(emptyClassType);

        Assert.IsTrue(File.Exists(expectedFileName), "File was not created.");

        string[] expectedLines = new[]
        {
            "class EmptyClass",
            "{",
            "}"
        };

        string[] actualLines = File.ReadAllLines(expectedFileName);
        Assert.That(expectedLines, Has.Length.EqualTo(actualLines.Length));

        for (int i = 0; i < expectedLines.Length; i++)
        {
            Assert.That(expectedLines[i], Is.EqualTo(actualLines[i]));
        }

        File.Delete(expectedFileName);
    }

    public class TestClass : ITestInterface
    {
        public int Field1;
        private string Field2;

        public void Method1() { }
        private void Method2(int param) { }
    }

    public interface ITestInterface { }

    public class EmptyClass { }
}