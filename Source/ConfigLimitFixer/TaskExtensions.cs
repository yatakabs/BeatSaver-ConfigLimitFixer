using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class TaskExtensions
{
    public static Task WaitOneAsTaskAsync(
        this WaitHandle waitHandle,
        CancellationToken cancellationToken = default)
    {
        return waitHandle
            .WaitOneAsTaskAsync<object>(
                result: null,
                cancellationToken: cancellationToken);
    }

    public static Task<TResult> WaitOneAsTaskAsync<TResult>(
        this WaitHandle waitHandle,
        Func<TResult> resultSelector,
        CancellationToken cancellationToken = default)
    {
        return waitHandle
            .WaitOneAsTask<object, TResult>(
                resultSelector: _ => resultSelector.Invoke(),
                state: null,
                cancellationToken: cancellationToken);
    }

    public static Task<TResult> WaitOneAsTaskAsync<TResult>(
        this WaitHandle waitHandle,
        TResult result,
        CancellationToken cancellationToken = default)
    {
        return waitHandle
            .WaitOneAsTaskAsync(
                resultSelector: () => result,
                cancellationToken: cancellationToken);
    }

    public static Task<TResult> WaitOneAsTask<TState, TResult>(
        this WaitHandle waitHandle,
        Func<TState, TResult> resultSelector,
        TState state = default,
        CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<TResult>();

        var cancellationTokenHandleRegistration = ThreadPool.RegisterWaitForSingleObject(
            waitObject: cancellationToken.WaitHandle,
            callBack: (_, __) =>
            {
                tcs.TrySetCanceled(cancellationToken);
            },
            state: state,
            millisecondsTimeOutInterval: -1,
            executeOnlyOnce: true);

        var waitHandleRegistration = ThreadPool.RegisterWaitForSingleObject(
            waitObject: waitHandle,
            callBack: (_, __) =>
            {
                try
                {
                    tcs.TrySetResult(resultSelector.Invoke(state));
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            },
            state: state,
            millisecondsTimeOutInterval: -1,
            executeOnlyOnce: true);

        _ = tcs.Task.ContinueWith(
            (_, __) =>
            {
                cancellationTokenHandleRegistration.Unregister(null);
                waitHandleRegistration.Unregister(null);
            },
            TaskContinuationOptions.ExecuteSynchronously);

        return tcs.Task;
    }

    public static WaitHandle WaitAny(
        this IEnumerable<WaitHandle> waitHandles,
        int chunkSize = 60,
        CancellationToken cancellationToken = default)
    {
        var signaled = waitHandles
            .Chunk(chunkSize)
            .Select(chunk =>
            {
                var handlesToWait = chunk
                    .Prepend(cancellationToken.WaitHandle)
                    .ToArray();

                var index = WaitHandle.WaitAny(handlesToWait);

                return index > 0
                    ? chunk[index - 1]
                    : null;
            })
            .FirstOrDefault(x => x != null);

        if (signaled == cancellationToken.WaitHandle)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        return signaled;
    }
}
