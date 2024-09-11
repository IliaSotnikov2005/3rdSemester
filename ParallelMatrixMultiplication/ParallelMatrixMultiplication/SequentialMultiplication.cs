namespace ParallelMatrixMultiplication;

public static class SequentialMultiplication
{
    public static int[][] Multiply(int[][] matrix1, int[][] matrix2)
    {
        if (matrix1.Length != matrix2[0].Length)
        {
            throw new ArgumentException("The number of rows of the first matrix must be equal to the number of columns of the other matrix.");
        }

        var result = new int[matrix1.Length][];
        for (int i = 0; i < matrix1.Length; ++i)
        {
            result[i] = new int[matrix2[0].Length];
        }

        for (int i = 0; i < matrix1.Length; ++i)
        {
            for (int j = 0; j < matrix2[0].Length; ++j)
            {
                result[i][j] = 0;
                for (int k = 0; k < matrix1[0].Length; ++k)
                {
                    result[i][j] += matrix1[i][k] * matrix2[k][j];
                }
            }
        }

        return result;
    }
}