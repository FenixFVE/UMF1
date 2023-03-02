
using UMF1;
using UMF1.Managers;
using UMF1.SLAE;

//var contour = FileManager.ReadPoints(".\\Input\\Contour.txt");
//var grid = new Grid(contour);
//
//grid.CreateDictionarys();
//
//foreach (var t in grid.NumberToCoordinates)
//{
//    var index = t.Key;
//    var point = t.Value;
//    Console.WriteLine($"{index}: ({point.x},{point.y})");
//}

var matrix = new BlockDiagMatrix();
matrix.N = 6;
matrix.DiagMatrix = new double[6,6];
matrix.Indexes = new int[5] {-3, -1, 0, 1, 3};

matrix.DiagMatrix[0, 0] = 0;
matrix.DiagMatrix[0, 1] = 0;
matrix.DiagMatrix[0, 2] = 0;
matrix.DiagMatrix[0, 3] = 1;
matrix.DiagMatrix[0, 4] = 1;
matrix.DiagMatrix[0, 5] = 1;
                  
matrix.DiagMatrix[1, 0] = 0;
matrix.DiagMatrix[1, 1] = 1;
matrix.DiagMatrix[1, 2] = 1;
matrix.DiagMatrix[1, 3] = 1;
matrix.DiagMatrix[1, 4] = 1;
matrix.DiagMatrix[1, 4] = 1;

matrix.DiagMatrix[2,0] = 10;
matrix.DiagMatrix[2,1] = 10;
matrix.DiagMatrix[2,2] = 10;
matrix.DiagMatrix[2,3] = 10;
matrix.DiagMatrix[2,4] = 10;
matrix.DiagMatrix[2,5] = 10;

matrix.DiagMatrix[3,0] = 1;
matrix.DiagMatrix[3,1] = 1;
matrix.DiagMatrix[3,2] = 1;
matrix.DiagMatrix[3,3] = 1;
matrix.DiagMatrix[3,4] = 1;
matrix.DiagMatrix[3,5] = 0;

matrix.DiagMatrix[4, 0] = 1;
matrix.DiagMatrix[4, 1] = 1;
matrix.DiagMatrix[4, 2] = 1;
matrix.DiagMatrix[4, 3] = 0;
matrix.DiagMatrix[4, 4] = 0;
matrix.DiagMatrix[4, 4] = 0;



var F = new double[5] {16, 29, 36, 49, 56};
var x = Enumerable.Repeat(0.0, 5).ToArray();

matrix.BlockSize = 2;
BlockRelaxation.Solve(matrix, F, x, 1.0, 10.0e-13, 10000);

for (var i = 0; i < 6; i++)
{
    Console.WriteLine(i + 1 - x[i]);
}