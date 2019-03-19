using System.Collections.Generic;
using MergeCoberturaXml.CoberturaModel;
using MergeCoberturaXml.Merging;
using MergeCoberturaXml.Test.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MergeCoberturaXml.Test.Merging
{
    public sealed class ReportCalculatorTest : BaseTest
    {
        [Fact]
        public void CalculateStatisticsTest()
        {
            var reports = LoadReport();

            var sanitizer = new ReportSanitizer(new NullLogger<ReportSanitizer>());
            sanitizer.Sanitize(reports);

            var reportMerger = new ReportMerger(new NullLogger<ReportMerger>());
            var mergedReport = reportMerger.Merge(reports);

            var logger = new MessageLogger<ReportCalculator>();
            var reportCalculator = new ReportCalculator(logger);
            reportCalculator.CalculateStatistics(mergedReport);

            Assert.Single(mergedReport.Packages.Package);

            var package = mergedReport.Packages.Package[0];
            Assert.Equal("DummyProject.Server", package.Name);
            Assert.Equal(1, package.LineRate);
            Assert.Equal(1, package.BranchRate);
            Assert.Equal(0, package.Complexity);
            Assert.Equal(2, package.Classes.Class.Count);

            var class0 = package.Classes.Class[0];
            Assert.Equal("DummyProject.Server.Controllers.DummyController (0)", class0.Name);
            Assert.EndsWith(@"src\DummyProject.Server\Controllers\DummyController.cs", class0.Filename);
            Assert.Equal(1, class0.LineRate);
            Assert.Equal(1, class0.BranchRate);
            Assert.Equal(0, class0.Complexity);
            Assert.Equal(3, class0.Methods.Method.Count);
            Assert.Equal(15, class0.Lines.Line.Count);
            Assert.Contains(class0.Lines.Line, l => l.Number == 1 && l.Hits == 12 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 2 && l.Hits == 12 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 3 && l.Hits == 12 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 4 && l.Hits == 12 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 5 && l.Hits == 12 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 6 && l.Hits == 3 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 7 && l.Hits == 3 && l.ConditionCoverage == "100% (2/2)");
            Assert.Contains(class0.Lines.Line, l => l.Number == 8 && l.Hits == 3 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 9 && l.Hits == 3 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 10 && l.Hits == 3 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 11 && l.Hits == 6 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 12 && l.Hits == 6 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 13 && l.Hits == 6 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 14 && l.Hits == 6 && l.ConditionCoverage == null);
            Assert.Contains(class0.Lines.Line, l => l.Number == 15 && l.Hits == 6 && l.ConditionCoverage == null);

            var method01 = class0.Methods.Method[0];
            Assert.Equal("DummyMethod1", method01.Name);
            Assert.Equal(1, method01.LineRate);
            Assert.Equal(1, method01.BranchRate);
            Assert.Contains(method01.Lines.Line, l => l.Number == 1 && l.Hits == 12 && l.ConditionCoverage == null);
            Assert.Contains(method01.Lines.Line, l => l.Number == 2 && l.Hits == 12 && l.ConditionCoverage == null);
            Assert.Contains(method01.Lines.Line, l => l.Number == 3 && l.Hits == 12 && l.ConditionCoverage == null);
            Assert.Contains(method01.Lines.Line, l => l.Number == 4 && l.Hits == 12 && l.ConditionCoverage == null);
            Assert.Contains(method01.Lines.Line, l => l.Number == 5 && l.Hits == 12 && l.ConditionCoverage == null);

            var method02 = class0.Methods.Method[1];
            Assert.Equal("DummyMethod2", method02.Name);
            Assert.Equal(1, method02.LineRate);
            Assert.Equal(1, method02.BranchRate);
            Assert.Contains(method02.Lines.Line, l => l.Number ==  6 && l.Hits == 3 && l.ConditionCoverage == null);
            Assert.Contains(method02.Lines.Line, l => l.Number ==  7 && l.Hits == 3 && l.ConditionCoverage == "100% (2/2)");
            Assert.Contains(method02.Lines.Line, l => l.Number ==  8 && l.Hits == 3 && l.ConditionCoverage == null);
            Assert.Contains(method02.Lines.Line, l => l.Number ==  9 && l.Hits == 3 && l.ConditionCoverage == null);
            Assert.Contains(method02.Lines.Line, l => l.Number == 10 && l.Hits == 3 && l.ConditionCoverage == null);

            var method03 = class0.Methods.Method[2];
            Assert.Equal("DummyMethod4", method03.Name);
            Assert.Equal(1, method03.LineRate);
            Assert.Equal(1, method03.BranchRate);
            Assert.Contains(method03.Lines.Line, l => l.Number == 11 && l.Hits == 6 && l.ConditionCoverage == null);
            Assert.Contains(method03.Lines.Line, l => l.Number == 12 && l.Hits == 6 && l.ConditionCoverage == null);
            Assert.Contains(method03.Lines.Line, l => l.Number == 13 && l.Hits == 6 && l.ConditionCoverage == null);
            Assert.Contains(method03.Lines.Line, l => l.Number == 14 && l.Hits == 6 && l.ConditionCoverage == null);
            Assert.Contains(method03.Lines.Line, l => l.Number == 15 && l.Hits == 6 && l.ConditionCoverage == null);

            var class1 = package.Classes.Class[1];
            Assert.Equal("DummyProject.Server.Controllers.DummyController (1)", class1.Name);
            Assert.EndsWith(@"src\DummyProject.Server\Controllers\DummyControllerPartial.cs", class1.Filename);
            Assert.Equal(1, class1.LineRate);
            Assert.Equal(1, class1.BranchRate);
            Assert.Single(class1.Methods.Method);
            Assert.Equal(5, class1.Lines.Line.Count);
            Assert.Contains(class1.Lines.Line, l => l.Number == 1 && l.Hits == 24 && l.ConditionCoverage == null);
            Assert.Contains(class1.Lines.Line, l => l.Number == 2 && l.Hits == 24 && l.ConditionCoverage == null);
            Assert.Contains(class1.Lines.Line, l => l.Number == 3 && l.Hits == 24 && l.ConditionCoverage == null);
            Assert.Contains(class1.Lines.Line, l => l.Number == 4 && l.Hits == 24 && l.ConditionCoverage == null);
            Assert.Contains(class1.Lines.Line, l => l.Number == 5 && l.Hits == 24 && l.ConditionCoverage == null);

            var method11 = class1.Methods.Method[0];
            Assert.Equal("DummyMethod3", method11.Name);
            Assert.Equal(1, method11.LineRate);
            Assert.Equal(1, method11.BranchRate);
            Assert.Contains(method11.Lines.Line, l => l.Number == 1 && l.Hits == 24 && l.ConditionCoverage == null);
            Assert.Contains(method11.Lines.Line, l => l.Number == 2 && l.Hits == 24 && l.ConditionCoverage == null);
            Assert.Contains(method11.Lines.Line, l => l.Number == 3 && l.Hits == 24 && l.ConditionCoverage == null);
            Assert.Contains(method11.Lines.Line, l => l.Number == 4 && l.Hits == 24 && l.ConditionCoverage == null);
            Assert.Contains(method11.Lines.Line, l => l.Number == 5 && l.Hits == 24 && l.ConditionCoverage == null);

            Assert.Single(logger.Messages);
            Assert.Equal("Calculating the statistics of the merged report...", logger.Messages[0]);
        }

        [Fact]
        public void ConditionCoverageTest()
        {
            var report = new CoverageReport
            {
                Packages = new Packages
                {
                    Package = new List<Package>
                    {
                        new Package
                        {
                            Classes = new Classes
                            {
                                Class = new List<Class>
                                {
                                    new Class
                                    {
                                        Methods = new Methods
                                        {
                                            Method = new List<Method>()
                                        },
                                        Lines = new Lines
                                        {
                                            Line = new List<Line>
                                            {
                                                new Line
                                                {
                                                    Number = 1,
                                                    Hits = 0,
                                                    Branch = "True",
                                                    ConditionCoverage = "33% (1/3)",
                                                    Conditions = new Conditions
                                                    {
                                                        Condition = new List<Condition>
                                                        {
                                                            new Condition
                                                            {
                                                                Number = "1",
                                                                Coverage = "33.3%",
                                                                Type = "jump"
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var logger = new MessageLogger<ReportCalculator>();
            var reportCalculator = new ReportCalculator(logger);
            reportCalculator.CalculateStatistics(report);
            // TODO: Not correct, but no exception
            Assert.Equal("33.3% (0.7/2)", report.Packages.Package[0].Classes.Class[0].Lines.Line[0].ConditionCoverage);
        }
    }
}
