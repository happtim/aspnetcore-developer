<Query Kind="Statements">
  <NuGetReference Version="5.1.1">Castle.Core</NuGetReference>
  <Namespace>Castle.DynamicProxy</Namespace>
</Query>



//此代理类型以接口为目标。它不需要提供目标对象。相反，拦截器应为代理的所有成员提供实现。
//接口本身是不可能存在的。这是一个定义其实现者可以做什么的合同。因此，您确实需要一个实施者。
//但是，如果创建没有目标的接口代理，则无需提供实现者。动态代理会为您创建它。这是非常强大的。

//出于各种原因，有时您不想创建新类来实现接口。

Action<int> Launch = (int delaySeconds) =>
{
	{
		Console.WriteLine(string.Format("Launching rocket in {0} seconds", delaySeconds));
		for (int i = delaySeconds; i > 0; i--)
		{
			Console.WriteLine($"* remaining {i} *");
			Thread.Sleep(1000);
		}
		Console.WriteLine("Congratulations! You have successfully launched the rocket.");
	}
};

var proxy = new ProxyGenerator().CreateInterfaceProxyWithoutTarget<IRocket>(new MethodInterceptor(Launch));

proxy.Launch(5);


public interface IRocket
{
	void Launch(int delaySeconds);
}


public class MethodInterceptor : IInterceptor
{
	private readonly Delegate _impl;

	public MethodInterceptor(Delegate @delegate)
	{
		this._impl = @delegate;
	}

	public void Intercept(IInvocation invocation)
	{
		var result = this._impl.DynamicInvoke(invocation.Arguments);
		invocation.ReturnValue = result;
	}
}