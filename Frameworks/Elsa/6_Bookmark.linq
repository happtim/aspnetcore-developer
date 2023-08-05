<Query Kind="Statements">
  <NuGetReference Prerelease="true">Elsa</NuGetReference>
  <Namespace>Elsa.Extensions</Namespace>
  <Namespace>Elsa.Workflows.Core</Namespace>
  <Namespace>Elsa.Workflows.Core.Activities</Namespace>
  <Namespace>Elsa.Workflows.Core.Contracts</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Elsa.Workflows.Core.Models</Namespace>
  <RemoveNamespace>System.Diagnostics</RemoveNamespace>
  <RemoveNamespace>System.Linq.Expressions</RemoveNamespace>
</Query>

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();

// Resolve a workflow runner to run the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

// Create a workflow.
var workflow = new Workflow
{
	Root = new Sequence
	{
		Activities =
		{
			new WriteLine("Hello World!"),
			new If( new Input<bool>(1 > 2)){
				Then = new MyEvent(), // Blocks until the event bookmark is resumed.
				Else = new MyEvent(), // Blocks until the event bookmark is resumed.
			},
            new WriteLine("Event received!")
		}
	}
};

// Run the workflow.
var result = await workflowRunner.RunAsync(workflow);

// Resume the workflow using te created bookmark.
var bookmark = result.WorkflowState.Bookmarks.Single();
var workflowState = result.WorkflowState;
await workflowRunner.RunAsync(workflow, workflowState, new RunWorkflowOptions(bookmarkId: bookmark.Id));


public class MyEvent : Activity
{
	protected override void Execute(ActivityExecutionContext context)
	{
		context.CreateBookmark();
	}
}