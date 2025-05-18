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

//您可以在工作流中定义多个独立的分支，并根据表达式值选择一个。

// 设置依赖注入
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging();
serviceCollection.AddWorkflow();
var serviceProvider = serviceCollection.BuildServiceProvider();
var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<DecisionBranchWorkflow, MyData>();

// 启动主机
host.Start();

// 启动带有不同数据的多个工作流实例
var data1 = new MyData { Value1 = "one" };
var data2 = new MyData { Value1 = "two" };
var data3 = new MyData { Value1 = "three" };

Console.WriteLine("Starting workflow with Value1 = 'one'");
var workflowId1 = await host.StartWorkflow("DecisionBranchWorkflow", 1, data1);

Console.WriteLine("\nStarting workflow with Value1 = 'two'");
var workflowId2 = await host.StartWorkflow("DecisionBranchWorkflow", 1, data2);

Console.WriteLine("\nStarting workflow with Value1 = 'three'");
var workflowId3 = await host.StartWorkflow("DecisionBranchWorkflow", 1, data3);

// 等待所有工作流完成
await Task.Delay(1000);

// 停止主机
host.Stop();

// 自定义消息步骤
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
    public string Value1 { get; set; }
}

// 工作流定义
public class DecisionBranchWorkflow : IWorkflow<MyData>
{
    public string Id => "DecisionBranchWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<MyData> builder)
    {
        // 创建分支1
        var branch1 = builder.CreateBranch()
            .StartWith<PrintMessage>()
                .Input(step => step.Message, data => "Hi from branch 1")
            .Then<PrintMessage>()
                .Input(step => step.Message, data => "Bye from branch 1");

        // 创建分支2
        var branch2 = builder.CreateBranch()
            .StartWith<PrintMessage>()
                .Input(step => step.Message, data => "Hi from branch 2")
            .Then<PrintMessage>()
                .Input(step => step.Message, data => "Bye from branch 2");

        // 主工作流
        builder
            .StartWith<PrintMessage>()
				.Input(step => step.Message, data => $"Starting workflow with Value1 = '{data.Value1}'")
			.Then(context => {})
			.Branch ("123" , branch1)
            .Decide(data => data.Value1)
                .Branch((data, outcome) => data.Value1 == "one", branch1)
                .Branch((data, outcome) => data.Value1 == "two", branch2)
            .Then<PrintMessage>()
                .Input(step => step.Message, data => 
                    data.Value1 == "one" || data.Value1 == "two" 
                        ? "Workflow completed after branch execution" 
                        : "No matching branch found for: " + data.Value1);
    }
}
