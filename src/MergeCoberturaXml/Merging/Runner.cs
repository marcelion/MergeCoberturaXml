using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MergeCoberturaXml.Merging
{
    /// <summary>
    /// The main tool which executes the merging.
    /// </summary>
    public sealed class Runner
    {
        private readonly ILogger<Runner> _logger;
        private readonly FileLoader _fileLoader;
        private readonly ReportSanitizer _reportSanitizer;
        private readonly ReportMerger _reportMerger;
        private readonly ReportCalculator _reportCalculator;
        private readonly ReportWriter _reportWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Runner"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="fileLoader">The file loader.</param>
        /// <param name="reportSanitizer">The report sanitizer.</param>
        /// <param name="reportMerger">The report merger.</param>
        /// <param name="reportCalculator">The report calculator.</param>
        /// <param name="reportWriter">The report writer.</param>
        public Runner(ILogger<Runner> logger, FileLoader fileLoader, ReportSanitizer reportSanitizer, ReportMerger reportMerger, ReportCalculator reportCalculator, ReportWriter reportWriter)
        {
            _logger = logger;
            _fileLoader = fileLoader;
            _reportSanitizer = reportSanitizer;
            _reportMerger = reportMerger;
            _reportCalculator = reportCalculator;
            _reportWriter = reportWriter;
        }

        /// <summary>
        /// Executes the merge of the given files into one single coverage report.
        /// </summary>
        /// <param name="inputFilePaths">The input file paths.</param>
        /// <param name="outputFilePath">The output file path.</param>
        /// <param name="disableSanitize">True = disable sanitize, false = sanitize reports.</param>
        /// <returns></returns>
        public bool Execute(IEnumerable<string> inputFilePaths, string outputFilePath, bool disableSanitize)
        {
            try
            {
                var inputCoverageReports = _fileLoader.LoadFiles(inputFilePaths);
                if (inputCoverageReports == null)
                {
                    return false;
                }

                if (!disableSanitize)
                {
                    _reportSanitizer.Sanitize(inputCoverageReports);
                }

                var mergedReport = _reportMerger.Merge(inputCoverageReports);
                if (mergedReport != null)
                {
                    _reportCalculator.CalculateStatistics(mergedReport);

                    _reportWriter.Write(mergedReport, outputFilePath);

                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An unexpected error occurred while merging reports!");
            }

            return false;
        }
    }
}