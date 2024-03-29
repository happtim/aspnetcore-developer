<Query Kind="Statements">
  <NuGetReference Version="3.0.0">Elsa</NuGetReference>
  <Namespace>Elsa.Extensions</Namespace>
  <Namespace>Elsa.Workflows.Activities</Namespace>
  <Namespace>Elsa.Workflows.Contracts</Namespace>
  <Namespace>Elsa.Workflows.Memory</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>


// Setup service container.
var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();

// Resolve a workflow runner to run the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

var variableInt = new Variable<int>(1);

var sequence =  new Sequence
{
	Variables = { variableInt },	
	Activities = 
	{
		new WriteLine( context => $"Current value: { variableInt.Get(context)}"),
		new SetVariable<int>( variableInt , context => variableInt.Get(context) + 1),
		new WriteLine( context => $"Current value: { variableInt.Get(context)}"),
	},
};

// Run the workflow.
await workflowRunner.RunAsync(sequence);
