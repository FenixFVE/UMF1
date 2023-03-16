using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMF1.Assembly;

public class Assembler
{
    public Grid grid { get; set; }

    public Assembler (Grid grid)
    {
        this.grid = grid;
    }
}