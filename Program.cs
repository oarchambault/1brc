using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _1brc;

public static class Program
{
    public const string MeasurementsFile = "measurements.txt";

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


        if (args.Any() && args[0].Equals("generate", StringComparison.InvariantCultureIgnoreCase))
        {
            if (args.Length >= 2 && uint.TryParse(args[1], out var size))
            {
                new MeasurementsGenerator().Generate(size, MeasurementsFile);
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
        if (!File.Exists(MeasurementsFile))
        {
            Console.WriteLine($"Measurements file ({MeasurementsFile}) is missing!");
            return;
        }

        Console.WriteLine($"Calculating averages of measurements...");
        var sw = Stopwatch.StartNew();

        using var reader = new StreamReader(MeasurementsFile);

        var stations = new Dictionary<string, WeatherStationStats>(410);

        var measurementRow = reader.ReadLine();
        while (!string.IsNullOrWhiteSpace(measurementRow))
        {
            var stationMeasurement = ParseRow(measurementRow);

            if (stations.TryGetValue(stationMeasurement.station, out var stats))
            {
                stats.AddMeasurement(stationMeasurement.measurement);
            }
            else
            {
                stations.Add(stationMeasurement.station, new WeatherStationStats(stationMeasurement.measurement));
            }

            measurementRow = reader.ReadLine();
        }

        var summary = BuildSummary(stations);

        Console.WriteLine(summary);


        Console.WriteLine($"Done calculating averages of measurements in {sw.Elapsed}.");
    }

    private static string BuildSummary(Dictionary<string, WeatherStationStats> stations)
    {
        var summary = stations
            .OrderBy(s => s.Key)
            .Select(s => $"{s.Key}={s.Value.GetAverages()}");
        return $"{{{string.Join(", ", summary)}}}";
    }

    private static (string station, double measurement) ParseRow(string measurementRow)
    {
        var split = measurementRow.Split(';');
        return (split[0], double.Parse(split[1]));
    }

    private sealed class WeatherStationStats
    {
        public WeatherStationStats(double initialMeasurement)
        {
            Min = initialMeasurement;
            Max = initialMeasurement;
            Sum = initialMeasurement;
            Count = 1;
        }

        public void AddMeasurement(double measurement)
        {
            if (measurement < Min) Min = measurement;
            if (measurement > Max) Max = measurement;
            Sum += measurement;
            Count++;
        }

        //<min>/<mean>/<max>
        public string GetAverages() => $"{Min:F1}/{Mean:F1}/{Max:F1}";

        public double Min { get; private set; }
        public double Max { get; private set; }
        public double Sum { get; private set; }
        public uint Count { get; private set; }
        public double Mean => Sum / Count;
    }
}