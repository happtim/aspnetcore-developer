<Query Kind="Statements" />

//Mutex 在进程之间的协调访问空间。



// 定义一个全局 Mutex 名称
string MutexName = "Global\\MyUniqueMutexName";

bool createdNew;

//当你创建 Mutex 时，你使用了 new Mutex(true, MutexName, out createdNew)。
//这里的第一个参数 true 表示创建 Mutex 时立即请求其所有权。
//createdNew 参数会告诉我们是否成功创建了新的 Mutex。

// 尝试创建或打开一个命名 Mutex
using (Mutex mutex = new Mutex(true, MutexName, out createdNew))
{
	if (createdNew)
	{
		Console.WriteLine("This is the first instance of the application.");

		try
		{
			// 模拟一些工作
			Console.WriteLine("Application is running. Press Enter to exit.");
			Console.ReadLine();
		}
		finally
		{
			// 确保在应用程序退出时释放 Mutex
			mutex.ReleaseMutex();
		}
	}
	else
	{
		Console.WriteLine("Another instance of the application is already running.");
		Console.WriteLine("Press Enter to exit.");
		Console.ReadLine();
	}
}