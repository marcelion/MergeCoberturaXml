using System.Collections.Generic;
using MergeCoberturaXml.CoberturaModel;
using MergeCoberturaXml.Merging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MergeCoberturaXml.Test
{
    public abstract class BaseTest
    {
        protected List<CoverageReport> LoadReport()
        {
            var inputFilePath1 = "TestFiles/TestFile1.xml";
            var inputFilePath2 = "TestFiles/TestFile2.xml";

            var fileLoader = new FileLoader(new NullLogger<FileLoader>());
            var result = fileLoader.LoadFiles(new[] {inputFilePath1, inputFilePath2});
            Assert.NotNull(result);

            return result;
        }
    }
}