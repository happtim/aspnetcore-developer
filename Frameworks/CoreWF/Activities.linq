<Query Kind="Statements">
  <NuGetReference Version="6.0.3">UiPath.Workflow</NuGetReference>
  <Namespace>System.Activities</Namespace>
</Query>

public class ReadLine : CodeActivity<string>
{
	protected override string Execute(CodeActivityContext context)
	{
		return Console.ReadLine();
	}
}