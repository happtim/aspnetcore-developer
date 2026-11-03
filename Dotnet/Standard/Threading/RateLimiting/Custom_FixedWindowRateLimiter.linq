<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//https://www.cnblogs.com/artech/p/window-based-rate-limiter.html

IRateLimiter limiter = new FixedWindowRateLimiter(window: TimeSpan.FromSeconds(1), permit: 1);

var index = 0;
await Task.WhenAll(Enumerable.Range(1, 100).Select(_ => Task.Run( async () =>
{
	while (true)
	{
		if (limiter.TryAcquire())
		{
			Console.WriteLine($"[{DateTimeOffset.Now}]{Interlocked.Increment(ref index)}");
		}
		else
		{
			//Console.WriteLine("Request rejected due to rate limiting");
		}
		
		await Task.Delay(100);
	}
})));

public sealed class FixedWindowRateLimiter : IRateLimiter
{
	private readonly long _windowTicks;
	private readonly int _permit;
	private long _nextWindowStartTimeTicks;
	private volatile int _count = 0;

	public FixedWindowRateLimiter(TimeSpan window, int permit)
	{
		_windowTicks = window.Ticks;
		_permit = permit;
		_nextWindowStartTimeTicks = DateTimeOffset.UtcNow.Add(window).Ticks;
	}

	public bool TryAcquire()
	{
		// 超出时间窗口，重置计数器，并调整下一个时间窗口的开始时间
		var now = DateTimeOffset.UtcNow.Ticks;
		var nextWindowStartTimeTicks = _nextWindowStartTimeTicks;
		if (now >= nextWindowStartTimeTicks && Interlocked.CompareExchange(ref _nextWindowStartTimeTicks, now + _windowTicks, nextWindowStartTimeTicks) == nextWindowStartTimeTicks)
		{
			Interlocked.Exchange(ref _count, 1);
			return true;
		}
		return _count < _permit && Interlocked.Increment(ref _count) <= _permit;
	}
}

public interface IRateLimiter
{
	bool TryAcquire();
}