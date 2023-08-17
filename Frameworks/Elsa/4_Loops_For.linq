<Query Kind="Statements">
  <NuGetReference Version="3.0.0-preview.604" Prerelease="true">Elsa</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Elsa.Extensions</Namespace>
  <Namespace>Elsa.Workflows.Core.Contracts</Namespace>
  <Namespace>Elsa.Workflows.Core.Memory</Namespace>
  <Namespace>Elsa.Workflows.Core.Activities</Namespace>
  <Namespace>Elsa.Workflows.Core.Models</Namespace>
</Query>

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();

// Resolve a workflow runner to run the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();


{ // for

	var currentValueVariable = new Variable<int>();

	var @for = new Sequence
	{
		Variables = { currentValueVariable },
		Activities =
		{
			new WriteLine("Counting down from 10 to 1:"),
			new For(10, 1)
			{
				CurrentValue = new Output<object?>(currentValueVariable),
				Body = new Sequence
				{
					Activities =
					{
						new WriteLine(context => $"Current value: {currentValueVariable.Get(context)}")
					}
				}
			},
			new WriteLine("Happy coding!\n")
		}
	};

	workflowRunner.RunAsync(@for);

}