using System;
using System.Runtime.CompilerServices;

namespace ConfigLimitFixer.Logging;

public interface IPluginLogger
{
    void Debug(
        string message,
        [CallerMemberName] string callerMemberName = null);

    void Debug(
        Exception exception,
        [CallerMemberName] string callerMemberName = null);

    void Error(
        string message,
        [CallerMemberName] string callerMemberName = null);

    void Error(
        Exception exception,
        [CallerMemberName] string callerMemberName = null);

    void Critical(
        string message,
        [CallerMemberName] string callerMemberName = null);

    void Critical(
        Exception exception,
        [CallerMemberName] string callerMemberName = null);

    void Info(
        string message,
        [CallerMemberName] string callerMemberName = null);

    void Info(
        Exception exception,
        [CallerMemberName] string callerMemberName = null);

    void Trace(
        string message,
        [CallerMemberName] string callerMemberName = null);

    void Warn(
        string message,
        [CallerMemberName] string callerMemberName = null);
    void Warn(
        Exception exception,
        [CallerMemberName] string callerMemberName = null);

    void Log(
        LogLevel logLevel,
        Exception exception,
        string message,
        [CallerMemberName] string callerMemberName = null);

    void Log(
        LogLevel logLevel,
        string message,
        [CallerMemberName] string callerMemberName = null);
}
