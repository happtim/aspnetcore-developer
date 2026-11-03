<Query Kind="Statements" />


//定时器精度
//可以看出来 1s中的执行次数大概60多次。最小时间间隔为系统时间片长度，通常为15.6毫秒。
var timer =  new TimerAccuracy();
timer.Start();

Thread.Sleep(3000);

timer.Dump();


class TimerAccuracy:IDisposable
{
	Timer _timer;
	public Dictionary<int,int> times = new Dictionary<int, int>();
	
	public void Start()
	{
		// 创建一个 TimerCallback 委托
		TimerCallback timerCallback = new TimerCallback(TimerElapsedHandler);
		// 创建一个 Timer 实例,初始延迟为0,每隔1毫秒执行一次回调方法
		_timer = new Timer(timerCallback, null, 0, 1);
	}

	private void TimerElapsedHandler(object state)
	{
		if (!times.ContainsKey(DateTime.Now.Second))
		{
			times[DateTime.Now.Second] = 0;
		}
		else
		{
			times[DateTime.Now.Second] ++;
		}
		
	}

	public void Dispose()
	{
		// 停止定时器
		_timer.Dispose();
	}
}

