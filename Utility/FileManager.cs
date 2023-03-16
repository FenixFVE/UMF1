using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMF1.Utility;

public class FileManager
{
    public static List<T> ReadLineToList<T>(StreamReader stream) where T : IConvertible
        => stream.ReadLine()
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => (T)Convert.ChangeType(x, typeof(T)))
            .ToList();

    public static void ReadParameters(string parametersFile, out int size, out double relaxation, out double eps,
        out int maxSteps, out int blockSize)
    {
        try
        {
            using var reader = new StreamReader(parametersFile);
            var data = reader
                .ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            size = int.Parse(data[0]);
            relaxation = double.Parse(data[1]);
            eps = double.Parse(data[2]);
            maxSteps = int.Parse(data[3]);
            blockSize = int.Parse(data[4]);
        }
        catch
        {
            throw new Exception($"Fal to read parameters: {parametersFile}");
        }
    }

    public static List<double> ReadVector(string vectorFile)
    {
        using var file = new StreamReader(vectorFile);
        var size = int.Parse(file.ReadLine());
        var vec = new List<double>(size);
        for (var i = 0; i < size; i++)
        {
            vec.Add(double.Parse(file.ReadLine()));
        }
        return vec;
    }

    public static void WriteVector(string vectorFile, List<double> vec)
    {
        using var writer = new StreamWriter(vectorFile);
        foreach (var x in vec)
        {
            writer.WriteLine(x.ToString("0.00000000000000e+00", CultureInfo.CreateSpecificCulture("en-US")));
        }
    }
}

