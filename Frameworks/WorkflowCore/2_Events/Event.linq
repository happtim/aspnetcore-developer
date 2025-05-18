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

//工作流也可以在继续之前等待外部事件。在以下示例中，工作流将等待一个名为"MyEvent"、键为 0 的事件。
//一旦外部源触发了该事件，工作流将唤醒并继续处理，将事件生成的数据传递到下一步。

//生效日期：您还可以在等待事件时指定生效日期，这使您能够响应可能已经发生的事件，或仅响应在生效日期之后发生的事件。

// 设置依赖注入
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddWorkflow(); // 添加 Workflow Core 服务

var serviceProvider = serviceCollection.BuildServiceProvider();

var host = serviceProvider.GetService<IWorkflowHost>();

// 注册工作流
host.RegisterWorkflow<EventSampleWorkflow, MyDataClass>();

// Start the host
host.Start();

// Start a new workflow instance
var workflowId = await host.StartWorkflow("EventSampleWorkflow");
Console.WriteLine($"Started workflow with ID: {workflowId}");

// Give the workflow a moment to reach the WaitFor step
await Task.Delay(1000);

// Publish an event that the workflow is waiting for
Console.WriteLine("Publishing event 'MyEvent' with key '0' and data 'hello'");
await host.PublishEvent("MyEvent", "0", "hello");

// Wait for a while to let the workflow complete
await Task.Delay(3000);

// Stop the host
host.Stop();

// The data class to hold workflow data
public class MyDataClass
{
    public string Value { get; set; }
}

// The workflow definition
public class EventSampleWorkflow : IWorkflow<MyDataClass>
{
    public string Id => "EventSampleWorkflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<MyDataClass> builder)
    {
        builder
            .StartWith(context => 
            {
                Console.WriteLine("Workflow started, waiting for event...");
                return ExecutionResult.Next();
            })
            .WaitFor("MyEvent", data => "0")
                .Output(data => data.Value, step => step.EventData)
            .Then<CustomMessage>()
                .Input(step => step.Message, data => "The data from the event is " + data.Value)
            .Then(context => 
            {
                Console.WriteLine("Workflow completed");
                return ExecutionResult.Next();
            });
    }
}

// A custom step that displays a message
public class CustomMessage : StepBody
{
    public string Message { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine(Message);
        return ExecutionResult.Next();
    }
}