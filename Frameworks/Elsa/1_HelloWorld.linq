<Query Kind="Statements">
  <NuGetReference Prerelease="true">Elsa</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Elsa.Extensions</Namespace>
  <Namespace>Elsa.Workflows.Core.Activities</Namespace>
  <Namespace>Elsa.Workflows.Core.Contracts</Namespace>
</Query>

// Setup service container.
var services = new ServiceCollection();

// Add Elsa services.
services.AddElsa();

// Build service container.
var serviceProvider = services.BuildServiceProvider();

// Resolve a workflow runner to run the workflow.
var workflowRunner = serviceProvider.GetRequiredService<IWorkflowRunner>();

// Define a simple workflow, which in this case is a very simple activity that writes something to the console:
var workflow = new WriteLine("Hello world!");

// Run the workflow.
await workflowRunner.RunAsync(workflow);