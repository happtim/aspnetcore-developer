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

// JSON 工作流定义
string jsonDefinition = @"{
    ""Id"": ""HelloWorld"",
    ""Version"": 1,
    ""Steps"": [
        {
            ""Id"": ""Hello"",
            ""StepType"": ""HelloWorld, LINQPadQuery"",
            ""NextStepId"": ""Bye""
        },        
        {
            ""Id"": ""Bye"",
            ""StepType"": ""GoodbyeWorld, LINQPadQuery""
        }
    ]
}";

// 加载定义
loader.LoadDefinition(jsonDefinition, Deserializers.Json);

// 启动主机
host.Start();

// 启动工作流实例（使用JSON中定义的ID）
var workflowId = await host.StartWorkflow("HelloWorld");

// 等待工作流完成
await Task.Delay(3000);

// 停止主机
host.Stop();

