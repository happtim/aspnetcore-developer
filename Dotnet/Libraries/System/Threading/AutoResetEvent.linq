<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//AutoResetEvent是C#中用于线程同步的类之一。它允许线程之间的互斥和通信。
//当一个线程等待一个AutoResetEvent信号时，它将被阻塞，直到另一个线程通过发送信号来释放它。

//初始状态没有信号
AutoResetEvent autoResetEvent = new AutoResetEvent(false);

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
