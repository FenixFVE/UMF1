using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMF1.SLAE;

public class SLAESolver
{
    public static void SolveSLAE(BlockDiagMatrix blockDiagMatrix, double[] y, double relaxation, int k0, int k1)
    {
        CalcZ(blockDiagMatrix, y, k0, k1);
        CalcY(blockDiagMatrix, y, k0, k1);
    }
    private static void CalcZ(BlockDiagMatrix blockDiagMatrix, double[] r, int k0, int k1)
    {
        var matrix = blockDiagMatrix.DiagMatrix;

        var j = 0;
        var z = r;
        z[j] = r[j] / matrix[3, k0];
        j++;
        for (var i = k0 + 1; i < k1; i++, j++)
        {
            z[j] = (r[j] - matrix[2, i] * z[j - 1]) / matrix[3, i];
        }
    }

    private static void CalcY(BlockDiagMatrix blockDiagMatrix, double[] z, int k0, int k1)
    {
        var matrix = blockDiagMatrix.DiagMatrix;
        var blockSize = blockDiagMatrix.BlockSize;

        var j = blockSize;
        var y = z;
        y[j - 1] = z[j - 1];
        j -= 2;
        for (var i = k1 - 2; i >= k0; i--, j--)
        {
            y[j] -= matrix[4, i] * y[j + 1];
        }
    }
}

