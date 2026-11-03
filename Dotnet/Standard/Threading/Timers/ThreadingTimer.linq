<Query Kind="Statements" />

//System.Threading.Timer 类
//设计为轻量级的计时器，用于在指定的时间间隔后执行回调方法。

//使用线程池线程执行回调方法，因此回调方法的执行速度取决于线程池的状态。如果线程池繁忙，可能会影响回调方法的执行。
//当之前的定时器还未完成，新的触发时间到了之后，从线程中新分配一个线程执行新的回调。

// 创建一个 TimerCallback 委托
TimerCallback timerCallback = new TimerCallback(TimerElapsedHandler);

// 创建一个 Timer 实例,初始延迟为0,每隔2秒执行一次回调方法
var _timer = new Timer(timerCallback, null, 0, 2000);

Console.WriteLine("按下任意键退出...");
Console.Read();

// 停止定时器
_timer.Dispose();


void TimerElapsedHandler(object state)
{
	Console.WriteLine($"Timer执行线程: {Thread.CurrentThread.ManagedThreadId.ToString()}, 当前时间: {DateTime.Now}");

	// 模拟一些耗时操作，如果线程没有完成，则从线程池中新开一个线程
	Thread.Sleep(3000);
}