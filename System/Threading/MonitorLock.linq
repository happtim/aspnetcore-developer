<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


//创建锁对象
object oo = new object();

Task.Run(() => 
{
	Task.Delay(10);
	//尝试获取锁时间
	Monitor.TryEnter(oo,5000).Dump();
});

//获取锁
lock (oo)
{
	Thread.Sleep(4000);
}