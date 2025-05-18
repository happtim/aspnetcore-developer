<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <NuGetReference>WorkflowCore</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>WorkflowCore.Interface</Namespace>
  <Namespace>WorkflowCore.Models</Namespace>
  <Namespace>WorkflowCore.Services</Namespace>
</Query>

//Setps ：工作流由一系列连接的步骤组成。每个步骤可以有输入，并产生可以传回其所在工作流的输出。
//IWorkflow ：我们通过组合一系列步骤来定义工作流结构。
//Host:工作流Host是负责执行工作流的服务。
//		1.它通过轮询持久性提供者获取准备运行的工作流实例，执行它们，然后将其传回持久性提供者以便在下次运行时存储。
//		2.它还负责向可能在等待某个事件的工作流发布事件。

// 设置依赖注入
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddWorkflow(); // 添加 Workflow Core 服务

var serviceProvider = serviceCollection.BuildServiceProvider();

var host = serviceProvider.GetService<IWorkflowHost>();

//注册工作流
host.RegisterWorkflow<HelloWorldWorkflow>();

// Start the host
host.Start();

// Create a new workflow instance
string workflowId = await host.StartWorkflow("HelloWorldWorkflow");


// Define the workflow program
public class HelloWorldWorkflow : IWorkflow
{
	public string Id => "HelloWorldWorkflow";
	public int Version => 1;

	public void Build(IWorkflowBuilder<object> builder)
	{
		builder
			.StartWith<HelloWorld>()
			.Then<GoodbyeWorld>();
	}
}

// Define the first step
public class HelloWorld : StepBody
{
	public override ExecutionResult Run(IStepExecutionContext context)
	{
		Console.WriteLine("Hello World!");
		return ExecutionResult.Next();
	}
}

// Define the second step
public class GoodbyeWorld : StepBody
{
	public override ExecutionResult Run(IStepExecutionContext context)
	{
		Console.WriteLine("Goodbye World!");
		return ExecutionResult.Next();
	}
}
