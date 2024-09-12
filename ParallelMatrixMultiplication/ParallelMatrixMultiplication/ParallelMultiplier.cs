namespace ParallelMatrixMultiplication;
public class ParallelMultiplier : IMatrixMultiplier
{
    private int[][] result = [];
    private int[][] matrix1 = [];
    private int[][] matrix2 = [];
    private int columns = 0;
    private int rows = 0;

    private void MultiplyRowByColumn(int rowIndex)
    {
        for (int column = 0; column < columns; ++column)
        {
            result[rowIndex][column] = 0;
            for (int i = 0; i < matrix1[0].Length; ++i)
            {
                result[rowIndex][column] += matrix1[rowIndex][i] * matrix2[i][column];
            }
        }
    }

    public int[][] Multiply(int[][] InputMatrix1, int[][] InputMatrix2)
    {
        if (InputMatrix1.Length != InputMatrix2[0].Length)
        {
            throw new ArgumentException("Number of rows in matrix1 must be equal to number of columns in matrix2.");
        }

        matrix1 = InputMatrix1;
        matrix2 = InputMatrix2;

        rows = matrix1.Length;
        columns = matrix2[0].Length;

        result = new int[rows][];
        for (int i = 0; i < rows; ++i)
        {
            result[i] = new int[columns];
        }

        Thread[] threads = new Thread[rows];

        for (int i = 0; i < rows; ++i)
        {
            int rowIndex = i;

            threads[i] = new Thread(() => MultiplyRowByColumn(rowIndex));
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return result;
    }
}
