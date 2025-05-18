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

//使用 .Recur 在您的工作流中设置一组重复的后台步骤，直到满足某个条件

// 设置依赖注入
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging();
serviceCollection.AddWorkflow();
var serviceProvider = serviceCollection.BuildServiceProvider();
var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<RecurWorkflow, MyData>();

// 启动主机
host.Start();

// 启动工作流实例
var workflowId = await host.StartWorkflow("RecurWorkflow", 1, new MyData { Counter = 0 });

// 等待工作流完成
await Task.Delay(10000);

// 停止主机
host.Stop();

// 数据类
public class MyData
{
    public int Counter { get; set; }
}

// 工作流定义
public class RecurWorkflow : IWorkflow<MyData>
{
    public string Id => "RecurWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<MyData> builder)
    {
        builder
            .StartWith(context =>
            {
                Console.WriteLine($"Main workflow started at {DateTime.Now}");
                return ExecutionResult.Next();
            })
            .Recur(data => TimeSpan.FromSeconds(1), data => data.Counter >= 3).Do(recur => recur
                .StartWith(context =>
                {
                    var data = context.Workflow.Data as MyData;
                    data.Counter++;
                    Console.WriteLine($"Recurring task execution #{data.Counter} at {DateTime.Now}");
                    return ExecutionResult.Next();
                })
            )
            .Then(context =>
            {
                Console.WriteLine($"Main workflow continues after recurring tasks at {DateTime.Now}");
                Console.WriteLine($"Final counter value: {context.Workflow.Data.As<MyData>().Counter}");
                return ExecutionResult.Next();
            })
            .Then(context =>
            {
                Console.WriteLine("Main workflow completed");
                return ExecutionResult.Next();
            });
    }
}
