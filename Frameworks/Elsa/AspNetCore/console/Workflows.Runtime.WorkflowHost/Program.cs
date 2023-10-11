using Elsa.Extensions;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Runtime.Contracts;
using Microsoft.Extensions.DependencyInjection;

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();

// Define a simple workflow, which in this case is a very simple activity that writes something to the console:
var workflow = new WriteLine("Hello world!");

// Resolve a workflow host factory to run the workflow.
var workflowHostFactory = serviceProvider.GetRequiredService<IWorkflowHostFactory>();

// Create a workflow host.
var workflowHost = await workflowHostFactory.CreateAsync(Workflow.FromActivity(workflow));

// Run the workflow.
await workflowHost.StartWorkflowAsync();