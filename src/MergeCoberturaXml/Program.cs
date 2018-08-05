using System;
using CommandLine;
using MergeCoberturaXml.Merging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MergeCoberturaXml
{
    static class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<MergeOptions>(args)
                .WithParsed(mergeOptions =>
                {
                    var serviceProvider = new ServiceCollection()
                        .AddLogging(builder =>
                        {
                            builder.AddDebug();
                            builder.AddConsole();
                        })
                        .Configure<LoggerFilterOptions>(options => options.MinLevel = mergeOptions.LogLevel)

                        .AddTransient<Runner>()
                        .AddTransient<FileLoader>()
                        .AddTransient<ReportSanitizer>()
                        .AddTransient<ReportMerger>()
                        .AddTransient<ReportCalculator>()
                        .AddTransient<ReportWriter>()
                        .BuildServiceProvider();

                    try
                    {
                        var runner = serviceProvider.GetService<Runner>();
                        var result = runner.Execute(mergeOptions.InputFiles, mergeOptions.OutputFile, mergeOptions.DisableSanitize);
                        if (result)
                        {
                            Environment.ExitCode = 0;
                            return;
                        }

                        Environment.ExitCode = 1;
                    }
                    finally
                    {
                        serviceProvider?.Dispose();
                    }
                })
                .WithNotParsed(errors =>
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new Parser(config => config.HelpWriter = Console.Out);

                    Environment.ExitCode = 1;
                });
        }
    }
}