using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UMF1.BlockRelaxation;

namespace UMF1.Assembly;

public class Assembler
{
    public Grid grid { get; set; }
    public DiagonalMatrix matrix { get; set; }
    public List<double> F { get; set; }

    public Assembler (Grid grid)
    {
        this.grid = grid;
        var n = grid.XPoints.Count * grid.YPoints.Count;
        matrix = new DiagonalMatrix(n, grid.YPoints.Count);
        F = Enumerable.Repeat(0.0, n).ToList();
    }

    public void ApplyingFirstCondition()
    {
        var firstBorders = grid.BorderPoints
            .Where(point => point.Condition == 1)
            .Select(point => point.Number)
            .ToList();

        foreach (var point in firstBorders)
        {
            Console.WriteLine($"border: {point}");
            var coord = grid.NumberToCoordinates[point];
            F[point] = Equation.U(coord.x, coord.y);
            matrix.Diagonal[2][point] = 1.0;
        }

        foreach (var point in grid.OuterPoints)
        {
            Console.WriteLine($"outer: {point}");
            var coord = grid.NumberToCoordinates[point];
            F[point] = Equation.U(coord.x, coord.y);
            matrix.Diagonal[2][point] = 1.0;
        }
    }

    public void ApplyingSecondCondition()
    {
        var secondBorders = grid.BorderPoints
            .Where(point => point.Condition == 2)
            .ToList();

        foreach (var point in secondBorders)
        {
            var index = point.Number;
            var coord = grid.NumberToCoordinates[index];
            var lambda = Equation.Lambda(coord.x, coord.y);
            var theta = Equation.Theta(coord.x, coord.y);
            //F[index] += theta;

            switch (point.NormalDirection)
            {
                case Direction.Up:
                {
                    var hy = grid.Hy(index, grid.DownPoint(index));
                    matrix.Diagonal[2][index] += lambda / hy;
                    matrix.Diagonal[0][index] += -lambda / hy;
                    F[index] += Equation.Udy(coord.x, coord.y);
                }
                    break;
                case Direction.Down:
                {
                    var hy = grid.Hy(index, grid.UpPoint(index));
                    matrix.Diagonal[2][index] += -lambda / hy;
                    matrix.Diagonal[4][index] += lambda / hy;
                    F[index] += -Equation.Udy(coord.x, coord.y);
                }
                    break;
                case Direction.Left:
                {
                    var hx = grid.Hx(index, grid.RightPoint(index));
                    matrix.Diagonal[2][index] += -lambda / hx;
                    matrix.Diagonal[3][index] += lambda / hx;
                    F[index] += -Equation.Udx(coord.x, coord.y);
                }
                    break;
                case Direction.Right:
                {
                    var hx = grid.Hx(index, grid.LeftPoint(index));
                    matrix.Diagonal[2][index] += lambda / hx;
                    matrix.Diagonal[1][index] += -lambda / hx;
                    F[index] += Equation.Udx(coord.x, coord.y);
                }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public void ApplyingCross()
    {
        foreach (var point in grid.InnerPoints)
        {
            Console.WriteLine($"Inner: {point}");
            var hx = grid.Hx(point, grid.RightPoint(point));
            var hx_1 = grid.Hx(point, grid.LeftPoint(point));
            var hy = grid.Hy(point, grid.UpPoint(point));
            var hy_1 = grid.Hy(point, grid.DownPoint(point));

            var coord = grid.NumberToCoordinates[point];
            var lambda = Equation.Lambda(coord.x, coord.y);
            var gamma = Equation.Gamma(coord.x, coord.y);

            // left
            matrix.Diagonal[1][point] += -lambda * 2.0 / (hx_1 * (hx + hx_1));
            // down
            matrix.Diagonal[0][point] += -lambda * 2.0 / (hy_1 * (hy + hy_1));
            // right
            matrix.Diagonal[3][point] += -lambda * 2.0 / (hx * (hx + hx_1));
            // up
            matrix.Diagonal[4][point] += -lambda * 2.0 / (hy * (hy + hy_1));
            // center
            matrix.Diagonal[2][point] += lambda * (2.0 / (hx_1*hx) + 2.0 / (hy_1*hy)) + gamma;


            F[point] += Equation.F(coord.x, coord.y);
        }
    }

    public static int SmallestFactor(int n)
    {
        for (int i = 2; i <= Math.Sqrt(n); i++)
        {
            if (n % i == 0)
            {
                return i;
            }
        }
        return n;
    }


    public List<double> Calculate(double relaxation, int blockSize)
    {
        var size = matrix.N;
        var eps = 10.0e-13;
        var maxSteps = 10000;
        var temporal = Enumerable.Repeat(0.0, size).ToList();
        var x = Enumerable.Repeat(0.0, size).ToList();

        ApplyingFirstCondition();
        ApplyingSecondCondition();
        ApplyingCross();

        //Console.WriteLine("data:");
        //for (var i = 0; i < 5; i++)
        //{
        //    Console.WriteLine($"{i}:");
        //    for (var j = 0; j < size; j++)
        //    {
        //        Console.WriteLine($"   {matrix.Diagonal[i][j]}");
        //    }
        //}
        //Console.WriteLine("F");
        //foreach (var t in F)
        //{
        //    Console.WriteLine(t);
        //}

        BlockRelaxation.BlockRelaxation.Block(matrix, F, x, relaxation, eps, maxSteps, blockSize, temporal);

        return x;
    }


    public List<double> UReal()
    {
        var u = Enumerable.Repeat(0.0, matrix.N).ToList();

        var counter = 0;
        foreach (var y in grid.YPoints)
        {
            foreach (var x in grid.XPoints)
            {
                u[counter++] = Equation.U(x, y);
            }
        }

        return u;
    }


    public static double Norm(List<double> vec)
    {
        return Math.Sqrt(vec.Select(p => p*p).Sum());
    }
}