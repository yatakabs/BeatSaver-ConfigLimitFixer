using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ConfigLimitFixer.Logging;
using IPA;
using IPA.Config;
using IPALogger = IPA.Logging.Logger;

namespace ConfigLimitFixer;

[Plugin(RuntimeOptions.SingleStartInit)]
public sealed class Plugin
{
    private IPluginLogger Logger { get; }

    [Init]
    public Plugin(IPALogger logger)
    {
        if (logger == null) { throw new ArgumentNullException(nameof(logger)); }

        this.Logger = new IpaPluginLogger(logger);

        try
        {
            // Starts the new thread for overriding the default save thread.
            this.Logger.Info("Overriding IPA's default SaveThread.");

            this.OverrideSaveThread();

            this.Logger.Info("Overridden IPA's default SaveThread.");
        }
        catch (Exception ex)
        {
            this.Logger.Critical(ex, "Failed to override IPA's default SaveThread.");
        }
    }

    [OnStart]
    public void OnApplicationStart()
    {
        // Nothing to do here.
        this.Logger.Debug("OnApplicationStart()");
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        // Nothing to do here.
        this.Logger.Debug("OnApplicationQuit()");
    }

    /// <summary>
    /// Overrides the default SaveThread of IPA.
    /// </summary>
    private void OverrideSaveThread()
    {
        this.Logger.Info("Overriding IPA's default SaveThread.");

        // Gets the ConfigRuntime type from the assembly containing Config class.
        var assembly = typeof(Config).Assembly;
        this.Logger.Debug($"Assembly: {assembly.FullName}");

        var types = assembly.GetTypes();

        // Gets the ConfigRuntime type.
        var configRuntimeType = Array
            .Find(types, x => x.FullName == Constants.ConfigRuntimeTypeFullName)
            ?? throw new InvalidOperationException($"Could not find type: {Constants.ConfigRuntimeTypeFullName}");
        this.Logger.Debug($"ConfigRuntime type found: {Constants.ConfigRuntimeTypeFullName}");

        // Gets the saveThread static field from the ConfigRuntime type.

        var saveThreadField = configRuntimeType
            .GetField(Constants.SaveThreadFieldName, BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Could not find field: {Constants.SaveThreadFieldName}");
        this.Logger.Debug($"Field found: {Constants.SaveThreadFieldName}");

        // If the field is already set, abort the existing thread, as it is IPA's default SaveThread.
        if (saveThreadField.GetValue(null) is Thread saveThread)
        {
            this.Logger.Debug("An old SaveThread is already running. Aborting...");

            var threadState = new
            {
                saveThread.Name,
                saveThread.IsAlive,
                saveThread.ThreadState,
                saveThread.ManagedThreadId,
            };

            this.Logger.Info($"Old SaveThread: {threadState}");

            try
            {
                saveThread.Abort();
                saveThreadField.SetValue(null, null);
            }
            catch
            {
                this.Logger.Error("Failed to abort existing thread.");

                // If the thread is not aborted, the new thread will not be started.
                throw;
            }
        }

        try
        {
            // Starts a new thread.
            this.StartSaveThread(
                Constants.ReplacedSaveThreadName,
                configRuntimeType,
                saveThreadField);
        }
        catch
        {
            this.Logger.Error("Failed to start the new thread.");
            throw;
        }
    }

    private void StartSaveThread(
        string threadName,
        Type configRuntimeType,
        FieldInfo saveThreadField)
    {
        // Gets the configs field from the ConfigRuntime type.

        var configsField = configRuntimeType
            .GetField(Constants.ConfigsFieldName, BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"{Constants.ConfigsFieldName} field not found.");

        var configs = configsField
            .GetValue(null) as ConcurrentBag<Config>
            ?? throw new InvalidOperationException($"{Constants.ConfigsFieldName} field not set.");

        var saveMethod = configRuntimeType
            .GetMethod(Constants.SaveMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"{Constants.SaveMethodName} method not found.");

        var configsChangedWatcherField = configRuntimeType
            .GetField(Constants.ConfigsChangedWatcherFieldName, BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"{Constants.ConfigsChangedWatcherFieldName} field not found.");

        var configsChangedWatcher = configsChangedWatcherField
            .GetValue(null) as AutoResetEvent
            ?? throw new InvalidOperationException($"{Constants.ConfigsChangedWatcherFieldName} field not set.");

        this.Logger.Info("Starting the new save thread.");

        var saveThreadParameters = new SaveThreadParameters(
            configs: configs,
            saveMethod: saveMethod,
            configsChangedWatcher: configsChangedWatcher);

        var newThread = new Thread(this.ReplacedSaveThreadMain)
        {
            Name = Constants.ReplacedSaveThreadName,
        };

        // Starts the new thread and set the field.
        saveThreadField.SetValue(null, newThread);
        newThread.Start(saveThreadParameters);
        this.Logger.Info("A new save thread started.");

        // Starts a monitor thread to check the status of the new thread.
        // Should be removed in release build.
        this.RunSaveThreadMonitor(
            saveThreadField,
            newThread);
    }

    [Conditional("DEBUG")]
    private void RunSaveThreadMonitor(
        FieldInfo saveThreadField,
        Thread newThread,
        TimeSpan monitorInterval = default,
        CancellationToken stoppingToken = default)
    {
        var actualMonitorInterval = monitorInterval == default
            ? TimeSpan.FromSeconds(3)
            : monitorInterval;

        Task.Run(async () =>
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var newThreadData = new
                    {
                        newThread.Name,
                        newThread.ThreadState,
                        newThread.IsAlive,
                        newThread.ManagedThreadId,
                        newThread.IsThreadPoolThread,
                        newThread.IsBackground,
                    };

                    this.Logger.Debug($"[Monitor] NewThread: {newThreadData}");

                    if (saveThreadField.GetValue(null) is Thread activeThread)
                    {
                        var activeThreadData = new
                        {
                            activeThread.Name,
                            activeThread.ThreadState,
                            activeThread.IsAlive,
                            activeThread.ManagedThreadId,
                            activeThread.IsThreadPoolThread,
                            activeThread.IsBackground,
                        };

                        this.Logger.Debug($"[Monitor] ActiveThread: {activeThreadData}");
                    }
                    else
                    {
                        this.Logger.Error("[Monitor] Failed to check the active thread.");
                    }

                    await Task
                        .Delay(monitorInterval, stoppingToken)
                        .ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == stoppingToken)
            {
                this.Logger.Debug("Monitor thread is canceled.");
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Monitor thread unexpectedly finished.");
            }
            finally
            {
                this.Logger.Error("Exiting the monitor thread.");
            }
        });
    }

    private void ReplacedSaveThreadMain(object parameter)
    {
        if (parameter is not SaveThreadParameters saveThreadParameters)
        {
            this.Logger?.Error("Failed to get SaveThreadParameters.");
            return;
        }

        var configsChangedWatcher = saveThreadParameters.ConfigsChangedWatcher;
        var chunkSize = saveThreadParameters.ChunkSize;

        try
        {
            var configsBag = saveThreadParameters.Configs;

            var configWaitHandleMap = new Dictionary<WaitHandle, Config>();

            void updateConfigSnapshot()
            {
                configWaitHandleMap = configsBag
                    .ToArray()
                    .Select(x => (config: x, waitHandle: x.GetInternalStore()?.SyncObject))
                    .Where(x => x.waitHandle != null)
                    .Prepend((config: null, waitHandle: configsChangedWatcher))
                    .ToDictionary(x => x.waitHandle, x => x.config);
            }

            // The main loop of the new thread.
            // This loop will be running until the thread is aborted.
            while (true)
            {
                this.Logger.Debug("A new iteration of config change detection loop is starting.");
                using var cts = new CancellationTokenSource();

                try
                {
                    var configsChanged = configsBag.Count != configWaitHandleMap.Count
                        || configsBag.Except(configWaitHandleMap.Values).Any();

                    if (configsChanged)
                    {
                        this.Logger.Debug("Config collection changed.");
                        var currentCount = configWaitHandleMap.Count;
                        updateConfigSnapshot();

                        var newCount = configWaitHandleMap.Count;

                        this.Logger.InfoFormat("Config collection changed. CurrentCount: {0}, NewCount: {1}", currentCount, newCount);
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Failed to update config snapshot.");
                    break;
                }

                try
                {
                    var signaled = configWaitHandleMap
                        .Keys
                        .WaitAny(chunkSize, cts.Token);

                    if (signaled != null && configWaitHandleMap.TryGetValue(signaled, out var config) && config != null)
                    {
                        try
                        {
                            this.Logger.Debug($"Config change detected. Name: {config.Name}");
                            saveThreadParameters.InvokeSave(config);

                            this.Logger.Debug($"Config saved. Name: {config.Name}");
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Critical(ex, $"Failed to save config. Name: {config.Name}");
                        }
                    }
                    else
                    {
                        this.Logger.Debug("No config change detected.");
                    }
                }
                catch (OperationCanceledException)
                {
                    this.Logger.Debug("Operation canceled for the current iteration.");
                    continue;
                }
                finally
                {
                    cts.Cancel();
                }
            }
        }
        catch (ThreadAbortException ex)
        {
            this.Logger.Warn($"Thread is being aborted, before cancellation request is handled.");
            this.Logger.Warn(ex);
        }
        catch (Exception ex)
        {
            this.Logger.Error($"{nameof(ReplacedSaveThreadMain)}() has been unexpectedly stopped.");
            this.Logger.Error(ex);
        }
        finally
        {
            this.Logger.Warn($"Exiting overriding thread. ThreadMainMethod: {nameof(ReplacedSaveThreadMain)}()");
        }
    }

    /// <summary>
    /// Represents the parameters for the save thread.
    /// </summary>
    public class SaveThreadParameters
    {
        /// <summary>
        /// Gets or sets the collection of configs.
        /// </summary>
        public ConcurrentBag<Config> Configs { get; }

        /// <summary>
        /// Gets or sets the save method.
        /// </summary>
        public MethodInfo SaveMethod { get; }

        /// <summary>
        /// Gets or sets the configs changed watcher.
        /// </summary>
        public AutoResetEvent ConfigsChangedWatcher { get; }

        /// <summary>
        /// Gets or sets the chunk size.
        /// </summary>
        public int ChunkSize { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveThreadParameters"/> class.
        /// </summary>
        /// <param name="configs">The collection of configs.</param>
        /// <param name="saveMethod">The save method.</param>
        /// <param name="configsChangedWatcher">The configs changed watcher.</param>
        /// <param name="chunkSize">The chunk size.</param>
        public SaveThreadParameters(
            ConcurrentBag<Config> configs,
            MethodInfo saveMethod,
            AutoResetEvent configsChangedWatcher,
            int chunkSize = 60)
        {
            this.Configs = configs ?? throw new ArgumentNullException(nameof(configs));
            this.SaveMethod = saveMethod ?? throw new ArgumentNullException(nameof(saveMethod));
            this.ConfigsChangedWatcher = configsChangedWatcher ?? throw new ArgumentNullException(nameof(configsChangedWatcher));

            if (chunkSize <= 0)
            {
                throw new ArgumentException(nameof(chunkSize), "Value must be greater than 0.");
            }

            this.ChunkSize = chunkSize;
        }

        /// <summary>
        /// Invokes the save method with the specified config.
        /// </summary>
        /// <param name="config">The config to save.</param>
        public void InvokeSave(Config config)
        {
            this.SaveMethod.Invoke(null, new[] { config });
        }
    }
}
