<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <NuGetReference>WorkflowCore</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>WorkflowCore.Interface</Namespace>
  <Namespace>WorkflowCore.Models</Namespace>
  <Namespace>WorkflowCore.Services</Namespace>
</Query>

//每个步骤都旨在作为一个黑箱，因此它们支持输入和输出。这些输入和输出可以映射到一个数据类，该数据类定义了与每个工作流实例相关的自定义数据。
//

// 设置依赖注入
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddWorkflow(); // 添加 Workflow Core 服务

var serviceProvider = serviceCollection.BuildServiceProvider();

var host = serviceProvider.GetService<IWorkflowHost>();

//注册工作流
host.RegisterWorkflow<PassingDataWorkflow, Dictionary<string, int>>();

// Start the host
host.Start();

var initialData = new Dictionary<string, int>
{
	["Value1"] = 7,
	["Value2"] = 2
};

await host.StartWorkflow("PassingDataWorkflow", 1, initialData);

//Our workflow definition with strongly typed internal data and mapped inputs & outputs
public class PassingDataWorkflow : IWorkflow<Dictionary<string, int>>
{
	public string Id => "PassingDataWorkflow";

	public int Version => 1;
	
	public void Build(IWorkflowBuilder<Dictionary<string, int>> builder)
	{
		  builder
                .StartWith(context =>
                {
                    Console.WriteLine("Starting workflow...");
                    return ExecutionResult.Next();
                })
                .Then<AddNumbers>()
                    .Input(step => step.Input1, data => data["Value1"])
                    .Input(step => step.Input2, data => data["Value2"])
                    .Output((step, data) => data["Value3"] = step.Output)
                .Then<CustomMessage>()
                    .Name("Print custom message")
					.Input(step => step.Message, data => "The answer is " + data["Value3"].ToString())
				.Then(context =>
					{
						Console.WriteLine("Workflow complete");
						return ExecutionResult.Next();
					});
	}
}

//Our workflow step with inputs and outputs
public class AddNumbers : StepBody
{
	public int Input1 { get; set; }

	public int Input2 { get; set; }

	public int Output { get; set; }

	public override ExecutionResult Run(IStepExecutionContext context)
	{
		Output = (Input1 + Input2);
		return ExecutionResult.Next();
	}
}

public class CustomMessage : StepBody
{

	public string Message { get; set; }

	public override ExecutionResult Run(IStepExecutionContext context)
	{
		Console.WriteLine(Message);
		return ExecutionResult.Next();
	}
}
