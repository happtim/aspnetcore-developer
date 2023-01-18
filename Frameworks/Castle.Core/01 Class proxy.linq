<Query Kind="Statements">
  <NuGetReference>Castle.Core</NuGetReference>
  <Namespace>Castle.DynamicProxy</Namespace>
</Query>

// Aspect - object oriented programming (AOP)是一种编程范式，旨在通过允许横切和分离关注点来提高模块化。
//aspect 是通常分散在方法、类和对象的层次结构中的常用函数。它看起来它的行为有结构，但没有办法使用传统的面向对象技术来表达它。
//一个很好的例子是日志记录。通常，您在代码库中编写信息丰富的日志，但日志记录是您的类或对象模型真正不应该关心的事情，因为它不表示域对象。
//使用 AOP 方法，我们可以创建这些交叉聚焦的方面，并使用各种技术将它们集中在域对象上。
//我将介绍使用Castel Windsor框架进行动态创建和应用的过程。

//DynamicProxy（简称DP）顾名思义，是一个框架，可以帮助您实现代理对象设计模式。这是代理部分。
//动态部分意味着代理类型的实际创建发生在运行时，您可以动态地组合代理对象。

//基于继承的代理(Inheritance-based)是通过继承代理类创建的。代理截获对类的虚拟成员的调用，并将其转发到基本实现。
//在这种情况下，代理对象和代理对象是一个对象。这也意味着您无法为预先存在的对象创建基于继承的代理。
//DynamicProxy中有一种基于继承的代理类型。

//为类创建基于继承的代理。只能截获类的虚拟成员。

var rocket = new ProxyGenerator().CreateClassProxy<Rocket>(new LoggingInterceptor());
rocket.Dump();
rocket.Launch(5); 

public interface IRocket
{
	void Launch(int delaySeconds);
	void Countdown (int delaySeconds);
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
		for (int i = delaySeconds; i > 0 ; i--)
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
			
			Util.Highlight (string.Format("\t Log: Inter Method:{0}, Arguments: {1}", methodName, string.Join(",", invocation.Arguments))).Dump();
			invocation.Proceed();
			Util.Highlight (string.Format("\t Log: Exit Method:{0}", methodName)).Dump();
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