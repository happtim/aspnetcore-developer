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

// 设置依赖注入
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging();
serviceCollection.AddWorkflow();
var serviceProvider = serviceCollection.BuildServiceProvider();
var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<WhileWorkflow, MyData>();

// 启动主机
host.Start();

// 启动工作流实例
Console.WriteLine("Starting While workflow");
var workflowId = await host.StartWorkflow("WhileWorkflow", 1, new MyData { Counter = 0 });

// 等待工作流完成
await Task.Delay(3000);

// 停止主机
host.Stop();

// 简单的步骤
public class SayHello : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("Starting workflow with while loop");
        return ExecutionResult.Next();
    }
}

public class SayGoodbye : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("While loop completed, ending workflow");
        return ExecutionResult.Next();
    }
}

public class DoSomething : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("Executing step inside while loop");
        return ExecutionResult.Next();
    }
}

// 递增计数器的步骤
public class IncrementStep : StepBody
{
    public int Value1 { get; set; }
    public int Value2 { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Value2 = Value1 + 1;
        Console.WriteLine($"Counter incremented from {Value1} to {Value2}");
        return ExecutionResult.Next();
    }
}

// 数据类
public class MyData
{
    public int Counter { get; set; }
}

// 工作流定义
public class WhileWorkflow : IWorkflow<MyData>
{
    public string Id => "WhileWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<MyData> builder)
    {
        builder
            .StartWith<SayHello>()
            .While(data => data.Counter < 3)
                .Do(x => x
                    .StartWith<DoSomething>()
                    .Then<IncrementStep>()
                        .Input(step => step.Value1, data => data.Counter)
                        .Output(data => data.Counter, step => step.Value2))
            .Then<SayGoodbye>();
    }
}
