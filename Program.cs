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
        //Testing.Test0();
        //Testing.Test1();
        //Testing.Test3();

        var grid = new Grid(".//Input//Tests//test_format.txt", ".//Input//Tests//test_conditions.txt");
        grid.DrawGrid();
    }
}



