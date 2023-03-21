using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UMF1.Utility;

namespace UMF1.Assembly;

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}

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

public struct BorderPoint
{
    public int Number;
    public Direction NormalDirection;
    public int Condition;

    public BorderPoint(int number, Direction normalDirection, int condition)
    {
        Number = number; NormalDirection = normalDirection; Condition = condition;
    }
}

public class Grid
{
    public List<double> XPoints { get; set; }
    public List<double> YPoints { get; set; }

    public Dictionary<Point, int> CoordinatesToNumber { get; set; }
    public Dictionary<int, Point> NumberToCoordinates { get; set; }

    public List<int> InnerPoints { get; set; }
    public List<BorderPoint> BorderPoints { get; set; }

    public int UpPoint(int index)
    {
        var point = NumberToCoordinates[index];
        var yIndex = YPoints.IndexOf(point.y);
        if (yIndex == YPoints.Count - 1)
            return -1;
        var newPoint = new Point(point.x, YPoints[yIndex + 1]);
        return CoordinatesToNumber[newPoint];
    }

    public int DownPoint(int index)
    {
        var point = NumberToCoordinates[index];
        var yIndex = YPoints.IndexOf(point.y);
        if (yIndex == 0)
            return -1;
        var newPoint = new Point(point.x, YPoints[yIndex - 1]);
        return CoordinatesToNumber[newPoint];
    }

    public int RightPoint(int index)
    {
        var point = NumberToCoordinates[index];
        var xIndex = XPoints.IndexOf(point.x);
        if (xIndex == XPoints.Count - 1)
            return -1;
        var newPoint = new Point(XPoints[xIndex + 1], point.y);
        return CoordinatesToNumber[newPoint];
    }

    public int LeftPoint(int index)
    {
        var point = NumberToCoordinates[index];
        var xIndex = XPoints.IndexOf(point.x);
        if (xIndex == 0)
            return -1;
        var newPoint = new Point(XPoints[xIndex - 1], point.y);
        return CoordinatesToNumber[newPoint];
    }

    public double Hx(int first, int second)
    {
        var a = NumberToCoordinates[first];
        var b = NumberToCoordinates[second];
        return Math.Abs(a.x - b.x);
    }

    public double Hy(int first, int second)
    {
        var a = NumberToCoordinates[first];
        var b = NumberToCoordinates[second];
        return Math.Abs(a.y - b.y);
    }

    public void InitializeDictionaries()
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

    public static void ReadCoordinates(StreamReader reader, out List<double> mainPoints, out List<double> allPoints)
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
            
            if (k == 1.0)
            {
                h /= n;
            }
            else
            {
                h *= k - 1.0;
                h /= Math.Pow(k, n) - 1.0;
            }

            //Console.WriteLine($"-- {n} {k} {h} {Math.Pow(k,n)}");
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


    public static Direction ParseDirection(string directionString)
    {
        return directionString.ToLower() switch
        {
            "up" => Direction.Up,
            "down" => Direction.Down,
            "left" => Direction.Left,
            "right" => Direction.Right,
            _ => throw new ArgumentException("Invalid direction string.")
        };
    }

