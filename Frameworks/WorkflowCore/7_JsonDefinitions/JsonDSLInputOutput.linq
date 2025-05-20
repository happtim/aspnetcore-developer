<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <NuGetReference>WorkflowCore</NuGetReference>
  <NuGetReference>WorkflowCore.DSL</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>WorkflowCore.Interface</Namespace>
  <Namespace>WorkflowCore.Models</Namespace>
  <Namespace>WorkflowCore.Services</Namespace>
  <Namespace>WorkflowCore.Primitives</Namespace>
  <Namespace>WorkflowCore.Services.DefinitionStorage</Namespace>
</Query>

#load "..\CommonSteps"

// 设置依赖注入
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging();
serviceCollection.AddWorkflow();
serviceCollection.AddWorkflowDSL(); // Add DSL support
var serviceProvider = serviceCollection.BuildServiceProvider();

var host = serviceProvider.GetService<IWorkflowHost>();

// 获取定义加载器
var loader = serviceProvider.GetService<IDefinitionLoader>();

// JSON 工作流定义，使用输入和输出
string jsonDefinition = @"{
    ""Id"": ""AddWorkflow"",
    ""Version"": 1,
    ""DataType"": ""MyDataClass, LINQPadQuery"",
    ""Steps"": [
        {
            ""Id"": ""Hello"",
            ""StepType"": ""HelloWorld, LINQPadQuery"",
            ""NextStepId"": ""Add""
        },
        {
            ""Id"": ""Add"",
            ""StepType"": ""AddNumbers, LINQPadQuery"",
            ""NextStepId"": ""PrintResult"",
            ""Inputs"": {
                ""Input1"": ""data.Value1"",
                ""Input2"": ""data.Value2""
            },
            ""Outputs"": {
                ""Value3"": ""step.Output""
            }
        },
        {
            ""Id"": ""PrintResult"",
            ""StepType"": ""CustomMessage, LINQPadQuery"",
            ""Inputs"": {
                ""Message"": ""data.Value3""
            }
        }
    ]
}";

// 加载定义
loader.LoadDefinition(jsonDefinition, Deserializers.Json);

// 启动主机
host.Start();

// 创建初始数据
var initialData = new MyDataClass
{
    Value1 = 5,
    Value2 = 7
};

// 启动工作流实例，传入初始数据
var workflowId = await host.StartWorkflow("AddWorkflow", 1, initialData);

// 等待工作流完成
await Task.Delay(3000);

// 停止主机
host.Stop();

// Define AddNumbers step class (copied from PassingData.linq)
public class AddNumbers : StepBody
{
	public int Input1 { get; set; }
	public int Input2 { get; set; }
	public int Output { get; set; }

	public override ExecutionResult Run(IStepExecutionContext context)
	{
		Output = (Input1 + Input2);
		return ExecutionResult.Next();
	}
}

// Define CustomMessage step class
public class CustomMessage : StepBody
{
	public string Message { get; set; }

	public override ExecutionResult Run(IStepExecutionContext context)
	{
		Console.WriteLine(Message);
		return ExecutionResult.Next();
	}
}

// Define data class
public class MyDataClass
{
	public int Value1 { get; set; }
	public int Value2 { get; set; }
	public int Value3 { get; set; }
}