using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace UMF1.BlockRelaxation;

public class BlockRelaxation
{

    public static void Block(DiagonalMatrix blockDiagonalMatrix, List<double> F, List<double> x, 
        double relaxation, double eps, int maxSteps, int blockSize, List<double> r)
    {
        Console.WriteLine("Block Relaxation start");
        blockDiagonalMatrix.LU_Decomposition(blockSize);
        var residual = double.MaxValue;
        for (var i = 1; i <= maxSteps && residual > eps; i++)
        {
            residual = BlockStep(blockDiagonalMatrix, x, F, relaxation, blockSize, r);
            //Console.WriteLine("X:");
            //foreach (var xi in x)
            //{
            //    Console.WriteLine(xi);
            //}
            Console.WriteLine($"Iteration number: {i} Residual: {residual.ToString("0.00000000000000e+00", CultureInfo.CreateSpecificCulture("en-US"))}");
        }
        var con = Cond(x, residual);
        Console.WriteLine(
            $"\nCond: {con.ToString("0.00000000000000e+00", CultureInfo.CreateSpecificCulture("en-US"))}");
        Console.WriteLine("Block Relaxation method end");
    }

    public static double BlockStep(DiagonalMatrix blockDiagonalMatrix, List<double> currentX, List<double> F,
        double relaxation, int blockSize, List<double> r)
    {
        var n = blockDiagonalMatrix.N;
        var matrix = blockDiagonalMatrix.Diagonal;
        var indexes = blockDiagonalMatrix.Indexes;

        double residual = 0.0;
        double sum_of_sq_vec_F = 0.0;
        var n_blocks = n / blockSize;
        for (int i = 0; i < n_blocks; i++)
        {
            var k0 = i * blockSize;
            var k1 = (i + 1) * blockSize;
            CalculateBlockPart(blockDiagonalMatrix, currentX, r, k0, k1);
            var bi = 0;
            for (int j = k0; j < k1; j++, bi++)
            {
                double sum = 0.0;
                for (int k = 0; k < 5; k++)
                {
                    if (indexes[k] + j < 0 || indexes[k] + j >= n) continue;

                    if (indexes[k] + j < k0 || indexes[k] + j >= k1)
                    {
                        sum += matrix[k][j] * currentX[indexes[k] + j];
                    }
                    
                }
                r[bi] = F[j] - (r[bi] + sum);
                residual += r[bi] * r[bi];
                r[bi] *= relaxation;
                sum_of_sq_vec_F += F[j] * F[j];
            }
            Solve_SLAE(blockDiagonalMatrix, r, relaxation, k0, k1, blockSize);
            bi = 0;
            for (int j = k0; j < k1; j++, bi++)
            {
                currentX[j] += r[bi];
            }
        }
        return Math.Sqrt(residual / sum_of_sq_vec_F);
    }

    public static void CalculateBlockPart(DiagonalMatrix blockDiagonalMatrix, 
        List<double> x, List<double> r, int k0, int k1)
    {
        var n = blockDiagonalMatrix.N;
        var matrix = blockDiagonalMatrix.Diagonal;
        var indexes = blockDiagonalMatrix.Indexes;

        int k = 0;
        for (int i = k0; i < k1; i++, k++)
        {
            double sum = 0.0;
            for (int j = 2; j < 4; j++)
            {
                if (indexes[j] + i < 0 || indexes[j] + i >= n) continue;

                if (indexes[j] + i < k0 || indexes[j] + i >= k1) continue;
                
                if (j == 2)
                {
                    sum += x[indexes[j] + i];
                }
                else
                {
                    sum += matrix[j][i] * x[indexes[j] + i];
                }

            }
            r[k] = sum;
        }

        var buf = new List<double>(r);
        k = 0;
        for (int i = k0; i < k1; i++, k++)
        {
            double sum = 0.0;
            for (int j = 1; j < 3; j++)
            {
                if (indexes[j] + i < 0 || indexes[j] + i >= n) continue;
                if (indexes[j] + i < k0 || indexes[j] + i >= k1) continue;

                sum += matrix[j][i] * buf[indexes[j] + k];
            }
            r[k] = sum;
        }

    }

    public static void Solve_SLAE(DiagonalMatrix blockDiagonalMatrix, 
        List<double> y, double relaxation, int k0, int k1, int blockSize) 
    {
        var matrix = blockDiagonalMatrix.Diagonal;

        int j = 0;
        y[j] = y[j] / matrix[2][k0]; j++;
        for (int i = k0 + 1; i < k1; i++, j++)
        {
            y[j] = (y[j] - matrix[1][i] * y[j - 1]) / matrix[2][i];
        }

        j = blockSize - 2;
        for (int i = k1 - 2; i >= k0; i--, j--)
        {
            y[j] -= matrix[3][i] * y[j + 1];
        }
    }

    public static double Cond(List<double> x, double residual)
    {
        int n = x.Count;
        var xStarNorm = 0.0;
        var xMinusXStarNorm = 0.0;
        for (var i = 0; i < n; i++)
        {
            var i1 = (double)(i + 1);
            xStarNorm += i1 * i1;
            xMinusXStarNorm += (x[i] - i1) * (x[i] - i1);
        }
        xStarNorm = Math.Sqrt(xStarNorm);
        xMinusXStarNorm = Math.Sqrt(xMinusXStarNorm);
        var error = xMinusXStarNorm / xStarNorm;
        return error / residual;
    }
}

