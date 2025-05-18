<Query Kind="Statements">
  <NuGetReference>WorkflowCore</NuGetReference>
  <Namespace>WorkflowCore.Models</Namespace>
  <Namespace>WorkflowCore.Interface</Namespace>
</Query>

// Define the first step
public class HelloWorld : StepBody
{
	public override ExecutionResult Run(IStepExecutionContext context)
	{
		Console.WriteLine("Hello World!");
		return ExecutionResult.Next();
	}
}

// Define the second step
public class GoodbyeWorld : StepBody
{
	public override ExecutionResult Run(IStepExecutionContext context)
	{
		Console.WriteLine("Goodbye World!");
		return ExecutionResult.Next();
	}
}
