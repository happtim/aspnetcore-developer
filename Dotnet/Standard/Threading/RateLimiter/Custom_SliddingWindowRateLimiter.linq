<Query Kind="Statements">
  <Namespace>System.Threading.Channels</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//https://www.cnblogs.com/artech/p/window-based-rate-limiter.html

var limiter = new SliddingWindowRateLimiter(TimeSpan.FromSeconds(2), 2);

var index = 0;
await Task.WhenAll(Enumerable.Range(1, 100).Select(_ => Task.Run(() =>
{
	while (true)
	{
		if (limiter.TryAcquire())
		{
			Console.WriteLine($"[{DateTimeOffset.Now}]{Interlocked.Increment(ref index)}");
		}
	}
})));


public sealed class SliddingWindowRateLimiter : IRateLimiter
{
	private readonly TimeSpan _window;
	private readonly ChannelReader<DateTimeOffset> _reader;
	private readonly ChannelWriter<DateTimeOffset> _writer;
	public SliddingWindowRateLimiter(TimeSpan window, int permit)
	{
		_window = window;
		var options = new BoundedChannelOptions(permit)
		{
			FullMode = BoundedChannelFullMode.Wait,
			SingleReader = false,
			SingleWriter = true
		};
		var channel = Channel.CreateBounded<DateTimeOffset>(options);
		_reader = channel.Reader;
		_writer = channel.Writer;
		Task.Factory.StartNew(Trim, TaskCreationOptions.LongRunning);
	}

	public bool TryAcquire() => _writer.TryWrite(DateTimeOffset.UtcNow);
	private void Trim()
	{
		if (!_reader.TryPeek(out var timestamp))
		{
			Task.Delay(_window).Wait();
			Trim();
		}
		else
		{
			var delay = _window - (DateTimeOffset.UtcNow - timestamp);
			if (delay > TimeSpan.Zero)
			{
				Task.Delay(delay).Wait();
				Trim();
			}
			else
			{
				var valueTask = _reader.ReadAsync();
				if (!valueTask.IsCompleted) _ = valueTask.Result;
				Trim();
			}
		}
	}
}

public interface IRateLimiter
{
	bool TryAcquire();
}