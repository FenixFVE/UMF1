using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMF1.Managers;

public class FileManager
{
    public static List<Point> ReadPoints(string gridFileName)
    {

        using var reader = new StreamReader(gridFileName, Encoding.UTF8);
        var number = int.Parse(reader.ReadLine());
        var contour = new List<Point>(number);
        string line;
        while ((line = reader.ReadLine()) is not null)
        {
            contour.Add(new Point(line
                .Split(' ')
                .Select(x => double.Parse(x))
                .ToList()
            ));
        }

        if (number != contour.Count)
        {
            throw new DataException("Wrong number of contour points");
        }

        //foreach(var p in contour) Console.WriteLine($"({p.x},{p.y})");

        return contour;
    }

}
