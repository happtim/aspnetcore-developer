<Query Kind="Statements">
  <Namespace>System.Timers</Namespace>
</Query>

//定时器精度
//可以看出来 1s中的执行次数大概60多次。最小时间间隔为系统时间片长度，通常为15.6毫秒。

var timer = new TimerAccuracy();
timer.Start();

Thread.Sleep(3000);

timer.Dump();


class TimerAccuracy : IDisposable
{
	// 创建一个 Timer 实例
	System.Timers.Timer _timer ;
		public Dictionary<int,int> times = new Dictionary<int, int>();
	
	public TimerAccuracy()
	{
		_timer = new System.Timers.Timer();
		// 设置定时器间隔为1毫秒
		_timer.Interval = 1;

		// 添加定时器的 Elapsed 事件处理程序
		_timer.Elapsed += TimerElapsedHandler;
	}

	public void Start()
	{
		// 启动定时器
		_timer.Start();
	}

	private void TimerElapsedHandler(object sender, ElapsedEventArgs e)
	{
		if (!times.ContainsKey(DateTime.Now.Second))
		{
			times[DateTime.Now.Second] = 0;
		}
		else
		{
			times[DateTime.Now.Second]++;
		}
	}


	public void Dispose()
	{
		// 停止定时器
		_timer.Stop();
		_timer.Dispose();

	}
}



