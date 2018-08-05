using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using MergeCoberturaXml.CoberturaModel;
using Microsoft.Extensions.Logging;

namespace MergeCoberturaXml.Merging
{
    /// <summary>
    /// Serializes and writes a report to the file system.
    /// </summary>
    public sealed class ReportWriter
    {
        private readonly ILogger<ReportWriter> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportWriter"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ReportWriter(ILogger<ReportWriter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Serializes the report and writes it to the file system.
        /// </summary>
        /// <param name="report">The report to serialize and write.</param>
        /// <param name="outputFilePath">The output file path.</param>
        public void Write(CoverageReport report, string outputFilePath)
        {
            report.Packages.Package = report.Packages.Package.OrderBy(p => p.Name).ToList();

            var xmlSerializer = new XmlSerializer(typeof(CoverageReport));
            using (var writer = XmlWriter.Create(outputFilePath, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  "
            }))
            {
                xmlSerializer.Serialize(writer, report);
            }

            _logger.LogInformation($"Wrote merged report at '{outputFilePath}'.");
        }
    }
}