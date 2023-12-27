using System;
using AutoFixture;
using ConfigLimitFixer.Logging;
using NSubstitute;
using Xunit;

namespace ConfigLimitFixer.Tests.Logging;

public class PluginLoggerBaseTests : UnitTestsBase
{
    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void TestShortcutMethodsWithException(LogLevel logLevel)
    {
        // Arrange
        var pluginLogger = Substitute.ForPartsOf<PluginLoggerBase>();
        var exception = this.Fixture.Create<Exception>();
        var callerMemberName = this.Fixture.Create<string>();

        // Act
        switch (logLevel)
        {
            case LogLevel.Trace:
                pluginLogger.Trace(exception, callerMemberName);
                break;

            case LogLevel.Debug:
                pluginLogger.Debug(exception, callerMemberName);
                break;

            case LogLevel.Info:
                pluginLogger.Info(exception, callerMemberName);
                break;

            case LogLevel.Warn:
                pluginLogger.Warn(exception, callerMemberName);
                break;

            case LogLevel.Error:
                pluginLogger.Error(exception, callerMemberName);
                break;

            case LogLevel.Critical:
                pluginLogger.Critical(exception, callerMemberName);
                break;

            default:
                throw new NotSupportedException($"The specified {nameof(LogLevel)} is not supported: {logLevel}");
        }

        // Assert
        pluginLogger.Received(1).Log(logLevel, exception, callerMemberName);
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void TestShortcutMethodsWithMessage(LogLevel logLevel)
    {
        // Arrange
        var pluginLogger = Substitute.ForPartsOf<PluginLoggerBase>();
        var message = this.Fixture.Create<string>();
        var callerMemberName = this.Fixture.Create<string>();

        // Act
        switch (logLevel)
        {
            case LogLevel.Trace:
                pluginLogger.Trace(message, callerMemberName);
                break;

            case LogLevel.Debug:
                pluginLogger.Debug(message, callerMemberName);
                break;

            case LogLevel.Info:
                pluginLogger.Info(message, callerMemberName);
                break;

            case LogLevel.Warn:
                pluginLogger.Warn(message, callerMemberName);
                break;

            case LogLevel.Error:
                pluginLogger.Error(message, callerMemberName);
                break;

            case LogLevel.Critical:
                pluginLogger.Critical(message, callerMemberName);
                break;

            default:
                throw new NotSupportedException($"The specified {nameof(LogLevel)} is not supported: {logLevel}");
        }

        // Assert
        pluginLogger.Received(1).Log(logLevel, null, message, callerMemberName);
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void TestLogWithException(LogLevel logLevel)
    {
        // Arrange
        var pluginLogger = Substitute.ForPartsOf<PluginLoggerBase>();
        var exception = this.Fixture.Create<Exception>();
        var callerMemberName = this.Fixture.Create<string>();

        // Act

        pluginLogger.Log(logLevel, exception, callerMemberName);

        // Assert
        pluginLogger.Received(1).Log(logLevel, exception, null, callerMemberName);
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void TestLogWithMessage(LogLevel logLevel)
    {
        // Arrange
        var pluginLogger = Substitute.ForPartsOf<PluginLoggerBase>();
        var message = this.Fixture.Create<string>();
        var callerMemberName = this.Fixture.Create<string>();

        // Act

        pluginLogger.Log(logLevel, message, callerMemberName);

        // Assert
        pluginLogger.Received(1).Log(logLevel, null, message, callerMemberName);
    }
}
