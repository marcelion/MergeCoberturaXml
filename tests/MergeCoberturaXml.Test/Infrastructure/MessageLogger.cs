using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MergeCoberturaXml.Test.Infrastructure
{
    public sealed class MessageLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = new List<string>();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
