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

//使用 .Schedule 注册一组将在您的工作流中异步在后台运行的未来步骤。

// 设置依赖注入
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging();
serviceCollection.AddWorkflow();
var serviceProvider = serviceCollection.BuildServiceProvider();
var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<ScheduleWorkflow>();

// 启动主机
host.Start();

// 启动工作流实例
var workflowId = await host.StartWorkflow("ScheduleWorkflow");

// 等待工作流完成（包括计划任务）
await Task.Delay(7000);

// 停止主机
host.Stop();

// 工作流定义
public class ScheduleWorkflow : IWorkflow
{
    public string Id => "ScheduleWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<object> builder)
    {
        builder
            .StartWith(context =>
            {
                Console.WriteLine($"Main workflow started at {DateTime.Now}");
                return ExecutionResult.Next();
            })
            .Schedule(data => TimeSpan.FromSeconds(5)).Do(schedule => schedule
                .StartWith(context =>
                {
                    Console.WriteLine($"Scheduled task started at {DateTime.Now} (after 5 seconds)");
                    return ExecutionResult.Next();
                })
                .Then(context =>
                {
                    Console.WriteLine("Scheduled task completed");
                    return ExecutionResult.Next();
                })
            )
            .Then(context =>
            {
                Console.WriteLine($"Main workflow continues at {DateTime.Now} without waiting for scheduled task");
                return ExecutionResult.Next();
            })
            .Then(context =>
            {
                Console.WriteLine("Main workflow completed");
                return ExecutionResult.Next();
            });
    }
}
