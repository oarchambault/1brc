

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _1brc;

public static class Program
{
    public static void Main(string[] args)
    {
#if DEBUG
        Console.WriteLine("Running in debug!");
#endif

        if (!args.Any())
        {
            Console.WriteLine("Missing arguments");
            return;
        }

        if (args[0].Equals("generate", StringComparison.InvariantCultureIgnoreCase))
        {
            if(args.Length >= 2 && uint.TryParse(args[1], out var size))
            {
                MeasurementsGenerator.Generate(size);
                return;
            }

            Console.WriteLine("Missing argument, expecting number of rows to generate.");
            return;
        }

        if (args[0].Equals("calculate", StringComparison.InvariantCultureIgnoreCase))
        {
            CalculateAverages();
            return;
        }

        Console.WriteLine("Invalid arguments.");
    }

    private static void CalculateAverages()
    {
        if (!File.Exists("measurements.txt"))
        {
            Console.WriteLine("Measurements file (measurements.txt) is missing!");
            return;
        }

        Console.WriteLine($"Calculating averages of measurements...");
        var sw = Stopwatch.StartNew();

        // TODO:

        Console.WriteLine($"Done calculating averages of measurements in {sw.Elapsed}.");
    }
}