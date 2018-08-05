using System;
using MergeCoberturaXml.Merging;
using MergeCoberturaXml.Test.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MergeCoberturaXml.Test.Merging
{
    public sealed class FileLoaderTest
    {
        [Fact]
        public void NullTest()
        {
            var fileLoader = new FileLoader(new NullLogger<FileLoader>());
            Assert.Throws<ArgumentNullException>(() => fileLoader.LoadFiles(null));
        }

        [Fact]
        public void NonExistingFile()
        {
            var inputFilePath = "nonexisting.xml";

            var logger = new MessageLogger<FileLoader>();
            var fileLoader = new FileLoader(logger);
            var result = fileLoader.LoadFiles(new[] {inputFilePath});
            Assert.Null(result);

            Assert.Single(logger.Messages);
            Assert.Equal($"Failed to parse the file at path '{inputFilePath}'!", logger.Messages[0]);
        }

        [Fact]
        public void ExistingFile()
        {
            var inputFilePath = "TestFiles/TestFile1.xml";

            var logger = new MessageLogger<FileLoader>();
            var fileLoader = new FileLoader(logger);
            var result = fileLoader.LoadFiles(new[] { inputFilePath });
            Assert.NotNull(result);

            Assert.Single(logger.Messages);
            Assert.Equal($"Loaded the report at path '{inputFilePath}'.", logger.Messages[0]);
        }

        [Fact]
        public void ExistingFileVersion20()
        {
            var inputFilePath = "TestFiles/TestFile1_2.0.xml";

            var logger = new MessageLogger<FileLoader>();
            var fileLoader = new FileLoader(logger);
            var result = fileLoader.LoadFiles(new[] { inputFilePath });
            Assert.NotNull(result);

            Assert.Equal(2, logger.Messages.Count);
            Assert.Equal($"Loaded the report at path '{inputFilePath}'.", logger.Messages[0]);
            Assert.Equal($"The report at path '{inputFilePath}' has an other version than 1.9. Merging may fail!", logger.Messages[1]);
        }
    }
}