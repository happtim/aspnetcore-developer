<Query Kind="Program" />

// ReaderWriterLockSlim 再.net framework 3.5 用来替换 ReaderWriterLock的。
// ReaderWriterLock的问题
// 1. 他比  Monitor.Enter 独占锁的更慢的时间。
// 2. ReaderWriterLock reader线程的优先级高于writer线程。


//使用ReaderWriterLockSlim 保护由多个线程读取并由一个线程写入的资源 一次线程。
//ReaderWriterLockSlim允许多个线程处于读取模式，允许一个线程处于写入状态 具有锁独占所有权的模式，
//并允许一个线程已读取 访问可升级的读取模式，线程可以从该模式升级到 写入模式，无需放弃对资源的读取访问权限。

//线程可以通过三种模式进入锁。
//
//Read mode
//Write mode
//upgradeable read mode


static ReaderWriterLockSlim rw = new ReaderWriterLockSlim();
static List<int> items = new List<int>();
static Random rand = new Random();

void Main()
{
	new Thread(Read).Start();
	new Thread(Read).Start();
	new Thread(Read).Start();
	new Thread(Write).Start("A");
	new Thread(Write).Start("B");
	new Thread(Write).Start("C");
	Console.Read();
}

static void Read()
{
	while (true)
	{
		rw.EnterReadLock();
		foreach (int i in items)
		{ 
			Thread.Sleep(10);
		}
		rw.ExitReadLock();
	}
}
static void Write(object threadID)
{
	while (true)
	{
		int newNumber = GetRandNum(50);
		rw.EnterWriteLock();
		items.Add(newNumber);
		rw.ExitWriteLock();
		Console.WriteLine("Thread " + threadID + " added " + newNumber);
		Thread.Sleep(100);
	}
}
static int GetRandNum(int max)
{
	lock (rand)
	{
		return rand.Next(max);
	}
}
