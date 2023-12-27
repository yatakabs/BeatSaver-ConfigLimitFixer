using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using ConfigLimitFixer.Logging;
using NSubstitute;
using Xunit;

namespace ConfigLimitFixer.Tests.Logging;

public class PluginLoggerExtensionsTests
{
    /// <summary>
    /// Tests all the methods in the PluginLoggerExtensions class works as expected.
    /// </summary>
    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void TestPluginLoggerExtensionsWithExceptionAndMessage(LogLevel logLevel)
    {
        // Arrange with AutoFixture with NSubstitute
        var autoFixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var pluginLogger = autoFixture.Create<IPluginLogger>();
        var exception = autoFixture.Create<Exception>();
        var message = autoFixture.Create<string>();
        var callerMemberName = autoFixture.Create<string>();

        // Act
        switch (logLevel)
        {
            case LogLevel.Trace:
                pluginLogger.Trace(exception, message, callerMemberName);
                break;

            case LogLevel.Debug:
                pluginLogger.Debug(exception, message, callerMemberName);
                break;

            case LogLevel.Info:
                pluginLogger.Info(exception, message, callerMemberName);
                break;

            case LogLevel.Warn:
                pluginLogger.Warn(exception, message, callerMemberName);
                break;

            case LogLevel.Error:
                pluginLogger.Error(exception, message, callerMemberName);
                break;

            case LogLevel.Critical:
                pluginLogger.Critical(exception, message, callerMemberName);
                break;

            default:
                throw new NotSupportedException($"The specified {nameof(LogLevel)} is not supported: {logLevel}");
        }

        // Assert
        pluginLogger
            .Received(1)
            .Log(logLevel, exception, message, callerMemberName);
    }

    /// <summary>
    /// Tests all the methods in the PluginLoggerExtensions class works as expected.
    /// </summary>
    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void TestPluginLoggerExtensionsWithFormat(LogLevel logLevel)
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var pluginLogger = fixture.Create<IPluginLogger>();
        var format = Guid.NewGuid().ToString();
        var args = new object[] { Guid.NewGuid().ToString() };

        // Act
        switch (logLevel)
        {
            case LogLevel.Trace:
                pluginLogger.TraceFormat(format, args);
                break;

            case LogLevel.Debug:
                pluginLogger.DebugFormat(format, args);
                break;

            case LogLevel.Info:
                pluginLogger.InfoFormat(format, args);
                break;

            case LogLevel.Warn:
                pluginLogger.WarnFormat(format, args);
                break;

            case LogLevel.Error:
                pluginLogger.ErrorFormat(format, args);
                break;

            case LogLevel.Critical:
                pluginLogger.CriticalFormat(format, args);
                break;

            default:
                throw new NotSupportedException($"The specified {nameof(LogLevel)} is not supported: {logLevel}");
        }

        // Assert
        pluginLogger
            .Received(1)
            .LogFormat(logLevel, format, args);
    }

    /// <summary>
    /// Tests all the methods in the PluginLoggerExtensions class works as expected.
    /// </summary>
    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void TestPluginLoggerExtensionsWithExceptionAndFormat(LogLevel logLevel)
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var pluginLogger = fixture.Create<IPluginLogger>();
        var exception = fixture.Create<Exception>();
        var format = Guid.NewGuid().ToString();
        var args = new object[] { Guid.NewGuid().ToString() };

        // Act
        switch (logLevel)
        {
            case LogLevel.Trace:
                pluginLogger.TraceFormat(exception, format, args);
                break;

            case LogLevel.Debug:
                pluginLogger.DebugFormat(exception, format, args);
                break;

            case LogLevel.Info:
                pluginLogger.InfoFormat(exception, format, args);
                break;

            case LogLevel.Warn:
                pluginLogger.WarnFormat(exception, format, args);
                break;

            case LogLevel.Error:
                pluginLogger.ErrorFormat(exception, format, args);
                break;

            case LogLevel.Critical:
                pluginLogger.CriticalFormat(exception, format, args);
                break;

            default:
                throw new NotSupportedException($"The specified {nameof(LogLevel)} is not supported: {logLevel}");
        }

        // Assert
        pluginLogger
            .Received(1)
            .LogFormat(logLevel, exception, format, args);
    }

    /// <summary>
    /// Tests Log() methods properly call its upstream Log() methods.
    /// </summary>
    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void TestLogFormat(LogLevel logLevel)
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var pluginLogger = fixture.Create<IPluginLogger>();
        var format = Guid.NewGuid().ToString();
        var args = new object[] { Guid.NewGuid().ToString() };

        var formattedMessage = string.Format(format, args);

        // Act
        pluginLogger.LogFormat(logLevel, format, args);

        // Assert
        pluginLogger
            .Received(1)
            .LogFormat(logLevel, formattedMessage);
    }

    /// <summary>
    /// Tests LogFormat() methods properly call its upstream LogFormat() or Log() methods.
    /// </summary>
    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void TestLogFormatWithException(LogLevel logLevel)
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var pluginLogger = fixture.Create<IPluginLogger>();
        var exception = fixture.Create<Exception>();
        var format = Guid.NewGuid().ToString();
        var args = new object[] { Guid.NewGuid().ToString() };
        var formattedMessage = string.Format(format, args);

        // Act
        pluginLogger.LogFormat(logLevel, exception, format, args);

        // Assert
        pluginLogger
            .Received(1)
            .LogFormat(logLevel, exception, formattedMessage);
    }
}
