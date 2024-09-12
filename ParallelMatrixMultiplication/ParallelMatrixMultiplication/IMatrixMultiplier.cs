namespace ParallelMatrixMultiplication;

public interface IMatrixMultiplier
{
    int[][] Multiply(int[][] matrix1, int[][] matrix2);
}