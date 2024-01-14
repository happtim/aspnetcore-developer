<Query Kind="Statements">
  <NuGetReference Version="3.0.0">Elsa</NuGetReference>
  <Namespace>Elsa.Expressions.Models</Namespace>
  <Namespace>Elsa.Extensions</Namespace>
  <Namespace>Elsa.Workflows.Activities</Namespace>
  <Namespace>Elsa.Workflows.Contracts</Namespace>
  <Namespace>Elsa.Workflows.Memory</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
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

// Declare a workflow variable for use in the workflow.
var ageVariable = new Variable<string>();

// Declare a workflow.
var ifelse = new Sequence
{
	// Register the variable.
	Variables = { ageVariable },

	// Setup the sequence of activities to run.
	Activities =
	{
		new WriteLine("Please tell me your age:"),
		new ReadLine(ageVariable), // Stores user input into the provided variable.,
        new If
		{
            // If aged 18 or up, beer is provided, soda otherwise.
            Condition = new (context => ageVariable.Get<int>(context) < 18),
			Then = new WriteLine("Enjoy your soda!"),
			Else = new WriteLine("Enjoy your beer!")
		},
		new WriteLine("Come again!")
	}
};

// Run the workflow.
await workflowRunner.RunAsync(ifelse);

var switchcase = new Sequence
{
	Variables = { ageVariable },
	
	Activities =
	{
		new WriteLine("Please tell me your age:"),
		new ReadLine(ageVariable), // Stores user input into the provided variable.,
		new Switch
		{
			Cases = 
			{
				new SwitchCase("<16",context => ageVariable.Get<int>(context) < 16,new WriteLine("Enjoy your soda!") ),
				new SwitchCase("17",context => ageVariable.Get<int>(context) == 17,new WriteLine("Enjoy your soda!") ),
				new SwitchCase("18",context => ageVariable.Get<int>(context) == 18,new WriteLine("Enjoy your beer!") ),
			},
			Default = new WriteLine("Enjoy your soda!"),
		}
	}
};

// Run the workflow.
await workflowRunner.RunAsync(switchcase);