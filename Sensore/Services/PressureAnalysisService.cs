using System;
using System.Collections.Generic;
using System.Linq;

namespace Sensore.Services
{
    public class PressureAnalysisService
    {
        private const int MatrixSize = 32;
        private const int MinClusterPixels = 10;
        private const int PressureThreshold = 20;   // for contact area
        private const int HighPressureThreshold = 200; // for alerts

        public int CalculatePeakPressure(int[][] matrix)
        {
            var flat = matrix.SelectMany(r => r).ToList();
            var high = flat.Where(p => p > PressureThreshold).ToList();
            return high.Count >= MinClusterPixels ? high.Max() : flat.Max();
        }

        public double CalculateContactArea(int[][] matrix)
        {
            var flat = matrix.SelectMany(r => r).ToList();
            var total = MatrixSize * MatrixSize; // 1024
            var above = flat.Count(p => p > PressureThreshold);
            return above / (double)total * 100.0;
        }

        public double CalculateRiskScore(int[][] matrix)
        {
            var peak = CalculatePeakPressure(matrix);
            var area = CalculateContactArea(matrix);
            var dist = AnalyzePressureDistribution(matrix);

            var pressureRisk = peak / 255.0 * 10.0;
            var contactRisk = Math.Abs(area - 50.0) / 50.0 * 5.0;
            var concentrationRisk = dist["concentration"] * 3.0;

            var score = pressureRisk * 0.5 + contactRisk * 0.3 + concentrationRisk * 0.2;
            return Math.Min(10.0, score);
        }

        public Dictionary<string, double> AnalyzePressureDistribution(int[][] matrix)
        {
            var flat = matrix.SelectMany(r => r).Where(p => p > 1).ToList();
            if (!flat.Any())
                return new() { ["concentration"] = 0, ["uniformity"] = 1 };

            var mean = flat.Average();
            var std = Math.Sqrt(flat.Average(x => Math.Pow(x - mean, 2)));
            var concentration = std / mean;
            var uniformity = 1.0 - Math.Min(1.0, concentration / 3.0);

            return new()
            {
                ["concentration"] = concentration,
                ["uniformity"] = uniformity,
                ["mean"] = mean,
                ["stdDev"] = std
            };
        }
    }
}
