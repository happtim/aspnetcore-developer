<Query Kind="Statements" />


// 预先加载
public class PreloadSingleton
{
	public static PreloadSingleton instance = new PreloadSingleton();

	private PreloadSingleton() {}
	
	public static PreloadSingleton GetInstance(){
		return instance;
	}
}


//懒加载模式

public class LazySingleton
{
	private static LazySingleton instance= null;
	private LazySingleton(){}

	public static LazySingleton GetInstance()
	{
		if (instance == null) 
		{
			instance = new LazySingleton();
		}
		
		return instance;
	}
}

//线程安全的懒加载

public class SafeLazySingleton
{
	private static volatile SafeLazySingleton instance = null;
	private SafeLazySingleton() { }
	private static object locker = new Object();

	public static SafeLazySingleton GetInstance()
	{
		if (instance == null)
		{
			lock(locker)
			{
				if (instance == null)
				{
					instance = new SafeLazySingleton();
				}
			}
		}

		return instance;
	}
}
