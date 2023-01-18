<Query Kind="Statements">
  <NuGetReference>Castle.Core</NuGetReference>
  <Namespace>Castle.DynamicProxy</Namespace>
</Query>



var proxy = new ProxyGenerator().CreateInterfaceProxyWithoutTarget<IPerson>(new Interceptor());
proxy.FirstName = "Scooby";
proxy.LastName = "Doo";

public interface IPerson
{
	string FirstName { get; set; }
	string LastName { get; set; }
}

public class Interceptor : IInterceptor
{
	public void Intercept(IInvocation invocation)
	{
		Console.WriteLine($"Before target call {invocation.Method.Name}");
		try
		{
			//invocation.Proceed();
		}
		catch (Exception e)
		{
			Console.WriteLine($"Target exception {e.Message}");
			throw;
		}
		finally
		{
			Console.WriteLine($"After target call {invocation.Method.Name}");
		}
	}
}