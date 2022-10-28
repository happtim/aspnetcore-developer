<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{

	"开始".Dump();

	var source = new CancellationTokenSource(TimeSpan.FromSeconds(20));

	CancellationToken token = source.Token;
	token.ThrowIfCancellationRequested();

	try
	{
		var task = Enumerable.Range(0, 150).Select(i => Task.Run(() => LongTimeTask(i))).ToList();
		Task.WhenAll(task);

	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}

}

// You can define other methods, fields, classes and namespaces here

private static void LongTimeTask(int index)
{
	var threadId = Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(3, ' ');
	var line = index.ToString().PadLeft(3, ' ');
	Console.WriteLine($"[{line}] [{threadId}] [{DateTime.Now:ss.fff}] 异步任务已开始……");

	// 这一句才是关键，等待。其他代码只是为了输出。
	Thread.Sleep(5000);

	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine($"[{line}] [{threadId}] [{DateTime.Now:ss.fff}] 异步任务已结束……");
	Console.ForegroundColor = ConsoleColor.White;
}