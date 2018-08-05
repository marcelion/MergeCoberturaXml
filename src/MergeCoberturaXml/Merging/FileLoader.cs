using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using MergeCoberturaXml.CoberturaModel;
using Microsoft.Extensions.Logging;

namespace MergeCoberturaXml.Merging
{
    /// <summary>
    /// Loads XMLs of type <see cref="CoverageReport"/> from the file system.
    /// </summary>
    public sealed class FileLoader
    {
        private readonly ILogger<FileLoader> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoader"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public FileLoader(ILogger<FileLoader> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Loads the files and parses the XML document.
        /// </summary>
        /// <param name="inputFilePaths">The input file paths.</param>
        /// <returns>The loaded and parsed reports of type <see cref="CoverageReport"/>.</returns>
        /// <exception cref="ArgumentNullException">inputFilePaths</exception>
        public List<CoverageReport> LoadFiles(IEnumerable<string> inputFilePaths)
        {
            if (inputFilePaths == null)
            {
                throw new ArgumentNullException(nameof(inputFilePaths));
            }

            var reports = new List<CoverageReport>();

            foreach (var inputFilePath in inputFilePaths)
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(CoverageReport));
                    using (var reader = XmlReader.Create(inputFilePath))
                    {
                        var coverageReport = (CoverageReport)serializer.Deserialize(reader);
                        reports.Add(coverageReport);

                        _logger.LogInformation($"Loaded the report at path '{inputFilePath}'.");

                        if (coverageReport.Version != "1.9")
                        {
                            _logger.LogWarning($"The report at path '{inputFilePath}' has an other version than 1.9. Merging may fail!");
                        }
                    }
                }
                catch(Exception exception)
                {
                    _logger.LogError(exception, $"Failed to parse the file at path '{inputFilePath}'!");
                    return null;
                }
            }

            if (!reports.Any())
            {
                return null;
            }

            return reports;
        }
    }
}