<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//Monitor类提供了互斥锁的实现，用于保护共享资源的同步访问。
//提供进入(Enter)离开(Exit)方法

//创建锁对象
object oo = new object();

Task.Run(() => 
{
	Task.Delay(10);
	//尝试获取锁时间
	var result = Monitor.TryEnter(oo,5000);
	if(result)
	{
		"Monitor获取锁成功".Dump();
	}
	else
	{
		"Monitor获取锁失败".Dump();
	}
	
	Monitor.Exit(oo);
	"Monitor释放锁".Dump();
});

//获取锁
lock (oo)
{
	Thread.Sleep(4000);
	"睡眠四秒完毕".Dump();
}