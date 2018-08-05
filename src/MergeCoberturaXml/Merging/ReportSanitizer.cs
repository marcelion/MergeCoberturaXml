using System;
using System.Collections.Generic;
using System.Linq;
using MergeCoberturaXml.CoberturaModel;
using Microsoft.Extensions.Logging;

namespace MergeCoberturaXml.Merging
{
    /// <summary>
    /// Sanitizes the reports. Fixing methods which are generated as own class. Rename classes if they are defined in multiple files.
    /// </summary>
    public sealed class ReportSanitizer
    {
        private readonly ILogger<ReportSanitizer> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportSanitizer"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ReportSanitizer(ILogger<ReportSanitizer> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sanitizes the reports.
        /// </summary>
        /// <param name="reports">The reports to sanitize.</param>
        public void Sanitize(ICollection<CoverageReport> reports)
        {
            _logger.LogInformation("Sanitizing reports...");

            foreach (var inputCoverageReport in reports)
            {
                try
                {
                    SanitizeReport(inputCoverageReport);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Sanitizing the report failed. Merging may fail or result in poor reports!");
                }
            }
        }

        private void SanitizeReport(CoverageReport report)
        {
            foreach (var package in report.Packages.Package)
            {
                SanitizePackage(package);
            }
        }

        private void SanitizePackage(Package package)
        {
            foreach (var cl in package.Classes.Class.ToList())
            {
                SanitizeClass(cl, package);
            }
        }

        private void SanitizeClass(Class cl, Package package)
        {
            if (cl.Name.Contains("/<"))
            {
                int start = cl.Name.IndexOf("/<", StringComparison.Ordinal) + 2;
                int end = cl.Name.LastIndexOf(">", StringComparison.Ordinal);
                var methodName = cl.Name.Substring(start, end - start);
                var className = cl.Name.Substring(0, start - 2);

                var existingClass = package.Classes.Class.FirstOrDefault(c => c.Name.StartsWith(className) && c.Filename == cl.Filename);
                if (existingClass != null)
                {
                    if (cl.Methods.Method.Any() || cl.Lines.Line.Any())
                    {
                        existingClass.Methods.Method.Add(new Method
                        {
                            Name = methodName,
                            Signature = cl.Methods.Method.First().Signature,
                            Lines = cl.Methods.Method.First().Lines
                        });

                        existingClass.Lines.Line.AddRange(cl.Lines.Line);
                    }

                    package.Classes.Class.Remove(cl);

                    _logger.LogDebug($"Removed class '{cl.Name}' which is really the method '{methodName}' of class '{existingClass.Name}'.");
                }
                else
                {
                    _logger.LogDebug($"Fixed the name of the class '{cl.Name}' which is really the method '{methodName}' of class '{className}'.");

                    cl.Name = className;
                    cl.Methods.Method.First().Name = methodName;
                }
            }

            var sameClasses = package.Classes.Class.Where(c => c.Name == cl.Name).ToList();
            if (sameClasses.Count > 1)
            {
                int index = 0;
                foreach (var sameClass in sameClasses)
                {
                    var newClassName = sameClass.Name + $" ({index++})";

                    _logger.LogDebug($"Renamed class '{sameClass.Name}' to '{newClassName}' to prevent mixup in the coverge report.");

                    sameClass.Name = newClassName;
                }
            }
        }
    }
}