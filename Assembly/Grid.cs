using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UMF1.Utility;

namespace UMF1.Assembly;

public struct Point
{
    public double x;
    public double y;

    public Point(double x, double y)
    {
        this.x = x; this.y = y;
    }

    public Point(Point p): this(p.x, p.y) { }
}

public class Grid
{
    public List<double> XPoints { get; set; }
    public List<double> YPoints { get; set; }
    public SortedSet<int> InnerPoints { get; set; }

    public Dictionary<Point, int> CoordinatesToNumber { get; set; }
    public Dictionary<int, Point> NumberToCoordinates { get; set; }
    private void InitializeDictionaries()
    {
        CoordinatesToNumber = new Dictionary<Point, int>(XPoints.Count * YPoints.Count);
        NumberToCoordinates = new Dictionary<int, Point>(XPoints.Count * YPoints.Count);
        int counter = 0;
        foreach (var y in YPoints)
        {
            foreach (var x in XPoints)
            {
                var point = new Point(x, y);
                NumberToCoordinates.Add(counter, point);
                CoordinatesToNumber.Add(point, counter++);
            }
        }
    }

    // !!! NEED TO TEST
    private void ReadCoordinates(StreamReader reader, out List<double> mainPoints, out List<double> allPoints)
    {
        mainPoints = FileManager.ReadLineToList<double>(reader);
        var thickeningNumber = FileManager.ReadLineToList<int>(reader);
        var thickeningK = FileManager.ReadLineToList<double>(reader);

        allPoints = new List<double>(mainPoints.Count);
        for (var i = 0; i < mainPoints.Count - 1; i++)
        {
            allPoints.Add(mainPoints[i]);
            var n = thickeningNumber[i];
            var k = thickeningK[i];
            var h = (mainPoints[i + 1] - mainPoints[i]);
            h *= k - 1.0;
            h /= Math.Pow(k, n) - 1.0;
            var temporal = mainPoints[i];
            var kProduct = 1.0;
            for (var step = 0; step < n - 1; step++)
            {
                temporal += h * kProduct;
                kProduct *= k;
                allPoints.Add(temporal);
            }
        }
        allPoints.Add(mainPoints[^1]);
    }
    public Grid(string GridFile)
    {
        using var reader = new StreamReader(GridFile);
        
        ReadCoordinates(reader, out var xMainPoints, out var tempXPoints);
        XPoints = tempXPoints;

        ReadCoordinates(reader, out var yMainPoints, out var tempYPoints);
        YPoints = tempYPoints;

        InitializeDictionaries();

        // ...
    }
}
