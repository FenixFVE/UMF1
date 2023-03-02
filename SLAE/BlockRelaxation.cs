using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMF1.SLAE;

public class BlockRelaxation
{
    public static void Solve(BlockDiagMatrix blockDiagMatrix, double[] F, double[] x, double relaxation, double eps, int max_iter)
    {
        Console.WriteLine("Block Relaxation");
        blockDiagMatrix.LUDecomposition();
        var residual = double.MaxValue;
        for (var i = 1; i <= max_iter && residual > eps; i++)
        {
            residual = NextIteration(blockDiagMatrix, x, F, relaxation);
            Log(i, residual);
        }
        Console.WriteLine();
    }

    public static void Log(int i, double residual)
    {
        Console.Write("Iteration: {0} Residual: {1}   \r", i, residual.ToString("0.00000000000000e+00", CultureInfo.CreateSpecificCulture("en-US")));
    }

    public static double NextIteration(BlockDiagMatrix blockDiagMatrix, double[] currentX, double[] F, double relaxation)
    {
        var n = blockDiagMatrix.N;
        var blockSize = blockDiagMatrix.BlockSize;
        var matrix = blockDiagMatrix.DiagMatrix;
        var indexes = blockDiagMatrix.Indexes;

        var residual = 0.0;
        var sumOfSqVecF = 0.0;
        var nBlocks = n / blockSize;
        for (var i = 0; i < nBlocks; i++)
        {
            var k0 = i * blockSize;
            var k1 = (i + 1) * blockSize;
            var r = CalcBlockPart(blockDiagMatrix, currentX, k0, k1);
            var bi = 0;
            for (var j = k0; j < k1; j++, bi++)
            {
                var sum = 0.0;
                for (var k = 0; k < 5; k++)
                {
                    if (indexes[k] + j < 0 || indexes[k] + j >= n) continue;
                    if (indexes[k] + j < k0 || indexes[k] + j >= k1)
                    {
                        sum += matrix[k, j] * currentX[indexes[k] + j];
                    }
                }
                r[bi] = F[j] - (r[bi] + sum);
                residual += r[bi] * r[bi];
                r[bi] *= relaxation;
                sumOfSqVecF += F[j] * F[j];
            }
            SLAESolver.SolveSLAE(blockDiagMatrix, r, relaxation, k0, k1);
            bi = 0;
            for (var j = k0; j < k1; j++, bi++)
            {
                currentX[j] += r[bi];
            }
        }
        return Math.Sqrt(residual) / Math.Sqrt(sumOfSqVecF);
    }

    public static double[] CalcBlockPart(BlockDiagMatrix blockDiagMatrix, double[] x, int k0, int k1)
    {
        var n = blockDiagMatrix.N;
        var blockSize = blockDiagMatrix.BlockSize;
        var matrix = blockDiagMatrix.DiagMatrix;
        var indexes = blockDiagMatrix.Indexes;
        var r = new double[blockSize];
        var k = 0;
        for (var i = k0; i < k1; i++, k++)
        {
            var sum = 0.0;
            for (var j = 2; j < 4; j++)
            {
                if (indexes[j] + i < 0 || indexes[j] + i >= n) continue;
                if (indexes[j] + i < k0 || indexes[j] + i >= k1) continue;
                if (j == 2)
                {
                    sum += 1.0 * x[indexes[j] + i];
                }
                else
                {
                    sum += matrix[j, i] * x[indexes[j] + i];
                }
            }
            r[k] = sum;
        }
        var buf = new double[blockSize];
        Array.Copy(r, buf, blockSize);
        k = 0;
        for (var i = k0; i < k1; i++, k++)
        {
            var sum = 0.0;
            for (var j = 1; j < 3; j++)
            {
                if (indexes[j] + i < 0 || indexes[j] + i >= n) continue;
                if (indexes[j] + i >= k0 && indexes[j] + i < k1)
                {
                    sum += matrix[j, i] * buf[indexes[j] + k];
                }
            }
            r[k] = sum;
        }

        return r;
    }
}

