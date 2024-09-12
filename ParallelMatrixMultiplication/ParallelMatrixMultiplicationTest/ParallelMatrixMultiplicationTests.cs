using ParallelMatrixMultiplication;

namespace ParallelMatrixMultiplicationTest;

public class ParallelMatrixMultiplicationTests
{
    [TestCaseSource(nameof(ParallelMultiplicationTestData))]
    public void MatrixMultiplication_ReturnsCorrect(IMatrixMultiplier matrixMultiplier)
    {
        int[][] matrix1 = [[1, 2], [3, 4]];

        int[][] matrix2 = [[5, 6], [7, 8]];

        int[][] expected = [[19, 22], [43, 50]];

        var result = matrixMultiplier.Multiply(matrix1, matrix2);

        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(ParallelMultiplicationTestData))]
    public void Multiplication_MismatchRowsColumns_ThrowsException(IMatrixMultiplier matrixMultiplier)
    {
        int[][] matrix1 = [[1, 2], [3, 4], [1, 1]];

        int[][] matrix2 = [[5, 6], [7, 8]];

        Assert.Throws<ArgumentException>(() => matrixMultiplier.Multiply(matrix1, matrix2));
    }

    private static IEnumerable<TestCaseData> ParallelMultiplicationTestData()
    {
        yield return new TestCaseData(new SequentialMultiplier());
        yield return new TestCaseData(new ParallelMultiplier());
    }
}