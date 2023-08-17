<Query Kind="Statements">
  <NuGetReference Version="3.0.0-preview.604" Prerelease="true">Elsa</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Elsa.Extensions</Namespace>
  <Namespace>Elsa.Workflows.Core.Contracts</Namespace>
  <Namespace>Elsa.Workflows.Core.Activities</Namespace>
  <Namespace>Elsa.Workflows.Core.Memory</Namespace>
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

{//while

 	var counterVariable = new Variable<int>(1);
 
	var @while = new Sequence
	{
		Variables = { counterVariable },
		Activities =
		{
			new WriteLine("Counting to 100:"),
            
            // Loop while counter < 100.
            new While(context => counterVariable.Get(context) <= 6)
			{
				Body = new Sequence
				{
					Activities =
					{
						new WriteLine(context => $"Current value: {counterVariable.Get(context)}"),
                        
                        // Increment counter variable.
                        new SetVariable<int>(counterVariable, context => counterVariable.Get(context) + 1)
					}
				}
			},
			new WriteLine("That was really fast!")
		}
	};
	
	workflowRunner.RunAsync(@while);
	
}



