<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//Mutex类可以创建一个互斥锁，用于确保同一时间只有一个线程可以访问受保护的资源。
//Mutex 的使用与Monitor类似。使用WaitOne请求所有权，ReleaseMutex 释放所有权


// 创建一个 Mutex 对象
Mutex mutex = new Mutex();

 // 共享资源
int sharedResource = 0;

// 创建并启动三个线程
for (int i = 0; i < 3; i++)
{
	Thread thread = new Thread(WorkOnSharedResource);
	thread.Name = $"Thread {i + 1}";
	thread.Start();
}

void WorkOnSharedResource()
{
	for (int i = 0; i < 5; i++)
	{
		UseResource();
		Thread.Sleep(1000); // 模拟其他工作
	}
}

void UseResource()
{
	Console.WriteLine($"{Thread.CurrentThread.Name} is requesting the mutex");

	mutex.WaitOne(); // 请求 Mutex

	try
	{
		Console.WriteLine($"{Thread.CurrentThread.Name} has entered the critical section");

		// 访问共享资源
		sharedResource++;
		Console.WriteLine($"{Thread.CurrentThread.Name} has incremented shared resource to {sharedResource}");

		// 模拟一些工作
		Thread.Sleep(2000);
	}
	finally
	{
		mutex.ReleaseMutex(); // 释放 Mutex
		Console.WriteLine($"{Thread.CurrentThread.Name} has left the critical section");
	}
}
