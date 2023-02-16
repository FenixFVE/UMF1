using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMF1.Managers;

public class FileManager
{
    public static Grid ReadGrid(string gridFileName)
    {
        using (var streamReader = new FileStream(gridFileName, FileMode.Open, FileAccess.Read))
        using (var reader = new StreamReader(streamReader, Encoding.UTF8))
        {

        }


        return null;
    }
}
