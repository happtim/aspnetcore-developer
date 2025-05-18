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

// 在 WorkflowCore 中，Activity 是需要由外部执行者（人或系统）完成的任务
// 示例场景：一个审批流程，需要人为审批一个请求

// 设置依赖注入
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddWorkflow(); // 添加 Workflow Core 服务

var serviceProvider = serviceCollection.BuildServiceProvider();

var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<ActivityWorkflow, MyData>();

// 启动主机
host.Start();

// 启动工作流实例，传入请求数据
var workflowId = await host.StartWorkflow("activity-sample", new MyData { Request = "Spend $1,000,000" });
Console.WriteLine($"Started workflow with ID: {workflowId}");

// 给工作流一点时间到达 Activity 步骤
await Task.Delay(1000);

// 模拟检查待处理的活动
Console.WriteLine("Worker checking for pending activities...");
var approval =await host.GetPendingActivity("get-approval", "worker1", TimeSpan.FromMinutes(1));

if (approval != null)
{                
    // 工作人员看到需要审批的请求
    Console.WriteLine("Approval required for " + approval.Parameters);
    
    // 进行审批并提交结果（审批人的名字）
    Console.WriteLine("Worker submitting approval...");
   	await host.SubmitActivitySuccess(approval.Token, "John Smith");
}
else
{
    Console.WriteLine("No pending activities found.");
}

// 等待工作流完成
await Task.Delay(2000);

// 停止主机
host.Stop();

// 数据类，用于保存工作流数据
public class MyData
{
    public string Request { get; set; }
    public string ApprovedBy { get; set; }
}

// 工作流定义
public class ActivityWorkflow : IWorkflow<MyData>
{
    public string Id => "activity-sample";
    public int Version => 1;

    public void Build(IWorkflowBuilder<MyData> builder)
    {
        builder
             .StartWith(context => ExecutionResult.Next())
            // 定义一个需要外部处理的活动
            // 活动名称为 "get-approval"，参数为请求内容，结果将保存到 ApprovedBy 字段
            .Activity("get-approval", data => data.Request)
				.Output(data => data.ApprovedBy, step => step.Result)
   			.Then<CustomMessage>()
					.Input(step => step.Message, data => "Approved by " + data.ApprovedBy)
			.Then(context =>
			{
				Console.WriteLine("Workflow completed");
				return ExecutionResult.Next();
			});
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
