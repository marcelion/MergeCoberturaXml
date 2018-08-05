using System.Collections.Generic;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace MergeCoberturaXml
{
    /// <summary>
    /// The command line options.
    /// </summary>
    public sealed class MergeOptions
    {
        /// <summary>
        /// Gets or sets the input files.
        /// </summary>
        [Option('i', "input", Required = true, Separator = ';', HelpText = "The cobertura XML files to merge into a single file.")]
        public IEnumerable<string> InputFiles { get; set; }

        /// <summary>
        /// Gets or sets the output file.
        /// </summary>
        [Option('o', "output", Required = true, HelpText = "The path to write the merged XML file to.")]
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        [Option('l', "log", Required = false, Default = LogLevel.Information, HelpText = "The log level of the output.")]
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable fixing errors or not.
        /// </summary>
        [Option("disable-sanitize", Default = false, HelpText = "Disables fixing errors in the input files.")]
        public bool DisableSanitize { get; set; }
    }
}
