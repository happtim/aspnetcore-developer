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

// 在 WorkflowCore 中，WaitFor 步骤可以设置 EffectiveDate 参数，用于指定从何时开始接收事件
// 示例场景：工作流只处理在特定时间之后发生的事件，忽略之前的事件

// 设置依赖注入
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddWorkflow(); // 添加 Workflow Core 服务

var serviceProvider = serviceCollection.BuildServiceProvider();

var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<EffectiveDateWorkflow, MyDataClass>();

// 启动主机
host.Start();

// 启动新的工作流实例
var workflowId = await host.StartWorkflow("EffectiveDateWorkflow");
Console.WriteLine($"Started workflow with ID: {workflowId}");

// 给工作流一点时间到达 WaitFor 步骤
await Task.Delay(1000);

// 现在时间是 T
var now = DateTime.Now;
Console.WriteLine($"Current time: {now}");

// 创建有效日期 - 设置为当前时间之后的 2 秒
var effectiveDate = now.AddSeconds(2);
Console.WriteLine($"Effective date: {effectiveDate}");

// 先发布一个事件 - 这个事件会被忽略，因为它在有效日期之前
Console.WriteLine("\nPublishing event 'TimedEvent' with key '0' and data 'too early' (before effective date)");
await host.PublishEvent("TimedEvent", "0", "too early");

// 等待 3 秒 (超过有效日期)
Console.WriteLine("Waiting for 3 seconds to pass the effective date...");
await Task.Delay(3000);

// 再发布一个事件 - 这个事件会被处理，因为它在有效日期之后
Console.WriteLine("\nPublishing event 'TimedEvent' with key '0' and data 'just right' (after effective date)");
await host.PublishEvent("TimedEvent", "0", "just right");

// 等待一会儿，让工作流完成
await Task.Delay(3000);

// 停止主机
host.Stop();

// 数据类，用于保存工作流数据
public class MyDataClass
{
    public string Value { get; set; }
}

// 工作流定义
public class EffectiveDateWorkflow : IWorkflow<MyDataClass>
{
    public string Id => "EffectiveDateWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<MyDataClass> builder)
    {
        builder
            .StartWith(context => ExecutionResult.Next())
            .WaitFor("TimedEvent", data => "0", data => DateTime.Now.AddSeconds(2) )
                .Output(data => data.Value, step => step.EventData)
            .Then<CustomMessage>()
                .Input(step => step.Message, data => $"Received event with data: {data.Value} at {DateTime.Now}")
            .Then(context => 
            {
                Console.WriteLine("Workflow completed");
                return ExecutionResult.Next();
            });
    }
}

// 自定义步骤，显示消息
public class CustomMessage : StepBody
{
    public string Message { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine(Message);
        return ExecutionResult.Next();
    }
}
