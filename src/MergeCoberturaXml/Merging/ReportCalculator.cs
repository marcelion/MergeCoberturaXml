using System.Collections.Generic;
using MergeCoberturaXml.CoberturaModel;
using Microsoft.Extensions.Logging;

namespace MergeCoberturaXml.Merging
{
    /// <summary>
    /// Calculates the coverage statistics of a report.
    /// </summary>
    public sealed class ReportCalculator
    {
        private readonly ILogger<ReportCalculator> _reportCalculator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportCalculator"/> class.
        /// </summary>
        /// <param name="reportCalculator">The report calculator.</param>
        public ReportCalculator(ILogger<ReportCalculator> reportCalculator)
        {
            _reportCalculator = reportCalculator;
        }

        /// <summary>
        /// Calculates the statistics of the coverage report.
        /// </summary>
        /// <param name="report">The coverage report.</param>
        public void CalculateStatistics(CoverageReport report)
        {
            _reportCalculator.LogInformation("Calculating the statistics of the merged report...");

            var reportCounters = new Counters();
            foreach (var package in report.Packages.Package)
            {
                CalculatePackage(package, reportCounters);
            }

            report.LinesCovered = reportCounters.LinesCovered;
            report.LinesValid = reportCounters.LinesValid;
            report.LineRate = GetRate(reportCounters.LinesCovered, reportCounters.LinesValid);

            report.BranchesCovered = reportCounters.BranchesCovered;
            report.BranchesValid = reportCounters.BranchesValid;
            report.BranchRate = GetRate(reportCounters.BranchesCovered, reportCounters.BranchesValid);
        }

        private void CalculatePackage(Package package, Counters reportCounters)
        {
            var packageCounters = new Counters();
            foreach (var cl in package.Classes.Class)
            {
                CalculateClass(cl, packageCounters);
            }

            package.LineRate = GetRate(packageCounters.LinesCovered, packageCounters.LinesValid);
            package.BranchRate = GetRate(packageCounters.BranchesCovered, packageCounters.BranchesValid);

            reportCounters.LinesCovered += packageCounters.LinesCovered;
            reportCounters.LinesValid += packageCounters.LinesValid;
            reportCounters.BranchesCovered += packageCounters.BranchesCovered;
            reportCounters.BranchesValid += packageCounters.BranchesValid;
        }

        private void CalculateClass(Class cl, Counters packageCounters)
        {
            var classCounters = new Counters();
            foreach (var method in cl.Methods.Method)
            {
                CalculateMethod(method, classCounters);
            }

            CalculateLines(cl.Lines.Line);

            cl.LineRate = GetRate(classCounters.LinesCovered, classCounters.LinesValid);
            cl.BranchRate = GetRate(classCounters.BranchesCovered, classCounters.BranchesValid);

            packageCounters.LinesCovered += classCounters.LinesCovered;
            packageCounters.LinesValid += classCounters.LinesValid;
            packageCounters.BranchesCovered += classCounters.BranchesCovered;
            packageCounters.BranchesValid += classCounters.BranchesValid;
        }

        private void CalculateLines(List<Line> lines)
        {
            foreach (var line in lines)
            {
                if (line.Conditions != null)
                {
                    int count = 0;
                    int sum = 0;
                    foreach (var condition in line.Conditions.Condition)
                    {
                        if (int.TryParse(condition.Coverage.Replace("%", ""), out int coverage))
                        {
                            count++;
                            sum += coverage;
                        }
                    }

                    var conditionCoverage = sum / count;

                    line.ConditionCoverage = $"{conditionCoverage}% ({sum / 50}/{count * 2})";
                }
            }
        }

        private void CalculateMethod(Method method, Counters classCounters)
        {
            var methodCounters = new Counters();
            foreach (var line in method.Lines.Line)
            {
                methodCounters.LinesValid++;
                if (line.Hits > 0)
                {
                    methodCounters.LinesCovered++;
                }
            }

            CalculateLines(method.Lines.Line);

            method.LineRate = GetRate(methodCounters.LinesCovered, methodCounters.LinesValid);
            method.BranchRate = GetRate(methodCounters.BranchesCovered, methodCounters.BranchesValid);

            classCounters.LinesCovered += methodCounters.LinesCovered;
            classCounters.LinesValid += methodCounters.LinesValid;
            classCounters.BranchesCovered += methodCounters.BranchesCovered;
            classCounters.BranchesValid += methodCounters.BranchesValid;
        }

        private float GetRate(int covered, int valid)
        {
            if (covered == 0 && valid == 0)
            {
                return 1;
            }

            if (valid == 0)
            {
                return 0;
            }

            return covered / (float) valid;
        }

        private sealed class Counters
        {
            public int LinesCovered { get; set; }
            public int LinesValid { get; set; }
            public int BranchesCovered { get; set; }
            public int BranchesValid { get; set; }
        }
    }
}