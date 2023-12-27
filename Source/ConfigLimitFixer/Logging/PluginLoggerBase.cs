using System;
using System.Runtime.CompilerServices;

namespace ConfigLimitFixer.Logging;

public abstract class PluginLoggerBase : IPluginLogger
{
    public virtual void Debug(
        string message,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Debug, message, callerMemberName);
    }

    public virtual void Debug(
        Exception exception,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Debug, exception, callerMemberName);
    }

    public virtual void Error(
        string message,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Error, message, callerMemberName);
    }

    public virtual void Error(
        Exception exception,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Error, exception, callerMemberName);
    }

    public virtual void Info(
        string message,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Info, message, callerMemberName);
    }

    public virtual void Info(
        Exception exception,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Info, exception, callerMemberName);
    }

    public virtual void Trace(
        string message,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Trace, message, callerMemberName);
    }

    public virtual void Trace(
        Exception exception,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Trace, exception, callerMemberName);
    }

    public virtual void Warn(
        string message,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Warn, message, callerMemberName);
    }

    public virtual void Warn(
        Exception exception,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Warn, exception, callerMemberName);
    }

    public virtual void Critical(
        string message,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Critical, message, callerMemberName);
    }

    public virtual void Critical(
        Exception exception,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(LogLevel.Critical, exception, callerMemberName);
    }

    public void Log(
        LogLevel logLevel,
        string message,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(
            logLevel: logLevel,
            exception: null,
            message: message,
            callerMemberName: callerMemberName);
    }

    public void Log(
        LogLevel logLevel,
        Exception exception,
        [CallerMemberName] string callerMemberName = null)
    {
        this.Log(
            logLevel: logLevel,
            exception: exception,
            message: null,
            callerMemberName: callerMemberName);
    }

    public abstract void Log(
        LogLevel logLevel,
        Exception exception,
        string message,
        [CallerMemberName] string callerMemberName = null);
}
