using MergeCoberturaXml.Merging;
using MergeCoberturaXml.Test.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MergeCoberturaXml.Test.Merging
{
    public sealed class RunnerTest
    {
        [Fact]
        public void ExecuteTest()
        {
            var mergeOptions = new MergeOptions
            {
                InputFiles = new[]
                {
                    @"TestFiles\TestFile1.xml",
                    @"TestFiles\TestFile2.xml"
                },
                OutputFile = "merged.xml",
                LogLevel = LogLevel.Trace,
                DisableSanitize = false
            };
            
            var fileLoader = new FileLoader(new NullLogger<FileLoader>());
            var reportSanitizer = new ReportSanitizer(new NullLogger<ReportSanitizer>());
            var reportMerger = new ReportMerger(new NullLogger<ReportMerger>());
            var reportCalculator = new ReportCalculator(new NullLogger<ReportCalculator>());
            var reportWriter = new ReportWriter(new NullLogger<ReportWriter>());

            var logger = new MessageLogger<Runner>();
            var runner = new Runner(logger, fileLoader, reportSanitizer, reportMerger, reportCalculator, reportWriter);
            Assert.True(runner.Execute(mergeOptions.InputFiles, mergeOptions.OutputFile, mergeOptions.DisableSanitize));

            Assert.Empty(logger.Messages);
        }
    }
}
