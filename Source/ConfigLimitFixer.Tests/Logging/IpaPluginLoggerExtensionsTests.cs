using System.Collections.Generic;
using System.Linq;
using ConfigLimitFixer.Logging;
using IPA.Logging;
using Xunit;

namespace ConfigLimitFixer.Tests.Logging;

public class IpaPluginLoggerExtensionsTests : UnitTestsBase
{
    public static (LogLevel, Logger.Level)[] LogLevelProjectionMap { get; } = new[]
    {
        (LogLevel.None, Logger.Level.None),
        (LogLevel.Trace, Logger.Level.Trace),
        (LogLevel.Debug, Logger.Level.Debug),
        (LogLevel.Info, Logger.Level.Info),
        (LogLevel.Warn, Logger.Level.Warning),
        (LogLevel.Error, Logger.Level.Error),
        (LogLevel.Critical, Logger.Level.Critical),
    };

    public static IEnumerable<object[]> PluginToIpaData =>
        from pair in LogLevelProjectionMap
        select new object[] { pair.Item1, pair.Item2 };

    public static IEnumerable<object[]> IpaToPluginData =>
        from pair in LogLevelProjectionMap
        select new object[] { pair.Item2, pair.Item1 };

    [Theory]
    [MemberData(nameof(PluginToIpaData))]
    public void TestToIpaLogLevel(
        LogLevel logLevel,
        Logger.Level expected)
    {
        // Act
        var actual = logLevel.ToIpaLogLevel();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(IpaToPluginData))]
    public void TestToPluginLogLevel(
        Logger.Level logLevel,
        LogLevel expected)
    {
        // Act
        var actual = logLevel.ToPluginLogLevel();

        // Assert
        Assert.Equal(expected, actual);
    }
}