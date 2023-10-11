<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


//initialCount：信号量的初始计数。默认值为1。
//maxCount：信号量的最大计数。该值指定了信号量的最大计数，即同时允许访问的线程数量的上限。
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
