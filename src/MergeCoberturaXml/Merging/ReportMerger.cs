using System.Collections.Generic;
using System.IO;
using System.Linq;
using MergeCoberturaXml.CoberturaModel;
using Microsoft.Extensions.Logging;

namespace MergeCoberturaXml.Merging
{
    /// <summary>
    /// Merges multiple reports of type <see cref="CoverageReport"/> into a single report.
    /// </summary>
    public sealed class ReportMerger
    {
        private readonly ILogger<ReportMerger> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportMerger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ReportMerger(ILogger<ReportMerger> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Merges the reports into one single report.
        /// </summary>
        /// <param name="reports">The reports.</param>
        /// <returns>The merged report.</returns>
        public CoverageReport Merge(ICollection<CoverageReport> reports)
        {
            _logger.LogInformation("Merging reports...");

            var mergedReport = new CoverageReport
            {
                Version = reports.Min(r => r.Version),
                Timestamp = reports.Max(r => r.Timestamp)
            };

            if (MergeSources(mergedReport, reports))
            {
                MergeReports(mergedReport, reports);

                return mergedReport;
            }

            return null;
        }

        #region Merge Reports
        private void MergeReports(CoverageReport mergedReport, ICollection<CoverageReport> reports)
        {
            mergedReport.Packages = new Packages
            {
                Package = new List<Package>()
            };
            foreach (var report in reports)
            {
                MergePackages(mergedReport, report.Packages.Package);
            }
        }

        private void MergePackages(CoverageReport mergedReport, List<Package> packages)
        {
            foreach (var package in packages)
            {
                var existingPackage = mergedReport.Packages.Package.FirstOrDefault(p => p.Name == package.Name);
                if (existingPackage == null)
                {
                    existingPackage = new Package
                    {
                        Name = package.Name,
                        LineRate = 0, // To be calculated
                        BranchRate = 0, // To be calculated
                        Complexity = 0, // To be calculated?
                        Classes = new Classes
                        {
                            Class = new List<Class>()
                        }
                    };
                    mergedReport.Packages.Package.Add(existingPackage);
                }

                MergeClasses(existingPackage, package.Classes.Class);
            }
        }

        private void MergeClasses(Package existingPackage, List<Class> classes)
        {
            foreach (var cl in classes)
            {
                if (cl.Name.Contains("Seed"))
                {

                }

                var existingClass = existingPackage.Classes.Class.FirstOrDefault(c => c.Name == cl.Name && c.Filename == cl.Filename);
                if (existingClass == null)
                {
                    existingClass = new Class
                    {
                        Name = cl.Name,
                        Filename = cl.Filename,
                        LineRate = 0, // To be calculated
                        BranchRate = 0, // To be calculated
                        Complexity = 0, // To be calculated?
                        Lines = new Lines
                        {
                            Line = new List<Line>()
                        },
                        Methods = new Methods
                        {
                            Method = new List<Method>()
                        }
                    };
                    existingPackage.Classes.Class.Add(existingClass);
                }

                MergeMethods(existingClass, cl.Methods.Method);
                MergeLines(existingClass.Lines, cl.Lines.Line);
            }
        }

        private void MergeMethods(Class existingClass, List<Method> methods)
        {
            foreach (var method in methods)
            {
                var existingMethod = existingClass.Methods.Method.FirstOrDefault(m => m.Name == method.Name && m.Signature == method.Signature);
                if (existingMethod == null)
                {
                    existingMethod = new Method
                    {
                        Name = method.Name,
                        Signature = method.Signature,
                        LineRate = 0, // To be calculated
                        BranchRate = 0, // To be calculated
                        Lines = new Lines
                        {
                            Line = new List<Line>()
                        }
                    };
                    existingClass.Methods.Method.Add(existingMethod);
                }

                MergeLines(existingMethod.Lines, method.Lines.Line);
            }
        }

        private void MergeLines(Lines existingLines, List<Line> lines)
        {
            foreach (var line in lines)
            {
                var existingLine = existingLines.Line.FirstOrDefault(l => l.Number == line.Number);
                if (existingLine == null)
                {
                    existingLine = new Line
                    {
                        Number = line.Number,
                        Hits = line.Hits,
                        Branch = line.Branch
                    };
                    existingLines.Line.Add(existingLine);
                }
                else
                {
                    existingLine.Hits += line.Hits;
                }

                if (line.Conditions != null)
                {
                    if (existingLine.Conditions == null)
                    {
                        existingLine.Conditions = new Conditions
                        {
                            Condition = new List<Condition>()
                        };
                    }

                    MergeConditions(existingLine, line.Conditions.Condition);
                }
            }
        }

        private void MergeConditions(Line existingLine, List<Condition> conditions)
        {
            foreach (var condition in conditions)
            {
                var existingCondition = existingLine.Conditions.Condition.FirstOrDefault(c => c.Number == condition.Number);
                if (existingCondition == null)
                {
                    existingCondition = new Condition
                    {
                        Number = condition.Number,
                        Type = condition.Type,
                        Coverage = condition.Coverage
                    };
                    existingLine.Conditions.Condition.Add(existingCondition);
                }
                else
                {
                    int.TryParse(existingCondition.Coverage.Replace("%", ""), out int existingCoverage);
                    if (int.TryParse(condition.Coverage.Replace("%", ""), out int newCoverage))
                    {
                        if (newCoverage > existingCoverage)
                        {
                            existingCondition.Coverage = condition.Coverage;
                        }
                    }
                }
            }
        }

        #endregion

        #region Merge Sources

        private bool MergeSources(CoverageReport mergedReport, ICollection<CoverageReport> reports)
        {
            var sharedSource = GetSharedSource(reports);
            if (!string.IsNullOrEmpty(sharedSource))
            {
                _logger.LogDebug($"Merging sources of reports to most common path '{sharedSource}'.");

                mergedReport.Sources = new Sources { Source = sharedSource };

                foreach (var report in reports)
                {
                    if (report.Sources.Source != sharedSource)
                    {
                        ChangeSource(report, sharedSource);
                    }
                }
            }

            return true;
        }

        private string GetSharedSource(ICollection<CoverageReport> reports)
        {
            var sources = new Dictionary<string, ICollection<CoverageReport>>();

            foreach (var report in reports)
            {
                if (!string.IsNullOrEmpty(report.Sources?.Source))
                {
                    var directoryInfo = new DirectoryInfo(report.Sources.Source.TrimEnd(Path.DirectorySeparatorChar));
                    while (directoryInfo != null)
                    {
                        if (!sources.TryGetValue(directoryInfo.FullName, out var mappedReports))
                        {
                            mappedReports = new List<CoverageReport>();
                            sources.Add(directoryInfo.FullName, mappedReports);
                        }

                        mappedReports.Add(report);

                        directoryInfo = directoryInfo.Parent;
                    }
                }
            }

            var sharedSource = sources.Where(r => r.Value.Count == reports.Count).OrderByDescending(r => r.Key.Length).Select(p => p.Key).FirstOrDefault();
            if (!string.IsNullOrEmpty(sharedSource))
            {
                return $"{sharedSource}{Path.DirectorySeparatorChar}";
            }

            _logger.LogError($"No common path in source paths was found! Source paths of reports: {string.Join(", ", reports.Select(r => r.Sources.Source))}");
            return null;
        }

        private void ChangeSource(CoverageReport report, string newSource)
        {
            var oldSource = report.Sources.Source;
            var diffSource = oldSource.Replace(newSource, "");

            foreach (var package in report.Packages.Package)
            {
                foreach (var cl in package.Classes.Class)
                {
                    cl.Filename = $"{diffSource}{cl.Filename}";
                }
            }
        }

        #endregion
    }
}