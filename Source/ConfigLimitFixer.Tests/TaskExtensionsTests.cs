using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Shouldly;
using Xunit;

namespace ConfigLimitFixer.Tests;

public class TaskExtensionsTests : UnitTestsBase
{
    [Fact]
    public async Task TestWaitOneAsTaskSuccess()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);

        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;
        var state = this.Fixture.Create<string>();

        // Act

        var task = waitHandle.WaitOneAsTask(
            resultSelector: x => x,
            state: state,
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeFalse();

        // Act
        manualResetEvent.Set();
        var result = await task;

        // Assert
        task.Status.ShouldBe(TaskStatus.RanToCompletion);
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeTrue();

        waitHandle.WaitOne(0).ShouldBeTrue();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeFalse();

        result.ShouldBe(state);
    }

    [Fact]
    public async Task TestWaitOneAsTaskCanceled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);

        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;
        var asyncState = this.Fixture.Create<string>();

        // Act

        var task = waitHandle.WaitOneAsTask(
            resultSelector: x => x,
            state: asyncState,
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeFalse();

        // Act
        cts.Cancel();
        var ex = await Should.ThrowAsync<OperationCanceledException>(task);

        // Assert
        task.IsCanceled.ShouldBeTrue();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeFalse();
        task.Status.ShouldBe(TaskStatus.Canceled);

        waitHandle.WaitOne(0).ShouldBeFalse();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeTrue();

        ex.CancellationToken.ShouldBe(cancellationToken);
    }

    [Fact]
    public async Task TestWaitOneAsTaskFaulted()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);

        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;
        var asyncState = this.Fixture.Create<string>();

        // Act

        var task = waitHandle.WaitOneAsTask(
            resultSelector: x => (null as string) ?? throw new Exception(),
            state: asyncState,
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeFalse();

        // Act
        manualResetEvent.Set();
        await Should.ThrowAsync<Exception>(task);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeTrue();
        task.Status.ShouldBe(TaskStatus.Faulted);
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeTrue();
    }

    [Fact]
    public async Task TestWaitOneAsTaskAsyncWithResultSuccess()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);

        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;
        var state = this.Fixture.Create<string>();

        // Act

        var task = waitHandle.WaitOneAsTaskAsync(
            result: state,
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeFalse();

        // Act
        manualResetEvent.Set();
        var result = await task;

        // Assert
        task.Status.ShouldBe(TaskStatus.RanToCompletion);
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeTrue();

        waitHandle.WaitOne(0).ShouldBeTrue();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeFalse();

        result.ShouldBe(state);
    }

    [Fact]
    public async Task TestWaitOneAsTaskAsyncWithResultCanceled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);

        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;
        var result = this.Fixture.Create<string>();

        // Act

        var task = waitHandle.WaitOneAsTaskAsync(
            result: result,
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeFalse();

        // Act
        cts.Cancel();
        var ex = await Should.ThrowAsync<OperationCanceledException>(task);

        // Assert
        task.IsCanceled.ShouldBeTrue();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeFalse();
        task.Status.ShouldBe(TaskStatus.Canceled);

        waitHandle.WaitOne(0).ShouldBeFalse();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeTrue();

        ex.CancellationToken.ShouldBe(cancellationToken);
    }

    [Fact]
    public async Task TestWaitOneAsTaskAsyncWithResultSelectorSuccess()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);

        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;
        var state = this.Fixture.Create<string>();

        // Act

        var task = waitHandle.WaitOneAsTaskAsync(
            resultSelector: () => state,
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeFalse();

        // Act
        manualResetEvent.Set();
        var result = await task;

        // Assert
        task.Status.ShouldBe(TaskStatus.RanToCompletion);
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeTrue();

        waitHandle.WaitOne(0).ShouldBeTrue();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeFalse();

        result.ShouldBe(state);
    }

    [Fact]
    public async Task TestWaitOneAsTaskAsyncWithResultSelectorCanceled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);

        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;
        var state = this.Fixture.Create<string>();

        // Act

        var task = waitHandle.WaitOneAsTaskAsync(
            resultSelector: () => state,
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeFalse();

        // Act
        cts.Cancel();
        var ex = await Should.ThrowAsync<OperationCanceledException>(task);

        // Assert
        task.IsCanceled.ShouldBeTrue();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeFalse();
        task.Status.ShouldBe(TaskStatus.Canceled);

        waitHandle.WaitOne(0).ShouldBeFalse();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeTrue();

        ex.CancellationToken.ShouldBe(cancellationToken);
    }

    [Fact]
    public async Task TestWaitOneAsTaskAsyncWithResultSelectorFaulted()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);

        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;

        // Act

        var task = waitHandle.WaitOneAsTaskAsync(
            resultSelector: () => (null as string) ?? throw new Exception(),
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();

        waitHandle.WaitOne(0).ShouldBeFalse();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeFalse();

        manualResetEvent.Set();
        await Should.ThrowAsync<Exception>(task);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeTrue();
        task.Status.ShouldBe(TaskStatus.Faulted);

        waitHandle.WaitOne(0).ShouldBeTrue();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeFalse();
    }

    [Fact]
    public async Task TestWaitOneAsTaskAsyncWithNoResultSuccess()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);
        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;

        // Act

        var task = waitHandle.WaitOneAsTaskAsync(
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeFalse();

        waitHandle.WaitOne(0).ShouldBeFalse();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeFalse();

        // Act
        manualResetEvent.Set();
        await task;

        // Assert
        task.Status.ShouldBe(TaskStatus.RanToCompletion);
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeFalse();
        manualResetEvent.IsSet.ShouldBeTrue();

        waitHandle.WaitOne(0).ShouldBeTrue();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeFalse();
    }

    [Fact]
    public async Task TestWaitOneAsTaskAsyncWithNoResultCanceled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        using var manualResetEvent = new ManualResetEventSlim(false);
        var waitHandle = manualResetEvent.WaitHandle;
        var cancellationToken = cts.Token;

        // Act

        var task = waitHandle.WaitOneAsTaskAsync(
            cancellationToken: cancellationToken);

        // Assert
        task.IsCanceled.ShouldBeFalse();
        task.IsCompleted.ShouldBeFalse();
        task.IsFaulted.ShouldBeFalse();

        waitHandle.WaitOne(0).ShouldBeFalse();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeFalse();

        // Act
        cts.Cancel();
        var ex = await Should.ThrowAsync<OperationCanceledException>(task);

        // Assert
        task.IsCanceled.ShouldBeTrue();
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeFalse();
        task.Status.ShouldBe(TaskStatus.Canceled);

        waitHandle.WaitOne(0).ShouldBeFalse();
        cancellationToken.WaitHandle.WaitOne(0).ShouldBeTrue();

        ex.CancellationToken.ShouldBe(cancellationToken);
    }
}
