<Query Kind="Statements">
  <NuGetReference Prerelease="true">Elsa</NuGetReference>
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

{//foreach

	var currentValueVariable = new Variable<string>();
	var shoppingList = new[] { "Apples", "Bananas", "Potatoes", "Coffee", "Honey", "Rice" };

	var @foreach = new Sequence
	{
		Variables = { currentValueVariable },
		Activities =
		{
			new WriteLine("Going through the shopping list..."),
			new ForEach(shoppingList)
			{
				CurrentValue = new Output<object>(currentValueVariable),
				Body = new Sequence
				{
					Activities =
					{
						new WriteLine(context => $"- [ ] {currentValueVariable.Get(context)}"),
					}
				}
			},
			new WriteLine("Let's not forget anything!\n")
		}
	};

	workflowRunner.RunAsync(@foreach);

}