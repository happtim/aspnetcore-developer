<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//针对这种大多数情况下是单线程访问，偶尔是多线程访问共享资源的情况，我们可以设计一个轻量级的锁。

var hybridLock = new SimpleHybirdLock();
int sharedData = 0;

// 多线程访问
for (int i = 0; i < 100; i++)
{
	Task.Run(() => AccessSharedResource());
}

void AccessSharedResource()
{
	
	try
	{	
		hybridLock.Enter();
		// 访问或修改共享资源
		sharedData++;
		Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} accessed shared resource. Value: {sharedData}");
	}
	finally
	{
		hybridLock.Leave();
	}
}


//
// Summary:
//     一个简单的混合线程同步锁，采用了基元用户加基元内核同步构造实现
public sealed class SimpleHybirdLock : IDisposable
{
	private bool disposedValue = false;

	//
	// Summary:
	//     基元用户模式构造同步锁
	private int m_waiters = 0;

	//
	// Summary:
	//     基元内核模式构造同步锁
	private AutoResetEvent m_waiterLock = new AutoResetEvent(initialState: false);

	//
	// Summary:
	//     获取当前锁是否在等待当中
	public bool IsWaitting => m_waiters != 0;

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}

			m_waiterLock.Close();
			disposedValue = true;
		}
	}

	//
	// Summary:
	//     释放资源
	public void Dispose()
	{
		Dispose(disposing: true);
	}

	//
	// Summary:
	//     获取锁
	public void Enter()
	{
		if (Interlocked.Increment(ref m_waiters) != 1)
		{
			m_waiterLock.WaitOne();
		}
	}

	//
	// Summary:
	//     离开锁
	public void Leave()
	{
		if (Interlocked.Decrement(ref m_waiters) != 0)
		{
			m_waiterLock.Set();
		}
	}
}