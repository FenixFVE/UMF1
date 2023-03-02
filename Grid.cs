using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMF1;

public struct Point
{
    public double x;
    public double y;

    public Point(double x, double y)
    {
        this.x = x; this.y = y;
    }

    public Point(List<double> num)
    {
        x = num[0]; y = num[1];
    }
}

public class Grid
{
    public List<Point> Contour { get; set; }
    public List<double> XBorder { get; set; }
    public List<double> YBorder { get; set; }

    public Dictionary<Point, int> CoordinatesToNumber { get; set; }
    public Dictionary<int, Point> NumberToCoordinates { get; set; }

    public List<int> InnerPoints { get; set; }

    public Grid(List<Point> contour)
    {
        Contour = contour;
        XBorder = Contour
            .Select(p => p.x)
            .OrderBy(x => x)
            .Distinct()
            .ToList();
        YBorder = Contour
            .Select(p => p.y)
            .OrderBy(y => y)
            .Distinct()
            .ToList();
    }

    public void AddFragmentation()
    {

    }

    public void CreateDictionarys()
    {
        CoordinatesToNumber = new Dictionary<Point, int>(XBorder.Count * YBorder.Count);
        NumberToCoordinates = new Dictionary<int, Point>(CoordinatesToNumber.Count);
        int counter = 0;
        for (var j = 0; j < YBorder.Count; j++)
        {
            for (var i = 0; i < XBorder.Count; i++)
            {
                var point = new Point(XBorder[i], YBorder[j]);
                CoordinatesToNumber.Add(point, counter);
                NumberToCoordinates.Add(counter++, point);
            }
        }
    }


}
