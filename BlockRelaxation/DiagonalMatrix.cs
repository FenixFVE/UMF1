using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMF1.Utility;

namespace UMF1.BlockRelaxation;

public class DiagonalMatrix
{
    public const int DiagonalNumber = 7;
    public int N { get; set; }
    public int M { get; set; }
    public List<int> Indexes { get; set; }
    public List<List<double>> Diagonal { get; set; }

    public DiagonalMatrix(string matrixFile)
    {
        try
        {
            var file = new StreamReader(matrixFile);
            var data = FileManager.ReadLineToList<int>(file);
            N = data[0]; 
            M = data[1];
            
            Indexes = FileManager.ReadLineToList<int>(file);
            
            Diagonal = new List<List<double>>(DiagonalNumber);
            for (var i = 0; i < DiagonalNumber; i++)
            {
                Diagonal.Add(FileManager.ReadLineToList<double>(file));
            }
        }
        catch
        {
            throw new Exception($"Fail to read matrix: {matrixFile}");
        }
    }

    public DiagonalMatrix(int n, int m)
    {
        N = n;
        M = m;

        Indexes = new List<int>() { -M, -1, 0, 1, M };

        Diagonal = Enumerable.Range(0, 5)
            .Select(i => Enumerable.Repeat(0.0, N).ToList())
            .ToList();

    }

    public void LU_Decomposition(int blockSize)
    {
        var n = N / blockSize;
        for (var i = 0; i < n; i++)
        {
            var k0 = i * blockSize;
            var k1 = (i + 1) * blockSize;
            for (var j = k0 + 1; j < k1; j++)
            {
                Diagonal[3][j - 1] /= Diagonal[2][j - 1];
                Diagonal[2][j] -= Diagonal[3][j - 1] * Diagonal[1][j];
            }
        }
    }

}

