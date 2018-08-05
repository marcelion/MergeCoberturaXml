using System.Collections.Generic;
using System.IO;
using MergeCoberturaXml.CoberturaModel;
using MergeCoberturaXml.Merging;
using MergeCoberturaXml.Test.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MergeCoberturaXml.Test.Merging
{
    public sealed class ReportWriterTest
    {
        [Fact]
        public void WriteTest()
        {
            var coverageReport = new CoverageReport
            {
                Version = "1.9",
                Timestamp = "123",
                LinesCovered = 1,
                LinesValid = 2,
                LineRate = 0.5f,
                BranchesCovered = 0,
                BranchesValid = 0,
                BranchRate = 0,
                Sources = new Sources
                {
                    Source = @"D:\dummy-project\src\"
                },
                Packages = new Packages
                {
                    Package = new List<Package>
                    {
                        new Package
                        {
                            Name = "Package1",
                            LineRate = 0.5f,
                            BranchRate = 0f,
                            Complexity = 0f,
                            Classes = new Classes
                            {
                                Class = new List<Class>
                                {
                                    new Class
                                    {
                                        Name = "Package1.Class1",
                                        Filename = "Class1.cs",
                                        LineRate = 0.5f,
                                        BranchRate = 0f,
                                        Complexity = 0f,
                                        Lines = new Lines
                                        {
                                            Line = new List<Line>
                                            {
                                                new Line { Number = 1, Hits = 1, Branch = "False" },
                                                new Line { Number = 2, Hits = 0, Branch = "False" }
                                            }
                                        },
                                        Methods = new Methods
                                        {
                                            Method = new List<Method>
                                            {
                                                new Method
                                                {
                                                    Name = "Method1",
                                                    Signature = "()",
                                                    LineRate = 0.5f,
                                                    BranchRate = 0f,
                                                    Lines = new Lines
                                                    {
                                                        Line = new List<Line>
                                                        {
                                                            new Line { Number = 1, Hits = 1, Branch = "False" },
                                                            new Line { Number = 2, Hits = 0, Branch = "False" }
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

            var outputFilePath = "output.xml";

            var logger = new MessageLogger<ReportWriter>();
            var reportWriter = new ReportWriter(logger);
            reportWriter.Write(coverageReport, outputFilePath);

            Assert.True(File.Exists(outputFilePath));

            var fileLoader = new FileLoader(new NullLogger<FileLoader>());
            var reports = fileLoader.LoadFiles(new[] {outputFilePath});
            Assert.Single(reports);

            var readReport = reports[0];

            Assert.Equal(coverageReport.LineRate, readReport.LineRate);
            Assert.Equal(coverageReport.LinesCovered, readReport.LinesCovered);
            Assert.Equal(coverageReport.LinesValid, readReport.LinesValid);
            Assert.Equal(coverageReport.BranchRate, readReport.BranchRate);
            Assert.Equal(coverageReport.BranchesCovered, readReport.BranchesCovered);
            Assert.Equal(coverageReport.BranchesValid, readReport.BranchesValid);

            Assert.Single(logger.Messages);
            Assert.Equal("Wrote merged report at 'output.xml'.", logger.Messages[0]);
        }
    }
}
