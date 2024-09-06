using ParallelMatrixMultiplication;

List<List<int>> m1 = [[1, 2], [3, 4]];
List<List<int>> m2 = [[3, 4], [1, 2]];

var m3 = SecquentialMatrixMultiplication.Multiply(m1, m2);
foreach (var innerList in m3)
{
    Console.WriteLine(string.Join(", ", innerList));
}