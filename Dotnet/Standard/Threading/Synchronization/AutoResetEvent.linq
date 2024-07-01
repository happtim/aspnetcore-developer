<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//AutoResetEvent是C#中用于线程同步的类之一。它允许一个线程通知另一个线程某个事件已经发生。

//initialState:
// 这是一个布尔值，决定 AutoResetEvent 的初始状态。
// 如果为 true，则初始状态为有信号（signaled）。
// 如果为 false，则初始状态为无信号（non - signaled）。
// 
// 当状态为有信号时（true），第一个调用 WaitOne() 的线程会立即通过，然后 AutoResetEvent 自动重置为无信号状态。
// 当状态为无信号时（false），调用 WaitOne() 的线程会被阻塞，直到其他线程调用 Set() 方法。

//主要方法：
//WaitOne():
// 如果 AutoResetEvent 处于有信号状态，调用线程会立即通过，并且事件自动重置为无信号状态。
// 如果处于无信号状态，调用线程会被阻塞，直到事件被设置为有信号。

//Set():
// 将 AutoResetEvent 设置为有信号状态。
// 如果有等待的线程，会释放其中一个线程，然后自动重置为无信号状态。

//Reset():
//将 AutoResetEvent 设置为无信号状态。


AutoResetEvent autoResetEvent = new AutoResetEvent(false); //初始状态没有信号

var task =  Task.Run(() => {

	"工作线程开始等待".Dump();
	// 等待AutoResetEvent信号
	autoResetEvent.WaitOne();
	
	"工作线程等待到信号，开始工作".Dump();
	
	Thread.Sleep(2000);
	
	autoResetEvent.Set();
});

"主线程睡2s".Dump();

Thread.Sleep(2000);

"主线程给工作线程发送信号".Dump();

// 发送AutoResetEvent信号，唤醒等待的工作线程
autoResetEvent.Set();

autoResetEvent.WaitOne();

"主线程等待工作线程完成了".Dump();

Task.WaitAll(task);
