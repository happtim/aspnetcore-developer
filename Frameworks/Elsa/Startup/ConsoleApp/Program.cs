// Setup service container.

using System.ComponentModel;
using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Core.Memory;
using Elsa.Workflows.Core.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();

// Define a simple workflow, which in this case is a very simple activity that writes something to the console:
//var workflow = new WriteLine("Hello world!");

var workflow = new Sequence
{
	Activities =
	{
		new WriteLine("Hello World!"),
		new WriteLine("Goodbye cruel world...")
	}
};

// Resolve a workflow runner to run the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

// Run the workflow.
await workflowRunner.RunAsync(workflow);