    public List<int> ReadBoundaries(string boundariesFile)
    {
        if (!File.Exists(boundariesFile))
        {
            throw new FileNotFoundException(boundariesFile);
        }

        var lines = new List<string>(File.ReadAllLines(boundariesFile));
        BorderPoints = new List<BorderPoint>();
        var path = new List<int>();

        foreach (var line in lines)
        {
            var data = line.Split(' ');

            var startX = double.Parse(data[0]);
            var startY = double.Parse(data[1]);
            var startPoint = new Point(startX, startY);
            var startPointNumber = CoordinatesToNumber[startPoint];

            var endX = double.Parse(data[2]);
            var endY = double.Parse(data[3]);
            var endPoint = new Point(endX, endY);
            var endPointNumber = CoordinatesToNumber[endPoint];

            var direction = ParseDirection(data[4]);

            var condition = int.Parse(data[5]);

            BorderPoints.Add(new BorderPoint(startPointNumber, direction, condition));
            BorderPoints.Add(new BorderPoint(endPointNumber, direction, condition));

            if (startX == endX)
            {
                var yMin = Math.Min(startY, endY);
                var yMax = Math.Max(startY, endY);
                var pointsNumbers = NumberToCoordinates
                    .Where(pair => pair.Value.x == startX && pair.Value.y > yMin && pair.Value.y < yMax)
                    .Select(pair => pair.Key);
                foreach (var num in pointsNumbers)
                {
                    BorderPoints.Add(new BorderPoint(num, direction, condition));
                }
            }
            else if (startY == endY)
            {
                var xMin = Math.Min(startX, endX);
                var xMax = Math.Max(startX, endX);
                var pointsNumbers = NumberToCoordinates
                    .Where(pair => pair.Value.y == startY && pair.Value.x > xMin && pair.Value.x < xMax)
                    .Select(pair => pair.Key);
                foreach (var num in pointsNumbers)
                {
                    BorderPoints.Add(new BorderPoint(num, direction, condition));
                }
            }

            path.Add(startPointNumber);

        }

        BorderPoints = BorderPoints.Distinct().ToList();

        return path;
    }

    public List<int> FindInnerPoints(List<int> border)
    {
        var innerPoints = new List<int>();

        bool IsInside(int point)
        {
            var numberIntersections = 0;

            var p = NumberToCoordinates[point];

            for (var i = 0; i < border.Count; i++)
            {
                var point1 = border[i];
                var point2 = border[(i + 1) % border.Count];
                if (point == point1 || point == point2) 
                    return false;

                var p1 = NumberToCoordinates[point1];
                var p2 = NumberToCoordinates[point2];

                var yMin = Math.Min(p1.y, p2.y);
                var yMax = Math.Max(p1.y, p2.y);
                var xMin = Math.Max(p1.x, p2.x);
                var xMax = Math.Max(p1.x, p2.x);

                if (yMin == yMax && p.y == yMin && xMin <= p.x && p.x <= xMax)
                    return false;

                if (xMin == xMax && p.x == xMin && yMin <= p.y && p.y <= yMax)
                    return false;

                if (yMin <= p.y && p.y <= yMax && p.x <= xMin)
                    numberIntersections++;

            }

            return (numberIntersections % 2 == 1);
        }


        for (var i = 0; i < XPoints.Count * YPoints.Count; i++)
        {
            if (IsInside(i))
            {
                innerPoints.Add(i);
            }
        }

        return innerPoints;
    }

    public Grid(string gridFile, string boundariesFile)
    {
        using var reader = new StreamReader(gridFile);
        
        ReadCoordinates(reader, out var xMainPoints, out var tempXPoints);
        XPoints = tempXPoints;

        ReadCoordinates(reader, out var yMainPoints, out var tempYPoints);
        YPoints = tempYPoints;

        InitializeDictionaries();

        var border = ReadBoundaries(boundariesFile);
        InnerPoints = FindInnerPoints(border);
    }

    public void DrawGrid()
    {
        using var writer = new StreamWriter("DrawGrid.txt");

        for (var i = 0; i < XPoints.Count * YPoints.Count; i++)
        {
            var point = NumberToCoordinates[i];

            string ending;

            if (InnerPoints.Contains(i)) ending = "inner";
            else
            {
                var condition = BorderPoints
                    .Where(p => p.Number == i)
                    .Select(p => (int?)p.Condition)
                    .FirstOrDefault();
                if (condition != null)
                {
                    if (condition == 1) ending = "first";
                    else ending = "second";
                }
                else
                {
                    ending = "outer";
                }
            }

            writer.WriteLine($"{point.x} {point.y} {ending}");
        }

        writer.Close();
    }
}
