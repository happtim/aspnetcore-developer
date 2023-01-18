<Query Kind="Statements">
  <NuGetReference>Castle.Core</NuGetReference>
  <Namespace>Castle.DynamicProxy</Namespace>
</Query>



//基于组合的代理(Composition-based)是一个新对象，它继承自代理类/实现代理接口，并（可选）将截获的调用转发到目标对象。

//具有目标的类代理 - 此代理类型以类为目标。它不是一个完美的代理，
//代理只代理类的虚拟方法，拦截虚拟方法并直接调用 target 类的对应方法。所以target类的虚拟方法内还有虚拟方法，这个方法将无法被拦截。
//从而使代理的用户对对象状态的视图不一致。因此，应谨慎使用。

var rocket = new Rocket();

var proxy = new ProxyGenerator().CreateClassProxyWithTarget<Rocket>(rocket,new LoggingInterceptor());

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