<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// 获取线程池中的工作线程和IO线程
ThreadPool.GetAvailableThreads(out int workerThreads, out int ioThreads);
ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIoThreads);
ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIoThreads);

Console.WriteLine($"Available Worker Threads: {workerThreads} Available IO Threads: {ioThreads}");
Console.WriteLine($"Max Worker Threads: {maxWorkerThreads} Max IO Threads: {maxIoThreads}");
Console.WriteLine($"Min Worker Threads: {minWorkerThreads} Min IO Threads: {minIoThreads}");

// 模拟任务来占用线程池线程
for (int i = 0; i < 10; i++)
{
	ThreadPool.QueueUserWorkItem(Work);
}

// 等待一段时间以便检查线程池状态
Thread.Sleep(1000);

// 再次获取并打印线程池状态
ThreadPool.GetAvailableThreads(out  workerThreads, out ioThreads);
ThreadPool.GetMaxThreads(out  maxWorkerThreads, out  maxIoThreads);
ThreadPool.GetMinThreads(out minWorkerThreads, out minIoThreads);
Console.WriteLine($"Available Worker Threads: {workerThreads} Available IO Threads: {ioThreads}");
Console.WriteLine($"Max Worker Threads: {maxWorkerThreads} Max IO Threads: {maxIoThreads}");
Console.WriteLine($"Min Worker Threads: {minWorkerThreads} Min IO Threads: {minIoThreads}");

static void Work(object state)
{
	Console.WriteLine("Working...");
	Thread.Sleep(2000); // 模拟一些工作
}