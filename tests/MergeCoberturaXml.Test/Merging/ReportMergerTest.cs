using MergeCoberturaXml.Merging;
using MergeCoberturaXml.Test.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MergeCoberturaXml.Test.Merging
{
    public sealed class ReportMergerTest : BaseTest
    {
        [Fact]
        public void MergeExistingFilesTest()
        {
            var reports = LoadReport();

            var sanitizer = new ReportSanitizer(new NullLogger<ReportSanitizer>());
            sanitizer.Sanitize(reports);

            var logger = new MessageLogger<ReportMerger>();
            var reportMerger = new ReportMerger(logger);
            var mergedReport = reportMerger.Merge(reports);

            Assert.NotNull(mergedReport);
            Assert.Null(mergedReport.Sources);

            Assert.Single(mergedReport.Packages.Package);
            var package = mergedReport.Packages.Package[0];
            Assert.Equal("DummyProject.Server", package.Name);

            Assert.Equal(2, package.Classes.Class.Count);

            var class0 = package.Classes.Class[0];
            Assert.Equal(3, class0.Methods.Method.Count);
            Assert.Contains(class0.Methods.Method, m => m.Name == "DummyMethod1");
            Assert.Contains(class0.Methods.Method, m => m.Name == "DummyMethod2");
            Assert.Contains(class0.Methods.Method, m => m.Name == "DummyMethod4");

            Assert.Equal(15, class0.Lines.Line.Count);

            var class1 = package.Classes.Class[1];
            Assert.Single(class1.Methods.Method);
            Assert.Equal(5, class1.Lines.Line.Count);
            Assert.Contains(class1.Methods.Method, m => m.Name == "DummyMethod3");

            Assert.Equal(2, logger.Messages.Count);
            Assert.Equal("Merging reports...", logger.Messages[0]);
            Assert.Equal("Making all file names absolute.", logger.Messages[1]);
        }
    }
}
