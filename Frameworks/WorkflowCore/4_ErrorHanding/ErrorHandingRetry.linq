<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <NuGetReference>WorkflowCore</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>WorkflowCore.Interface</Namespace>
  <Namespace>WorkflowCore.Models</Namespace>
  <Namespace>WorkflowCore.Services</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "..\CommonSteps"

// 在 WorkflowCore 中，Activity 是需要由外部执行者（人或系统）完成的任务
// 示例场景：一个审批流程，需要人为审批一个请求

// 设置依赖注入
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddWorkflow(); // 添加 Workflow Core 服务

var serviceProvider = serviceCollection.BuildServiceProvider();

var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<ErrorHandlingWorkflow>();

// 启动主机
host.Start();

// Create a new workflow instance
string workflowId = await host.StartWorkflow("ErrorHandlingWorkflow");
Console.WriteLine($"Started workflow with ID: {workflowId}");

// 等待工作流完成
await Task.Delay(5000);
// 停止主机
host.Stop();


// Define the workflow program with error handling
public class ErrorHandlingWorkflow : IWorkflow
{
	public string Id => "ErrorHandlingWorkflow";
	public int Version => 1;
	public void Build(IWorkflowBuilder<object> builder)
	{
		// 主干流程
		builder
			.StartWith<HelloWorld>()
			.Then<PotentialFailingStep>()
				.OnError(WorkflowErrorHandling.Retry, TimeSpan.FromSeconds(2)) // 1. 重试策略: 发生错误时，等待2秒后重试
			.Then<GoodbyeWorld>();
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

// 可能会失败的步骤
public class PotentialFailingStep : StepBody
{
	private static int attemptCount = 0;

	public override ExecutionResult Run(IStepExecutionContext context)
	{
		attemptCount++;
		Console.WriteLine($"尝试执行可能失败的步骤 - 尝试次数: {attemptCount} 时间:{DateTime.Now}");

		// 模拟前两次尝试出错
		if (attemptCount <= 2)
		{
			throw new Exception("步骤执行失败，将重试");
		}

		Console.WriteLine("步骤执行成功！");
		return ExecutionResult.Next();
	}
}
