<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.0.3">UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
  <Namespace>System.Activities.Statements</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <RemoveNamespace>System.Diagnostics</RemoveNamespace>
</Query>

// 设置依赖注入  
var services = new ServiceCollection();
services.AddSingleton<IMyService, MyService>();
var serviceProvider = services.BuildServiceProvider();

var dependencyInjectActivity = new Sequence
{
	Activities =
	{
		new MyActivity()
	}
};

var workflowInvoker = new WorkflowInvoker(dependencyInjectActivity);
workflowInvoker.Extensions.Add (new DependencyInjectExtension(serviceProvider));
workflowInvoker.Invoke();

public class MyActivity : CodeActivity
{
	protected override void Execute(CodeActivityContext context)
	{
		var extension = context.GetExtension<DependencyInjectExtension>();
		var serviceProvider = extension.GetServiceProvider();
		// 使用注入的服务  
		var myService = serviceProvider.GetService<IMyService>();
		
		myService.DoSomething();
	}
}

public class DependencyInjectExtension 
{
	private readonly IServiceProvider _serviceProvider;

	public DependencyInjectExtension(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IServiceProvider GetServiceProvider()
	{
		return _serviceProvider;
	}
}

public interface IMyService 
{ 	
	void DoSomething();
}

public class MyService : IMyService
{
	public void DoSomething()
	{
		"MyService DoSomething".Dump();
	}
}