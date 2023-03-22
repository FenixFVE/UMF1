using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMF1.Assembly;
using UMF1.BlockRelaxation;
using UMF1.Utility;

namespace UMF1;


public class MainProgram
{

    public static void Main()
    {
        //Testing.Test1();
        var grid = new Grid(".//Input//Tests//test_format.txt", ".//Input//Tests//test_conditions.txt");
        var writer = new StreamWriter(".//Output//output.csv");

        grid.DrawGrid();

        var assembler = new Assembler(grid);

        var u = assembler.Calculate(1.0, 1);

        var uStar = assembler.UReal();

        var dif = new List<double>();
        for (var i = 0; i < u.Count; i++)
        {
            dif.Add(Math.Abs(u[i] - uStar[i]));
            var coord = assembler.grid.NumberToCoordinates[i];
            writer.WriteLine($"({coord.x:F2},{coord.y:F2})\t{uStar[i]}\t{u[i]}\t{dif[i]}");
        }

        var N = Assembler.Norm(dif) / Assembler.Norm(uStar);

        writer.WriteLine($"|U*-U|/|U*| = {N}");
        writer.WriteLine($"Node number: {grid.YPoints.Count * grid.XPoints.Count}");
        Console.WriteLine($"|U*-U|/|U*| = {N}");
        Console.WriteLine($"Node number: {grid.YPoints.Count * grid.XPoints.Count}");

        writer.Close();
    }
}



