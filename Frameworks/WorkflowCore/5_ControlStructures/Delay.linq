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

//Delay 步骤将暂停您工作流的当前分支一段指定时间。

// 设置依赖注入
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging();
serviceCollection.AddWorkflow();
var serviceProvider = serviceCollection.BuildServiceProvider();
var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<DelayWorkflow>();

// 启动主机
host.Start();

// 启动工作流实例
Console.WriteLine($"Starting delay workflow at {DateTime.Now}");
var workflowId = await host.StartWorkflow("DelayWorkflow");

// 等待工作流完成
await Task.Delay(6000);

// 停止主机
host.Stop();

// 工作流定义
public class DelayWorkflow : IWorkflow
{
    public string Id => "DelayWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<object> builder)
    {
        builder
            .StartWith(context =>
            {
                Console.WriteLine($"Workflow started at {DateTime.Now}");
                return ExecutionResult.Next();
            })
            .Then(context =>
            {
                Console.WriteLine("Now we'll delay for 3 seconds...");
                return ExecutionResult.Next();
            })
            .Delay(data => TimeSpan.FromSeconds(3))
            .Then(context =>
            {
                Console.WriteLine($"Workflow resumed after delay at {DateTime.Now}");
                return ExecutionResult.Next();
            })
            .Then(context =>
            {
                Console.WriteLine("Workflow completed");
                return ExecutionResult.Next();
            });
    }
}
