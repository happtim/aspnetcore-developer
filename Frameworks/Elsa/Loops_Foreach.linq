<Query Kind="Statements">
  <NuGetReference Version="3.0.0">Elsa</NuGetReference>
  <Namespace>Elsa.Extensions</Namespace>
  <Namespace>Elsa.Workflows.Activities</Namespace>
  <Namespace>Elsa.Workflows.Contracts</Namespace>
  <Namespace>Elsa.Workflows.Memory</Namespace>
  <Namespace>Elsa.Workflows.Models</Namespace>
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

{//foreach

	var currentValueVariable = new Variable<string>();
	var shoppingList = new[] { "Apples", "Bananas", "Potatoes", "Coffee", "Honey", "Rice" };

	var @foreach = new Sequence
	{
		Variables = { currentValueVariable },
		Activities =
		{
			new WriteLine("Going through the shopping list..."),
			new ForEach<string>(shoppingList)
			{
				CurrentValue = new Output<string>(currentValueVariable),
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