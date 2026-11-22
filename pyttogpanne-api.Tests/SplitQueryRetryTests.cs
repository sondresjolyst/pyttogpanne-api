using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Infrastructure;
using Xunit;

namespace pyttogpanne_api.Tests;

public class SplitQueryRetryTests
{
    [Fact]
    public async Task Success_RunsOnce()
    {
        var calls = 0;
        var result = await SplitQueryRetry.ExecuteAsync(() => { calls++; return Task.FromResult(42); });
        Assert.Equal(42, result);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task ConcurrentChange_RetriesOnceAndReturnsSecondResult()
    {
        var calls = 0;
        var result = await SplitQueryRetry.ExecuteAsync(() =>
        {
            calls++;
            if (calls == 1) throw new DbQueryConcurrencyException("changed");
            return Task.FromResult(7);
        });
        Assert.Equal(7, result);
        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task ConcurrentChangeTwice_Throws()
    {
        var calls = 0;
        await Assert.ThrowsAsync<DbQueryConcurrencyException>(() => SplitQueryRetry.ExecuteAsync<int>(() =>
        {
            calls++;
            throw new DbQueryConcurrencyException("changed");
        }));
        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task OtherException_IsNotRetried()
    {
        var calls = 0;
        await Assert.ThrowsAsync<InvalidOperationException>(() => SplitQueryRetry.ExecuteAsync<int>(() =>
        {
            calls++;
            throw new InvalidOperationException("boom");
        }));
        Assert.Equal(1, calls);
    }
}
