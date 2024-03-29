﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using UMF1.BlockRelaxation;
using UMF1.Assembly;

namespace UMF1.Utility;


public class Testing
{
    public static void Test0()
    {
        FileManager.ReadParameters(".\\Input\\Tests\\test_parameters.txt", out var size, out var relaxation, out var eps, out var maxSteps, out var blockSize);
        Console.WriteLine($"Relaxation: {relaxation}, Eps: {eps}, Max steps: {maxSteps}, Block_size: {blockSize}\n");

        var temporal = Enumerable.Repeat(0.0, size).ToList();
        var xBlock = new List<double>(temporal);
        var F = FileManager.ReadVector(".\\Input\\test_vector_Fa.txt");
        var matrix = new DiagonalMatrix(".\\Input\\Tests\\test_matrix_A.txt");

        BlockRelaxation.BlockRelaxation.Block(matrix, F, xBlock, relaxation, eps, maxSteps, blockSize, temporal);
        foreach (var x in xBlock) Console.WriteLine(x);
        FileManager.WriteVector(".\\Output\\Tests\\test_block_output.txt", xBlock);
    }

    public static void Test1()
    {
        var size = 10;
        var relaxation = 0.5;
        var eps = 10.0e-13;
        var maxSteps = 10000;
        var blockSize = 2;
        var matrix = new DiagonalMatrix(10, 5);
        var temporal = Enumerable.Repeat(0.0, size).ToList();
        var xBlock = new List<double>(temporal);
        var F = new List<double>() { 18, 31, 44, 57, 70, 73, 86, 99, 112, 114 };

        matrix.Diagonal[0] = Enumerable.Repeat(1.0, 10).ToList();
        for (var i = 0; i < 5; i++)
        {
            matrix.Diagonal[0][i] = 0.0;
        }
        matrix.Diagonal[1] = Enumerable.Repeat(1.0, 10).ToList();
        matrix.Diagonal[2][0] = 0.0;
        matrix.Diagonal[2] = Enumerable.Repeat(10.0, 10).ToList();
        matrix.Diagonal[3] = Enumerable.Repeat(1.0, 10).ToList();
        matrix.Diagonal[3][9] = 0.0;
        matrix.Diagonal[4] = Enumerable.Repeat(1.0, 10).ToList();
        for (var i = 5; i < 10; i++)
        {
            matrix.Diagonal[4][i] = 0.0;
        }

        BlockRelaxation.BlockRelaxation.Block(matrix, F, xBlock, relaxation, eps, maxSteps, blockSize, temporal);
        foreach (var x in xBlock) Console.WriteLine(x);
        FileManager.WriteVector(".\\Output\\Tests\\test_block_output.txt", xBlock);
    }

    public static void Test3()
    {
        var reader = new StreamReader(".\\Input\\Tests\\test_format.txt");
        Grid.ReadCoordinates(reader, out var MainPoints, out var tempXPoints);
        var AllPoints = tempXPoints;

        Console.WriteLine("1 Main points:");
        foreach (var x in MainPoints) Console.Write(x + " ");
        Console.WriteLine("\n1 All points");
        foreach (var x in AllPoints) Console.Write(x + " ");
        Console.WriteLine();

        Grid.ReadCoordinates(reader, out var MainPoints2, out var tempXPoints2);
        var AllPoints2 = tempXPoints2;

        Console.WriteLine("2 Main points:");
        foreach (var x in MainPoints2) Console.Write(x + " ");
        Console.WriteLine("\n2 All points");
        foreach (var x in AllPoints2) Console.Write(x + " ");
        Console.WriteLine();
    }

}

