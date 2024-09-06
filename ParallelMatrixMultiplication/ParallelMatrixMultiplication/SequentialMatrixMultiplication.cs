namespace ParallelMatrixMultiplication;

public static class SecquentialMatrixMultiplication
{
    public static List<List<int>> Multiply(List<List<int>> matrix1, List<List<int>> matrix2)
    {
        if (matrix1.Count != matrix2[0].Count)
        {
            throw new ArgumentException("The number of rows of the first matrix must be equal to the number of columns of the other matrix.");
        }

        var result = new List<List<int>>();
        for (int i = 0; i < matrix1.Count; ++i)
        {
            result.Add(new List<int>(new int[matrix2[0].Count]));
        }

        for (int i = 0; i < matrix1.Count; ++i)
        {
            for (int j = 0; j < matrix2[0].Count; ++j)
            {
                result[i][j] = 0;
                for (int k = 0; k < matrix1[0].Count; ++k)
                {
                    result[i][j] += matrix1[i][k] * matrix2[k][j];
                }
            }
        }

        return result;
    }
}