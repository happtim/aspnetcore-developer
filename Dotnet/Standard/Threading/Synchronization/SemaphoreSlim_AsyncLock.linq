<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//SemaphoreSlim 是 .NET 中的一个轻量级同步原语，用于限制可以同时访问资源或执行特定代码段的线程数量。

//initialCount：信号量的初始计数。
// 这是信号量初始化时允许的并发访问数。
// 必须大于或等于 0，且不能大于 maxCount。
// 如果设为 0，则开始时所有线程都会被阻塞。

//maxCount：
// 信号量的最大计数。该值指定了信号量的最大计数，即同时允许访问的线程数量的上限。
var asyncLock = new SemaphoreSlim(1, 1);
int count = 0;
List<Task> tasks = new List<Task>();

for (int i = 0; i < 5000; i++)
{
	var task = Task.Run( async () => await myFunAsync());
	tasks.Add(task);
}

await Task.WhenAll(tasks.ToArray());

Console.WriteLine("所有任务执行完成，实际运行:"+count);

async Task myFunAsync()
{
	await asyncLock.WaitAsync();

	try
	{
		count ++;
	}
	finally
	{
		asyncLock.Release();
	}
}
