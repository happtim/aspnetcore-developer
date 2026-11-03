<Query Kind="Statements">
  <NuGetReference Version="7.0.0">System.Threading.RateLimiting</NuGetReference>
  <Namespace>System.Threading.RateLimiting</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>



var calculator = new RateLimitedCalculator();

for (int i = 0; i < 1000; i++)
{
	try
	{
		var result = await calculator.CalculateAsync(async () =>
		{
			Console.WriteLine($"Calculating at {DateTime.Now}");
			return i * i;
		});

		Console.WriteLine($"Result: {result}");
		
	}
	catch (Exception)
	{
		Console.WriteLine("Request rejected due to rate limiting");
	}
	await Task.Delay(100); // 模拟一些计算工作
}

public class RateLimitedCalculator
{
	private readonly FixedWindowRateLimiter _limiter;

	public RateLimitedCalculator()
	{
		// 创建一个固定时间窗口的限流器,每秒允许1个请求
		_limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
		{
			AutoReplenishment = true,//用于确定是否在每个新的时间窗口开始时自动重置许可数量。
			PermitLimit = 1,// 在每个时间窗口内允许的最大请求或操作数。
			QueueProcessingOrder = QueueProcessingOrder.NewestFirst ,//当达到限制时,新请求的处理顺序。
			QueueLimit = 0,//当达到速率限制时,允许等待的最大请求数。
			Window = TimeSpan.FromSeconds(1)
		});
	}

	public async Task<T> CalculateAsync<T>(Func<Task<T>> calculation)
	{
		using RateLimitLease lease = await _limiter.AcquireAsync();
		if (lease.IsAcquired)
		{
			return await calculation();
		}
		else
		{
			throw new Exception("Rate limit exceeded");
		}
	}
}