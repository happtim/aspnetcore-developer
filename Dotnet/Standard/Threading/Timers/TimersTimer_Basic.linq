<Query Kind="Statements">
  <Namespace>System.Timers</Namespace>
</Query>

//使用一个单独的线程执行回调方法，不受线程池状态的影响，但需要额外的线程开销。
//当一个任务没有完成时，新的任务不会重新调用。

// 创建一个 Timer 实例
System.Timers.Timer timer = new System.Timers.Timer();

// 设置定时器间隔为1秒
timer.Interval = 2000;

// 添加定时器的 Elapsed 事件处理程序
timer.Elapsed += TimerElapsedHandler;

// 启动定时器
timer.Start();

Console.WriteLine("按下任意键退出...");
Console.Read();

// 停止定时器
timer.Stop();
timer.Dispose();

static void TimerElapsedHandler(object sender, ElapsedEventArgs e)
{
	Console.WriteLine($"Timer执行线程: {Thread.CurrentThread.ManagedThreadId.ToString()}, 当前时间: {DateTime.Now}");

	// 模拟一些耗时操作，如果线程没有完成，则从线程池中新开一个线程
	Thread.Sleep(3000);
}