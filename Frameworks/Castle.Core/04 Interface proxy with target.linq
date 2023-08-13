<Query Kind="Statements">
  <NuGetReference Version="5.1.1">Castle.Core</NuGetReference>
  <Namespace>Castle.DynamicProxy</Namespace>
</Query>


// 顾名思义，包装对象实现选定接口，将对这些接口的调用转发到目标对象。

var rocket = new Rocket();

var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget<IRocket>(rocket, new LoggingInterceptor());
proxy.Launch(5);

public interface IRocket
{
	void Launch(int delaySeconds);
	void Countdown(int delaySeconds);
}

public class Rocket : IRocket
{
	public string Name { get; set; }
	public string Model { get; set; }

	public virtual void Launch(int delaySeconds)
	{
		Console.WriteLine(string.Format("Launching rocket in {0} seconds", delaySeconds));
		Countdown(delaySeconds);
		Congratulation();
	}

	//为类创建基于继承的代理。只能截获类的虚拟成员。不能拦截公用方法。
	public void Congratulation()
	{
		Console.WriteLine("Congratulations! You have successfully launched the rocket.");
	}

	public virtual void Countdown(int delaySeconds)
	{
		for (int i = delaySeconds; i > 0; i--)
		{
			Console.WriteLine($"* remaining {i} *");
			Thread.Sleep(1000);
		}
	}
}

public class LoggingInterceptor : IInterceptor
{
	public void Intercept(IInvocation invocation)
	{
		var methodName = invocation.Method.Name;
		try
		{

			Util.Highlight(string.Format("\t Log: Inter Method:{0}, Arguments: {1}", methodName, string.Join(",", invocation.Arguments))).Dump();
			invocation.Proceed();
			Util.Highlight(string.Format("\t Log: Exit Method:{0}", methodName)).Dump();
		}
		catch (Exception e)
		{
			Console.WriteLine(string.Format("Method:{0}, Exception:{1}", methodName, e.Message));
			throw;
		}
		finally
		{
			//Console.WriteLine(string.Format("Exiting Method:{0}", methodName));
		}
	}
}