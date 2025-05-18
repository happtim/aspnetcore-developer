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

//使用 .Parallel() 方法来分支并行任务

// 设置依赖注入
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging();
serviceCollection.AddWorkflow();
var serviceProvider = serviceCollection.BuildServiceProvider();
var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<ParallelWorkflow, MyData>();

// 启动主机
host.Start();

// 启动工作流实例
Console.WriteLine("Starting parallel workflow");
var workflowId = await host.StartWorkflow("parallel-sample");

// 等待工作流完成
await Task.Delay(4000);

// 停止主机
host.Stop();

// 简单的步骤
public class SayHello : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("Starting workflow with parallel paths");
        return ExecutionResult.Next();
    }
}

public class SayGoodbye : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("All parallel paths completed, ending workflow");
        return ExecutionResult.Next();
    }
}

// 各条并行路径的任务
public class Task1dot1 : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Path 1 - Task 1 (Thread: {Thread.CurrentThread.ManagedThreadId})");
        Thread.Sleep(500); // 模拟工作
        return ExecutionResult.Next();
    }
}

public class Task1dot2 : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Path 1 - Task 2 (Thread: {Thread.CurrentThread.ManagedThreadId})");
        Thread.Sleep(500); // 模拟工作
        return ExecutionResult.Next();
    }
}

public class Task2dot1 : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Path 2 - Task 1 (Thread: {Thread.CurrentThread.ManagedThreadId})");
        Thread.Sleep(800); // 模拟工作
        return ExecutionResult.Next();
    }
}

public class Task2dot2 : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Path 2 - Task 2 (Thread: {Thread.CurrentThread.ManagedThreadId})");
        Thread.Sleep(800); // 模拟工作
        return ExecutionResult.Next();
    }
}

public class Task3dot1 : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Path 3 - Task 1 (Thread: {Thread.CurrentThread.ManagedThreadId})");
        Thread.Sleep(300); // 模拟工作
        return ExecutionResult.Next();
    }
}

public class Task3dot2 : StepBody
{
    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine($"Path 3 - Task 2 (Thread: {Thread.CurrentThread.ManagedThreadId})");
        Thread.Sleep(300); // 模拟工作
        return ExecutionResult.Next();
    }
}

// 数据类
public class MyData
{
}

// 工作流定义
public class ParallelWorkflow : IWorkflow<MyData>
{
    public string Id => "parallel-sample";
    public int Version => 1;

    public void Build(IWorkflowBuilder<MyData> builder)
    {
        builder
            .StartWith<SayHello>()
            .Parallel()
                .Do(then => 
                    then.StartWith<Task1dot1>()
                        .Then<Task1dot2>())
                .Do(then =>
                    then.StartWith<Task2dot1>()
                        .Then<Task2dot2>())
                .Do(then =>
                    then.StartWith<Task3dot1>()
                        .Then<Task3dot2>())
            .Join()
            .Then<SayGoodbye>();
    }
}
