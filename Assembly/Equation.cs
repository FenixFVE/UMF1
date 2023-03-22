using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMF1.Assembly;

public class Equation
{
    public static double Lambda(double x, double y) => 1.0;
    public static double Gamma(double x, double y) => 1.0;
    public static double Theta(double x, double y) => 1.0;
    // U
    public static double U(double x, double y)
        => x * x * x + y * y * y;

    // dx
    public static double Udx(double x, double y)
        => 3 * x * x;
    public static double Udx2(double x, double y)
        => 6 * x;

    // dy
    public static double Udy(double x, double y)
        => 3 * y * y;
    public static double Udy2(double x, double y)
        => 6 * y;


    // F = -Lambda * (Udx2 + Udy2) + Gamma * U
    public static double F(double x, double y)
        => -Lambda(x, y) * (Udx2(x, y) + Udy2(x, y)) + Gamma(x, y) * U(x, y);
}