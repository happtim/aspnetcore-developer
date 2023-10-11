<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//Mutex类可以创建一个互斥锁，用于确保同一时间只有一个线程可以访问受保护的资源。

Mutex mutex = new Mutex(true);

Task.Run(() =>
{
	Thread.Sleep(10);
	try
	{
		// 获取互斥锁的所有权
		mutex.WaitOne();

		"调用线程获取到互斥锁的所有权".Dump();
		
		Thread.Sleep(1000);
	}
	finally 
	{
		// 释放互斥锁
		mutex.ReleaseMutex();	
	}
	
});

Thread.Sleep(1000);

mutex.ReleaseMutex();

Thread.Sleep(10);

mutex.WaitOne();

"主线程获取到互斥锁的所有权".Dump();


