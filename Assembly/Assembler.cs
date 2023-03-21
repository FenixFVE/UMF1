using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMF1.BlockRelaxation;

namespace UMF1.Assembly;

public class Assembler
{
    public Grid grid { get; set; }
    public DiagonalMatrix matrix { get; set; }


    public Assembler (Grid grid)
    {
        this.grid = grid;
        matrix = new DiagonalMatrix(grid.XPoints.Count * grid.YPoints.Count, grid.YPoints.Count);
    }

    public (int, int) ElementInMatrix(int a, int b)
    {
        if (a == b)
        {
            return (0, a);
        }

        if (a == b + 1)
        {

        }
    }

    public void ApplyingFirstCondition()
    {
        var firstBorders = grid.BorderPoints
            .Where(point => point.Condition == 1)
            .Select(point => point.Number)
            .ToList();

        foreach (var point in firstBorders)
        {

        }
    }

    public void ApplyingSecondCondition()
    {

    }

    public void ApplyingCross
}