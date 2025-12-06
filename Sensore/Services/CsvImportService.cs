using System.Globalization;
using Sensore.Services; // for PressureAnalysisService

namespace Sensore.Services
{
    public class CsvImportService
    {
        private readonly PressureAnalysisService _analysis;

        public CsvImportService(PressureAnalysisService analysis)
        {
            _analysis = analysis;
        }

        // Reads a CSV with N×32 and yields frames of 32×32
        public IEnumerable<int[][]> ReadFrames(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var rows = new List<int[]>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',', StringSplitOptions.TrimEntries);
                if (parts.Length != 32) continue; // skip bad rows

                var row = parts
                    .Select(p => int.Parse(p, CultureInfo.InvariantCulture))
                    .ToArray();

                rows.Add(row);

                // Once we have 32 rows, yield one 32×32 frame
                if (rows.Count == 32)
                {
                    var frame = rows.ToArray();
                    rows.Clear();
                    yield return frame;
                }
            }
        }

        public (int peak, double area, double risk) AnalyzeFrame(int[][] matrix)
        {
            var peak = _analysis.CalculatePeakPressure(matrix);
            var area = _analysis.CalculateContactArea(matrix);
            var risk = _analysis.CalculateRiskScore(matrix);
            return (peak, area, risk);
        }
    }
}