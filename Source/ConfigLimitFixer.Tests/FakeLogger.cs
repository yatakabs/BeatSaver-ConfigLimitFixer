using System;
using System.Runtime.CompilerServices;
using ConfigLimitFixer.Logging;

namespace ConfigLimitFixer.Tests;

public class FakeLogger : PluginLoggerBase
{
    public override void Log(
        LogLevel logLevel,
        Exception exception,
        string message,
        [CallerMemberName] string callerMemberName = null)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            var logMessage = callerMemberName != null
                ? $"[{callerMemberName}] {message}"
                : message;

            Console.WriteLine(
                $"{logLevel.ToString().ToUpperInvariant()}: {logMessage}");
        }

        if (exception != null)
        {
            Console.WriteLine(
                $"{logLevel.ToString().ToUpperInvariant()}: {exception}");
        }
    }
}
