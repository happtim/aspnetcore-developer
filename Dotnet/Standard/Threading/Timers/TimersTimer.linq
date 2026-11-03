<Query Kind="Statements">
  <Namespace>System.Timers</Namespace>
</Query>

//System.Timers.Timer
//https://learn.microsoft.com/zh-cn/dotnet/standard/threading/timers#the-systemthreadingperiodictimer-class

//设计为高级的计时器，提供更多功能和灵活性。
//提供事件驱动模型（与ThreadingTimer区别是后者回调），适合需要在特定时间间隔触发事件的应用。


System.Timers.Timer timer = new System.Timers.Timer(2000);
timer.Elapsed += Timer_Elapsed;
timer.Start();

Console.WriteLine("按下任意键退出...");
Console.Read();

// 停止定时器
timer.Dispose();

static void Timer_Elapsed(object sender, ElapsedEventArgs e)
{
	Console.WriteLine($"Timer执行线程: {Thread.CurrentThread.ManagedThreadId.ToString()}, 当前时间: {DateTime.Now}");

	// 模拟一些耗时操作，如果线程没有完成，则从线程池中新开一个线程
	Thread.Sleep(3000);
}
