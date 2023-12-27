using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using ConfigLimitFixer.Logging;
using NSubstitute;
using Shouldly;
using Xunit;
using IPALogger = IPA.Logging.Logger;
namespace ConfigLimitFixer.Tests.Logging;

public class IpaPluginLoggerTests : UnitTestsBase
{
    /// <summary>
    /// Log message formatter for test methods.
    /// </summary>
    /// <remarks>
    /// Returns the message as is if the caller member name is null.
    /// Otherwise, returns the message with the caller member name.
    /// </remarks>
    /// <returns>
    /// The formatted message.
    /// </returns>
    private static string FormatMessage(
        string callerMemberName,
        string message)
    {
        return callerMemberName == null
            ? message
            : $"{callerMemberName}: {message}";
    }

    #region Test parameters

    /// <summary>
    /// Test parameter values of <see cref="IPALogger"/>.
    /// </summary>
    public static IPALogger[] IpaLoggers { get; } = new IPALogger[] { null, Substitute.ForPartsOf<IPALogger>() };

    /// <summary>
    /// Test parameter values of <see cref="LogLevel"/>.
    /// </summary>
    public static LogLevel[] LogLevels { get; } = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>().ToArray();

    /// <summary>
    /// Test parameter values of <see cref="Exception"/>.
    /// </summary>
    public static Exception[] Exceptions { get; } = new Exception[] { null, new() };

    /// <summary>
    /// Test parameter values of <see cref="string"/>.
    /// </summary>
    public static string[] Messages { get; } = new string[] { null, "", " ", "Test" };

    /// <summary>
    /// Test parameter values of <see cref="CallerMemberNameAttribute"/>.
    /// </summary>
    public static string[] CallerMemberNames { get; } = new string[] { null, "", " ", "Test" };

    /// <summary>
    /// Test parameter values of <see cref="IpaPluginLogger.LogMessageFormatter"/>.
    /// </summary>
    public static IpaPluginLogger.LogMessageFormatter[] LogMessageFormatters { get; } = new[] {
        IpaPluginLogger.DefaultMessageFormatter,
        FormatMessage,
        null,
    };

    #endregion Test parameters

    /// <summary>
    /// Test parameter matrix for the <see cref="IpaPluginLogger.Log(LogLevel, Exception, string, string)"/> method.
    /// </summary>
    /// <remarks>
    /// Test matrix with the following parameters:
    /// - LogLevel: Trace, Debug, Info, Warn, Error, Critical
    /// - Exception: null, not null
    /// - Message: null, not null
    /// - CallerMemberName: null, not null
    /// - MessageFormatter: DefaultMessageFormatter, [custom], null
    /// </remarks>
    public static IEnumerable<object[]> LogMethodParameterMatrix =>
        from logLevel in LogLevels
        from exception in Exceptions
        from message in Messages
        from callerMemberName in CallerMemberNames
        from logMessageFormatter in LogMessageFormatters
        select new object[] { logLevel, exception, message, callerMemberName, logMessageFormatter };

    #region Test parameter matrix

    /// <summary>
    /// Test parameter matrix for the <see cref="IpaPluginLogger.IpaPluginLogger(IPALogger)"/> constructor.
    /// </summary>
    /// <remarks>
    /// Test matrix with the following parameters:
    /// - Logger: null, not null
    // </remarks>
    public static IEnumerable<object[]> Constructor1ParameterMatrix =>
        from ipaLogger in IpaLoggers
        let shouldFail = ipaLogger == null
        select new object[] { ipaLogger, shouldFail };

    /// <summary>
    /// Test parameter matrix for the <see cref="IpaPluginLogger.IpaPluginLogger(IPALogger, IpaPluginLogger.LogMessageFormatter)"/> constructor.
    /// </summary>
    /// <remarks>
    /// Test matrix with the following parameters:
    /// - Logger: null, not null
    /// - MessageFormatter: null, not null
    // </remarks>
    public static IEnumerable<object[]> Constructor2ParameterMatrix =>
        from ipaLogger in IpaLoggers
        from logMessageFormatter in LogMessageFormatters
        let shouldFail = ipaLogger == null
        select new object[] { ipaLogger, logMessageFormatter, shouldFail };

