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

//使用 .If 方法来开始一个 if 条件

// 设置依赖注入
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging();
serviceCollection.AddWorkflow();
var serviceProvider = serviceCollection.BuildServiceProvider();
var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<IfWorkflow, MyData>();

// 启动主机
host.Start();

// 启动带有不同数据的工作流实例
await host.StartWorkflow("IfWorkflow", 1, new MyData { Counter = 2 });

await Task.Delay(1000);

await host.StartWorkflow("IfWorkflow", 1, new MyData { Counter = 4 });

await Task.Delay(1000);

await host.StartWorkflow("IfWorkflow", 1, new MyData { Counter = 6 });

// 等待工作流完成
await Task.Delay(2000);

// 停止主机
host.Stop();

// 简单的步骤
public class SayHello : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var data = context.Workflow.Data as MyData;
        Console.WriteLine($"Starting workflow with Counter = {data.Counter}");
        return ExecutionResult.Next();
    }
}

public class SayGoodbye : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("Workflow completed");
        return ExecutionResult.Next();
    }
}

// 打印消息步骤
public class PrintMessage : StepBody
{
    public string Message { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine(Message);
        return ExecutionResult.Next();
    }
}

// 数据类
public class MyData
{
    public int Counter { get; set; }
}

// 工作流定义
public class IfWorkflow : IWorkflow<MyData>
{
    public string Id => "IfWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<MyData> builder)
    {
        builder
            .StartWith<SayHello>()
            .If(data => data.Counter < 3)
				.Do(then => then
	                .StartWith<PrintMessage>()
	                    .Input(step => step.Message, data => "Condition 1: Value is less than 3")
            )
            .If(data => data.Counter < 5)
				.Do(then => then
	                .StartWith<PrintMessage>()
	                    .Input(step => step.Message, data => "Condition 2: Value is less than 5")
            )
            .Then<SayGoodbye>();
    }
}
