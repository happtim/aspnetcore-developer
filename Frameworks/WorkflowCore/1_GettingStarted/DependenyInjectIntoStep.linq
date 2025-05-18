<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <NuGetReference>WorkflowCore</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>WorkflowCore.Interface</Namespace>
  <Namespace>WorkflowCore.Models</Namespace>
  <Namespace>WorkflowCore.Services</Namespace>
</Query>

//如果您将Step类注册到 IoC 容器中，工作流主机将使用 IoC 容器来构造它们，从而注入任何所需的依赖项。

// 设置依赖注入
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddWorkflow(); // 添加 Workflow Core 服务
serviceCollection.AddTransient<DoSomething>();
serviceCollection.AddTransient<IMyService, MyService>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var host = serviceProvider.GetService<IWorkflowHost>();

//注册工作流
host.RegisterWorkflow<PassingDataWorkflow>();

// Start the host
host.Start();

await host.StartWorkflow("MyWorkflow");

//Our workflow definition with strongly typed internal data and mapped inputs & outputs
public class PassingDataWorkflow : IWorkflow
{
	public string Id => "MyWorkflow";

	public int Version => 1;
	
	public void Build(IWorkflowBuilder<object> builder)
	{
		  builder
                .StartWith(context =>
                {
                    Console.WriteLine("Starting workflow...");
                    return ExecutionResult.Next();
                })
				.Then<DoSomething>()
				.Then(context =>
					{
						Console.WriteLine("Workflow complete");
						return ExecutionResult.Next();
					});
	}
}

public class DoSomething : StepBody
{
	private IMyService _myService;

	public DoSomething(IMyService myService)
	{
		_myService = myService;
	}

	public override ExecutionResult Run(IStepExecutionContext context)
	{
		_myService.DoTheThings();
		return ExecutionResult.Next();
	}
}

public interface IMyService
{
	void DoTheThings();
}

public class MyService : IMyService
{
	public void DoTheThings()
	{
		Console.WriteLine("Doing stuff...");
	}
}