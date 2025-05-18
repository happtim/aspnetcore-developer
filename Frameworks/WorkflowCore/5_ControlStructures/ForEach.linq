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

//使用 .ForEach 方法启动并行 for 循环

// 设置依赖注入
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging();
serviceCollection.AddWorkflow();
var serviceProvider = serviceCollection.BuildServiceProvider();
var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<ForEachWorkflow>();

// 启动主机
host.Start();

// 启动工作流实例
Console.WriteLine("Starting ForEach workflow");
var workflowId = await host.StartWorkflow("ForEachWorkflow");

// 等待工作流完成
await Task.Delay(5000);

// 停止主机
host.Stop();

// 显示当前上下文项的步骤
public class DisplayContext : StepBody
{
    public string Message { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Processing item: {Message}");
        return ExecutionResult.Next();
    }
}

// 简单的步骤
public class SayHello : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("Hello from workflow");
        return ExecutionResult.Next();
    }
}

public class SayGoodbye : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("Goodbye from workflow");
        return ExecutionResult.Next();
    }
}

public class DoSomething : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        // 模拟某些处理
        Console.WriteLine($"  - Doing some work on item in thread {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(1500); // 模拟耗时操作
        return ExecutionResult.Next();
    }
}

// 工作流定义
public class ForEachWorkflow : IWorkflow
{
    public string Id => "ForEachWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<object> builder)
    {
        builder
            .StartWith<SayHello>()
			//是否并行运行通过后面参数控制。
            .ForEach(data => new List<int>() { 1, 2, 3, 4 }, data => true)
                .Do(x => x
                    .StartWith<DisplayContext>()
                        .Input(step => step.Message, (data, context) => context.Item.ToString())
                    .Then<DoSomething>())
            .Then<SayGoodbye>();
    }
}
