using MergeCoberturaXml.Merging;
using MergeCoberturaXml.Test.Infrastructure;
using Xunit;

namespace MergeCoberturaXml.Test.Merging
{
    public sealed class ReportSanitizerTest : BaseTest
    {
        [Fact]
        public void SanitizeExistingFileTest()
        {
            var reports = LoadReport();

            var logger = new MessageLogger<ReportSanitizer>();
            var sanitizer = new ReportSanitizer(logger);
            sanitizer.Sanitize(reports);
            
            var report1 = reports[0];
            Assert.NotNull(report1);

            var package1 = report1.Packages.Package[0];
            Assert.NotNull(package1);

            Assert.Equal(2, package1.Classes.Class.Count);

            var class10 = package1.Classes.Class[0];
            Assert.Equal(3, class10.Methods.Method.Count);
            Assert.Contains(class10.Methods.Method, m => m.Name == "DummyMethod1");
            Assert.Contains(class10.Methods.Method, m => m.Name == "DummyMethod2");
            Assert.Contains(class10.Methods.Method, m => m.Name == "DummyMethod4");

            var class11 = package1.Classes.Class[1];
            Assert.Single(class11.Methods.Method);
            Assert.Contains(class11.Methods.Method, m => m.Name == "DummyMethod3");

            var report2 = reports[1];
            Assert.NotNull(report2);

            var package2 = report2.Packages.Package[0];
            Assert.NotNull(package2);

            Assert.Equal(2, package2.Classes.Class.Count);

            var class20 = package2.Classes.Class[0];
            Assert.Equal(3, class20.Methods.Method.Count);
            Assert.Contains(class20.Methods.Method, m => m.Name == "DummyMethod1");
            Assert.Contains(class20.Methods.Method, m => m.Name == "DummyMethod2");
            Assert.Contains(class20.Methods.Method, m => m.Name == "DummyMethod4");

            var class21 = package2.Classes.Class[1];
            Assert.Single(class21.Methods.Method);
            Assert.Contains(class21.Methods.Method, m => m.Name == "DummyMethod3");

            Assert.Equal(7, logger.Messages.Count);
            Assert.Equal("Sanitizing reports...", logger.Messages[0]);
            Assert.Equal("Renamed class 'DummyProject.Server.Controllers.DummyController' to 'DummyProject.Server.Controllers.DummyController (0)' to prevent mixup in the coverge report.", logger.Messages[1]);
            Assert.Equal("Renamed class 'DummyProject.Server.Controllers.DummyController' to 'DummyProject.Server.Controllers.DummyController (1)' to prevent mixup in the coverge report.", logger.Messages[2]);
            Assert.Equal("Removed class 'DummyProject.Server.Controllers.DummyController/<DummyMethod4>d__12' which is really the method 'DummyMethod4' of class 'DummyProject.Server.Controllers.DummyController (0)'.", logger.Messages[3]);
            Assert.Equal("Renamed class 'DummyProject.Server.Controllers.DummyController' to 'DummyProject.Server.Controllers.DummyController (0)' to prevent mixup in the coverge report.", logger.Messages[4]);
            Assert.Equal("Renamed class 'DummyProject.Server.Controllers.DummyController' to 'DummyProject.Server.Controllers.DummyController (1)' to prevent mixup in the coverge report.", logger.Messages[5]);
            Assert.Equal("Removed class 'DummyProject.Server.Controllers.DummyController/<DummyMethod4>d__12' which is really the method 'DummyMethod4' of class 'DummyProject.Server.Controllers.DummyController (0)'.", logger.Messages[6]);
        }
    }
}