    #endregion Test parameter matrix

    [Theory]
    [MemberData(nameof(Constructor1ParameterMatrix))]
    public void TestConstructorWithParameterLogger(
        IPALogger ipaLogger,
        bool shouldFail)
    {
        // Act
        var exception = Record.Exception(() => new IpaPluginLogger(ipaLogger));

        // Assert
        if (shouldFail)
        {
            exception.ShouldBeOfType<ArgumentNullException>();
        }
        else
        {
            exception.ShouldBeNull();
        }
    }

    [Theory]
    [MemberData(nameof(Constructor2ParameterMatrix))]
    public void TestConstructorWithParameterMatrix(
        IPALogger ipaLogger,
        IpaPluginLogger.LogMessageFormatter logMessageFormatter,
        bool shouldFail)
    {
        // Act
        var exception = Record.Exception(() => new IpaPluginLogger(ipaLogger, logMessageFormatter));

        // Assert
        if (shouldFail)
        {
            exception.ShouldBeOfType<ArgumentNullException>();
        }
        else
        {
            exception.ShouldBeNull();
        }
    }

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
        var ipaLogger = this.MockIpaLogger();

        var pluginLogger = Substitute.ForPartsOf<IpaPluginLogger>(ipaLogger);
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
                // Should never happen
                throw new NotSupportedException($"The specified {nameof(LogLevel)} is not supported: {logLevel}");
        }

        // Assert
        pluginLogger
            .Received(1)
            .Log(logLevel, exception, callerMemberName);

        ipaLogger
            .Received(1)
            .Log(logLevel.ToIpaLogLevel(), exception);
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
        var ipaLogger = this.MockIpaLogger();

        var pluginLogger = Substitute.ForPartsOf<IpaPluginLogger>(ipaLogger);

        var message = this.Fixture.Create<string>();
        var callerMemberName = this.Fixture.Create<string>();

        var expectedMessage = IpaPluginLogger.DefaultMessageFormatter(message, callerMemberName);

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
                // Should never happen
                throw new NotSupportedException($"The specified {nameof(LogLevel)} is not supported: {logLevel}");
        }

        // Assert
        pluginLogger
            .Received(1)
            .Log(logLevel, message, callerMemberName);

        ipaLogger
            .Received(1)
            .Log(logLevel.ToIpaLogLevel(), expectedMessage);
    }

    [Theory]
    [MemberData(nameof(LogMethodParameterMatrix))]
    public void TestLogWithParameterMatrix(
        LogLevel logLevel,
        Exception exception,
        string message,
        string callerMemberName,
        IpaPluginLogger.LogMessageFormatter logMessageFormatter)
    {
        // Arrange
        var ipaLogger = this.MockIpaLogger();

        var pluginLogger = new IpaPluginLogger(ipaLogger, logMessageFormatter);

        var expectedMessage =
            (logMessageFormatter ?? IpaPluginLogger.DefaultMessageFormatter)
            .Invoke(message, callerMemberName);
        var expectedLogLevel = logLevel.ToIpaLogLevel();

        // Act
        pluginLogger.Log(logLevel, exception, message, callerMemberName);

        // Assert
        if (exception != null)
        {
            ipaLogger
                .Received(1)
                .Log(expectedLogLevel, exception);
        }

        if (!string.IsNullOrWhiteSpace(message))
        {
            ipaLogger
                .Received(1)
                .Log(expectedLogLevel, expectedMessage);
        }

        if (exception == null && string.IsNullOrWhiteSpace(message))
        {
            ipaLogger
                .DidNotReceive();
        }
    }
}
